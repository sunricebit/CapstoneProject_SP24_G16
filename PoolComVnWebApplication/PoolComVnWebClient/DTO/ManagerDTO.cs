namespace PoolComVnWebClient.DTO
{
    public class ManagerDTO
    {
        public IEnumerable<AccountDTO> Accounts { get; set; }
        public IEnumerable<ClubDTO> Clubs { get; set; }
        public IEnumerable<PlayerDTO> Players { get; set; }
    }
}
