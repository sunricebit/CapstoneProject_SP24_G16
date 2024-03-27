using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SoloMatchDAO
    {
        private readonly poolcomvnContext _context;
        public SoloMatchDAO(poolcomvnContext context)
        {
            _context = context;
        }
        public void AddSoloMatch(SoloMatch soloMatch)
        {
            _context.SoloMatches.Add(soloMatch);
            _context.SaveChanges();
        }
        public SoloMatch GetSoloMatchById(int solomatchId)
        {
            return _context.SoloMatches.FirstOrDefault(p => p.SoloMatchId == solomatchId);
        }
        public List<SoloMatch> GetAllSoloMatchByClubID(int clubID)
        {
            return _context.SoloMatches.Where(s => s.ClubId == clubID).ToList();
        }
        public SoloMatch GetLastestSoloMatch()
        {
            try
            {
                var soloMatch = _context.SoloMatches.OrderByDescending(e => e.SoloMatchId).FirstOrDefault();

                return soloMatch;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
