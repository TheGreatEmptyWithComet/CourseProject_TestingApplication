using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingClientApp
{
    internal class StoredQuestionRecord
    {
        public string Question { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
        public List<int> CorrectAnswerNumbers { get; set; } = new List<int>();
        
    }
}
