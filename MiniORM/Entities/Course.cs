using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniORM.Entities
{
    public class Course:IId
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal Fees { get; set; }
        public List<Topic>? Topics { get; set; }
        public Instructor? Teacher { get; set; }
        public List<AdmissionTest>? Tests { get; set; }

    }
}
