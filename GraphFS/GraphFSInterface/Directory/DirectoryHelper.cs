using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

namespace sones.GraphFS
{

    public static class DirectoryHelper
    {

        #region static Data

        private static Regex _PathSeperatorRegExpr = new Regex(String.Concat("(", FSPathConstants.PathDelimiter, ")+"), RegexOptions.Compiled);

        #endregion

        public static String GetObjectPath(String myObjectLocation)
        {

            // "/home////" -> "/home"
            while (myObjectLocation.EndsWith(FSPathConstants.PathDelimiter) && !myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                myObjectLocation = myObjectLocation.Substring(0, myObjectLocation.LastIndexOf(FSPathConstants.PathDelimiter));

            if (myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                return FSPathConstants.PathDelimiter;

            Int32 Position = myObjectLocation.LastIndexOf(FSPathConstants.PathDelimiter);

            if (Position > 0)
                return myObjectLocation.Substring(0, Position);

            else
                return FSPathConstants.PathDelimiter;

        }

        public static String GetObjectName(String myObjectLocation)
        {

            // "/home////" -> "/home"
            while (myObjectLocation.EndsWith(FSPathConstants.PathDelimiter) && !myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                myObjectLocation = myObjectLocation.Substring(0, myObjectLocation.LastIndexOf(FSPathConstants.PathDelimiter));

            if (myObjectLocation.Equals(FSPathConstants.PathDelimiter))
                return "";

            return myObjectLocation.Substring(myObjectLocation.LastIndexOf(FSPathConstants.PathDelimiter) + FSPathConstants.PathDelimiter.Length);

        }

        public static String Combine(String myObjectPath, String myObjectName)
        {

            String Combined;
            
            if (myObjectPath == null)  myObjectPath = "";
            if (myObjectName == null)  myObjectName = "";

            Combined = String.Concat(myObjectPath, FSPathConstants.PathDelimiter, myObjectName);

            if (Combined.Equals(""))
                Combined = FSPathConstants.PathDelimiter;

            if (!Combined.StartsWith(FSPathConstants.PathDelimiter))
                Combined = String.Concat(FSPathConstants.PathDelimiter, Combined);

            Combined = _PathSeperatorRegExpr.Replace(Combined, FSPathConstants.PathDelimiter);

            while (Combined.Length > FSPathConstants.PathDelimiter.Length && Combined.EndsWith(FSPathConstants.PathDelimiter))
                Combined = Combined.Substring(0, Combined.Length - FSPathConstants.PathDelimiter.Length);

            if (Combined.EndsWith("."))
                Combined.Remove(Combined.Length - 1);

            return Combined;

        }

    }

}
