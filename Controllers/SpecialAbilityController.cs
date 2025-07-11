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
    public class SpecialAbilityController : ControllerBase
    {
        private readonly DnDContext _context;

        public SpecialAbilityController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/SpecialAbility
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialAbility>>> GetSpecialAbilities()
        {
            return await _context.SpecialAbilities.ToListAsync();
        }

        // GET: api/SpecialAbility/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialAbility>> GetSpecialAbility(Guid id)
        {
            var specialAbility = await _context.SpecialAbilities.FindAsync(id);

            if (specialAbility == null)
            {
                return NotFound();
            }

            return specialAbility;
        }

        // PUT: api/SpecialAbility/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialAbility(Guid id, SpecialAbility specialAbility)
        {
            if (id != specialAbility.Id)
            {
                return BadRequest();
            }

            _context.Entry(specialAbility).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialAbilityExists(id))
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

        // POST: api/SpecialAbility
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SpecialAbility>> PostSpecialAbility(SpecialAbility specialAbility)
        {
            _context.SpecialAbilities.Add(specialAbility);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecialAbility", new { id = specialAbility.Id }, specialAbility);
        }

        // DELETE: api/SpecialAbility/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialAbility(Guid id)
        {
            var specialAbility = await _context.SpecialAbilities.FindAsync(id);
            if (specialAbility == null)
            {
                return NotFound();
            }

            _context.SpecialAbilities.Remove(specialAbility);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecialAbilityExists(Guid id)
        {
            return _context.SpecialAbilities.Any(e => e.Id == id);
        }
    }
}
