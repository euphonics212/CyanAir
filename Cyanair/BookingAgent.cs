using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Cyanair
{
    public class BookingAgent : Person
    {
        public string employeeId;
        public string adminRole;
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
