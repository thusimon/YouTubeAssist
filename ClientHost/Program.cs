// See https://aka.ms/new-console-template for more information
using ClientHost;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Text;

// https://www.nuget.org/packages/NativeMessaging/#


NamedPipeClientStream pipeClient = null;
static void connectToPipe()
{
    NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);
    pipeClient.Connect(5000);
    //return pipeClient;
}

//var pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
//                        PipeDirection.InOut, PipeOptions.None,
//                        TokenImpersonationLevel.Impersonation);

//pipeClient.Connect();
//Console.WriteLine("Connected to server pipe.\n");

//var ss = new StreamString(pipeClient);
//var sr = new StreamReader(pipeClient);
//var sw = new StreamWriter(pipeClient);
//sw.AutoFlush = true;

static async Task<string?> consoleReadLine()
{
    return await Task.Run(() =>
    {
        return Console.ReadLine();
    });
}

async Task writePipeStream(StreamString ss)
{
    string? message = "";
    while ((message = await consoleReadLine()) != "q")
    {
        ss.WriteString(message!);
    }
}

async Task writePipeStream2(StreamWriter sw, StreamReader sr)
{
    string? message = "";
    //while ((message = await consoleReadLine()) != "q")
    //{
    //    sw.WriteLine(message!);
    //}
    message = await consoleReadLine();
    string responseFromServer = sr.ReadLine();
    Console.WriteLine("Received from server: " + responseFromServer);
}

async Task readPipeStream(StreamString ss)
{
    string? message = "";
    while ((message = ss.ReadString()) != null)
    {
        Console.WriteLine(message);
    }
}

async Task readPipeStream2(StreamReader sr)
{
    string? message = "";
    while ((message = sr.ReadLine()) != null)
    {
        Console.WriteLine(message);
    }
}

async Task hostTasks(StreamString ss)
{
    await Task.Run(() => writePipeStream(ss));
    //await Task.Run(() => readPipeStream(ss));
}

async Task hostTasks2(StreamReader sr, StreamWriter sw)
{
    await Task.Run(() => writePipeStream2(sw, sr));
    //await Task.Run(() => readPipeStream2(sr));
}

//Console.WriteLine("Running r/w PipeStream tasks");
//await hostTasks2(sr, sw);
//await hostTasks(ss);

static void ReadData(object pipeClientObj)
{
    //NamedPipeClientStream pipeClient = (NamedPipeClientStream)pipeClientObj;
    //while (true)
    //{
    //    string input = Console.ReadLine();
    //    byte[] data = Encoding.UTF8.GetBytes(input);
    //    pipeClient.Write(data, 0, data.Length);
    //}

    NamedPipeClientStream pipeClient = (NamedPipeClientStream)pipeClientObj;

    byte[] buffer = new byte[1024];
    int bytesRead;

    while ((bytesRead = pipeClient.Read(buffer, 0, buffer.Length)) > 0)
    {
        string input = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received from server: " + input);
    }
}

static void WriteData(object pipeClientObj)
{
    NamedPipeClientStream pipeClient = (NamedPipeClientStream)pipeClientObj;

    byte[] buffer = new byte[1024];
    int bytesRead;
    //string input = null;
    //while ((input = Console.ReadLine()) != null)
    //{
    //    byte[] d = Encoding.UTF8.GetBytes(input);
    //    pipeClient.Write(d, 0, d.Length);
    //}

    while (true)
    {
        string input = Console.ReadLine();
        byte[] d = Encoding.UTF8.GetBytes(input);
        pipeClient.Write(d, 0, d.Length);
        pipeClient.Flush();
    }

    // Send a message to the server (replace with your desired message)
    //string message = "Hello from the client!";
    //byte[] data = Encoding.UTF8.GetBytes(message);
    //pipeClient.Write(data, 0, data.Length);

    //while (true)
    //{
    //    // Read a response from the server (optional)
    //    bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
    //    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //    Console.WriteLine("Received from server: " + response);
    //}

    //NamedPipeClientStream pipeClient = (NamedPipeClientStream)pipeClientObj;

    //// Send a message to the server (replace with your desired message)
    //string message = "Hello from the client!";
    //byte[] data = Encoding.UTF8.GetBytes(message);
    //pipeClient.Write(data, 0, data.Length);

    // Send data to the server
    //string input = Console.ReadLine();
    //byte[] data = Encoding.UTF8.GetBytes("Test");
    //byte[] data = Encoding.UTF8.GetBytes(input);
    //while (true)
    //{
    //    string input = Console.ReadLine();
    //    writer.WriteLine(input);

    //    // Read a response from the server (optional)
    //    using (StreamReader reader = new StreamReader(pipeClient))
    //    {
    //        string response = reader.ReadLine();
    //        Console.WriteLine("Received from server: " + response);
    //    }
    //}

    //pipeClient.Write(data, 0, data.Length);
    //pipeClient.Flush();
}

