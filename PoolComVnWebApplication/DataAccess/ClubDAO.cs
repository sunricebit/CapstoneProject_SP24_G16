﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class ClubDAO
    {
        private readonly PoolComContext _context;

        public ClubDAO(PoolComContext poolComContext)
        {
            _context = poolComContext;
        }

        // Create
        public void AddClub(Club club)
        {
            _context.Clubs.Add(club);
            _context.SaveChanges();
        }

        // Read
        public Club GetClubById(int clubId)
        {
            return _context.Clubs.Find(clubId);
        }

        public List<Club> GetAllClubs()
        {
            return _context.Clubs.ToList();
        }

        // Update
        public void UpdateClub(Club updatedClub)
        {
            var existingClub = _context.Clubs.Find(updatedClub.ClubId);

            if (existingClub != null)
            {
                // Avoid updating ClubId
                existingClub.ClubName = updatedClub.ClubName;
                existingClub.Address = updatedClub.Address;
                existingClub.Phone = updatedClub.Phone;
                existingClub.Facebook = updatedClub.Facebook;
                existingClub.Avatar = updatedClub.Avatar;

                _context.SaveChanges();
            }
        }

        // Delete
        public void DeleteClub(int clubId)
        {
            var clubToDelete = _context.Clubs.Find(clubId);

            if (clubToDelete != null)
            {
                _context.Clubs.Remove(clubToDelete);
                _context.SaveChanges();
            }
        }
    }
}