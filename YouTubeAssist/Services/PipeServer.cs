using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Diagnostics;
using YouTubeAssist.UI;

namespace YouTubeAssist.Services
{
    class PipeServer
    {
        public string messageIncome = "";
        public string messageOutcome = "";
        public PipeView pipeView;
        NamedPipeServerStream pipeServer = null;
        public PipeServer(PipeView pv)
        {
            Debug.WriteLine("\n*** Named pipe server stream with impersonation example ***\n");
            Debug.WriteLine("Waiting for client connect...\n");

            pipeView = pv;
            pipeView.MessageIncome += "\n*** Named pipe server stream with impersonation example ***\n";
            pipeView.MessageIncome += "\nWaiting for client connect...\n";

            startPipeServer();
        }

        private void startPipeServer()
        {
            Task.Run(async () =>
            {
                try
                {
                    // Create the named pipe
                    using (pipeServer = new NamedPipeServerStream("com.utticus.youtube.assist", PipeDirection.InOut, 1, PipeTransmissionMode.Message))
                    {
                        await pipeServer.WaitForConnectionAsync(); // Block until a client connects

                        string connectLog = $"\nClient connected on thread [{Thread.CurrentThread.ManagedThreadId}] as user {pipeServer.GetImpersonationUserName()}\n";
                        Debug.WriteLine(connectLog);
                        pipeView.MessageIncome += connectLog;

                        // Read and process messages
                        // Start reading and processing messages
                        using (var reader = new StreamReader(pipeServer, Encoding.UTF8, leaveOpen: true))
                        {
                            while (pipeServer.IsConnected)
                            {
                                string message = reader.ReadLine();
                                if (!string.IsNullOrEmpty(message))
                                {
                                    string messageLog = $"Received from WebExt: {message}\n";
                                    Debug.WriteLine(messageLog);

                                    pipeView.MessageIncome += messageLog;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Pipe server error: {ex.Message}");
                }
            });

        }

        public void sendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.WriteLine("empty message to send, exit");
                return;
            }

            if (pipeServer == null || !pipeServer.IsConnected)
            {
                Debug.WriteLine("pipeServer not connected, exit");
                return;
            }

            using (var writer = new StreamWriter(pipeServer, Encoding.UTF8, leaveOpen: true) { AutoFlush = true})
            {
                try
                {
                    writer.WriteLine(message);
                    Debug.WriteLine($"Write to pipeServer {message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }
        } 
    }
}
