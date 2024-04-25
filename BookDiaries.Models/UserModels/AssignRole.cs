using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class AssignRole
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool Exist { get; set; }
    }
}
