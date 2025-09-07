using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace DnDAPI.Hubs
{
    public class CombatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, CombatRoom> _combatRooms = new();

        public override async Task OnConnectedAsync()
        {
            var combatId = Context.GetHttpContext()?.Request.Query["combatId"].ToString();
            
            if (string.IsNullOrEmpty(combatId))
            {
                Context.Abort();
                return;
            }

            var room = _combatRooms.GetOrAdd(combatId, id => new CombatRoom(id));
            room.Clients[Context.ConnectionId] = Context;

            await Groups.AddToGroupAsync(Context.ConnectionId, combatId);
            
            await base.OnConnectedAsync();
            
            Console.WriteLine($"Client {Context.ConnectionId} connected to combat {combatId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var combatId = GetCombatIdForConnection(Context.ConnectionId);
            
            if (!string.IsNullOrEmpty(combatId) && _combatRooms.TryGetValue(combatId, out var room))
            {
                room.Clients.TryRemove(Context.ConnectionId, out _);

                if (room.Clients.IsEmpty)
                {
                    _combatRooms.TryRemove(combatId, out _);
                }
            }

            await base.OnDisconnectedAsync(exception);
            
            Console.WriteLine($"Client {Context.ConnectionId} disconnected");
        }

        public async Task SendToCombat(string message)
        {
            var combatId = GetCombatIdForConnection(Context.ConnectionId);
            
            if (!string.IsNullOrEmpty(combatId))
            {
                await Clients.OthersInGroup(combatId).SendAsync("ReceiveMessage", message);
            }
        }
        
        public async Task BroadcastPlayerMove(object moveData)
        {
            var combatId = GetCombatIdForConnection(Context.ConnectionId);
            
            if (!string.IsNullOrEmpty(combatId))
            {
                await Clients.Group(combatId).SendAsync("ReceivePlayerMove", moveData);
            }
        }
        
        public async Task ConfirmMasterAction(object combatData, object logData)
        {
            var combatId = GetCombatIdForConnection(Context.ConnectionId);
            
            if (!string.IsNullOrEmpty(combatId))
            {
                await Clients.Group(combatId).SendAsync("ReceiveMasterConfirm", combatData, logData);
            }
        }

        public async Task SendToClient(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", message);
        }

        private string GetCombatIdForConnection(string connectionId)
        {
            foreach (var room in _combatRooms.Values)
            {
                if (room.Clients.ContainsKey(connectionId))
                {
                    return room.Id;
                }
            }
            return null;
        }
    }

    public class CombatRoom
    {
        public string Id { get; }
        public ConcurrentDictionary<string, HubCallerContext> Clients { get; }

        public CombatRoom(string id)
        {
            Id = id;
            Clients = new ConcurrentDictionary<string, HubCallerContext>();
        }
    }
}