using BusinessObject.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccess
{
    public class PlayerDAO
    {
        private readonly poolcomvnContext _context;

        public PlayerDAO(poolcomvnContext context)
        {
            _context = context;
        }

        // CREATE
        public void AddPlayer(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _context.Players.Add(player);
            _context.SaveChanges();
        }

        // READ
        public Player GetPlayerById(int playerId)
        {
            return _context.Players.Find(playerId);
        }

        public List<Player> GetAllPlayers()
        {
            return _context.Players.ToList();
        }

        // UPDATE
        public void UpdatePlayer(Player updatedPlayer)
        {
            if (updatedPlayer == null)
            {
                throw new ArgumentNullException(nameof(updatedPlayer));
            }

            var existingPlayer = _context.Players.Find(updatedPlayer.PlayerId);

            if (existingPlayer != null)
            {
                existingPlayer.PlayerName = updatedPlayer.PlayerName;
                existingPlayer.AccountId = updatedPlayer.AccountId;
                existingPlayer.Level = updatedPlayer.Level;

                _context.SaveChanges();
            }
            else
            {
                // Handle the case where the player with the given ID doesn't exist
                throw new ArgumentException($"Player with ID {updatedPlayer.PlayerId} not found");
            }
        }

        // DELETE
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
                // Handle the case where the player with the given ID doesn't exist
                throw new ArgumentException($"Player with ID {playerId} not found");
            }
        }

        // IMPORT PLAYERS FROM EXCEL
        public void ImportPlayersFromExcel(Stream stream)
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // Skip the header row
                reader.Read();

                while (reader.Read())
                {
                    try
                    {
                        var playerName = reader.GetString(0)?.Trim();
                        var accountIDText = reader.GetString(1)?.Trim();
                        var level = reader.GetString(2)?.Trim();

                        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(accountIDText) || string.IsNullOrEmpty(level))
                        {
                            continue;
                        }

                        if (!int.TryParse(accountIDText, out int accountID))
                        {
                            continue;
                        }

                        var player = new Player
                        {
                            PlayerName = playerName,
                            AccountId = accountID,
                            Level = int.Parse(level)
                        };

                        _context.Players.Add(player);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing row: {ex.Message}");
                    }
                }

                // Save changes after processing all rows
                _context.SaveChanges();
            }
        }
    }
}
