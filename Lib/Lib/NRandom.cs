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


//-- RSA
//public string EncryptString( string inputString, int dwKeySize, 
//                             string xmlString )
//{
//    // TODO: Add Proper Exception Handlers
//    RSACryptoServiceProvider rsaCryptoServiceProvider = 
//                                  new RSACryptoServiceProvider( dwKeySize );
//    rsaCryptoServiceProvider.FromXmlString( xmlString );
//    int keySize = dwKeySize / 8;
//    byte[] bytes = Encoding.UTF32.GetBytes( inputString );
//    // The hash function in use by the .NET RSACryptoServiceProvider here 
//    // is SHA1
//    // int maxLength = ( keySize ) - 2 - 
//    //              ( 2 * SHA1.CreateStorage().ComputeHash( rawBytes ).Length );
//    int maxLength = keySize - 42;
//    int dataLength = bytes.Length;
//    int iterations = dataLength / maxLength;
//    StringBuilder stringBuilder = new StringBuilder();
//    for( int i = 0; i <= iterations; i++ )
//    {
//        byte[] tempBytes = new byte[ 
//                ( dataLength - maxLength * i > maxLength ) ? maxLength : 
//                                              dataLength - maxLength * i ];
//        Buffer.BlockCopy( bytes, maxLength * i, tempBytes, 0, 
//                          tempBytes.Length );
//        byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt( tempBytes,
//                                                                  true );
//        // Be aware the RSACryptoServiceProvider reverses the order of 
//        // encrypted bytes. It does this after encryption and before 
//        // decryption. If you do not require compatibility with Microsoft 
//        // Cryptographic API (CAPI) and/or other vendors. Comment out the 
//        // next line and the corresponding one in the DecryptString function.
//        Array.Reverse( encryptedBytes );
//        // Why convert to base 64?
//        // Because it is the largest power-of-two base printable using only 
//        // ASCII characters
//        stringBuilder.Append( Convert.ToBase64String( encryptedBytes ) );
//    }
//    return stringBuilder.ToString();
//}

//public string DecryptString( string inputString, int dwKeySize, 
//                             string xmlString )
//{
//    // TODO: Add Proper Exception Handlers
//    RSACryptoServiceProvider rsaCryptoServiceProvider
//                             = new RSACryptoServiceProvider( dwKeySize );
//    rsaCryptoServiceProvider.FromXmlString( xmlString );
//    int base64BlockSize = ( ( dwKeySize / 8 ) % 3 != 0 ) ?
//      ( ( ( dwKeySize / 8 ) / 3 ) * 4 ) + 4 : ( ( dwKeySize / 8 ) / 3 ) * 4;
//    int iterations = inputString.Length / base64BlockSize; 
//    ArrayList arrayList = new ArrayList();
//    for( int i = 0; i < iterations; i++ )
//    {
//        byte[] encryptedBytes = Convert.FromBase64String( 
//             inputString.Substring( base64BlockSize * i, base64BlockSize ) );
//        // Be aware the RSACryptoServiceProvider reverses the order of 
//        // encrypted bytes after encryption and before decryption.
//        // If you do not require compatibility with Microsoft Cryptographic 
//        // API (CAPI) and/or other vendors.
//        // Comment out the next line and the corresponding one in the 
//        // EncryptString function.
//        Array.Reverse( encryptedBytes );
//        arrayList.AddRange( rsaCryptoServiceProvider.Decrypt( 
//                            encryptedBytes, true ) );
//    }
//    return Encoding.UTF32.GetString( arrayList.ToArray( 
//                              Type.GetType( "System.Byte" ) ) as byte[] );
//}




    }
}
