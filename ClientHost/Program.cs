// See https://aka.ms/new-console-template for more information
using CommonLibrary;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;

// https://www.nuget.org/packages/NativeMessaging/#

void Log(string log, bool showInView = true)
{
    Debug.WriteLine(log);
    if (showInView)
    {
        
        Message message = new Message("HOST_TO_EXT", "MESSAGE", new Dictionary<string, string>{ {"message", log} });
        IOService.WriteMessage(message);
    }
}

try
{
    NamedPipeClientStream? pipeClient = null;

    Log("Connecting to server...", false);

    Task connectPipeTask = Task.Run(async () =>
    {
        pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.Asynchronous,
                        TokenImpersonationLevel.Impersonation);

        await pipeClient.ConnectAsync(5000);

        var readTask = Task.Run(() =>
        {
            using (var reader = new StreamReader(pipeClient, Encoding.UTF8, leaveOpen: true))
            {
                while (pipeClient.IsConnected)
                {
                    string? messageBuffer = reader.ReadLine();
                    if (!string.IsNullOrEmpty(messageBuffer))
                    {
                        Debug.WriteLine($"Received from Native app: {messageBuffer}");

                        Message? message = IOService.DeserializedMessage(messageBuffer);
                        if (message == null || message.type != "NATIVE_TO_HOST")
                        {
                            Debug.WriteLine("Can not deserialize message or invalid message type");
                        }
                        else
                        {
                            // update message type
                            message.type = "HOST_TO_EXT";
                            IOService.WriteMessage(message);
                        }
                    }
                }
                Log("pipeClient is disconnected");
            }
        });
    });

    Message? message;
    while ((message = IOService.ReadMessage()) != null)
    {
        Debug.WriteLine($"Get {message.type}:{message.action} from WebExt");

        Task writePipeTask = Task.Run(() =>
        {
            if (pipeClient != null && pipeClient.IsConnected)
            {
                using(var writer = new StreamWriter(pipeClient, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
                {
                    // change the message type
                    message.type = "HOST_TO_NATIVE";
                    writer.WriteLine(IOService.SerializedMessage(message));
                }
            }
        });
    }

}
catch (Exception ex)
{
    Debug.WriteLine("Error: " + ex.Message);
}
