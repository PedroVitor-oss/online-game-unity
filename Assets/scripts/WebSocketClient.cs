using System;
using UnityEngine;
using System.Net.WebSockets;
using System.Text;
// using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;



public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;
    public bool useLocal = false;
    public string urlGame = "wss://server-game-unity.onrender.com/ws";
    public string urlLocal = "ws://localhost:8080/ws";
    public static WebSocketClient Instance { get; private set; }

    async void Start()
    {
        Instance = this;

        webSocket = new ClientWebSocket();
        try
        {
            if(useLocal)
            {
                await webSocket.ConnectAsync(
                    new System.Uri(urlLocal),
                    CancellationToken.None
                );
            }
            else
            {
                
            await webSocket.ConnectAsync(
                new System.Uri(urlGame),
                CancellationToken.None
            );
            }
            Debug.Log("Conectado ao servidor!");

            // Recebe mensagens
            ReceiveMessages();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro de conexão: " + e.Message);
        }
    }

    async void ReceiveMessages()
    {
        var buffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new System.ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                JObject data = JObject.Parse(message);
                string eventType = data["event"]?.ToString();
                switch (eventType)
                {
                    case "dataPlayer":
                    PlayersManage.Instance.SetStartData(message);
                    break;
                    case "newPlayerConnected":
                    PlayersManage.Instance.AddNewPlayer(message);
                    break;
                    case "playerMoved":
                    PlayersManage.Instance.UpdatePlayerPosition(message);
                    break;
                    case "playerDisconnected":
                    PlayersManage.Instance.PlayerDisconnect(message);
                    break;
                }
                // Debug.Log("Mensagem recebida: " + data);
            }
        }
    }

    public void TesteSend()
    {
        Debug.Log("Teste acesses websoceketclient");
    }

    public async Task SendJsonDataAsync<T>(string eventType, T data)
    {
        
        // 1. Enviar objeto simples
        var usuario = new
        {
            Nome = "João Silva",
            Email = "joao@email.com",
            Idade = 30,
            Ativo = true
        };
        
        // Debug.Log("Enviar dados, sem tyoe event ");
        await SendJsonAsync(webSocket, data,eventType);
        
        // // 2. Enviar lista de produtos
        // var produtos = new
        // {
        //     Action = "produtos:atualizar",
        //     Data = new[]
        //     {
        //         new { Id = 1, Nome = "Notebook", Preco = 2500.99 },
        //         new { Id = 2, Nome = "Mouse", Preco = 89.90 },
        //         new { Id = 3, Nome = "Teclado", Preco = 150.00 }
        //     },
        //     Timestamp = DateTime.Now
        // };
        
        // await SendJsonAsync(ws, produtos);
        
        // 3. Enviar comando específico
        // var comando = new
        // {
        //     Command = "disconnect",
        //     Reason = "Manutenção programada",
        //     Time = DateTime.UtcNow.AddMinutes(5)
        // };
        
        // await SendJsonAsync(ws, comando, "system:command");
    }


    private  async Task SendJsonAsync<T>(
      WebSocket webSocket,
      T data,
      string eventType = null)
    {
        if(webSocket.State != WebSocketState.Open)
        {
            Debug.LogWarning("WebSocket não está conectado.");
            return;
        }
        var message = new
        {
            Event = eventType ?? "message",
            Data = data,
            Timestamp = DateTime.UtcNow,
            Source = "CSharpClient"
        };

        string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        });


        byte[] buffer = Encoding.UTF8.GetBytes(json);
        
        

        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        // Debug.Log($"JSON enviado: {json}");
    }

    void OnDestroy()
    {
        if (webSocket != null)
        {
            webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Fechando",
                CancellationToken.None
            );
        }
    }
}