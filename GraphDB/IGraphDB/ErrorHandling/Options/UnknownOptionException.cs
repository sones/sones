using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.ErrorHandling
{
    public class UnknownOptionException: AGraphDBException
    {
        public String WrongParameter { get; private set; }
        public PluginParameters<Type> AllowedParameters { get; private set; }

        public UnknownOptionException(string myOption, PluginParameters<Type> myParameters)
        {
            WrongParameter = myOption;
            AllowedParameters = myParameters;

            var parameterString = myParameters.Select(_=> string.Format("{0}({1})", _.Key, _.Value.Name));
            _msg = String.Format("The option {0} is not allowed for this plugin. Please use only the following parameters: {1}", myOption, String.Join(", ", parameterString));
        }
    }
}
