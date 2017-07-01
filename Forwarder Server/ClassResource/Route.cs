using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    class Route
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String DepartureDate { get; set; }
        public String CarType { get; set; }
        public String ReturnDate { get; set; }
        public String RouteStatus { get; set; }
        public String CityCountryDeparture { get; set; }
        public String Note { get; set; }
        public String IDForwarder { get; set; }

        public Route(String id, String name, String departureDate, String carType, String returnDate, String routeStatus, String cityCountryDeparture, String note, String idForwarder)
        {
            this.ID = id;
            this.Name = name;
            this.DepartureDate = departureDate;
            this.CarType = carType;
            this.ReturnDate = returnDate;
            this.RouteStatus = routeStatus;
            this.CityCountryDeparture = cityCountryDeparture;
            this.Note = note;
            this.IDForwarder = idForwarder;
        }
    }
}
