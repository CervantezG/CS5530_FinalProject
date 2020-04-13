using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public uint UId { get; set; }
        public uint AssignmentId { get; set; }
        public uint Score { get; set; }
        public DateTime SubmissionTime { get; set; }
        public string Contents { get; set; }

        public virtual Assignments Assignment { get; set; }
        public virtual Students U { get; set; }
    }
}
