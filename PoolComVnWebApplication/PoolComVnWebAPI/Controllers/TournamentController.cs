using BusinessObject.Models;
using DataAccess;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolComVnWebAPI.DTO;
using System.IdentityModel.Tokens.Jwt;

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

        [HttpGet("{tourId}")]
        public IActionResult ViewTournament(int tourId)
        {
            try
            {
                Tournament p = _tournamentDAO.GetTournament(tourId);

                return Ok(p);
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        [HttpPost("{tourId}")]
        public IActionResult UpdateTournament()
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

            int clubId = _clubDAO.GetClubIdByAccountId(Int32.Parse(account.Value));

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

        public async Task<ActionResult> CreateTourStFour([FromForm] List<IFormFile> banner, int tourID)
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
                if (banner != null)
                {
                    foreach (var ban in banner)
                    {
                        if (ban != null && ban.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ban.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                            using (FileStream memoryStream = new FileStream(filePath, FileMode.Create))
                            {
                                ban.CopyTo(memoryStream);


                            }
                            var fileStream2 = new FileStream(filePath, FileMode.Open);
                            var downloadLink = await UploadFromFirebase(fileStream2, ban.FileName);
                            fileStream2.Close();
                            System.IO.File.Delete(filePath);
                            Tournament tour = _tournamentDAO.GetTournament(tourID);
                            tour.Flyer = downloadLink;
                            _tournamentDAO.UpdateTournament(tour);
                        }


                    }
                }

                //  _tournamentDAO.CreateTournament(tour);
                //return Ok(_tournamentDAO.GetLastestTournament().TourId);
                return Ok(tourID);
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
