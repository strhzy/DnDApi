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
    public class CombatLogController : ControllerBase
    {
        private readonly DnDContext _context;

        public CombatLogController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/CombatLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CombatLog>>> GetCombatLogs( [FromQuery] Guid combatId)
        {
            if (combatId != null)
            {
                return await _context.CombatLog.Where(cl => cl.CombatId == combatId).ToListAsync();
            }
            return await _context.CombatLog.ToListAsync();
        }

        // GET: api/CombatLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CombatLog>> GetCombatLog(Guid id)
        {
            var combatLog = await _context.CombatLog.FindAsync(id);

            if (combatLog == null)
            {
                return NotFound();
            }

            return combatLog;
        }

        // PUT: api/CombatLog/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombatLog(Guid id, CombatLog combatLog)
        {
            if (id != combatLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(combatLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CombatLogExists(id))
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

        // POST: api/CombatLog
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CombatLog>> PostCombatLog(CombatLog combatLog)
        {
            _context.CombatLog.Add(combatLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCombatLog", new { id = combatLog.Id }, combatLog);
        }

        // DELETE: api/CombatLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombatLog(Guid id)
        {
            var combatLog = await _context.CombatLog.FindAsync(id);
            if (combatLog == null)
            {
                return NotFound();
            }

            _context.CombatLog.Remove(combatLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CombatLogExists(Guid id)
        {
            return _context.CombatLog.Any(e => e.Id == id);
        }
    }
}
