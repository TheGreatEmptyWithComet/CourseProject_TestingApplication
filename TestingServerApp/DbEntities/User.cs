﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingServerApp
{
    [Index("Login", IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [MaxLength(16)]
        public byte[] PasswordSalt { get; set; } = null!;

        [StringLength(64)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        public UserGroup UserGroup { get; set; } = null!;


        // Navigation property
        public ObservableCollection<ShortResult> ShortResults { get; set; } = null!;

    }
}
