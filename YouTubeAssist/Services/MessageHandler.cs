using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            switch (message)
            {
                case "WebExt::Auth:request":
                    handleAuthenticate();
                    break;
                default:
                    break;
            }
        }

        private async void handleAuthenticate()
        {
            bool authResult = await credService.Authenticate();
            Debug.WriteLine($"Auth Result: {authResult}");
        }
    }
}
