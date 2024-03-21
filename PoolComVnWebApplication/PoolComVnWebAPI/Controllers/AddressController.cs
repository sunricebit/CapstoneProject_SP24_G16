using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace PoolComVnWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressDAO _addressDAO;

        public AddressController(AddressDAO addressDAO)
        {
            _addressDAO = addressDAO;
        }

        [HttpGet("provinces")]
        public IActionResult GetAllProvinces()
        {
            try
            {
                var provinces = _addressDAO.GetAllProvincesAsync();
                return Ok(provinces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("districts/{provinceCode}")]
        public IActionResult GetDistrictsByProvinceCode(string provinceCode)
        {
            try
            {
                var districts = _addressDAO.GetDistrictsByProvinceCodeAsync(provinceCode);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("wards/{districtCode}")]
        public IActionResult GetWardsByDistrictCode(string districtCode)
        {
            try
            {
                var wards = _addressDAO.GetWardsByDistrictCodeAsync(districtCode);
                return Ok(wards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
