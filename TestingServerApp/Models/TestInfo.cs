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
        public DateTime ExiredDate { get; set; }

        public TestInfo(IssuedTest issuedTest)
        {
            Id = issuedTest.Test.Id;
            Name = issuedTest.Test.Name;
            Image = issuedTest.Test.ImagePath != null ? File.ReadAllBytes(issuedTest.Test.ImagePath) : null;
            QuestionsAmountForTest = issuedTest.Test.QuestionsAmountForTest;
            MinutsForTest = issuedTest.Test.MinutsForTest;
            MaxTestScores = issuedTest.Test.MaxTestScores;
            TestCategory = issuedTest.Test.TestCategory.Name;
            AttemptsAmount = issuedTest.AttemptsAmount;
            ExiredDate = issuedTest.ExpiredDate;
        }
    }
}
