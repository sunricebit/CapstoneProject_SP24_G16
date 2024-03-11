using BusinessObject.Models;
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

        public const int TournamentIncoming = 0;
        public const int TournamentInProgress = 1;
        public const int TournamentComplete = 2;

        public const int AccessPublic = 1;
        public const int AccessPrivate = 2;

        public const int SysRandomDraw = 1;
        public const int UserRandomDraw = 2;
        public const int UserCustom = 3;

        public const int Game8Ball = 1;
        public const int Game9Ball = 2;
        public const int Game10Ball = 3;
        public const string String8Ball = "8 bi";
        public const string String9Ball = "9 bi";
        public const string String10Ball = "10 bi";

        public const int SingleEliminate = 1;
        public const int DoubleEliminate = 2;

    }
}
