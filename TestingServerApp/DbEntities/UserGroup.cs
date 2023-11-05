using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    [Index("Name", IsUnique = true)]
    public class UserGroup
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        // Navigation properties
        public ObservableCollection<User> Users { get; set; } = null!;
        public ObservableCollection<IssuedTest> IssuedTests { get; set; } = null!;
    }
}
