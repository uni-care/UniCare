using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Enums
{
    public enum ItemStatus
    {
        Draft = 0,
        Available = 1,
        Rented = 2,
        Unavailable = 3,
        Archived = 4
    }
}
