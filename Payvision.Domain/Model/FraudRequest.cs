using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Domain.Model
{
    public class FraudRequest
    {
        public string Directory;
        public string SearchPattern;
        public string FileName;

        public void Validate(Interfaces.IFraudService fraudService)
        {
            var requestValidation = fraudService.IsValidRequest(this);

            if (!requestValidation.Success)
                throw new ArgumentException(requestValidation.Message);
        }
    }
}
