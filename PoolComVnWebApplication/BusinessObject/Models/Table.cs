using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Table
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = null!;
        public int ClubId { get; set; }

        public virtual Club Club { get; set; } = null!;
    }
}
