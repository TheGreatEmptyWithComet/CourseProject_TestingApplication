using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TestingServerApp
{
    class QuestionVM : NotifyPropertyChangeHandler
    {
        public Question Model { get; set; }
        public int Id { get => Model.Id; }

        public string Text
        {
            get => Model.Text;
            set
            {
                if (Model.Text != value)
                {
                    Model.Text = value;
                    NotifyPropertyChanged(nameof(Text));
                }
            }
        }
        public byte[] SetImage
        {
            set
            {
                if (Model.Image != value)
                {
                    Model.Image = value;
                    // Notify that property of type BitmapImage (for reading) is changed
                    NotifyPropertyChanged(nameof(GetImage));
                }
            }
        }
        public BitmapImage? GetImage
        {
            get => Model.Image != null ? ImageConverter.ByteArrayToBitmapImage(Model.Image) : null;
        }
        public bool MultipleAnswersAllowed
        {
            get => Model.MultipleAnswersAllowed;
            set
            {
                if (Model.MultipleAnswersAllowed != value)
                {
                    Model.MultipleAnswersAllowed = value;
                    NotifyPropertyChanged(nameof(MultipleAnswersAllowed));
                }
            }
        }
        public bool PartialAnswersAllowed
        {
            get => Model.PartialAnswersAllowed;
            set
            {
                if (Model.PartialAnswersAllowed != value)
                {
                    Model.PartialAnswersAllowed = value;
                    NotifyPropertyChanged(nameof(PartialAnswersAllowed));
                }
            }
        }
        public int QuestionWeight
        {
            get => Model.QuestionWeight;
            set
            {
                if (Model.QuestionWeight != value)
                {
                    Model.QuestionWeight = value;
                    NotifyPropertyChanged(nameof(Text));
                }
            }
        }
        public TestVM TestCategory
        {
            get => new TestVM(Model.Test);
            set
            {
                if (Model.Test != null && Model.Test != value.Model)
                {
                    Model.Test = value.Model;
                    NotifyPropertyChanged(nameof(TestCategory));
                }
            }
        }


        public QuestionVM(Question question)
        {
            Model = question;
        }
    }
}
