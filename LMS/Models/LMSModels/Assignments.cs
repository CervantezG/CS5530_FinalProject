using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public uint AssignmentId { get; set; }
        public string Name { get; set; }
        public uint Points { get; set; }
        public string Contents { get; set; }
        public DateTime DueDate { get; set; }
        public uint AssignmentCategoryId { get; set; }

        public virtual AssignmentCategories AssignmentCategory { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
