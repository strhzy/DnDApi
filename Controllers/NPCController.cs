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
    public class NPCController : ControllerBase
    {
        private readonly DnDContext _context;

        public NPCController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/NPC
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NPC>>> GetNPCs()
        {
            return await _context.NPCs.ToListAsync();
        }

        // GET: api/NPC/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NPC>> GetNPC(Guid id)
        {
            var nPC = await _context.NPCs.FindAsync(id);

            if (nPC == null)
            {
                return NotFound();
            }

            return nPC;
        }

        // PUT: api/NPC/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNPC(Guid id, NPC nPC)
        {
            if (id != nPC.Id)
            {
                return BadRequest();
            }

            _context.Entry(nPC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NPCExists(id))
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

        // POST: api/NPC
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NPC>> PostNPC(NPC nPC)
        {
            _context.NPCs.Add(nPC);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNPC", new { id = nPC.Id }, nPC);
        }

        // DELETE: api/NPC/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNPC(Guid id)
        {
            var nPC = await _context.NPCs.FindAsync(id);
            if (nPC == null)
            {
                return NotFound();
            }

            _context.NPCs.Remove(nPC);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NPCExists(Guid id)
        {
            return _context.NPCs.Any(e => e.Id == id);
        }
    }
}
