using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Threading;
using YouTubeAssist.UI;

namespace YouTubeAssist.Services
{
    class PipeServer
    {
        public string messageIncome = "";
        public string messageOutcome = "";
        public PipeView pipeView;
        public PipeServer(PipeView pv)
        {
            Debug.WriteLine("\n*** Named pipe server stream with impersonation example ***\n");
            Debug.WriteLine("Waiting for client connect...\n");

            Thread pipeServerThread = new Thread(ServerThread);
            pipeView = pv;
            pipeView.MessageIncome += "\n*** Named pipe server stream with impersonation example ***\n";
            pipeView.MessageIncome += "\nWaiting for client connect...\n";
            pipeServerThread.Start();
            Debug.WriteLine("\nServer thread created.");
        }

        private async void ServerThread(object? data)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("com.utticus.youtube.assist", PipeDirection.InOut);

            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            Debug.WriteLine("Client connected on thread[{0}].", threadId);
            pipeView.MessageIncome += String.Format("\nClient connected on thread [{0}] as user {1}\n", threadId, pipeServer.GetImpersonationUserName());

            try
            {
                // Read from client
                //using (StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8, false, 1024))
                //{
                //    string messageFromClient = reader.ReadLine();  // Read client's message
                //    Console.WriteLine("Received from client: " + messageFromClient);
                //}

                //// Write response to client
                //using (StreamWriter writer = new StreamWriter(pipeServer, Encoding.UTF8, 1024) { AutoFlush = true })
                //{
                //    string response = "Hello from server!";
                //    writer.WriteLine(response);  // Send response
                //    writer.Flush();  // Ensure response is sent
                //    Console.WriteLine("Sent to client: " + response);
                //}
                //pipeServer.WaitForPipeDrain();

                //byte[] buffer = new byte[1024];
                //int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);
                //string d = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                //Debug.WriteLine("Received message from client: " + d);

                //string response = "Hello from the server!";
                //byte[] responseData = Encoding.UTF8.GetBytes(response);
                //pipeServer.Write(responseData, 0, responseData.Length);

                //using (StreamReader reader = new StreamReader(pipeServer))
                //{
                //    string d;

                //    while ((d = reader.ReadLine()) != null)
                //    {
                //        // Process the received string
                //        Debug.WriteLine("Received string: " + d);
                //    }
                //}

                //// Send a response (optional)
                //using (StreamWriter writer = new StreamWriter(pipeServer))
                //{
                //    writer.WriteLine("Hello from the server!");
                //}

                //byte[] buffer = new byte[1024];
                //int bytesRead;

                //while ((bytesRead = pipeServer.Read(buffer, 0, buffer.Length)) > 0)
                //{
                //    // Process the received bytes
                //    Debug.WriteLine("Received bytes: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));
                //}

                //// Send a response (optional)
                //byte[] responseData = Encoding.UTF8.GetBytes("Hello from the server!");
                //pipeServer.Write(responseData, 0, responseData.Length);

                //Thread readThread = new Thread(ReadData);
                //readThread.Start(pipeServer);

                //Thread writeThread = new Thread(WriteData);
                //writeThread.Start(pipeServer);

                //readThread.Join();
                //writeThread.Join();

                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = pipeServer.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string input = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.WriteLine("Received from client: " + input);

                    // Send a response (optional)
                    string response = "Received input: " + input;
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    pipeServer.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: {0}", e.Message);
                pipeServer.Close();
            }
            

            //try
            //{
            //    // Read the request from the client. Once the client has
            //    // written to the pipe its security token will be available.

            //    StreamString ss = new StreamString(pipeServer);
            //    StreamString ss2 = new StreamString(pipeServer);
            //    //StreamWriter sw = new StreamWriter(pipeServer);
            //    //StreamReader sr = new StreamReader(pipeServer);
            //    //sw.AutoFlush = true;

            //    // Verify our identity to the connected client using a
            //    // string that the client anticipates.

            //    await serverTasks(ss, ss2, threadId, pipeServer.GetImpersonationUserName());

            //    //await Task.Run(() => handlePipeStream2(sr, sw, threadId, pipeServer.GetImpersonationUserName()));
            //    //await Task.Run(() => readPipeStream(ss));

            //    //string message = "";
            //    //while ((message = ss.ReadString()) != "!q")
            //    //{
            //    //    // Display the name of the user we are impersonating.
            //    //    Debug.WriteLine("Get message: {0} on thread[{1}] as user: {2}.",
            //    //        message, threadId, pipeServer.GetImpersonationUserName());
            //    //    ss.WriteString(String.Format("Forwarded from server pipe: {0}", message));
            //    //}
            //    //Debug.WriteLine("Get quite message on thread[{0}] as user: {1}, will close the pipe.",
            //    //        threadId, pipeServer.GetImpersonationUserName());
            //    //pipeServer.Close();
            //}
            //// Catch the IOException that is raised if the pipe is broken or disconnected.
            //catch (IOException e)
            //{
            //    Debug.WriteLine("ERROR: {0}", e.Message);
            //    pipeServer.Close();
            //}
        }

        void handlePipeStream(StreamString ss, StreamString ss2,int threadId, string impersonationUsername)
        {
            while (true)
            {
                string message = ss.ReadString();
                Debug.WriteLine("Get message: {0} on thread[{1}] as user: {2}.",
                            message, threadId, impersonationUsername);
                //ss.WriteString(String.Format("Forwarded from server pipe: {0}", message));
                messageIncome += "Get message from Client:" + message;
                pipeView.MessageIncome += "\nGet message from Client:" + message + '\n';
                //ss2.WriteString("YOYO");
            }
        }

        void handlePipeStream2(StreamReader sr, StreamWriter sw)
        {
            //while (true)
            //{
            //    string? message = sr.ReadLine();
            //    Debug.WriteLine("Get message: {0}", message);
            //    sw.WriteLine(String.Format("Forwarded from server pipe: {0}", message));
            //}
            string? message = sr.ReadLine();
            Debug.WriteLine("Get message: {0}", message);
            sw.WriteLine(String.Format("Forwarded from server pipe: {0}", message));
        }

        async Task serverTasks(StreamString ss, StreamString ss2, int threadId, string impersonationUsername)
        {
            await Task.Run(() => handlePipeStream(ss, ss2, threadId, impersonationUsername));
            //await Task.Run(() => readPipeStream(ss));
        }

        async Task serverTasks2(StreamReader sr, StreamWriter sw)
        {
            await Task.Run(() => handlePipeStream2(sr, sw));
            //await Task.Run(() => readPipeStream(ss));
        }

        async Task readPipeStream(StreamString ss)
        {
            string message = ss.ReadString();
            Debug.WriteLine(message);
        }

        static void ReadData(object pipeServerObj)
        {
            NamedPipeServerStream pipeServer = (NamedPipeServerStream)pipeServerObj;

            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = pipeServer.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Process the received data
                Debug.WriteLine("Received data: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }

        static void WriteData(object pipeServerObj)
        {
            NamedPipeServerStream pipeServer = (NamedPipeServerStream)pipeServerObj;

            // Send data to the client
            byte[] responseData = Encoding.UTF8.GetBytes("Hello from the server!");
            pipeServer.Write(responseData, 0, responseData.Length);
        }
    }
}
