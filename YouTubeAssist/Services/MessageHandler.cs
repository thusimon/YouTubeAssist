using CommonLibrary;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using YouTubeAssist.UI;

namespace YouTubeAssist.Services
{
    class MessageHandler
    {
        NamedPipeServerStream PipeServerStream;
        PipeView PipeView;
        CredService credService;
        public MessageHandler(NamedPipeServerStream p, PipeView pv) {
            PipeServerStream = p;
            PipeView = pv;
            credService = new CredService();
        }

        public void handleMessage(Message? message)
        {
            if (message == null)
            {
                Log("null message", false);
                return;
            }
            if (message.type != "HOST_TO_NATIVE")
            {
                Log("message not from host", false);
                return;
            }
            switch (message.action)
            {
                case "MESSAGE":
                    {
                        string messageToDisplay = message.data.GetValueOrDefault("message", "");
                        Log($"MSG From WebExt: {messageToDisplay}", true);
                        break;
                    }
                case "AUTH":
                    {
                        string from = message.data.GetValueOrDefault("from", "");
                        string uuid = message.data.GetValueOrDefault("uuid", "");
                        Log($"AUTH from WebExt: {uuid}", true);
                        handleAuthenticate(from, uuid);
                        break;
                    }
                default:
                    {
                        Log("Unkown message action", false);
                        break;
                    }
            }
        }

        private async void handleAuthenticate(string from, string uuid)
        {
            bool authResult = await credService.Authenticate();
            Log($"AuthResult: {authResult}", true);
            if (PipeServerStream != null && PipeServerStream.IsConnected)
            {
                using (var writer = new StreamWriter(PipeServerStream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
                {
                    try
                    {
                        Message authRespMessage = new Message("NATIVE_TO_HOST", "AUTH", new Dictionary<string, string> {
                            { "result", $"{authResult}"},
                            { "from", $"{from}" },
                            { "uuid", uuid }
                        });
                        string respMessage = IOService.SerializedMessage(authRespMessage);
                        writer.WriteLine(respMessage);
                        Debug.WriteLine($"Write to pipeServer {respMessage}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            Debug.WriteLine($"Auth Result: {authResult}");
        }

        private void Log(string message, bool showInView)
        {
            Debug.WriteLine(message);
            if (showInView)
            {
                PipeView.MessageIncome += $"{message}\n";
            }
        }
    }
}
