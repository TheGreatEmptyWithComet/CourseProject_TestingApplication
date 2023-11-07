using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class UserGroupVM
    {
        public UserGroup Model { get; set; }

        public int Id { get => Model.Id; }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }


        public UserGroupVM(UserGroup userGroup)
        {
            Model = userGroup;
        }


        public override bool Equals(object? obj)
        {
            if (obj != null && obj is UserGroupVM userGroupVM && userGroupVM.Model != null)
            {
                return Model.Id.Equals(userGroupVM.Model.Id);
            }
            return false;
        }
    }
}
