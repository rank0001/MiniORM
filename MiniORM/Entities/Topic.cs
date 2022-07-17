using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniORM.Entities
{
    public class Topic:IId
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public List<Session>? Sessions { get; set; }

    }
}
