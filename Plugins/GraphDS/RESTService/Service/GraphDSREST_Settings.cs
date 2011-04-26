
#region Usings

using System;
using sones.Library.LanguageExtensions;

#endregion

namespace sones.Plugins.GraphDS.RESTService
{

    public class GraphDSREST_Settings
    {

        public String Username { get; set; }
        //public String Password { get; set; }
        public String DBName { get; set; }
        public GraphDSREST_OutputFormat OutputFormat { get; set; }

        public override String ToString()
        {
            return String.Format("user={0},password={1},db={2},outputformat={3}", "", "", DBName, OutputFormat);
        }

        public String ToBase64()
        {
            return ToString().ToBase64();
        }

    }

}
