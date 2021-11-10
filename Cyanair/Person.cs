using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyanair
{
    public class Person
    {
        
      
        
        private string firstName;
        private string lastName;
        public string email;
        public string phoneNumber;


        //properties for private variables 
        public string FirstName  
        {
            get { return firstName; }  
            set { firstName = value; }  
        }
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
    }

}
