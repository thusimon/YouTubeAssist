// See https://aka.ms/new-console-template for more information
using ClientHost;
using System.IO.Pipes;
using System.Security.Principal;

Console.WriteLine("Hello, World!");


var pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);

Console.WriteLine("Connecting to server...\n");
pipeClient.Connect();

var ss = new StreamString(pipeClient);
string? message = "";
while ((message = Console.ReadLine()) != "q")
{
    ss.WriteString(message!);
}
