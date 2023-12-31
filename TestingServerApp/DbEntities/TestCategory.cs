﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TestingServerApp
{
    [Index("Name", IsUnique = true)]
    public class TestCategory
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;


        // Navigation properties
        public ObservableCollection<Test> Tests { get; set; } = null!;
    }
}
