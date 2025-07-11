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
            return await _context.Combats.ToListAsync();
        }

        // GET: api/Combat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Combat>> GetCombat(Guid id)
        {
            var combat = await _context.Combats.FindAsync(id);

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

        private bool CombatExists(Guid id)
        {
            return _context.Combats.Any(e => e.Id == id);
        }
    }
}
