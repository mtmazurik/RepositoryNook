using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Exceptions
{

    public class ConfigFileReadError : Exception
    {
        public ConfigFileReadError()
        {
        }
        public ConfigFileReadError(string message)
            : base(message)
        {
        }
    }
}

