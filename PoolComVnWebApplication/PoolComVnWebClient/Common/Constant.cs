using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolComVnWebClient.Common
{
    public class Constant
    {
        public const int SaltRound = 12;
        public const int BusinessRole = 3;
        public const int UserRole = 2;
        public const int AdminRole = 1;
        public const int BusinessManagerRole = 4;

        public const string StrBusinessRole = "Business";
        public const string StrUserRole = "User";
        public const string StrAdminRole = "Admin";
        public const string StrBusinessManagerRole = "BusinessManager";

        public const int TournamentIncoming = 0;
        public const int TournamentInProgress = 1;
        public const int TournamentComplete = 2;
        public const byte SvCreateTourStepOne = 11;
        public const byte SvCreateTourStepTwo = 12;
        public const byte SvCreateTourStepThree = 13;
        public const byte SvCreateTourStepFour = 14;
        public const byte SvCreateTourStepFive = 15;
        public const byte SvCreateTourStepSix = 16;
        public const byte CreateTourStepOne = 1;
        public const byte CreateTourStepTwo = 2;
        public const byte CreateTourStepThree = 3;
        public const byte CreateTourStepFour = 4;
        public const byte CreateTourStepFive = 5;
        public const byte CreateTourStepSix = 6;

        public const int AccessPublic = 1;
        public const int AccessPrivate = 2;

        public const int SysRandomDraw = 1;
        public const int UserRandomDraw = 2;
        public const int UserCustom = 3;

        public const int Game8Ball = 1;
        public const int Game9Ball = 2;
        public const int Game10Ball = 3;

        public const int SingleEliminate = 1;
        public const int DoubleEliminate = 2;

        public const string ApiUrl = "https://localhost:5000/api";
    }
}
