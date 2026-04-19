using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.ChatAggregate
{
    public enum MessageType
    {
        Text = 1,

        /// System-generated event message injected automatically at key lifecycle moments,
        /// e.g. "Handover verified at 14:32" or "Rental return confirmed."
        SystemEvent = 2
    }
}
