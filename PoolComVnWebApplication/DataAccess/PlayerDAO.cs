using BusinessObject.Models;
using DataAccess.DTO;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccess
{
    public class PlayerDAO
    {
        private readonly poolcomvnContext _context;
        public List<PlayerDTO> ProcessedPlayers { get; set; } = new List<PlayerDTO>();
        public PlayerDAO(poolcomvnContext context)
        {
            _context = context;
        }

        public void AddPlayer(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _context.Players.Add(player);
            _context.SaveChanges();
        }

       
        public Player GetPlayerById(int playerId)
        {
            return _context.Players.Find(playerId);
        }

       
        public Player GetPlayerByName(string playerName)
        {
            return _context.Players.FirstOrDefault(p => p.PlayerName == playerName);
        }


        
        public List<Player> GetAllPlayers()
        {
            return _context.Players.Include(player => player.User)
                    .ThenInclude(user => user.Account)
                .Include(player => player.Country)
                .ToList();
        }

        
        public void UpdatePlayer(Player updatedPlayer)
        {
            if (updatedPlayer == null)
            {
                throw new ArgumentNullException(nameof(updatedPlayer));
            }

            var existingPlayer = _context.Players.Find(updatedPlayer.PlayerId);

            if (existingPlayer != null)
            {
                // Update player properties
                existingPlayer.PlayerName = updatedPlayer.PlayerName;
                existingPlayer.Level = updatedPlayer.Level;

                // If User and Account properties are not null, update them
                if (updatedPlayer.User != null && updatedPlayer.User.Account != null)
                {
                    existingPlayer.User.Account.PhoneNumber = updatedPlayer.User.Account.PhoneNumber;
                }

                _context.SaveChanges();
            }
            else
            {
                
                throw new ArgumentException($"Player with ID {updatedPlayer.PlayerId} not found");
            }
        }

        
        public void DeletePlayer(int playerId)
        {
            var playerToDelete = _context.Players.Find(playerId);

            if (playerToDelete != null)
            {
                _context.Players.Remove(playerToDelete);
                _context.SaveChanges();
            }
            else
            {
                
                throw new ArgumentException($"Player with ID {playerId} not found");
            }
        }

        public void AddPlayersFromExcel(IEnumerable<PlayerDTO> playerDtos)
        {
            foreach (var playerDto in playerDtos)
            {
                try
                {
                    var playerName = playerDto.PlayerName?.Trim();
                    var countryName = playerDto.CountryName?.Trim();
                    var phoneNumber = playerDto.PhoneNumber?.Trim();
                    var level = playerDto.Level.ToString(); 

                    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(countryName) ||
                        string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(level))
                    {
                        continue;
                    }

                    if (!int.TryParse(level, out int parsedLevel))
                    {
                        Console.WriteLine($"Invalid level format for player {playerName}. Skipping.");
                        continue;
                    }

                    var processedPlayer = new PlayerDTO
                    {
                        PlayerName = playerName,
                        CountryName = countryName,
                        PhoneNumber = phoneNumber,
                        Level = parsedLevel
                    };

                    
                    ProcessedPlayers.Add(processedPlayer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row: {ex.Message}");
                }
            }

        }

        public IEnumerable<Player> GetPlayersByTournament(int tourId)
        {
            try
            {
                var players = _context.Players.Where(p => p.TourId == tourId);
                return players;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
