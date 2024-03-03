﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Constant
    {
        public const int SaltRound = 12;
        public const int BusinessRole = 3;
        public const int UserRole = 2;
        public const int AdminRole = 1;
        public const int BusinessManager = 4;

        public const string StrBusinessRole = "Business";
        public const string StrUserRole = "User";
        public const string StrAdminRole = "Admin";
        public const string StrBusinessManagerRole = "BusinessManager";
    }
}
