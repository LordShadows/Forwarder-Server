using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forwarder_Server.ClassResource
{
    class User
    {
        public String Login { get; set; }
        public String Name { get; set; }
        public String Role { get; set; }
        public String Snapping { get; set; }
        public String Engineer { get; set; }
        public String Forwarder { get; set; }
        public String Password { get; set; }

        public User (String login, String name, String role, String snapping, String enginner, String forwarder, String password)
        {
            this.Login = login;
            this.Name = name;
            this.Role = role;
            this.Snapping = snapping;
            this.Engineer = enginner;
            this.Forwarder = forwarder;
            this.Password = password;
        }
    }
}
