using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingClientApp
{
   public enum AnswerType { SingleAnswer, MultipleAnswers }
    internal class QuizQuestionRecord
    {
        public string Question { get; set; }
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; } = new ObservableCollection<AnswerOption>();
        public bool IsCorrectlyAnswered { get; set; } = false;
        
        public QuizQuestionRecord(StoredQuestionRecord questionRecord)
        {
            Question = questionRecord.Question;
            FillAnswerOptions(questionRecord);
        }

        // transform stored data to format that is usable in wpf forms
        private void FillAnswerOptions(StoredQuestionRecord questionRecord)
        {
            // set question type
            AnswerType type;
            if (questionRecord.CorrectAnswerNumbers.Count == 1)
            {
                type = AnswerType.SingleAnswer;
            }
            else
            {
                type = AnswerType.MultipleAnswers;
            }

            // set list of answer options
            bool isCorrectAnswer;

            for (int i = 0; i < questionRecord.Answers.Count; ++i)
            {
                if (questionRecord.CorrectAnswerNumbers.Contains(i + 1))
                {
                    isCorrectAnswer = true;
                }
                else
                {
                    isCorrectAnswer = false;
                }

                AnswerOptions.Add(new AnswerOption(questionRecord.Answers[i], isCorrectAnswer, type));
            }
        }
    }
}
