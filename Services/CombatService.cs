using DnDAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DnDAPI.Services;

public class CombatService
{
    private readonly DnDContext _context;

    public CombatService(DnDContext context)
    {
        _context = context;
    }

    public async Task SyncParticipantWithSource(Guid participantId)
    {
        var participant = await _context.CombatParticipants
            .Include(p => p as PlayerCombatParticipant).ThenInclude(p => p.PlayerCharacter)
            .Include(p => p as NpcCombatParticipant).ThenInclude(p => p.Npc)
            .Include(p => p as EnemyCombatParticipant).ThenInclude(p => p.Enemy)
            .FirstOrDefaultAsync(p => p.Id == participantId);

        participant?.SyncWithSource(_context);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateParticipantStats(Guid participantId, Action<CombatParticipant> updateAction)
    {
        var participant = await _context.CombatParticipants.FindAsync(participantId);
        if (participant != null)
        {
            updateAction(participant);
            participant.SyncWithSource(_context);
            await _context.SaveChangesAsync();
        }
    }
}