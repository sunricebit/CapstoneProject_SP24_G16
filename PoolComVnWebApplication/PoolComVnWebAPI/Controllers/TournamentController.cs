﻿using BusinessObject.Models;
using DataAccess;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using static PoolComVnWebAPI.DTO.CreateTourStepOneDTO;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly TournamentDAO _tournamentDAO;
        private readonly ClubDAO _clubDAO;
        private static string ApiKey = "AIzaSyDbVNJE6bbQdXlcr3TZqxkZh3xqi5CqKIc";
        private static string Bucket = "poolcomvn-82664.appspot.com";
        private static string AuthEmail = "vuducduy@gmail.com";
        private static string AuthPassword = "123456";

        public TournamentController(TournamentDAO tournamentDAO, ClubDAO clubDAO)
        {
            _tournamentDAO = tournamentDAO;
            _clubDAO = clubDAO;
        }

        [HttpGet("GetAllTournament")]
        [Authorize]
        public IActionResult Index()
        {
            List<Tournament> tournaments = _tournamentDAO.GetAllTournament().ToList();
            return Ok(tournaments);
        }

        [HttpGet("GetTournament")]
        public IActionResult GetTournament(int tourId)
        {
            try
            {
                Tournament tour = _tournamentDAO.GetTournament(tourId);
                TournamentDetailDTO tournamentDetailDTO = new TournamentDetailDTO() {
                    Address = tour.Club.Address,
                    ClubName = tour.Club.ClubName,
                    TournamentId = tour.TourId,
                    TournamentName = tour.TourName,
                    Description = tour.Description,
                    StartTime = tour.StartDate,
                    EndTime = tour.EndDate,
                    Flyer = tour.Flyer,
                    GameType = tour.GameTypeId == Constant.Game8Ball ? Constant.String8Ball
                                    : (tour.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                    Status = tour.Status,
                    TourTypeId = tour.TournamentTypeId,
                    RaceWin = GetRaceWinNumbers(tour.RaceToString),
                    RaceLose = GetRaceLoseNumbers(tour.RaceToString),
                    RegisterDate = tour.RegistrationDeadline,
                    MaxPlayer = tour.MaxPlayerNumber,
                    Access = tour.Access == null ? true : tour.Access.Value,
                    EntryFee = tour.EntryFee,
                    TotalPrize = tour.TotalPrize,
                };
                return Ok(tournamentDetailDTO);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        private List<RaceNumber> GetRaceWinNumbers(string raceString)
        {
            List<RaceNumber> raceNumbers = new List<RaceNumber>();

            string[] parts = raceString.Split(',');

            foreach (var part in parts)
            {
                string[] subParts = part.Split('-');

                if (subParts.Length == 2 && int.TryParse(subParts[1], out int gameToWin) && subParts[0].StartsWith("W"))
                {
                    string round = "R" + subParts[0].Substring(1);
                    RaceNumber raceNumber = new RaceNumber
                    {
                        Round = subParts[0],
                        GameToWin = gameToWin
                    };

                    raceNumbers.Add(raceNumber);
                }
            }

            raceNumbers.Last().Round = "CK";
            return raceNumbers;
        }

        private List<RaceNumber> GetRaceLoseNumbers(string raceString)
        {
            List<RaceNumber> raceNumbers = new List<RaceNumber>();

            string[] parts = raceString.Split(',');

            foreach (var part in parts)
            {
                string[] subParts = part.Split('-');

                if (subParts.Length == 2 && int.TryParse(subParts[1], out int gameToWin) && subParts[0].StartsWith("L"))
                {
                    string round = "R" + subParts[0].Substring(1);
                    RaceNumber raceNumber = new RaceNumber
                    {
                        Round = subParts[0],
                        GameToWin = gameToWin
                    };

                    raceNumbers.Add(raceNumber);
                }
            }

            return raceNumbers;
        }

        [HttpPost]
        public IActionResult UpdateTournament([FromBody] TournamentDTO tournamentDTO)
        {
            return Ok();
        }

        [HttpPost("CreateTourStOne")]
        [Authorize]
        public IActionResult CreateTourStOne([FromBody] CreateTourStepOneDTO inputDto)
        {
            // Lấy giá trị token từ header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Giải mã token để lấy các claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Xử lý logic của bạn với các claims
            var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            if (!Constant.BusinessRole.ToString().Equals(roleClaim.Value))
            {
                return BadRequest("Unauthorize");
            }
            var club = _clubDAO.GetClubByAccountId(Int32.Parse(account.Value));
            int clubId = club.ClubId;

            try
            {
                Tournament tour = new Tournament()
                {
                    TourName = inputDto.TournamentName,
                    Access = inputDto.Access,
                    ClubId = clubId,
                    Description = inputDto.Description,
                    StartDate = inputDto.StartTime,
                    EndDate = inputDto.EndTime,
                    EntryFee = inputDto.EntryFee.Value,
                    KnockoutPlayerNumber = inputDto.TournamentTypeId == Constant.DoubleEliminate ? inputDto.KnockoutNumber : null,
                    GameTypeId = inputDto.GameTypeId,
                    TotalPrize = inputDto.PrizeMoney,
                    TournamentTypeId = inputDto.TournamentTypeId,
                    MaxPlayerNumber = inputDto.MaxPlayerNumber,
                    RegistrationDeadline = inputDto.RegistrationDeadline,
                    RaceToString = inputDto.RaceNumberString,
                    Status = Constant.TournamentIncoming,
                };
                _tournamentDAO.CreateTournament(tour);
                return Ok(_tournamentDAO.GetLastestTournament().TourId);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        [HttpPost("CreateTourStFour")]
       // [Authorize]

        public async Task<ActionResult> CreateTourStFour([FromBody] CreateTourStepFourDTO BannerDTO)
        {
           
            //var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

       
            //var handler = new JwtSecurityTokenHandler();
            //var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

           
            //var roleClaim = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Role"));
            //var account = jsonToken?.Claims.FirstOrDefault(claim => claim.Type.Equals("Account"));
            ////if (!Constant.BusinessRole.ToString().Equals(roleClaim.Value))
            ////{
            ////    return BadRequest("Unauthorize");
            ////}

            //int clubId = _clubDAO.GetClubIdByAccountId(Int32.Parse(account.Value));

            try
            {
                
                    
                       
                            Tournament tour = _tournamentDAO.GetTournament(BannerDTO.TourID);
                            tour.Flyer = BannerDTO.Flyer;
                            _tournamentDAO.UpdateTournament(tour);
                        


                    
                

                //  _tournamentDAO.CreateTournament(tour);
                //return Ok(_tournamentDAO.GetLastestTournament().TourId);
                return Ok(BannerDTO.TourID);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        private async Task<string> UploadFromFirebase(FileStream stream, string filename)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                }
                ).Child("Tournaments")
                 .Child(filename)
                 .PutAsync(stream, cancellation.Token);
            try
            {
                string link = await task;
                return link;

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception was thrown : {0}", ex);
                return null;
            }
        }


        [HttpGet("GetAllTour")]
        public IActionResult GetAllTour()
        {
            try
            {
                List<TournamentOutputDTO> allTourDto = new List<TournamentOutputDTO>();
                var allTourLst = _tournamentDAO.GetAllTournament();
                foreach(var item in allTourLst)
                {
                    TournamentOutputDTO tour = new TournamentOutputDTO()
                    {
                        TournamentId = item.TourId,
                        TournamentName = item.TourName,
                        StartTime = item.StartDate,
                        EndTime = item.EndDate,
                        Address = item.Club.Address,
                        ClubName = item.Club.ClubName,
                        Description = item.Description,
                        GameType = item.GameTypeId == Constant.Game8Ball ? Constant.String8Ball 
                                    : (item.GameTypeId == Constant.Game9Ball ? Constant.String9Ball : Constant.String10Ball),
                        Status = item.Status,
                        Flyer = item.Flyer,
                    };
                    allTourDto.Add(tour);
                }
                return Ok(allTourDto);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
