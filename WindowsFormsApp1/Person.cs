using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WindowsFormsApp1
{
    public class Person : User
    {
        public string Name { get; set; }

        public string Surname { get; set; }
       
        public string Oib { get; set; }

        public string Address { get; set; }
     
        public string Email { get; set; }
       
        public string Mob { get; set; }


        public Person(string name, string surname, string oib, string address, string email, string mob, User user)
          : base(user) //Poziv baznog konstruktra
        {
            this.Name = name;
            this.Surname = surname;
            this.Oib = oib;
            this.Address = address;
            this.Email = email;
            this.Mob = mob;
        }

        public Person(string name, string surname, string oib, string address, string email, string mob)
        {
            this.Name = name;
            this.Surname = surname;
            this.Oib = oib;
            this.Address = address;
            this.Email = email;
            this.Mob = mob;
        }



        public Person(User user)

        : base(user) //Poziv baznog konstruktra:base(user) //Poziv baznog konstruktra:base(user) //Poziv baznog konstruktra:base(user) //Poziv baznog konstruktra:base(user) //Poziv baznog konstruktra
        { }


    }
}
