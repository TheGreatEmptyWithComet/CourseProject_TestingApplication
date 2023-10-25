using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TestingServerApp
{
    public class ShortResult
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TestMaxScores { get; set; }
        public double UserScores { get; set; }
        
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Test Test { get; set; } = null!;
        public ObservableCollection<DetailedResult> DetailedResults { get; set; } = null!;
    }
}
