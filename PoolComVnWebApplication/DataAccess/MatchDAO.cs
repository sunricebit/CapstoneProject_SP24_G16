using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MatchDAO
    {
        private readonly poolcomvnContext _context;

        public MatchDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        public List<MatchOfTournament> GetMatchOfTournaments(int tourId)
        {
            try
            {
                var lstMatchOfTournament = _context.MatchOfTournaments.Where(item => item.TourId == tourId);

                return lstMatchOfTournament.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AddMatch(MatchOfTournament match)
        {
            try
            {
                var lstMatchOfTournament = _context.MatchOfTournaments.Add(match);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
