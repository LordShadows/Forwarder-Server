using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    public class Request
    {
        public String ID { get; set; }
        public String Number { get; set; }
        public String ProductName { get; set; }
        public String ProductWeight { get; set; }
        public String ProductDimensions { get; set; }
        public String Quantity { get; set; }
        public String IDCompany { get; set; }
        public String IDEngineer { get; set; }
        public String Note { get; set; }
        public String Date { get; set; }

        public Request(String id, String number, String productName, String productWeight, String productDimensions, String quantity, String idCompany, String idEngineer, String note, String date)
        {
            this.ID = id;
            this.Number = number;
            this.ProductName = productName;
            this.ProductWeight = productWeight;
            this.ProductDimensions = productDimensions;
            this.Quantity = quantity;
            this.IDCompany = idCompany;
            this.IDEngineer = idEngineer;
            this.Note = note;
            this.Date = date;
        }
    }
}
