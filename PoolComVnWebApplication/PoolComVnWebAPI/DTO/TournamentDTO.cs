namespace PoolComVnWebAPI.DTO
{
    public class TournamentDTO
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int GameTypeId { get; set; }
        public int TournamentTypeId { get; set; }
        public int PlayerNumber { get; set; }
        public int EntryFee { get; set; }
        public int TotalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DrawType { get; set; } = null!;
        public string RaceToString { get; set; } = null!;
        public string PaymentType { get; set; } = null!;
        public DateTime RegistrationDeadline { get; set; }
        public int AccessId { get; set; }
        public int MaxPlayerNumber { get; set; }
        public string Rules { get; set; } = null!;
        public byte Status { get; set; }
        public int ClubId { get; set; }
    }

    public class TournamentBracketOutputDTO {
        public List<MatchOfTournamentOutputDTO> matchOfTournamentOutputDTOs;
    }

    public class MatchOfTournamentOutputDTO
    {
        public int MatchId { get; set; }
        public string MatchCode { get; set; }
        public string P1Country { get; set; }
        public string P2Country { get; set; }
        public string P1Name { get; set; }
        public string P2Name { get; set; }
        public int P1Score { get; set; }
        public int P2Score { get; set; }
        public int? LoseNextMatch { get; set; }
        public int? WinNextMatch { get; set; }
    }
}
