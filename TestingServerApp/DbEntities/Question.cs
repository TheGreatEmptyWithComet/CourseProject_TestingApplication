using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class Question
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;
        
        [MaxLength(1024)]
        public byte[]? Image { get; set; }

        public bool MultipleAnswersAllowed { get; set; } = false;
        public bool PartialAnswersAllowed { get; set; } = false;
        public int QuestionWeight { get; set; }
        public Test Test { get; set; } = null!;

        
        // Navigation properties
        public ObservableCollection<Answer> Answers { get; set; } = null!;
    }
}
