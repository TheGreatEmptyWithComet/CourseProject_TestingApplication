using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TestingServerApp
{
    public class QuestionVM : NotifyPropertyChangedHandler
    {
        public Question Model { get; set; }

        public int Id { get => Model.Id; }

        public string Text
        {
            get => Model.Text;
            set => Model.Text = value;
        }

        public byte[]? Image
        {
            get => Model.Image ?? null;
            set => Model.Image = value;
        }

        public string? ImagePath
        {
            get => Model.ImagePath ?? null;
            set
            {
                if (Model.ImagePath != value)
                {
                    Model.ImagePath = value;
                    NotifyPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public bool MultipleAnswersAllowed
        {
            get => Model.MultipleAnswersAllowed;
            set
            {
                Model.MultipleAnswersAllowed = value;
                NotifyPropertyChanged(nameof(Answers));
            }

        }

        public bool PartialAnswersAllowed
        {
            get => Model.PartialAnswersAllowed;
            set => Model.PartialAnswersAllowed = value;
        }

        public int QuestionWeight
        {
            get => Model.QuestionWeight;
            set => Model.QuestionWeight = value;
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

        public ObservableCollection<AnswerVM> Answers
        {
            get
            {
                if (Model.Answers != null)
                {
                    return new ObservableCollection<AnswerVM>(Model.Answers.Select((a) => new AnswerVM(a)));
                }
                else
                {
                    return new ObservableCollection<AnswerVM>();
                }
            }
        }


        public QuestionVM(Question question)
        {
            Model = question;
        }


        public void NotifyAnswersChanged()
        {
            NotifyPropertyChanged(nameof(Answers));
        }
    }
}
