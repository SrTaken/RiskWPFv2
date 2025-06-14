﻿using Model;
using Newtonsoft.Json;
using Stomp.Net.Stomp.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Datos
{
    public class Conection
    {
        public static ClientWebSocket Client;
        public static event Action<string> OnMessageReceived;
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1000);

        private static Task listenTask;
        private static CancellationTokenSource listenCts;

        public static async void startConection()
        {
            string host = ConfigurationManager.AppSettings["serverURl"];
            Client = new ClientWebSocket();
            await Client.ConnectAsync(new Uri(host), CancellationToken.None);
            StartListening();
        }
        public static void StartListening()
        {
            if (listenTask == null || listenTask.IsCompleted)
            {
                listenCts = new CancellationTokenSource();
                listenTask = Task.Run(() => ListenAsync(listenCts.Token));  // FORZAMOS SEGUNDO PLANO
            }
        }

        public static void StopListening()
        {
            if (listenCts != null && !listenCts.IsCancellationRequested)
                listenCts.Cancel();
        }

        private static async Task ListenAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (Client != null && Client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        string msg = await ReceiveMessage();
                        if (msg != null)
                            OnMessageReceived?.Invoke(msg);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error en ListenAsync: " + ex.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fatal Listener error: " + ex);
            }
        }


        public static async Task SendMessage(object message, string consulta)
        {
            string json = JsonConvert.SerializeObject(message);

            json = "{\"request\":\"" + consulta + "\"," + "\"token\":\"" + Utils.user.Token +"\","+json.Substring(1);

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public static async Task SendMessage(string consulta)
        {
            string json;

            json = "{\"request\":\"" + consulta + "\"," + "\"token\":\"" + Utils.user.Token + "\"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task SendMessageUpdateSala(object message, string consulta)
        {
            string json = JsonConvert.SerializeObject(message);

            json = "{\"request\":\"" + consulta + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"sala\":{" + json.Substring(1)+"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task SendMessageSala(object user, string nombreSala)
        {
            string usuarioJson = JsonConvert.SerializeObject(user);

            string json = "{\"request\":\"" + Constants.RQ.CrearSala + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"name\":\"" + nombreSala + "\",\"user\":" + usuarioJson + "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task SendMessage(string consulta, int userid)
        {
            string json;

            json = "{\"request\":\"" + consulta + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"user\":" + userid+"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task<string> ReceiveMessage()
        {
            Debug.WriteLine("Recibiendo mensaje...");
            byte[] buffer = new byte[50000]; 
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

            using (var cts = new CancellationTokenSource(Timeout))
            {
                try
                {
                    Debug.WriteLine("Leyendo....");
                    WebSocketReceiveResult result = await Client.ReceiveAsync(segment, cts.Token);
                    Debug.WriteLine("Leido...");
                    return Encoding.UTF8.GetString(buffer, 0, result.Count);
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static async Task CloseConnection()
        {
            await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", new CancellationTokenSource(Timeout).Token);
            Client.Dispose();
        }

        public static async Task SendMessageJoinLeaveSala(int salaId, int id, bool join)
        {
            string json;
            if (join){
                json = "{\"request\":\"" + Constants.RQ.JoinSala + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"user\":" + id + ",\"sala\":" + salaId + "}";
            }
            else
            {
                json = "{\"request\":\"" + Constants.RQ.SalirSala + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"idUsuari\":" + id + ",\"idSala\":" + salaId + "}";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public async static Task SendSeleccion(string token, string nombre)
        {
            string json;
            string request = Constants.RQ.SeleccionarPais;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + Utils.user.Token + "\",\"pais\":\"" + nombre + "\"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task EnviarRefuerzo(string token, string nombre, int tropasParaPoner, Estat fase)
        {
            string json;
            string request = fase == Estat.REFORC_PAIS ? Constants.RQ.ReforzarPais : Constants.RQ.ReforzarTurno;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token + "\",\"nom\":\"" + nombre+ "\",\"tropas\":\"" + tropasParaPoner + "\"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task FinalizaTurno(string token)
        {
            string json;
            string request = Constants.RQ.SaltarTurno;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token  + "\"}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task AtacarMensaje(string token, string nombre1, string nombre2, int numDadosAtacar)
        {
            string json;
            string request = Constants.RQ.Ataque;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token + "\"," + "\"paisAtacante\":\"" + nombre1 + "\"," + "\"paisDefensor\":\"" + nombre2 + "\"," + "\"numTropas\":" + numDadosAtacar + "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task DefenderMensaje(string token, string paisDefensaActual, int dados, string paisAtacante, int numDadosAtaque)
        {
            string json;
            string request = Constants.RQ.Defensa;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token + "\"," + "\"paisAtacante\":\"" + paisAtacante + "\"," + "\"paisDefensor\":\"" + paisDefensaActual + "\"," + "\"numTropasDefensa\":" + dados + "," + "\"numTropasAtaque\":" + numDadosAtaque+ "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task MoverTropasConquista(string token, string ultimoPaisAtacante, string ultimoPaisConquistado, int numDadosAtacar)
        {
            string json;
            string request = Constants.RQ.Conquista;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token + "\"," + "\"paisOrigen\":\"" + ultimoPaisAtacante + "\"," + "\"paisDestino\":\"" + ultimoPaisConquistado + "\"," + "\"numTropas\":" + numDadosAtacar + "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }

        public static async Task MoverTropasReagrupe(string token, string ultimoPaisAtacante, string ultimoPaisConquistado, int tropasAMover)
        {
            string json;
            string request = Constants.RQ.MoverTropas;

            json = "{\"request\":\"" + request + "\"," + "\"token\":\"" + token + "\"," + "\"paisOrigen\":\"" + ultimoPaisAtacante + "\"," + "\"paisDestino\":\"" + ultimoPaisConquistado + "\"," + "\"numTropas\":" + tropasAMover + "}";

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            await Client.SendAsync(segment, WebSocketMessageType.Text, true, new CancellationTokenSource(Timeout).Token);
        }
    }
}
