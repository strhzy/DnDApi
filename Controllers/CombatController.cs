using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Channels;
using DnDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnDAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombatController : ControllerBase
    {
        private readonly DnDContext _context;

        private static readonly ConcurrentDictionary<Guid, CombatRoom> _combatRooms = new();

        public CombatController(DnDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Combat>>> GetCombats([FromQuery] Guid campaignId)
        {
            if (campaignId != Guid.Empty)
            {
                return await _context.Combats
                    .Where(c => c.CampaignId == campaignId)
                    .Include(c => c.Participants)
                    .ToListAsync();
            }

            return await _context.Combats.Include(c => c.Participants).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Combat>> GetCombat(Guid id)
        {
            var combat = await _context.Combats.Include(c => c.Participants).FirstOrDefaultAsync(c => c.Id == id);

            if (combat == null)
            {
                return NotFound();
            }

            return combat;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombat(Guid id, Combat combat)
        {
            if (id != combat.Id)
            {
                return BadRequest();
            }

            _context.Entry(combat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CombatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Combat>> PostCombat(Combat combat)
        {
            _context.Combats.Add(combat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCombat", new { id = combat.Id }, combat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombat(Guid id)
        {
            var combat = await _context.Combats.FindAsync(id);
            if (combat == null)
            {
                return NotFound();
            }

            _context.Combats.Remove(combat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{combatId}/participants")]
        public async Task<ActionResult<CombatParticipant>> AddParticipant(
            Guid combatId,
            [FromBody] CombatParticipant participant)
        {
            var combat = await _context.Combats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == combatId);

            if (combat == null) return NotFound();

            participant.CombatId = combatId;
            participant.SyncWithSource();

            _context.CombatParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCombat), new { id = combat.Id }, combat);
        }

        private bool CombatExists(Guid id)
        {
            return _context.Combats.Any(e => e.Id == id);
        }

        [HttpGet("{combatId}/stream")]
        public async Task Stream(Guid combatId)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");

            var room = _combatRooms.GetOrAdd(combatId, id => new CombatRoom(id));
            room.Combat = _context.Combats
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == combatId);
            var channel = Channel.CreateUnbounded<string>();
            room.Clients[HttpContext.TraceIdentifier] = channel;

            try
            {
                await foreach (var msg in channel.Reader.ReadAllAsync(HttpContext.RequestAborted))
                {
                    await Response.WriteAsync($"data: {msg}\n\n");
                    await Response.Body.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // клиент отключился
            }
            finally
            {
                room.Clients.TryRemove(HttpContext.TraceIdentifier, out _);
            }
        }

        [HttpPost("{combatId}/player-move")]
        public IActionResult SendPlayerMove(Guid combatId, [FromBody] CombatLog log)
        {
            if (!_combatRooms.TryGetValue(combatId, out var room)) return NotFound();

            log.CombatId = combatId;
            

            room.PendingLogs.Add(log);

            room.Broadcast(new { eventType = "PendingMove", log });

            return Ok();
        }

        [HttpPost("{combatId}/master-confirm")]
        public IActionResult ConfirmMasterAction(Guid combatId, [FromBody] CombatConfirmRequest request)
        {
            if (!_combatRooms.TryGetValue(combatId, out var room)) return NotFound();

            var log = request.Log;

            var target = room.Combat.Participants.FirstOrDefault(p => p.Id == log.TargetId);
            if (target != null && log.Damage.HasValue)
            {
                if (log.Type == "attack")
                    target.CurrentHitPoints = Math.Max(0, target.CurrentHitPoints - log.Damage.Value);
                else if (log.Type == "heal")
                    target.CurrentHitPoints += log.Damage.Value;
            }

            var targetDb = _context.CombatParticipants.FirstOrDefault(p => p.Id == log.TargetId);
            if (targetDb != null)
            {
                targetDb.CurrentHitPoints = target.CurrentHitPoints;
                _context.SaveChanges();
            }

            room.Combat.CombatLogs.Add(log);

            room.Broadcast(new { eventType = "MasterConfirm", combat = room.Combat, log });
            return Ok();
        }

        [HttpPost("{combatId}/npc-move")]
        public IActionResult SendNpcMove(Guid combatId, [FromBody] CombatLog log)
        {
            if (!_combatRooms.TryGetValue(combatId, out var room)) return NotFound();

            log.CombatId = combatId;
            _context.CombatLog.Add(log);

            var target = room.Combat.Participants.FirstOrDefault(p => p.Id == log.TargetId);
            if (target != null && log.Damage.HasValue && log.Type == "attack")
            {
                target.CurrentHitPoints = Math.Max(0, target.CurrentHitPoints - log.Damage.Value);
                _context.CombatParticipants.FirstOrDefault(p => p.Id == log.TargetId).CurrentHitPoints =
                    target.CurrentHitPoints - log.Damage.Value;
            }
            else if (target != null && log.Damage.HasValue && log.Type == "heal")
            {
                target.CurrentHitPoints += log.Damage.Value;
                _context.CombatParticipants.FirstOrDefault(p => p.Id == log.TargetId).CurrentHitPoints =
                    target.CurrentHitPoints + log.Damage.Value;
            }
            else
            {
                return BadRequest();
            }

            _context.SaveChanges();

            room.Broadcast(new { eventType = "NpcMove", combat = room.Combat, log });
            return Ok();
        }

        [HttpPost("{combatId}/enemy-move")]
        public IActionResult SendEnemyMove(Guid combatId, [FromBody] CombatLog log)
        {
            if (!_combatRooms.TryGetValue(combatId, out var room)) return NotFound();

            log.CombatId = combatId;
            _context.CombatLog.Add(log);

            var target = room.Combat.Participants.FirstOrDefault(p => p.Id == log.TargetId);
            if (target != null && log.Damage.HasValue && log.Type == "attack")
            {
                target.CurrentHitPoints = Math.Max(0, target.CurrentHitPoints - log.Damage.Value);
                _context.CombatParticipants.FirstOrDefault(p => p.Id == log.TargetId).CurrentHitPoints =
                    target.CurrentHitPoints - log.Damage.Value;
            }
            else if (target != null && log.Damage.HasValue && log.Type == "heal")
            {
                target.CurrentHitPoints += log.Damage.Value;
                _context.CombatParticipants.FirstOrDefault(p => p.Id == log.TargetId).CurrentHitPoints =
                    target.CurrentHitPoints + log.Damage.Value;
            }
            else
            {
                return BadRequest();
            }

            _context.SaveChanges();

            room.Broadcast(new { eventType = "EnemyMove", combat = room.Combat, log });
            return Ok();
        }
    }


    public class CombatRoom
    {
        public Guid Id { get; }
        public Combat Combat { get; set;}
        public ObservableCollection<CombatLog> PendingLogs = new();
        public ConcurrentDictionary<string, Channel<string>> Clients { get; }

        public CombatRoom(Guid id)
        {
            Id = id;
            Clients = new();
        }

        public void Broadcast(object message)
        {
            var json = JsonSerializer.Serialize(message);
            foreach (var client in Clients.Values)
            {
                client.Writer.TryWrite(json);
            }
        }
    }

    public class CombatConfirmRequest
    {
        public Combat Combat { get; set; }
        public CombatLog Log { get; set; }
    }
}
