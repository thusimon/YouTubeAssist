﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientHost
{
    internal class ResponseMessage
    {
        public string? resp { get; set; }

        public ResponseMessage(string responseMessage)
        {
            resp = responseMessage;
        }

        public ResponseMessage() { }
    }
}
