using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    class Company
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String Country { get; set; }
        public String City { get; set; }
        public String Address { get; set; }
        public String NameСontactPerson { get; set; }
        public String PhoneContactPerson { get; set; }

        public Company(String id, String name, String country, String city, String address, String nameСontactPerson, String phoneContactPerson)
        {
            this.ID = id;
            this.Name = name;
            this.Country = country;
            this.City = city;
            this.Address = address;
            this.NameСontactPerson = nameСontactPerson;
            this.PhoneContactPerson = phoneContactPerson;
        }
    }
}
