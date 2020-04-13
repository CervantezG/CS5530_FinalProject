using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Users
    {
        public uint UId { get; set; }

        public virtual Administrators Administrators { get; set; }
        public virtual Professors Professors { get; set; }
        public virtual Students Students { get; set; }
    }
}
