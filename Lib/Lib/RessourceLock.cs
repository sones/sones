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

/* <id name="Lib – RessourceLock" />
 * <copyright file="RessourceLock.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A simple ressource locker.</summary>
 */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib
{

    #region LockItem

    /// <summary>
    /// A LockItem of the RessourceLock
    /// </summary>
    public class LockItem
    {
        public String Location { get; set; }
        public String LockToken { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime Created { get; set; }
        public TimeSpan ExpiresIn
        {
            get
            {
                return Created.Add(Lifetime).Subtract(TimestampNonce.Now);
            }
        }

        public LockItem(String myLocation, String myLockToken, TimeSpan myLifetime)
        {
            Location = myLocation;
            LockToken = myLockToken;
            Lifetime = myLifetime;

            Created = TimestampNonce.Now;
        }
    }

    #endregion

    /// <summary>
    /// A simple ressource locker
    /// </summary>
    public class RessourceLock
    {

        /// <summary>
        /// Dictionary: Locktoken, LockItem
        /// </summary>
        private static Dictionary<String, LockItem> _LockedRessources;
        /// <summary>
        /// Dictionary: Location, Locktoken
        /// </summary>
        private static Dictionary<String, String> _LockedLocations;

        private static Object _LockObj = new Object();

        public static Boolean Contains(String myLockToken)
        {
            lock (_LockObj)
            {
                if (_LockedRessources.ContainsKey(myLockToken))
                    return true;
            }
            return false;
        }

        public static Boolean LockRessource(String myLockToken, String myRessourceLocation, TimeSpan myLifeTime)
        {
            lock (_LockObj)
            {
                if (_LockedRessources.ContainsKey(myLockToken))
                    return false;

                _LockedRessources.Add(myLockToken, new LockItem(myRessourceLocation, myLockToken, myLifeTime));
                _LockedLocations.Add(myRessourceLocation, myLockToken);
            }
            return true;
        }

        public static Boolean UnLockRessource(String myLockToken)
        {
            lock (_LockObj)
            {
                if (!_LockedRessources.ContainsKey(myLockToken))
                    return false;

                _LockedLocations.Remove(_LockedRessources[myLockToken].Location);
                _LockedRessources.Remove(myLockToken);
            }
            return true;
        }

        public static LockItem GetLockedRessource(String myLockToken)
        {
            lock (_LockObj)
            {
                if (_LockedLocations.ContainsKey(myLockToken))
                    return _LockedRessources[_LockedLocations[myLockToken]];
            }
            return null;
        }

        public static String GetLockToken(String myRessourceLocation)
        {
            lock (_LockObj)
            {
                //if (_LockedRessources.ContainsValue(myRessourceLocation))
                //{

                    var Token = from Ressource in _LockedRessources
                                where Ressource.Value.Location == myRessourceLocation
                                select Ressource.Key;

                    return Token.First<String>();

                //}
            }
        }

        public static Boolean RessourceIsLocked(String myRessourceLocation)
        {
            lock (_LockObj)
            {
                if (_LockedLocations.ContainsKey(myRessourceLocation))
                {
                    String LockToken = _LockedLocations[myRessourceLocation];
                    if (_LockedRessources[LockToken].Created.Add(_LockedRessources[LockToken].Lifetime) < TimestampNonce.Now)
                    {
                        _LockedLocations.Remove(myRessourceLocation);
                        _LockedRessources.Remove(LockToken);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static String ParseToken(String mySourceString)
        {
            Regex Regex = new Regex("({.*})", RegexOptions.Compiled);
            Match Match = Regex.Match(mySourceString);
            return Match.Value;
        }

        public static String CreateLockToken()
        {
            return String.Concat("{" + Guid.NewGuid().ToString() + "}");
        }

        static RessourceLock()
        {
            _LockedRessources = new Dictionary<String, LockItem>();
            _LockedLocations = new Dictionary<String,String>();

        }

    }
}
