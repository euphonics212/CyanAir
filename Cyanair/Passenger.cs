using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyanair
{
    public class Passenger : Person
    {
        private string passportNum;
      
        public string PassportNum
        {
            get { return passportNum; }
            set { passportNum = value; }
        }
    }
}
