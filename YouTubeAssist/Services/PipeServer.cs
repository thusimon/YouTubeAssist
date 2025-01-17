﻿using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Diagnostics;
using YouTubeAssist.UI;
using CommonLibrary;

namespace YouTubeAssist.Services
{
    class PipeServer
    {
        public string messageIncome = "";
        public string messageOutcome = "";
        public PipeView pipeView;
        NamedPipeServerStream? pipeServer = null;
        MessageHandler? messageHandler;
        public PipeServer(PipeView pv)
        {
            pipeView = pv;
            Log("*** Named pipe server stream with impersonation example ***");
            Log("Waiting for client connect...");
            startPipeServer();
        }

        private void startPipeServer()
        {
            Task.Run(async () =>
            {
                try
                {
                    // Create the named pipe
                    using (pipeServer = new NamedPipeServerStream("com.utticus.youtube.assist", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                    {
                        await pipeServer.WaitForConnectionAsync(); // Block until a client connects

                        Log($"Client connected on thread [{Thread.CurrentThread.ManagedThreadId}] as user {pipeServer.GetImpersonationUserName()}");

                        messageHandler = new MessageHandler(pipeServer, pipeView);

                        // Read and process messages
                        // Start reading and processing messages
                        using (var reader = new StreamReader(pipeServer, Encoding.UTF8, leaveOpen: true))
                        {
                            while (pipeServer.IsConnected)
                            {
                                string message = reader.ReadLine();
                                if (!string.IsNullOrEmpty(message))
                                {
                                    Log($"{message}", false);
                                    Message? messageObj = IOService.DeserializedMessage(message);
                                    messageHandler.handleMessage(messageObj);
                                }
                            }
                            Log("pipeServer is disconnected");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Pipe server error: {ex.Message}");
                }
            });

        }

        public void sendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Log("can not send empty message");
                return;
            }

            if (pipeServer == null || !pipeServer.IsConnected)
            {
                Log("pipeServer not connected, can not send message");
                return;
            }

            using (var writer = new StreamWriter(pipeServer, Encoding.UTF8, leaveOpen: true) { AutoFlush = true})
            {
                try
                {
                    Message messageObj = new Message("NATIVE_TO_HOST", "MESSAGE", new Dictionary<string, string> { { "message", message } });
                    string messageToPipe = IOService.SerializedMessage(messageObj);
                    writer.WriteLine(messageToPipe);
                    Debug.WriteLine($"Write to pipeServer {messageToPipe}");
                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Message}");
                }
            }
        }
        
        private void Log(string message, bool showInView)
        {
            Debug.WriteLine(message);
            if (showInView)
            {
                pipeView.MessageIncome += $"{message}\n";
            }
        }

        private void Log(string message)
        {
            Log(message, true);
        }
    }
}
