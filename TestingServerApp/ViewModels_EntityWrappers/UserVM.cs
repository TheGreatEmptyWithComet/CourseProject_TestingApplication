using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class UserVM : NotifyPropertyChangeHandler
    {
        public User Model { get; set; }

        public int Id { get => Model.Id; }

        public string Login
        {
            get => Model.Login;
            set => Model.Login = value;
        }

        public byte[] PasswordSalt
        {
            get => Model.PasswordSalt;
        }

        public string PasswordHash
        {
            get => Model.PasswordHash;
        }

        public string Email
        {
            get => Model.Email;
            set => Model.Email = value;
        }

        public UserGroupVM UserGroup
        {
            get => new UserGroupVM(Model.UserGroup);
            set
            {
                if (value != null && Model.UserGroup != value.Model)
                {
                    Model.UserGroup = value.Model;
                    NotifyPropertyChanged(nameof(TestCategory));
                }
            }
        }

        // Password as string can only be set but not read, because it is stored as hash + salt values
        private string password = string.Empty;
        public string Password
        {
            get => password;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    password = value;
                    // Store password salt and hash
                    Model.PasswordSalt = PasswordEncryption.GenerateSaltAsBytes();
                    Model.PasswordHash = PasswordEncryption.GetPasswordHash(value, Model.PasswordSalt);
                    // Notify hash property changed
                    NotifyPropertyChanged(nameof(PasswordHash));
                }
            }
        }

        public UserVM(User user)
        {
            Model = user;
        }
    }
}
