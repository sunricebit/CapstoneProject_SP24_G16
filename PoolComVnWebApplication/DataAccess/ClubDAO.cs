﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class ClubDAO
    {
        private readonly poolcomvnContext _context;

        public ClubDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        // Create
        public void AddClub(Club club)
        {
            _context.Clubs.Add(club);
            _context.SaveChanges();
            _context.MatchOfTournaments.Include( m => m.PlayerInMatches).ToList();

        }
        public List<MatchOfTournament> matchOfTournaments()
        {
            try
            {
                return _context.MatchOfTournaments.Include(m => m.PlayerInMatches).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public Club GetClubById(int clubId)
        {
            return _context.Clubs.Find(clubId);
        }
        public Account GetAccount(int AccID)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == AccID);
            return account;
        }
        public Club GetClubByName(string name)
        {
            try
            {
                
                var club = _context.Clubs.FirstOrDefault(c => c.ClubName == name);

                return club;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin câu lạc bộ: {ex.Message}");
                return null;
            }
        }
        public List<Club> GetClubsBySearch(string searchQuery)
        {
            try
            {
                searchQuery = searchQuery.ToLower();

                var clubs = _context.Clubs
                                    .Where(c => c.ClubName.ToLower().Contains(searchQuery) 
                                    || c.Address.ToLower().Contains(searchQuery))
                                    .ToList();
                return clubs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm câu lạc bộ: {ex.Message}");
                return null;
            }
        }
        public List<Club> GetAllClubs()
        {
            return _context.Clubs.ToList();
        }
        public void UpdateClub(Club updatedClub)
        {
            var existingClub = _context.Clubs.Find(updatedClub.ClubId);

            if (existingClub != null)
            {
                existingClub.ClubName = updatedClub.ClubName;
                existingClub.Address = updatedClub.Address;
                existingClub.Phone = updatedClub.Phone;
                existingClub.Facebook = updatedClub.Facebook;
                existingClub.Avatar = updatedClub.Avatar;

                _context.SaveChanges();
            }
        }

        public void DeleteClub(int clubId)
        {
            var clubToDelete = _context.Clubs.Find(clubId);

            if (clubToDelete != null)
            {
                _context.Clubs.Remove(clubToDelete);
                _context.SaveChanges();
            }
        }

        public Club GetClubByAccountId(int accountId)
        {
            var club = _context.Clubs.FirstOrDefault(c => c.AccountId == accountId);

            return club;
        }
    }
}