string? message = "";
//while ((message = await consoleReadLine()) != "q")
//{
//    sw.WriteLine(message!);
//}
//message = await consoleReadLine();
//string responseFromServer = sr.ReadLine();
//Console.WriteLine("Received from server: " + responseFromServer);


static string OpenStandardStreamIn()
{
    //// We need to read first 4 bytes for length information
    //Stream stdin = Console.OpenStandardInput();
    //int length = 0;
    //byte[] bytes = new byte[4];
    //stdin.Read(bytes, 0, 4);
    //length = System.BitConverter.ToInt32(bytes, 0);

    //string input = "";
    //for (int i = 0; i < length; i++)
    //{
    //    input += (char)stdin.ReadByte();
    //}

    //return input;
    string message = "";
    byte[] lengthBytes = new byte[4];
    Console.OpenStandardInput().Read(lengthBytes, 0, 4);  // Read the 4-byte length prefix
    int messageLength = BitConverter.ToInt32(lengthBytes, 0);
    if (messageLength == 0)
    {
        return message;
    }

    byte[] messageBytes = new byte[messageLength];
    Console.OpenStandardInput().Read(messageBytes, 0, messageLength);  // Read the actual message
    message = Encoding.UTF8.GetString(messageBytes);
    return message;
}

static void OpenStandardStreamOut(string stringData)
{
    //// We need to send the 4 btyes of length information
    //string msgdata = "{\"resp\":\"" + stringData + "\"}";
    int DataLength = stringData.Length;
    byte[] responseBytes = Encoding.UTF8.GetBytes(stringData);
    Stream stdout = Console.OpenStandardOutput();
    //stdout.WriteByte((byte)((DataLength >> 0) & 0xFF));
    //stdout.WriteByte((byte)((DataLength >> 8) & 0xFF));
    //stdout.WriteByte((byte)((DataLength >> 16) & 0xFF));
    //stdout.WriteByte((byte)((DataLength >> 24) & 0xFF));
    //Available total length : 4,294,967,295 ( FF FF FF FF )
    byte[] lengthBytes = BitConverter.GetBytes(responseBytes.Length);
    Console.OpenStandardOutput().Write(lengthBytes, 0, 4);  // Write the 4-byte length prefix
    Console.OpenStandardOutput().Write(responseBytes, 0, responseBytes.Length);  // Write the actual message
    Console.Out.Flush();  // Flush the output to ensure it's sent
}

