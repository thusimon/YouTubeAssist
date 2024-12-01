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
            if (string.Equals(message, "WebExt::Auth:request", StringComparison.InvariantCulture))
            {
                handleAuthenticate();
                return;
            }
        }

        private async void handleAuthenticate()
        {
            bool authResult = await credService.Authenticate();
            if (PipeServerStream != null && PipeServerStream.IsConnected)
            {
                using (var writer = new StreamWriter(PipeServerStream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
                {
                    try
                    {
                        writer.WriteLine($"WebExt::Auth:{authResult}");
                        Debug.WriteLine($"Write to pipeServer {authResult}");
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
