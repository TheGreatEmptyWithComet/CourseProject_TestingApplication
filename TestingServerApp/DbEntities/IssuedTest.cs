using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class IssuedTest
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int AttemptsAmount { get; set; }
        public UserGroup UserGroup { get; set; } = null!;
        public Test Test { get; set; } = null!;

    }
}
