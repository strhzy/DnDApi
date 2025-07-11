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
    public class AttackController : ControllerBase
    {
        private readonly DnDContext _context;

        public AttackController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/Attack
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attack>>> GetAttacks()
        {
            return await _context.Attacks.ToListAsync();
        }

        // GET: api/Attack/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attack>> GetAttack(Guid id)
        {
            var attack = await _context.Attacks.FindAsync(id);

            if (attack == null)
            {
                return NotFound();
            }

            return attack;
        }

        // PUT: api/Attack/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttack(Guid id, Attack attack)
        {
            if (id != attack.Id)
            {
                return BadRequest();
            }

            _context.Entry(attack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttackExists(id))
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

        // POST: api/Attack
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Attack>> PostAttack(Attack attack)
        {
            _context.Attacks.Add(attack);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AttackExists(attack.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAttack", new { id = attack.Id }, attack);
        }

        // DELETE: api/Attack/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttack(Guid id)
        {
            var attack = await _context.Attacks.FindAsync(id);
            if (attack == null)
            {
                return NotFound();
            }

            _context.Attacks.Remove(attack);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttackExists(Guid id)
        {
            return _context.Attacks.Any(e => e.Id == id);
        }
    }
}
