using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TournamentDAO
    {
        private readonly poolcomvnContext _context;

        public TournamentDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        public Tournament GetTournament(int tourId)
        {
            try
            {
                var tournament = _context.Tournaments.Include(x => x.Club)
                    .FirstOrDefault(item => item.TourId == tourId);

                return tournament;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Tournament GetLastestTournament()
        {
            try
            {
                var tournament = _context.Tournaments.OrderByDescending(e => e.TourId).FirstOrDefault();

                return tournament;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Tournament> GetAllTournament()
        {
            try
            {
                var tournaments = _context.Tournaments.Include(t => t.Club).ToList();
                return tournaments;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void UpdateTournament(Tournament tournament)
        {
            try
            {
                var updateTournament = _context.Tournaments.FirstOrDefault(t => t.TourId == tournament.TourId);
                updateTournament = tournament;
                _context.Tournaments.Update(updateTournament);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void CreateTournament(Tournament tournament)
        {
            try
            {
                var updateTournament = _context.Tournaments.FirstOrDefault(t => t.TourId == tournament.TourId);
                updateTournament = tournament;
                _context.Tournaments.Add(updateTournament);
                _context.SaveChanges();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //public bool IsTournamentExist(Tournament tournament)
        //{
        //    try
        //    {
        //        var updateTournament = _context.Tournaments.FirstOrDefault(t => t.TourId == tournament.TourId);
        //        updateTournament = tournament;
        //        _context.Tournaments.Update(updateTournament);
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
        //}

        public void DeleteTournament(int tourId)
        {
            try
            {
                var removeTournament = _context.Tournaments.FirstOrDefault(t => t.TourId == tourId);

                _context.Tournaments.Remove(removeTournament);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
