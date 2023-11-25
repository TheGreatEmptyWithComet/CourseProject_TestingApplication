using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingClientApp
{
    internal class AnswerOption : NotifyPropertyChangedHandler
    {
        public string Answer { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; } = false;
        public AnswerType Type { get; private set; }

        private bool isUserChecked = false;
        public bool IsUserChecked
        {
            get { return isUserChecked; }
            set
            {
                if (isUserChecked != value)
                {
                    isUserChecked = value;
                    NotifyPropertyChanged(nameof(IsUserChecked));
                }
            }
        }

        public AnswerOption(string answer, bool isCorrect, AnswerType type)
        {
            Answer = answer;
            IsCorrect = isCorrect;
            Type = type;
        }

    }
}
