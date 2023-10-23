﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class Context : DbContext
    {
        // Entities
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<IssuedTest> IssuedTests { get; set; }
        public DbSet<ShortResult> ShortResults { get; set; }
        public DbSet<DetailedResult> DetailedResults { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnectionString());
        }

        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder();
            // Set apth to the current directory
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // Get config from file Appconfig.json
            builder.AddJsonFile("Appconfig.json");
            // Create config
            var config = builder.Build();
            // Get connection string
            string connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;

            return connectionString;
        }
    }
}
