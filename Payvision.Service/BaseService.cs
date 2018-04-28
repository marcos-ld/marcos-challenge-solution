using Payvision.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Service
{
    public class BaseService
    {
        protected ILoggerService Logger { get; private set; }

        public BaseService(ILoggerService loggerService)
        {
            Logger = loggerService;
        }
    }
}
