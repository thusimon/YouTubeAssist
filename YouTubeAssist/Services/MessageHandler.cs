using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace YouTubeAssist.Services
{
    class MessageHandler
    {
        NamedPipeServerStream PipeServerStream;
        CredService credService;
        public MessageHandler(NamedPipeServerStream p) {
            PipeServerStream = p;
            credService = new CredService();
        }

        public void handleMessage(string message)
        {
            if (message != null && message.StartsWith("WebExt::Auth:request"))
            {
                string[] messageParts = message.Split("_");
                string? uuid = null;
                if (messageParts.Length == 2)
                {
                    uuid = messageParts[1];
                }
                handleAuthenticate(uuid);
                return;
            }
        }

        private async void handleAuthenticate(string? uuid)
        {
            bool authResult = await credService.Authenticate();
            if (PipeServerStream != null && PipeServerStream.IsConnected)
            {
                using (var writer = new StreamWriter(PipeServerStream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
                {
                    try
                    {
                        string respMessage = $"WebExt::Auth:{authResult}";
                        if (!string.IsNullOrEmpty(uuid))
                        {
                            respMessage = $"{respMessage}_{uuid}";
                        }
                        writer.WriteLine(respMessage);
                        Debug.WriteLine($"Write to pipeServer {respMessage}");
                    }
                    catch (Exception ex)
                    {
                        authResult = false;
                        writer.WriteLine($"WebExt::Auth:{authResult}");
                        Debug.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            Debug.WriteLine($"Auth Result: {authResult}");
        }
    }
}
