using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingServerApp.DbEntities;

namespace TestingServerApp
{
    public class UserGroup
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        // Navigation properties
        public ICollection<User> Users { get; set; } = null!;
        public ICollection<IssuedTest> IssuedTests { get; set; } = null!;
    }
}
