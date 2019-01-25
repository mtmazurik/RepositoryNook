using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Exceptions
{
    public class SigSvcHeaderError: Exception
    {
        public SigSvcHeaderError(string message) : 
            base(message)
        {
        }   
    }
}
