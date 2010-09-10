/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/// A short and sweet C# 3.5 / LINQ implementation of 99 Bottles of Beer
/// Jeff Dietrich, jd@discordant.org - October 26, 2007

//NOTE by rivantsov: this sample does not work in Irony
// Irony's c# parser cannot parse this file because LINQ queries are not supported yet.

using System;
using System.Linq;
using System.Text;

namespace NinetyNineBottles
{
  class Beer
  {
    static void Main(string[] args)
    {
        StringBuilder beerLyric = new StringBuilder();
        string nl = System.Environment.NewLine;

        var beers =
            (from n in Enumerable.Range(0, 100)
             select new { 
               Say =  n == 0 ? "No more bottles" : 
                     (n == 1 ? "1 bottle" : n.ToString() + " bottles"),
               Next = n == 1 ? "no more bottles" : 
                     (n == 0 ? "99 bottles" : 
                     (n == 2 ? "1 bottle" : n.ToString() + " bottles")),
               Action = n == 0 ? "Go to the store and buy some more" : 
                                 "Take one down and pass it around"
             }).Reverse();

        foreach (var beer in beers)
        {
            beerLyric.AppendFormat("{0} of beer on the wall, {1} of beer.{2}",
                                    beer.Say, beer.Say.ToLower(), nl);
            beerLyric.AppendFormat("{0}, {1} of beer on the wall.{2}", 
                                    beer.Action, beer.Next, nl);
            beerLyric.AppendLine();
        }
        Console.WriteLine(beerLyric.ToString());
        Console.ReadLine();
    }
  }
}
