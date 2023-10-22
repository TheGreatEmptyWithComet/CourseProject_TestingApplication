using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class DetailedResult
    {
        public int Id { get; set; }
        public bool IsAnsweredByUser { get; set; } = false;

        
        // Navigation properties
        public ShortResult ShortResult { get; set; } = null!;
        public Answer Answer { get; set; } = null!;
    }
}
