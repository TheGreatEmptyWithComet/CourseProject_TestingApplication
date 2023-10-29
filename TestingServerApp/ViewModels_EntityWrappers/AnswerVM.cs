using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class AnswerVM : NotifyPropertyChangeHandler
    {
        public Answer Model { get; set; }
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

        public bool IsCorrect
        {
            get => Model.IsCorrect;
            set
            {
                if (Model.IsCorrect != value)
                {
                    Model.IsCorrect = value;
                    NotifyPropertyChanged(nameof(IsCorrect));
                }
            }
        }

        private bool multipleAnswersAllowed;
        public bool MultipleAnswersAllowed
        {
            get => multipleAnswersAllowed;
            set
            {
                if (multipleAnswersAllowed != value)
                {
                    multipleAnswersAllowed = value;
                    NotifyPropertyChanged(nameof(MultipleAnswersAllowed));
                }
            }
        }

        private bool isUserAnswered;
        public bool IsUserAnswered
        {
            get => isUserAnswered;
            set
            {
                if (isUserAnswered != value)
                {
                    isUserAnswered = value;
                    NotifyPropertyChanged(nameof(IsUserAnswered));
                }
            }
        }

        public AnswerVM(Answer answer)
        {
            Model = answer;
        }
    }
}
