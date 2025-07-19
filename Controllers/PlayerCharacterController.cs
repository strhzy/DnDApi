using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DnDAPI.Models;
using Microsoft.IdentityModel.Tokens;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using RazorLight;

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
        public async Task<ActionResult<IEnumerable<PlayerCharacter>>> GetPlayerCharacters([FromQuery] string? query)
        {
            var characters = _context.PlayerCharacters.AsQueryable();
            if (!query.IsNullOrEmpty())
            {
                characters = characters.Where(p => p.UserId.ToString().Contains(query));
            }

            return await characters.Include(pc => pc.Attacks).ToListAsync();
        }

        // GET: api/PlayerCharacter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerCharacter>> GetPlayerCharacter(Guid id)
        {
            var playerCharacter = await _context.PlayerCharacters
                .Include(pc => pc.Attacks)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (playerCharacter == null)
            {
                return NotFound();
            }

            return playerCharacter;
        }
        
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetCharacterPdf(Guid id)
        {
            try
            {
                var character = await _context.PlayerCharacters
                    .Include(pc => pc.Attacks)
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (character == null)
                {
                    return NotFound("Character not found");
                }

                // 1. Генерация HTML из Razor шаблона
                var templatePath = Path.Combine("Templates", "CharacterSheetTemplate.cshtml");
                var engine = new RazorLightEngineBuilder()
                    .UseFileSystemProject(Directory.GetCurrentDirectory())
                    .Build();
    
                var html = await engine.CompileRenderAsync(templatePath, character);

                // 2. Конвертация HTML в PDF с помощью DinkToPdf
                var converter = new BasicConverter(new PdfTools());
        
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings() { 
                            Top = 10, 
                            Right = 10, 
                            Bottom = 10, 
                            Left = 10 
                        }
                    },
                    Objects = {
                        new ObjectSettings() {
                            HtmlContent = html,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        }
                    }
                };

                var pdfBytes = converter.Convert(doc);

                // 3. Возврат PDF как файла
                return File(pdfBytes, "application/pdf", $"{character.Name}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
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
