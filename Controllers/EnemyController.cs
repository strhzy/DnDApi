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
    public class EnemyController : ControllerBase
    {
        private readonly DnDContext _context;

        public EnemyController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/Enemy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enemy>>> GetEnemies()
        {
            return await _context.Enemies.ToListAsync();
        }

        // GET: api/Enemy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enemy>> GetEnemy(Guid id)
        {
            var enemy = await _context.Enemies.FindAsync(id);

            if (enemy == null)
            {
                return NotFound();
            }

            return enemy;
        }

        // PUT: api/Enemy/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnemy(Guid id, Enemy enemy)
        {
            if (id != enemy.Id)
            {
                return BadRequest();
            }

            _context.Entry(enemy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnemyExists(id))
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

        // POST: api/Enemy
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Enemy>> PostEnemy(Enemy enemy)
        {
            _context.Enemies.Add(enemy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnemy", new { id = enemy.Id }, enemy);
        }

        // DELETE: api/Enemy/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnemy(Guid id)
        {
            var enemy = await _context.Enemies.FindAsync(id);
            if (enemy == null)
            {
                return NotFound();
            }

            _context.Enemies.Remove(enemy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnemyExists(Guid id)
        {
            return _context.Enemies.Any(e => e.Id == id);
        }
    }
}
