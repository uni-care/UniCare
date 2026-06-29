using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with ID {key} not found.")
        {
        }
    }

}
