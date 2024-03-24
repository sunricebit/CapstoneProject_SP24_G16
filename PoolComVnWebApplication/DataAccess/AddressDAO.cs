using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AddressDAO
    {
        private readonly poolcomvnContext _context;

        public AddressDAO(poolcomvnContext context)
        {
            _context = context;
        }

        public  List<Province> GetAllProvincesAsync()
        {
            return  _context.Provinces.ToList();
        }

        public List<District> GetDistrictsByProvinceCodeAsync(string provinceCode)
        {
            return  _context.Districts.Where(d => d.ProvinceCode == provinceCode).ToList();
        }

        public List<Ward> GetWardsByDistrictCodeAsync(string districtCode)
        {
            return _context.Wards.Where(w => w.DistrictCode == districtCode).ToList();
        }

        public Ward GetWardsByWardCode(string? wardCode)
        {
            return _context.Wards.FirstOrDefault(w => w.Code == wardCode);
        }
    }
}
