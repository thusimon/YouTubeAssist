// See https://aka.ms/new-console-template for more information
using ClientHost;
using System.IO.Pipes;
using System.Security.Principal;


var pipeClient = new NamedPipeClientStream(".", "com.utticus.youtube.assist",
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);

pipeClient.Connect();
Console.WriteLine("Connected to server pipe.\n");

var ss = new StreamString(pipeClient);
var sr = new StreamReader(pipeClient);
var sw = new StreamWriter(pipeClient);

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

async Task writePipeStream2(StreamWriter sw)
{
    string? message = "";
    while ((message = await consoleReadLine()) != "q")
    {
        sw.WriteLine(message!);
    }
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
    await Task.Run(() => writePipeStream2(sw));
    //await Task.Run(() => readPipeStream(ss));
}

sw.AutoFlush = true;
Console.WriteLine("Running r/w PipeStream tasks");
//await hostTasks2(sr, sw);
await hostTasks(ss);
