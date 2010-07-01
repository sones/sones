using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidImportLocation : GraphDBError
    {
        public String ImportLocation { get; private set; }
        public String[] ValidImportLocations { get; private set; }

        public Error_InvalidImportLocation(String myImportLocation, params String[] myValidImportLocationPrefixes)
        {
            ImportLocation = myImportLocation;
            ValidImportLocations = myValidImportLocationPrefixes;
        }

        public override string ToString()
        {
            var validLocations = ValidImportLocations.Aggregate<String, StringBuilder>(new StringBuilder(), (ret, item) =>
            {
                ret.Append(item); ret.Append(", ");
                return ret;
            });
            return "The location [" + ImportLocation + " is not valid. Expected: " + validLocations;
        }
    }
}
