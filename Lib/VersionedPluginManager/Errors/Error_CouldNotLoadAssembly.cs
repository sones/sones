using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.Lib.VersionedPluginManager.Errors
{
    /// <summary>
    /// This error occurs if a assembly could not be loaded due to a incompatible platform etc.
    /// </summary>
    public class Error_CouldNotLoadAssembly : GeneralError
    {

        private   string _AssemblyFile;

        public Error_CouldNotLoadAssembly()
        {
        }

        public Error_CouldNotLoadAssembly(string myAssemblyFile)
        {
            this._AssemblyFile = myAssemblyFile;
        }

        public override string ToString()
        {
            return "Could not load the file \"" + _AssemblyFile + "\"";
        }

    }
}
