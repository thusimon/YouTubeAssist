using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace CommonLibrary
{
    public class IOService
    {
        static public Message? ReadMessage()
        {
            char[]? buffer = ReadStdInput();
            if (buffer == null)
            {
                return null;
            }
            return DeserializedMessage(new string(buffer));
        }

        static public char[]? ReadStdInput()
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

        static public void WriteStdOut(string rawMessage)
        {
            Debug.WriteLine("Sending Message:" + rawMessage);
            byte[] bytes = Encoding.UTF8.GetBytes(rawMessage);
            Stream stdout = Console.OpenStandardOutput();

            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }
        static public void WriteMessage(Message message)
        {
            string messageOut = JsonSerializer.Serialize<Message>(message);
            WriteStdOut(messageOut);
        }
        static public string SerializedMessage(Message message)
        {
            return JsonSerializer.Serialize(message);
        }

        static public Message? DeserializedMessage(string message)
        {
            // Detect and remove BOM if present
            if (message[0] == '\uFEFF')
            {
                message = message.Substring(1);
            }

            return JsonSerializer.Deserialize<Message>(message);
        }
    }
}
