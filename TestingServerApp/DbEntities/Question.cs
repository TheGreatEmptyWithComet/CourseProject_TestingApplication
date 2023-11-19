using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public byte[]? Image { get; set; }
        
        [MaxLength(2048)]
        public string? ImagePath { get; set; }

        public bool MultipleAnswersAllowed { get; set; } = false;
        public bool PartialAnswersAllowed { get; set; } = false;
        public int QuestionWeight { get; set; }
        public Test Test { get; set; } = null!;
        
        [NotMapped]
        public UserAnswerType UserAnswerType { get; set; }


        // Navigation properties
        public ObservableCollection<Answer> Answers { get; set; } = null!;
    }
}
