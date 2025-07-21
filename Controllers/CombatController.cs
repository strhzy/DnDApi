using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDAPI.Models;

namespace DnDAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombatController : ControllerBase
    {
        private readonly DnDContext _context;

        public CombatController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/Combat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Combat>>> GetCombats()
        {
            return await _context.Combats.Include(c => c.Participants).ToListAsync();
        }

        // GET: api/Combat/5
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

        // PUT: api/Combat/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Combat
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Combat>> PostCombat(Combat combat)
        {
            _context.Combats.Add(combat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCombat", new { id = combat.Id }, combat);
        }

        // DELETE: api/Combat/5
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
        public async Task<ActionResult<CombatParticipant>> AddParticipant(Guid combatId, [FromBody] CombatParticipant participant)
        {
            // 1. Находим бой
            var combat = await _context.Combats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == combatId);

            if (combat == null)
                return NotFound("Бой не найден");

            // 2. Проверяем тип участника и создаем соответствующую сущность
            CombatParticipant newParticipant;

            switch (participant.Type)
            {
                case ParticipantType.Player:
                    var playerChar = await _context.PlayerCharacters.FindAsync(participant.SourceId);
                    if (playerChar == null)
                        return BadRequest("Персонаж игрока не найден");

                    newParticipant = new PlayerCombatParticipant
                    {
                        PlayerCharacterId = playerChar.Id,
                        PlayerCharacter = playerChar,
                        Name = string.IsNullOrEmpty(participant.Name) ? playerChar.Name : participant.Name,
                        CurrentHitPoints = participant.CurrentHitPoints > 0
                            ? participant.CurrentHitPoints
                            : playerChar.CurrentHitPoints,
                        MaxHitPoints =
                            participant.MaxHitPoints > 0 ? participant.MaxHitPoints : playerChar.MaxHitPoints,
                        ArmorClass = participant.ArmorClass > 0 ? participant.ArmorClass : playerChar.ArmorClass,
                        Initiative = participant.Initiative,
                        IsActive = true
                    };
                    break;

                case ParticipantType.Npc:
                    var npc = await _context.NPCs.FindAsync(participant.SourceId);
                    if (npc == null)
                        return BadRequest("NPC не найден");

                    newParticipant = new NpcCombatParticipant
                    {
                        NpcId = npc.Id,
                        Npc = npc,
                        Name = string.IsNullOrEmpty(participant.Name) ? npc.Name : participant.Name,
                        CurrentHitPoints = participant.CurrentHitPoints > 0
                            ? participant.CurrentHitPoints
                            : npc.CurrentHitPoints,
                        MaxHitPoints = participant.MaxHitPoints > 0 ? participant.MaxHitPoints : npc.CurrentHitPoints,
                        ArmorClass = participant.ArmorClass > 0 ? participant.ArmorClass : npc.ArmorClass,
                        Initiative = participant.Initiative,
                        IsActive = true
                    };
                    break;

                case ParticipantType.Enemy:
                    var enemy = await _context.Enemies.FindAsync(participant.SourceId);
                    if (enemy == null)
                        return BadRequest("Враг не найден");

                    newParticipant = new EnemyCombatParticipant
                    {
                        EnemyId = enemy.Id,
                        Enemy = enemy,
                        Name = string.IsNullOrEmpty(participant.Name) ? enemy.Name : participant.Name,
                        CurrentHitPoints = participant.CurrentHitPoints > 0 ? participant.CurrentHitPoints : enemy.CurrentHitPoints,
                        MaxHitPoints = participant.MaxHitPoints > 0 ? participant.MaxHitPoints : enemy.CurrentHitPoints,
                        ArmorClass = participant.ArmorClass > 0 ? participant.ArmorClass : enemy.ArmorClass,
                        Initiative = participant.Initiative,
                        IsActive = true
                    };
                    break;

                default:
                    return BadRequest("Неизвестный тип участника");
            }

            combat.Participants.Add(newParticipant);

            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetCombat), combat.Id, combat);
        }

        private bool CombatExists(Guid id)
        {
            return _context.Combats.Any(e => e.Id == id);
        }
    }
}