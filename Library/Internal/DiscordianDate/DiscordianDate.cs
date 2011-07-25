/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.DiscordianDate
{

    public class DiscordianDate
    {

        #region Properties

        #region hastur

        private DiscordianDateStructure _hastur;

        public DiscordianDateStructure hastur
        {

            get
            {
                return _hastur;
            }

            set
            {
                _hastur = value;
            }

        }

        #endregion

        #endregion


        #region Constructors

        #region DiscordianDate()

        public DiscordianDate()
        {
            hastur = MakeDay(DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year);
        }

        #endregion

        #region DiscordianDate(myNonNormalDateTime)

        public DiscordianDate(DateTime myNonNormalDateTime)
        {
            hastur = MakeDay(myNonNormalDateTime.Month, myNonNormalDateTime.Day, myNonNormalDateTime.Year);
        }

        #endregion

        #endregion



        #region Private Names

        #region Days

        private String[] Days = new String[5]
        {
            "Sweetmorn",
            "Boomtime",
            "Pungenday",
            "Prickle-Prickle",
            "Setting Orange"
        };

        #endregion

        #region Seasons

        private String[] Seasons = new String[5]
        {
            "Chaos",
            "Discord",
            "Confusion",
            "Bureaucracy",
            "The Aftermath"
        };

        #endregion

        #region Holidays

        private String[,] Holidays = new String[5, 2]
        {
            {"Mungday", "Chaoflux"},
            {"Mojoday", "Discoflux"},
            {"Syaday",  "Confuflux"},
            {"Zaraday", "Bureflux"},
            {"Maladay", "Afflux"}
        };

        #endregion

        #endregion


        #region Helpers

        #region Ending(myNumber)

        private String Ending(Int32 myNumber)
        {

            String _Output;

            switch (myNumber % 10)
            {

                case 1: _Output = "st"; break;
                case 2: _Output = "nd"; break;
                case 3: _Output = "rd"; break;

                default: _Output = "th"; break;

            }

            return _Output;

        }

        #endregion

        private DiscordianDateStructure Convert(int nday, int nyear)
        {
            DiscordianDateStructure Output = new DiscordianDateStructure();

            Output.year = nyear + 3066;
            Output.day = nday;
            Output.season = 0;

            if ((Output.year % 4) == 2)
            {
                if (Output.day == 59)
                    Output.day = -1;
                else
                    if (Output.day > 59)
                        Output.day -= 1;
            }
            Output.yday = Output.day;

            while (Output.day > 73)
            {
                Output.season++;
                Output.day -= 73;
            }
            return Output;
        }


        public DiscordianDateStructure MakeDay(int imonth, int iday, int iyear)
        {
            DiscordianDateStructure Output = new DiscordianDateStructure();

            if ((imonth == 2) && (iday == 29))
                Output.StTibsDay = true;
            else
                Output.StTibsDay = false;

            int[,] cal = new int[2, 12]
            {
                {31,28,31,30,31,30,31,31,30,31,30,31},
                { 31,29,31,30,31,30,31,31,30,31,30,31}
            };

            int dayspast = 0;
            imonth--;
            Output.year = iyear + 1166;

            while (imonth > 0)
            {
                dayspast += cal[(Output.year % 4) == 2 ? 1 : 0, --imonth];

            }
            Output.day = dayspast + iday;
            Output.season = 0;

            if ((Output.year % 4) == 2)
            {
                if (Output.day == 59)
                    Output.day = -1;
                else
                    if (Output.day > 59)
                        Output.day -= 1;
            }

            Output.yday = Output.day;

            while (Output.day > 73)
            {
                Output.season++;
                Output.day -= 73;
            }

            return Output;
        }

        private String GetDayName(int DayNumber)
        {
            while (DayNumber > 5)
            {
                DayNumber -= 5;
            }

            return Days[DayNumber - 1];
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            String Holiday = "";

            if ((hastur.day == 5) || (hastur.day == 50))
            {
                if (hastur.day == 5)
                    Holiday = " Celebrate " + Holidays[hastur.season, 0];
                else
                    Holiday = " Celebrate " + Holidays[hastur.season, 1];
            }

            if (hastur.StTibsDay)
                Holiday = " Celebrate St. Tib's Day";

            return "Today is " + GetDayName(hastur.yday) + ", the " + hastur.day + Ending(hastur.day) + " day of " + Seasons[hastur.season] + " in the YOLD " + hastur.year + Holiday + "\n";
        }

        #endregion

    }

}
