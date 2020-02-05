using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketAndNetCore.Web
{
    public class SquareService
    {
        public Dictionary<string, WebSocket> users = new Dictionary<string, WebSocket>();

        public async Task AddUser(WebSocket socket)
        {
            var name = GenerateName();
            users.Add(name, socket);
        }

        private string GenerateName()
        {
            var prefix = "WebUser";
            Random ran = new Random();
            var name = prefix + ran.Next(1, 1000);
            while (users.ContainsKey(name))
            {
                name = prefix + ran.Next(1, 1000);
            }
            return name;
        }

        private async Task SendAll(string message)
        {
            foreach (var userItem in users)
            {
                var stringAsBytes  = System.Text.Encoding.UTF8.GetBytes(message);
                var byteArraySegment = new ArraySegment<byte>(stringAsBytes,0,stringAsBytes.Length);
                await userItem.Value.SendAsync(byteArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
