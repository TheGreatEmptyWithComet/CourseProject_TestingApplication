using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class AnswerVM : NotifyPropertyChangedHandler
    {
        public Answer Model { get; set; }

        public int Id { get => Model.Id; }

        public string Text
        {
            get => Model.Text;
            set=> Model.Text = value;
        }

        public bool IsCorrect
        {
            get => Model.IsCorrect;
            set=> Model.IsCorrect = value;
        }

        public bool MultipleAnswersAllowed
        {
            get => Model.Question != null ? Model.Question.MultipleAnswersAllowed : false;
        }

        public bool IsUserAnswered
        {
            get => Model.IsUserAnswered;
            set=> Model.IsUserAnswered = value;
        }


        public AnswerVM(Answer answer)
        {
            Model = answer;
        }

    }
}
