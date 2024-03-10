using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class TableDAO
    {
        private readonly poolcomvnContext _context;

        public TableDAO(poolcomvnContext poolComContext)
        {
            _context = poolComContext;
        }

        // Create
        public void AddTable(Table table, int clubId)
        {
            // Set the clubId for the table
            table.ClubId = clubId;

            _context.Tables.Add(table);
            _context.SaveChanges();
        }

        // Read
        public Table GetTableById(int id)
        {
            return _context.Tables.Find(id);
        }

        public List<Table> GetAllTables()
        {
            return _context.Tables.ToList();
        }

        public List<Table> GetAllTablesForClub(int clubId)
        {
            return _context.Tables.Where(t => t.ClubId == clubId).ToList();
        }
        // Update
        public void UpdateTable(Table updatedTable)
        {
            var existingTable = _context.Tables.Find(updatedTable.TableId);

            if (existingTable != null)
            {
                // Update properties as needed
                existingTable.TableName = updatedTable.TableName;
                existingTable.ClubId = updatedTable.ClubId;
                existingTable.TagName = updatedTable.TagName;
                existingTable.Status = updatedTable.Status;
                existingTable.Size = updatedTable.Size;
                existingTable.Image = updatedTable.Image;
                existingTable.IsScheduling = updatedTable.IsScheduling;
                existingTable.IsUseInTour = updatedTable.IsUseInTour;

                _context.SaveChanges();
            }
        }

        // Delete
        public void DeleteTable(int tableId)
        {
            var tableToDelete = _context.Tables.Find(tableId);

            if (tableToDelete != null)
            {
                _context.Tables.Remove(tableToDelete);
                _context.SaveChanges();
            }
        }
    }
}
