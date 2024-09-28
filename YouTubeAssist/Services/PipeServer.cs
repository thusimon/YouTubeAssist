using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace YouTubeAssist.Services
{
    class PipeServer
    {
        private static int numThreads = 1;

        public PipeServer()
        {
            int i;
            Thread?[] servers = new Thread[numThreads];

            Debug.WriteLine("\n*** Named pipe server stream with impersonation example ***\n");
            Debug.WriteLine("Waiting for client connect...\n");
            for (i = 0; i < numThreads; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i]?.Start();
            }
            Thread.Sleep(250);
            //while (i > 0)
            //{
            //    for (int j = 0; j < numThreads; j++)
            //    {
            //        if (servers[j] != null)
            //        {
            //            if (servers[j]!.Join(250))
            //            {
            //                Console.WriteLine("Server thread[{0}] finished.", servers[j]!.ManagedThreadId);
            //                servers[j] = null;
            //                i--;    // decrement the thread watch count
            //            }
            //        }
            //    }
            //}
            Debug.WriteLine("\nServer thread created.");
        }

        private static void ServerThread(object? data)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("com.utticus.youtube.assist", PipeDirection.InOut, numThreads);

            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            Debug.WriteLine("Client connected on thread[{0}].", threadId);
            try
            {
                // Read the request from the client. Once the client has
                // written to the pipe its security token will be available.

                StreamString ss = new StreamString(pipeServer);

                // Verify our identity to the connected client using a
                // string that the client anticipates.

                //ss.WriteString("I am the one true server!");
                string message = ss.ReadString();

                // Read in the contents of the file while impersonating the client.
                //MessageHandler messageHandler = new MessageHandler(ss);

                // Display the name of the user we are impersonating.
                Debug.WriteLine("Get message: {0} on thread[{1}] as user: {2}.",
                    message, threadId, pipeServer.GetImpersonationUserName());

                ss.WriteString(String.Format("Forwarded from server pipe: {0}", message));
                //pipeServer.RunAsClient(messageHandler.Start);
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Debug.WriteLine("ERROR: {0}", e.Message);
            }
            pipeServer.Close();
        }
    }
}
