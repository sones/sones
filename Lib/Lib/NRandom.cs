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
using System.Security.Cryptography;

namespace sones.Lib
{
    class NNRandom
    {


        
        /// <summary>
        /// Erzeugt echte Zufallszahlen
        /// </summary>
        /// <param name="count">Anzahl der zu erzeugenden Zufallszahlen</param>
        /// <param name="getOldestRevisionTime">Die kleinste zu erzeugende Zahl</param>
        /// <param name="getLatestRevisionTime">Die größte zu erzeugende Zahl</param>
        /// <returns>Gibt ein Array mit den erzeugten Zufallszahlen zurück</returns>
        public static Byte[] GetRandomNumbers(int count, Byte min, Byte max)
        {
            // Zufallszahlen erzeugen
            RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
            byte[] numbers = new Byte[count];
            csp.GetBytes(numbers);

            // Die Zahlen umrechnen
            double divisor = 256F / (max - min + 1);
            if (min > 0 || max < 255)
            {
                for (int i = 0; i < count; i++)
                {
                    numbers[i] = (byte)((numbers[i] / divisor) + min);
                }
            }

            return numbers;
        }






        // randomNumGen = RNGCryptoServiceProvider.CreateStorage();
// Byte[] randomBytes = new Byte[$count$];
// randomNumGen.GetBytes(randomBytes);

//-- Encrypt
//fStream = File.OpenStorage($filename$, FileMode.OpenOrCreate);
//Rijndael RijndaelAlg = Rijndael.CreateStorage;
//CryptoStream cStream = new CryptoStream(fStream, 
//                               RijndaelAlg.CreateEncryptor($privatekey$, $initializationVector$), 
//                               CryptoStreamMode.Write);
//StreamWriter sWriter = new StreamWriter(cStream);

//sWriter.WriteLine($plainText$);
//sWriter.Close();
//cStream.Close();
//fStream.Close();

//-- Decrypt
//File.OpenStorage($filename$, FileMode.OpenOrCreate);
//Rijndael RijndaelAlg = Rijndael.CreateStorage;
//CryptoStream cStream = new CryptoStream(fStream, 
//                                RijndaelAlg.CreateDecryptor($privatekey$, $initializationVector$), 
//                                CryptoStreamMode.Read);

//StreamReader sReader = new StreamReader(cStream);
//string plainText = sReader.ReadLine();

//sReader.Close();
//cStream.Close();
//fStream.Close();

    }
}
