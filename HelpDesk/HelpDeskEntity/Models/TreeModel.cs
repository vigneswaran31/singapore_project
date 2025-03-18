using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity.Models
{
    public class TreeModel
    {
        public int id { get; set; }
        public string text { get; set; }

        public bool children { get; set; }
    }
}
