using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.Sources
{
    class Message
    {
        public String Keyword { get; set; }
        public String[] TextArguments { get; set; }
        public String Signature { get; set; }

        public Message(String keyword, String[] textArguments, String signature)
        {
            this.Keyword = keyword;
            this.TextArguments = textArguments;
            this.Signature = signature;
        }
    }
}
