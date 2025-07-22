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
    public class StoryElementController : ControllerBase
    {
        private readonly DnDContext _context;

        public StoryElementController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/StoryElement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryElement>>> GetStoryElements()
        {
            return await _context.StoryElements.Include(s => s.Campaign).ToListAsync();
        }

        // GET: api/StoryElement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryElement>> GetStoryElement(Guid id)
        {
            var storyElement = await _context.StoryElements.Include(s => s.Campaign).FirstOrDefaultAsync(s => s.Id == id);

            if (storyElement == null)
            {
                return NotFound();
            }

            return storyElement;
        }

        // PUT: api/StoryElement/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStoryElement(Guid id, StoryElement storyElement)
        {
            if (id != storyElement.Id)
            {
                return BadRequest();
            }

            _context.Entry(storyElement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoryElementExists(id))
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

        // POST: api/StoryElement
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StoryElement>> PostStoryElement(StoryElement storyElement)
        {
            _context.StoryElements.Add(storyElement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStoryElement", new { id = storyElement.Id }, storyElement);
        }

        // DELETE: api/StoryElement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoryElement(Guid id)
        {
            var storyElement = await _context.StoryElements.FindAsync(id);
            if (storyElement == null)
            {
                return NotFound();
            }

            _context.StoryElements.Remove(storyElement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoryElementExists(Guid id)
        {
            return _context.StoryElements.Any(e => e.Id == id);
        }
    }
}
