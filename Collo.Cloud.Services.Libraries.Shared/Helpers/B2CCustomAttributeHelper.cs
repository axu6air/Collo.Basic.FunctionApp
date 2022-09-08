using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public class B2CCustomAttributeHelper
    {
        internal readonly string _b2cExtensionAppClientId;

        B2CCustomAttributeHelper(string b2cExtensionAppClientId)
        {
            _b2cExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");
        }

        public string GetCompleteAttributeName(string attributeName)
        {
            return $"extension_{_b2cExtensionAppClientId}_{attributeName}";
        }
    }
}
