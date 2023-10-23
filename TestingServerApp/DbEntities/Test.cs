﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class Test
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1024)]
        public byte[]? Image { get; set; }

        public int QuestionsAmountForTest { get; set; }
        public int MinutsForTest { get; set; }
        public int MaxTestScores { get; set; }

        
        // Navigation properties
        public TestCategory TestCategory { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = null!;
        public ICollection<IssuedTest> IssuedTests { get; set; } = null!;
        public ICollection<ShortResult> ShortResult { get; set; } = null!;
    }
}