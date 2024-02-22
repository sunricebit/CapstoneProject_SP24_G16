using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
        public class News
        {
            [Key]
            public int NewsID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int AccountID { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
            public Account Account { get; set; }
        }
}
