using Newtonsoft.Json;
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
        private Dictionary<string, WebSocket> _users = new Dictionary<string, WebSocket>();
        private List<Square> _squares = new List<Square>(Square.GetInitialSquares());
        public async Task AddUser(WebSocket socket)
        {
            var name = GenerateName();
            _users.Add(name, socket);
            await GiveUserTheirName(name, socket);
            await AnnounceNewUser(name);
            await SendSquares(socket);
            while (socket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var bufferAsString = System.Text.Encoding.ASCII.GetString(buffer);
                var changeRequest = SquareChangeRequest.FromJson(bufferAsString);
                await HandleSquareChangeRequest(changeRequest);
            }
        }

        private string GenerateName()
        {
            var prefix = "WebUser";
            Random ran = new Random();
            var name = prefix + ran.Next(1, 1000);
            while (_users.ContainsKey(name))
            {
                name = prefix + ran.Next(1, 1000);
            }
            return name;
        }

        private async Task SendSquareChangeToAll(SquareChangeRequest request)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "change",
                Payload = $"{request.Id},{request.Color}"
            }.ToJson();

            await SendAll(message);
        }

        private async Task SendSquares(WebSocket socket)
        {
            var message = new SocketMessage<List<Square>>()
            {
                MessageType = "squares",
                Payload = _squares
            };

            await Send(message.ToJson(), socket);
        }

        private async Task SendAll(string message)
        {
            await Send(message, _users.Values.ToArray());
        }

        private async Task Send(string message, params WebSocket[] socketToSendTo)
        {
            foreach (var theSocket in socketToSendTo)
            {
                var stringAsBytes = System.Text.Encoding.ASCII.GetBytes(message);
                var byteArraySegment = new ArraySegment<byte>(stringAsBytes, 0, stringAsBytes.Length);
                await theSocket.SendAsync(byteArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task GiveUserTheirName(string name, WebSocket socket)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "name",
                Payload = name
            };
            await Send(message.ToJson(), socket);
        }

        private async Task AnnounceNewUser(string name)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "announce",
                Payload = $"{name} has joined"
            };
            await SendAll(message.ToJson());
        }

        private async Task AnnounceSquareChange(SquareChangeRequest request)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "announce",
                Payload = $"{request.Name} has changed square #{request.Id} to {request.Color}"
            };
            await SendAll(message.ToJson());
        }

        private async Task HandleSquareChangeRequest(SquareChangeRequest request)
        {
            var theSquare = _squares.First(sq => sq.Id == request.Id);
            theSquare.Color = request.Color;
            await SendSquareChangeToAll(request);
        }
    }
}
