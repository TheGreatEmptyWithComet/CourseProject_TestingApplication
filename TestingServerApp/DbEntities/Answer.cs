using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class Answer
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string Text { get; set; } = string.Empty;

        [NotMapped]
        public byte[]? Image { get; set; }

        [MaxLength(2048)]
        public string? ImagePath { get; set; }

        public bool IsCorrect { get; set; } = false;

        [NotMapped]
        public bool IsUserAnswered { get; set; } = false;


        // Navigation properties
        public Question Question { get; set; } = null!;
    }
}
