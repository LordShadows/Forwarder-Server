using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    class Engineer
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String ContactNumber { get; set; }
        public String Note { get; set; }

        public Engineer(String id, String name, String contactNumber, String note)
        {
            this.ID = id;
            this.Name = name;
            this.ContactNumber = contactNumber;
            this.Note = note;
        }
    }
}
