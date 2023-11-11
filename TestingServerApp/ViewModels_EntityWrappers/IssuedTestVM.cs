using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class IssuedTestVM : NotifyPropertyChangeHandler
    {
        public IssuedTest Model { get; set; }

        public int Id { get => Model.Id; }

        public DateTime IssueDate
        {
            get => Model.IssueDate;
            set => Model.IssueDate = value;
        }

        public DateTime ExpiredDate
        {
            get => Model.ExpiredDate;
            set => Model.ExpiredDate = value;
        }

        public int AttemptsAmount
        {
            get => Model.AttemptsAmount;
            set => Model.AttemptsAmount = value;
        }

        public UserGroupVM UserGroup
        {
            get => new UserGroupVM(Model.UserGroup);
            set
            {
                if (value != null && Model.UserGroup != value.Model)
                {
                    Model.UserGroup = value.Model;
                    NotifyPropertyChanged(nameof(UserGroup));
                }
            }
        }

        public TestVM Test
        {
            get => new TestVM(Model.Test);
            set
            {
                if (value != null && Model.Test != value.Model)
                {
                    Model.Test = value.Model;
                    NotifyPropertyChanged(nameof(Test));
                }
            }
        }


        public IssuedTestVM(IssuedTest issuedTest)
        {
            Model = issuedTest;
        }

    }
}
