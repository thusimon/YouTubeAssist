using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeAssist.Services
{
    class MessageHandler
    {
        private StreamString ss;
        public MessageHandler(StreamString str) {
            ss = str;
        }
        public void Start()
        {
            string contents = "hello from service pipe";
            ss.WriteString(contents);
        }
    }
}
