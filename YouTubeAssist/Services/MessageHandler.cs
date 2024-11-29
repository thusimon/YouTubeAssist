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
        public MessageHandler(NamedPipeServerStream p) {
            PipeServerStream = p;
        }

        public void handleMessage(string message)
        {
            switch (message)
            {
                case "WebExt::Auth":
                    handleAuthenticate();
                    break;
                default:
                    break;
            }
        }

        private void handleAuthenticate()
        {
            bool authResult = CredService.Authenticate();
            Debug.WriteLine($"Auth Result: {authResult}");
        }
    }
}
