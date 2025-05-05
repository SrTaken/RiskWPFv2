using Newtonsoft.Json;
using Stomp.Net.Stomp.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class Conection
    {
        public static ClientWebSocket Client; 
        public static event Action<string> OnMessageReceived; // Un evento

        public async static void startConection()
        {
            string host = ConfigurationManager.AppSettings["serverURl"];
            Client = new ClientWebSocket();
            await Client.ConnectAsync(new Uri(host), CancellationToken.None);
            _ = ListenAsync();
        }

        public static async Task ListenAsync()
        {
            while (Client != null && Client.State == WebSocketState.Open)
            {
                try
                {
                    string msg = await ReceiveMessage();
                    OnMessageReceived?.Invoke(msg); 
                }
                catch (Exception ex)
                {
                    break; 
                }
            }
        }

        public static async Task SendMessage(object message, string consulta)
        {
            string json = JsonConvert.SerializeObject(message);

            json = "{\"request\":\"" + consulta + "\"," + json.Substring(1);

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task SendMessage(string consulta)
        {
            string json;

            json = "{\"request\":\"" + consulta + "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task<string> ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = await Client.ReceiveAsync(segment, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public static async Task CloseConnection()
        {
            await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Client.Dispose();
        }
    }
}
