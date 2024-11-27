// See https://aka.ms/new-console-template for more information
using ClientHost;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

// https://www.nuget.org/packages/NativeMessaging/#
RequestMessage? ReadStdInput()
{
    Stream stdin = Console.OpenStandardInput();

    byte[] lengthBytes = new byte[4];
    stdin.Read(lengthBytes, 0, 4);

    char[] buffer = new char[BitConverter.ToInt32(lengthBytes, 0)];

    using (StreamReader reader = new StreamReader(stdin))
    {
        if (reader.Peek() >= 0)
        {
            reader.Read(buffer, 0, buffer.Length);
            return JsonSerializer.Deserialize<RequestMessage>(new string(buffer));
        }
        else
        {
            return null;
        }
    }
}

void WriteStdOut(string resp)
{
    ResponseMessage responseMessage = new ResponseMessage();
    responseMessage.resp = resp;
    string respOut = JsonSerializer.Serialize<ResponseMessage>(responseMessage);
    Debug.WriteLine("Sending Message:" + respOut);

    byte[] bytes = Encoding.UTF8.GetBytes(respOut);
    Stream stdout = Console.OpenStandardOutput();

    stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
    stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
    stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
    stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
    stdout.Write(bytes, 0, bytes.Length);
    stdout.Flush();
}


try
{
    NamedPipeClientStream pipeClient = null;

    Debug.WriteLine("Connecting to server...");

    Task connectPipeTask = Task.Run(async () =>
    {
        pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);

        await pipeClient.ConnectAsync(5000);

        var readTask = Task.Run(() =>
        {
            using (var reader = new StreamReader(pipeClient, Encoding.UTF8, leaveOpen: true))
            {
                while (pipeClient.IsConnected)
                {
                    //string? message = reader.ReadLine();
                    //if (!string.IsNullOrEmpty(message))
                    //{
                    //    string messageLog = $"Received from Native app: {message}\n";
                    //    Debug.WriteLine(messageLog);
                    //}
                }
            }
        });
    });


    RequestMessage? requestMessage;
    while ((requestMessage = ReadStdInput()) != null)
    {
        Debug.WriteLine("Get message from WebExt: " + requestMessage.req);

        Task writePipeTask = Task.Run(() =>
        {
            if (pipeClient != null && pipeClient.IsConnected)
            {
                using(var writer = new StreamWriter(pipeClient, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
                {
                    writer.WriteLine(requestMessage.req);
                    writer.Flush();
                }
            }
        });
    }

}
catch (Exception ex)
{
    Debug.WriteLine("Error: " + ex.Message);
}
