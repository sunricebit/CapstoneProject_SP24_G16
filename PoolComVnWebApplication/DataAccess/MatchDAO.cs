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

        public bool CheckExistMatch(int tourId, int matchNumber)
        {
            try
            {
                var match = _context.MatchOfTournaments.FirstOrDefault(m => m.TourId == tourId 
                                                            && m.MatchNumber == matchNumber);
                if (match != null)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public int GetLastest(int tourId, int matchNumber)
        {
            try
            {
                var match = _context.MatchOfTournaments.FirstOrDefault(m => m.TourId == tourId
                                                            && m.MatchNumber == matchNumber);
                return match.MatchId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public MatchOfTournament GetMatchOfTournamentsByNumber(int tourId, int matchNumber)
        {
            try
            {
                var match = _context.MatchOfTournaments.FirstOrDefault(m => m.TourId == tourId
                                                            && m.MatchNumber == matchNumber);
                return match;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateMatch(MatchOfTournament matchOfTournament)
        {
            try
            {
                var match = _context.MatchOfTournaments.FirstOrDefault(m => m.MatchId == matchOfTournament.MatchId);
                match.Status = matchOfTournament.Status;
                match.WinToMatch = matchOfTournament.WinToMatch;
                match.LoseToMatch = matchOfTournament.LoseToMatch;
                match.TableId = matchOfTournament.TableId;
                _context.Update(match);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
