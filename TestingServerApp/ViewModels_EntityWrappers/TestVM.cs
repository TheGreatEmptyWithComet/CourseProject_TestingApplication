using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TestingServerApp
{
    public class TestVM : NotifyPropertyChangeHandler
    {
        public Test Model { get; set; }

        public int Id { get => Model.Id; }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public byte[]? Image
        {
            get => Model.Image ?? null;
            set
            {
                if (Model.Image != value)
                {
                    Model.Image = value;
                    NotifyPropertyChanged(nameof(Image));
                }
            }
        }
        
        public int QuestionsAmountForTest
        {
            get => Model.QuestionsAmountForTest;
            set=> Model.QuestionsAmountForTest = value;
        }

        public int MinutsForTest
        {
            get => Model.MinutsForTest;
            set
            {
                if (Model.MinutsForTest != value)
                {
                    Model.MinutsForTest = value;
                    NotifyPropertyChanged(nameof(MinutsForTest));
                }
            }
        }
        public int MaxTestScores
        {
            get => Model.MaxTestScores;
            set
            {
                if (Model.MaxTestScores != value)
                {
                    Model.MaxTestScores = value;
                    NotifyPropertyChanged(nameof(MaxTestScores));
                }
            }
        }
        public TestCategoryVM TestCategory
        {
            get => new TestCategoryVM(Model.TestCategory);
            set
            {
                if (value != null && Model.TestCategory != value.Model)
                {
                    Model.TestCategory = value.Model;
                    NotifyPropertyChanged(nameof(TestCategory));
                }
            }
        }


        public TestVM(Test test)
        {
            Model = test;
        }
    }
}
