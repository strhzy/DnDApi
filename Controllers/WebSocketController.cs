using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Text;

namespace DnDAPI.Controllers
{
    [ApiController]
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _combatClients = new();

        [HttpGet]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var combatId = HttpContext.Request.Query["combatId"].ToString();
                if (string.IsNullOrEmpty(combatId))
                {
                    HttpContext.Response.StatusCode = 400;
                    await HttpContext.Response.WriteAsync("Missing combatId");
                    return;
                }
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var clientId = Guid.NewGuid().ToString();
                var clients = _combatClients.GetOrAdd(combatId, _ => new());
                clients[clientId] = socket;
                await Receive(socket, clientId, combatId);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Receive(WebSocket socket, string clientId, string combatId)
        {
            var buffer = new byte[4096];
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        if (_combatClients.TryGetValue(combatId, out var clients))
                        {
                            clients.TryRemove(clientId, out _);
                            if (clients.IsEmpty) _combatClients.TryRemove(combatId, out _); // Очистка пустых комнат
                        }
                        break;
                    }
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await BroadcastAsync(message, clientId, combatId);
                }
                catch (Exception)
                {
                    // Логируй, если нужно, но не крашь
                    break;
                }
            }
        }

        private async Task BroadcastAsync(string message, string senderId, string combatId)
        {
            if (_combatClients.TryGetValue(combatId, out var clients))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var tasks = new List<Task>();
                foreach (var kvp in clients)
                {
                    if (kvp.Value.State == WebSocketState.Open)
                    {
                        // Здесь: отправляем ВСЕМ, включая отправителя (для sync, если нужно)
                        // Если хочешь исключить отправителя: if (kvp.Key != senderId && ...)
                        tasks.Add(kvp.Value.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
                    }
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}