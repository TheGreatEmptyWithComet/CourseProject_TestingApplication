using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingClientApp
{
    internal class User : NotifyPropertyChangedHandler
    {
        private string login;
        public string Login
        {
            get { return login; }
            set
            {
                if (login != value)
                {
                    login = value;
                    NotifyPropertyChanged(nameof(Login));
                }
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    NotifyPropertyChanged(nameof(Email));
                }
            }
        }

        private DateTime birthDate;
        public DateTime BirthDate
        {
            get { return birthDate; }
            set
            {
                if (birthDate != value)
                {
                    birthDate = value;
                    NotifyPropertyChanged(nameof(BirthDate));
                }
            }
        }

        public User()
        {
            ResetUserData();
        }
        public User(string login, string email, string password, DateTime birthDate)
        {
            this.login = login;
            this.email = email;
            this.password = password;
            this.birthDate = birthDate;
        }


        public void ResetUserData()
        {
            Login = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            BirthDate = DateTime.MinValue;
        }
    }
}
