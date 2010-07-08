using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidDumpLocation : GraphDBError
    {
        public String DumpLocation { get; private set; }
        public String[] ValidDumpLocations { get; private set; }

        public Error_InvalidDumpLocation(String myDumpLocation, params String[] myValidDumpLocationPrefixes)
        {
            DumpLocation = myDumpLocation;
            ValidDumpLocations = myValidDumpLocationPrefixes;
        }

        public override string ToString()
        {
            var validLocations = ValidDumpLocations.Aggregate<String, StringBuilder>(new StringBuilder(), (ret, item) =>
            {
                ret.Append(item); ret.Append(", ");
                return ret;
            });
            return "The location [" + DumpLocation + "] is not valid. Expected: " + validLocations;
        }
    }
}
