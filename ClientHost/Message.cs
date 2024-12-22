using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientHost
{
    internal class Message
    {
        public string type { get; set; }
        public Dictionary<string, string>? data { get; set; }

        public Message(string _type) {
            type = _type;
        }

        public Message(string _type, Dictionary<string, string> _data)
        {
            type = _type;
            data = _data;
        }
    }
}
