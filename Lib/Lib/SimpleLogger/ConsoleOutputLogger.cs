/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sones.Lib.SimpleLogger
{
    /// <summary>
    /// This Class stores a number of Console Output Lines into a ring buffer
    /// </summary>
    public static class ConsoleOutputLogger
    {
        private static int Max_Number_Of_Entries = 50;
        private static LinkedList<String> LoggerList = new LinkedList<String>();
        public static bool verbose = false;
        public static String Logfilename = "Graph.log";
        public static bool writeLogfile = false;

        public static StreamWriter Logfile = null;

        public static void SetNumberOfMaxEntries(int Number)
        {
            // TODO: It would be nice to keep at least the Number of Lines we're setting
            lock (LoggerList)
            {
                LoggerList.Clear();
            }
            Max_Number_Of_Entries = Number;
        }

        public static int GetMaxNumberOfEntries()
        {
            return Max_Number_Of_Entries;
        }

        public static void LogToFile(String text)
        {
            if (Logfile == null)
            {
                Logfile = new StreamWriter(Logfilename, true);
            }
            lock (Logfile)
            {
                Logfile.WriteLine(text);
                Logfile.Flush();
            }
        }

        public static void WriteLine(String text)
        {
            DateTime TimeDate = DateTime.Now;

            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;

            // write it to the console
            if (verbose) Console.WriteLine(text);
            if (writeLogfile) LogToFile(text);

            lock (LoggerList)
            {
                if (LoggerList.Count == Max_Number_Of_Entries)
                {
                    LoggerList.RemoveFirst();
                }

                LoggerList.AddLast(text);
            }
        }

        public static void WriteLine_NotIntoLogfile(String text)
        {
            DateTime TimeDate = DateTime.Now;

            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;

            // write it to the console
            if (verbose) Console.WriteLine(text);
            //if (writeLogfile) LogToFile(text);

            lock (LoggerList)
            {
                if (LoggerList.Count == Max_Number_Of_Entries)
                {
                    LoggerList.RemoveFirst();
                }

                LoggerList.AddLast(text);
            }
        }



        public static String[] GetLoggedLines()
        {
            String[] Output = new String[Max_Number_Of_Entries];
            int Current_Position = 0;

            lock (LoggerList)
            {
                foreach (String line in LoggerList)
                {
                    if (line != null)
                    {
                        Output[Current_Position] = line;
                        Current_Position++;
                    }
                }
            }
            return Output;
        }
    }
}
