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
    public class CombatParticipantController : ControllerBase
    {
        private readonly DnDContext _context;

        public CombatParticipantController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/CombatParticipant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CombatParticipant>>> GetCombatParticipants()
        {
            return await _context.CombatParticipants.ToListAsync();
        }

        // GET: api/CombatParticipant/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CombatParticipant>> GetCombatParticipant(Guid id)
        {
            var combatParticipant = await _context.CombatParticipants.FindAsync(id);

            if (combatParticipant == null)
            {
                return NotFound();
            }

            return combatParticipant;
        }

        // PUT: api/CombatParticipant/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombatParticipant(Guid id, CombatParticipant combatParticipant)
        {
            if (id != combatParticipant.Id)
            {
                return BadRequest();
            }

            _context.Entry(combatParticipant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CombatParticipantExists(id))
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

        // POST: api/CombatParticipant
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CombatParticipant>> PostCombatParticipant(CombatParticipant combatParticipant)
        {
            _context.CombatParticipants.Add(combatParticipant);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CombatParticipantExists(combatParticipant.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCombatParticipant", new { id = combatParticipant.Id }, combatParticipant);
        }

        // DELETE: api/CombatParticipant/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombatParticipant(Guid id)
        {
            var combatParticipant = await _context.CombatParticipants.FindAsync(id);
            if (combatParticipant == null)
            {
                return NotFound();
            }

            _context.CombatParticipants.Remove(combatParticipant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CombatParticipantExists(Guid id)
        {
            return _context.CombatParticipants.Any(e => e.Id == id);
        }
    }
}
