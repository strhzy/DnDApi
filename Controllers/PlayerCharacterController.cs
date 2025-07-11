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
    public class PlayerCharacterController : ControllerBase
    {
        private readonly DnDContext _context;

        public PlayerCharacterController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/PlayerCharacter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerCharacter>>> GetPlayerCharacters()
        {
            return await _context.PlayerCharacters.ToListAsync();
        }

        // GET: api/PlayerCharacter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerCharacter>> GetPlayerCharacter(Guid id)
        {
            var playerCharacter = await _context.PlayerCharacters.FindAsync(id);

            if (playerCharacter == null)
            {
                return NotFound();
            }

            return playerCharacter;
        }

        // PUT: api/PlayerCharacter/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerCharacter(Guid id, PlayerCharacter playerCharacter)
        {
            if (id != playerCharacter.Id)
            {
                return BadRequest();
            }

            _context.Entry(playerCharacter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerCharacterExists(id))
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

        // POST: api/PlayerCharacter
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerCharacter>> PostPlayerCharacter(PlayerCharacter playerCharacter)
        {
            _context.PlayerCharacters.Add(playerCharacter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerCharacter", new { id = playerCharacter.Id }, playerCharacter);
        }

        // DELETE: api/PlayerCharacter/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerCharacter(Guid id)
        {
            var playerCharacter = await _context.PlayerCharacters.FindAsync(id);
            if (playerCharacter == null)
            {
                return NotFound();
            }

            _context.PlayerCharacters.Remove(playerCharacter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerCharacterExists(Guid id)
        {
            return _context.PlayerCharacters.Any(e => e.Id == id);
        }
    }
}
