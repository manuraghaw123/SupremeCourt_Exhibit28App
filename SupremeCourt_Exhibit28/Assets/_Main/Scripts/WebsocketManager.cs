using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WebsocketManager : MonoBehaviour
{
    [SerializeField] private string websocketUrl = "ws://127.0.0.1:8765";

    private WebSocket websocket;
    private bool isReconnecting = false;

    async void Start()
    {
        await InitializeWebSocket();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(new byte[] { 10, 20, 30 });
            await websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
        await websocket.Close();
    }

    async System.Threading.Tasks.Task InitializeWebSocket()
    {
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            isReconnecting = false;
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.LogWarning("Connection closed! Attempting to reconnect...");
            if (!isReconnecting)
            {
                isReconnecting = true;
                StartCoroutine(TryReconnect());
            }
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            bool isEnglish = message == "English";
            ConfigManager.instance.LanguageSelection(isEnglish);
            
        };

        await websocket.Connect();
    }

    private IEnumerator TryReconnect()
    {
        while (isReconnecting)
        {
            yield return new WaitForSeconds(5f);
            var connectTask = InitializeWebSocket();
            while (!connectTask.IsCompleted) yield return null;

            if (websocket.State == WebSocketState.Open)
            {
                isReconnecting = false;
            }
        }
    }



}
