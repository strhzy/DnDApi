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
        
        // GET: api/PlayerCharacter/{id}/pdf
        [HttpGet("{id}/pdf")]
        public async Task<string> GetCharacterPdf(Guid id)
        {
            var character = await _context.PlayerCharacters
                .Include(pc => pc.Attacks)
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (character == null)
            {
                return null;
            }

            string html = "123";

            string template = @"
                @model DnDAPI.Models.PlayerCharacter
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <title>@Model.Name - Character Sheet</title>
                    <style>
                        body {
                            font-family: 'Garamond', serif;
                            font-size: 12px;
                            margin: 20px;
                        }
                        .sheet {
                            display: grid;
                            grid-template-columns: 25% 25% 50%;
                            grid-gap: 10px;
                        }
                        .section {
                            border: 2px solid black;
                            padding: 5px 10px;
                            border-radius: 8px;
                        }
                        .title {
                            font-weight: bold;
                            text-transform: uppercase;
                            border-bottom: 1px solid black;
                            margin-bottom: 5px;
                        }
                        .row {
                            display: flex;
                            justify-content: space-between;
                        }
                        .ability-block {
                            border: 1px solid black;
                            text-align: center;
                            margin-bottom: 10px;
                        }
                        .ability-block .label {
                            font-weight: bold;
                        }
                        .skill-list {
                            margin-left: 10px;
                        }
                        .attack-list, .equipment-list {
                            list-style: none;
                            padding-left: 0;
                        }
                        .attack-list li, .equipment-list li {
                            border-bottom: 1px dotted #333;
                            padding: 2px 0;
                        }
                    </style>
                </head>
                <body>
                    <h1>Dungeons & Dragons Character Sheet</h1>
                    <div class=""sheet"">
                        <!-- Left Column -->
                        <div>
                            <div class=""section"">
                                <div class=""title"">Character Name</div>
                                @Model.Name
                            </div>
                            @foreach (var ability in new[] { ""Strength"", ""Dexterity"", ""Constitution"", ""Intelligence"", ""Wisdom"", ""Charisma"" }) {
                                var score = (int)Model.GetType().GetProperty(ability).GetValue(Model)!;
                                <div class=""ability-block"">
                                    <div class=""label"">@ability</div>
                                    <div>@score</div>
                                    <div>(@((score - 10) / 2 >= 0 ? ""+"" : """")@((score - 10) / 2))</div>
                                </div>
                            }
                            <div class=""section"">
                                <div class=""title"">Saving Throws</div>
                                <ul class=""skill-list"">
                                    <li>Strength: @Model.SavingThrowStrength</li>
                                    <li>Dexterity: @Model.SavingThrowDexterity</li>
                                    <li>Constitution: @Model.SavingThrowConstitution</li>
                                    <li>Intelligence: @Model.SavingThrowIntelligence</li>
                                    <li>Wisdom: @Model.SavingThrowWisdom</li>
                                    <li>Charisma: @Model.SavingThrowCharisma</li>
                                </ul>
                            </div>
                            <div class=""section"">
                                <div class=""title"">Skills</div>
                                <ul class=""skill-list"">
                                    <li>Acrobatics: @Model.Acrobatics</li>
                                    <li>Animal Handling: @Model.AnimalHandling</li>
                                    <li>Arcana: @Model.Arcana</li>
                                    <li>Athletics: @Model.Athletics</li>
                                    <li>Deception: @Model.Deception</li>
                                    <li>History: @Model.History</li>
                                    <li>Insight: @Model.Insight</li>
                                    <li>Intimidation: @Model.Intimidation</li>
                                    <li>Investigation: @Model.Investigation</li>
                                    <li>Medicine: @Model.Medicine</li>
                                    <li>Nature: @Model.Nature</li>
                                    <li>Perception: @Model.Perception</li>
                                    <li>Performance: @Model.Performance</li>
                                    <li>Persuasion: @Model.Persuasion</li>
                                    <li>Religion: @Model.Religion</li>
                                    <li>Sleight of Hand: @Model.SleightOfHand</li>
                                    <li>Stealth: @Model.Stealth</li>
                                    <li>Survival: @Model.Survival</li>
                                </ul>
                            </div>
                        </div>

                        <!-- Center Column -->
                        <div>
                            <div class=""section"">
                                <div class=""title"">Class & Level</div>
                                @Model.ClassType, Level @Model.Level
                            </div>
                            <div class=""section"">
                                <div class=""title"">Inspiration / Proficiency</div>
                                Inspiration: @(Model.Inspiration ? ""✓"" : ""✗"")<br />
                                Proficiency Bonus: +@Model.ProficiencyBonus
                            </div>
                            <div class=""section"">
                                <div class=""title"">Combat</div>
                                <div>Armor Class: @Model.ArmorClass</div>
                                <div>Initiative: @Model.Initiative</div>
                                <div>Speed: @Model.Speed</div>
                                <div>HP: @Model.CurrentHitPoints / @Model.MaxHitPoints (+@Model.TemporaryHitPoints temp)</div>
                                <div>Hit Dice: @Model.HitDice</div>
                                <div>Death Saves: @Model.DeathSaveSuccesses ✓ / @Model.DeathSaveFailures ✗</div>
                            </div>
                            <div class=""section"">
                                <div class=""title"">Attacks & Spellcasting</div>
                                <ul class=""attack-list"">
                                    @foreach (var atk in Model.Attacks)
                                    {
                                        <li><strong>@atk.Name</strong> — +@atk.AttackBonus to hit, @atk.DamageDice<br />@atk.Description</li>
                                    }
                                </ul>
                            </div>
                        </div>

                        <!-- Right Column -->
                        <div>
                            <div class=""section"">
                                <div class=""title"">Personality Traits</div>
                                @Model.PersonalityTraits
                            </div>
                            <div class=""section"">
                                <div class=""title"">Ideals</div>
                                @Model.Ideals
                            </div>
                            <div class=""section"">
                                <div class=""title"">Bonds</div>
                                @Model.Bonds
                            </div>
                            <div class=""section"">
                                <div class=""title"">Flaws</div>
                                @Model.Flaws
                            </div>
                            <div class=""section"">
                                <div class=""title"">Equipment</div>
                                <ul class=""equipment-list"">
                                    @foreach (var item in Model.Equipment.Split('\n'))
                                    {
                                        if (!string.IsNullOrWhiteSpace(item))
                                        {
                                            <li>@item</li>
                                        }
                                    }
                                </ul>
                            </div>
                            <div class=""section"">
                                <div class=""title"">Features & Traits</div>
                                @Model.FeaturesAndTraits
                            </div>
                            <div class=""section"">
                                <div class=""title"">Languages & Proficiencies</div>
                                @Model.ProficienciesAndLanguages
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Program)) // или UseMemoryCaching()
                .Build();
            
            string result = await engine.CompileRenderStringAsync("templateKey", template, character);
    
            //return File(result, "application/pdf", $"{character.Name}.pdf");
            return result;
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
