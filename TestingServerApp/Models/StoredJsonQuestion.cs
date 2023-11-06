using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp.Models
{
    public class StoredJsonQuestion
    {
        public string Text { get; set; }
        public bool PartialAnswersAllowed { get; set; }
        public int QuestionWeight { get; set; }
        public List<string> Answers { get; set; }
        public List<int> CorrectAnswerNumbers { get; set; }
    }
}
