using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientHost
{
    internal class IOService
    {
        static private string? ReadMessage()
        {
            char[]? buffer = ReadStdInput();
            if (buffer == null)
            {
                return null;
            }
            Message? message = JsonSerializer.Deserialize<Message>(new string(buffer));
            if (message?.type != "IN_MSG")
            {
                return null;
            }

            return message?.data?.GetValueOrDefault("message", null);

        }
        static char[]? ReadStdInput()
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
                    return buffer;
                }
                else
                {
                    return null;
                }
            }
        }

        static private void WriteStdOut(string resp)
        {
            ResponseMessage responseMessage = new ResponseMessage(resp);
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
    }
}
