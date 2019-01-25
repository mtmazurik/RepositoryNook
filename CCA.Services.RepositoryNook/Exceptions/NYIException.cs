using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Exceptions
{

    public class NYIException : Exception
    {
        public NYIException()
        {
        }
        public NYIException(string message)
            : base(message)
        {
        }
    }
}

