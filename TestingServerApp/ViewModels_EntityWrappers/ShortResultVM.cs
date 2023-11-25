using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class ShortResultVM : NotifyPropertyChangedHandler
    {

        public ShortResult Model { get; set; }

        public int Id { get => Model.Id; }

        public DateTime Date
        {
            get => Model.Date;
            set => Model.Date = value;
        }

        public int TestMaxScores
        {
            get => Model.TestMaxScores;
            set => Model.TestMaxScores = value;
        }

        public double UserScores
        {
            get => Model.UserScores;
            set => Model.UserScores = value;
        }

        public UserVM User
        {
            get => new UserVM(Model.User);
            set
            {
                if (value != null && Model.User != value.Model)
                {
                    Model.User = value.Model;
                    NotifyPropertyChanged(nameof(User));
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


        public ShortResultVM(ShortResult shortResult)
        {
            Model = shortResult;
        }
    }
}
