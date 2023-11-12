using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestingServerApp
{
    public class TestInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public int QuestionsAmountForTest { get; set; }
        public int MinutsForTest { get; set; }
        public int MaxTestScores { get; set; }
        public string TestCategory { get; set; } = string.Empty;
        public int AttemptsAmount { get; set; }
        public int AttemptsLeft { get; set; } = 0;
        public DateTime ExiredDate { get; set; }
    }
}
