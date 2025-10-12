using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rira.Domain.Enums
{
    public enum TaskStatus
    {
        /// <summary>در انتظار شروع</summary>
        Pending = 1,

        /// <summary>در حال انجام</summary>
        InProgress = 2,

        /// <summary>تکمیل شده</summary>
        Completed = 3,

        /// <summary>لغو شده</summary>
        Cancelled = 4
    }
}
