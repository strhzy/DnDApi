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
    public class CampaignController : ControllerBase
    {
        private readonly DnDContext _context;

        public CampaignController(DnDContext context)
        {
            _context = context;
        }

        // GET: api/Campaign
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns(Guid userId, Guid masterId)
        {
            if (userId != Guid.Empty)
            {
                return await _context.Campaigns.Where(c => c.PlayerCharacters.Any(ch => ch.UserId == userId)).ToListAsync();
            }
            else if (masterId != Guid.Empty)
            {
                return await _context.Campaigns.Where(c => c.MasterId == masterId).ToListAsync();
            }
            return await _context.Campaigns.Include(c => c.PlayerCharacters).ToListAsync();
        }

        // GET: api/Campaign/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Campaign>> GetCampaign(Guid id)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.PlayerCharacters)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campaign == null)
            {
                return NotFound();
            }

            return campaign;
        }

        // PUT: api/Campaign/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(Guid id, Campaign campaign)
        {
            if (id != campaign.Id)
            {
                return BadRequest();
            }

            _context.Entry(campaign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignExists(id))
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

        // POST: api/Campaign
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Campaign>> PostCampaign(Campaign campaign)
        {
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampaign", new { id = campaign.Id }, campaign);
        }

        // DELETE: api/Campaign/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{campaignId}/add_char/{characterId}")]
        public async Task<ActionResult<Campaign>> AddCharacter(Guid campaignId, Guid characterId)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.PlayerCharacters)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            var character = await _context.PlayerCharacters.FindAsync(characterId);

            if (campaign == null || character == null)
                return NotFound();

            if (campaign.PlayerCharacters.Contains(character))
                return Conflict("Character already in this campaign.");

            campaign.PlayerCharacters.Add(character);

            await _context.SaveChangesAsync();

            return Ok(campaign);
        }

        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaignsByUser(Guid userId)
        {
            return await _context.Campaigns.Where(c => c.PlayerCharacters.Any(ch => ch.UserId == userId)).ToListAsync();
        }
        

        private bool CampaignExists(Guid id)
        {
            return _context.Campaigns.Any(e => e.Id == id);
        }
    }
}