try
{


    Debug.WriteLine("Connecting to server...");

    Thread connectThread = new Thread(connectToPipe);
    Task<NamedPipeClientStream> connectPipeTask = Task.Run(() =>
    {
        pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);
        pipeClient.Connect(5000);
        return pipeClient;
    });

    //pipeClient = connectPipeTask.Result;

    ////pipeClient.Connect(5000);  // Timeout after 5 seconds if the server isn't available
    //Debug.WriteLine("Connected to server.");

    //Task readConsoleTask = Task.Run(() =>
    //{
    //    while (true)
    //    {
    //        byte[] lengthBytes = new byte[4];
    //        if (Console.OpenStandardInput().Read(lengthBytes, 0, 4) == 0)
    //        {
    //            return;
    //        }

    //        int messageLength = BitConverter.ToInt32(lengthBytes, 0);

    //        // Read the message
    //        byte[] messageBytes = new byte[messageLength];
    //        Console.OpenStandardInput().Read(messageBytes, 0, messageLength);
    //        string message = Encoding.UTF8.GetString(messageBytes);

    //        Debug.WriteLine(message);
    //    }
    //});

    //readConsoleTask.Start();

    //using (StreamWriter writer = new StreamWriter(pipeClient, Encoding.UTF8) { AutoFlush = true })
    //{
    //    string messageToServer = "Hello from client!";
    //    writer.WriteLine(messageToServer);  // Send message
    //    writer.Flush();  // Ensure it is sent immediately
    //    Console.WriteLine("Sent to server: " + messageToServer);
    //}

    //// Read response from server
    //using (StreamReader reader = new StreamReader(pipeClient, Encoding.UTF8))
    //{
    //    string responseFromServer = reader.ReadLine();  // Read response
    //    Console.WriteLine("Received from server: " + responseFromServer);
    //}

    //pipeClient.WaitForPipeDrain();

    //string msg = "Hello from the client!";
    //byte[] data = Encoding.UTF8.GetBytes(msg);
    //pipeClient.Write(data, 0, data.Length);

    //byte[] buffer = new byte[1024];
    //int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
    //string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    //Console.WriteLine("Received response  from server: " + response);

    //using (StreamWriter writer = new StreamWriter(pipeClient))
    //{
    //    writer.WriteLine("Hello from the client!");
    //}

    //using (StreamReader reader = new StreamReader(pipeClient))
    //{
    //    string data;

    //    while ((data = reader.ReadLine()) != null)
    //    {
    //        // Process the received string
    //        Console.WriteLine("Received response from server: " + data);
    //    }
    //}

    //byte[] data = Encoding.UTF8.GetBytes("Hello from the client!");
    //pipeClient.Write(data, 0, data.Length);

    //byte[] buffer = new byte[1024];
    //int bytesRead;

    //while ((bytesRead = pipeClient.Read(buffer, 0, buffer.Length)) > 0)
    //{
    //    // Process the received bytes
    //    Console.WriteLine("Received response from server: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));
    //}

    //Thread readThread = new Thread(ReadData);
    //readThread.Start(pipeClient);

    //Thread writeThread = new Thread(WriteData);
    //writeThread.Start(pipeClient);

    //readThread.Join();
    //writeThread.Join();

    //byte[] buffer = new byte[1024];
    //int bytesRead;

    //while (true)
    //{
    //    string input = Console.ReadLine();
    //    byte[] data = Encoding.UTF8.GetBytes(input);
    //    pipeClient.Write(data, 0, data.Length);

    //    // Read a response from the server (optional)
    //    bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
    //    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //    Console.WriteLine("Received from server: " + response);
    //}

    //Thread readThread = new Thread(ReadData);
    //readThread.Start(pipeClient);

    //Thread writeThread = new Thread(WriteData);
    //writeThread.Start(pipeClient);

    //readThread.Join();
    //writeThread.Join();



    //while (true)
    //{
    //    string input = OpenStandardStreamIn();
    //    if (input !="")
    //    {
    //        OpenStandardStreamOut("Received to Native App: hello");
    //    }

    //    //OpenStandardStreamOut("Recieved: " + OpenStandardStreamIn());
    //}

    // Reading from Chrome (binary input
    bool flag = true;
    while(flag)
    {
        byte[] lengthBytes = new byte[4];
        int readLength = Console.OpenStandardInput().Read(lengthBytes, 0, 4);  // Read the 4-byte length prefix
        int messageLength = BitConverter.ToInt32(lengthBytes, 0);

        if (readLength == 0)
        {
            continue;
        }

        byte[] messageBytes = new byte[messageLength];
        Console.OpenStandardInput().Read(messageBytes, 0, messageLength);  // Read the actual message
        string message2 = Encoding.UTF8.GetString(messageBytes);

        // Responding to Chrome (binary output)
        string dateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        string response = "{\"response\": \"Hello from native host: " + dateTime + "\"}";
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);

        //Thread.Sleep(10000);
        byte[] responseLength = BitConverter.GetBytes(responseBytes.Length);
        Console.OpenStandardOutput().Write(responseLength, 0, 4);  // Write the 4-byte length prefix
        Console.OpenStandardOutput().Write(responseBytes, 0, responseBytes.Length);  // Write the actual message
        Console.Out.Flush();  // Ensure the message is sent

        //flag = false;
    }
    
}
catch (Exception ex)
{
    Debug.WriteLine("Error: " + ex.Message);
}
