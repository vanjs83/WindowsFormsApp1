using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public enum Prava {
        Admin,
        User
    }

    public class User
    {
      public  int Id { get; set; }
      public  string Username { get; set; }
      public   string Password { get; set; }
      public  Prava prava; 

        public User(int _id, string _username, string _password)
        {
            this.Id = _id;
            this.Username = _username;
            this.Password = _password;
        }

        public User(string _username, string _password)
        {
           
            this.Username = _username;
            this.Password = _password;
        }
           //Default constructor
        public User()
        {
        }

        // Copy constructor
        public User(User user)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.Password = user.Password;
            this.prava = user.prava;
        }

    }
}
