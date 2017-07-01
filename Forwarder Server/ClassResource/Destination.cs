using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    class Destination
    {
        public String ID { get; set; }
        public String ArrivalDate { get; set; }
        public String Note { get; set; }
        public String IDRoute { get; set; }
        public String IDRequest { get; set; }
        public String Number { get; set; }

        public Destination(String id, String arrivalDate, String note, String idRoute, String idRequest, String number)
        {
            this.ID = id;
            this.ArrivalDate = arrivalDate;
            this.Note = note;
            this.IDRoute = idRoute;
            this.IDRequest = idRequest;
            this.Number = number;
        }
    }
}
