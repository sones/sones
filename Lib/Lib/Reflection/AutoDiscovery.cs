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


/*
 * AutoDiscovery<T>
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.Lib.Reflection
{

    /// <summary>
    /// An abstract factory which uses reflection to generate a apropriate
    /// implementation of T for you.
    /// </summary>
    public abstract class AutoDiscovery<T>
    {

        #region Data

        protected Dictionary<String, Type> _DictionaryT;

        #endregion

        #region Properties

        #region RegisteredTypes

        /// <summary>
        /// Returns a list of activated types of the registered implementations of T
        /// </summary>
        public IEnumerable<T> RegisteredTypes
        {
            get
            {

                try
                {

                    return from _TypesT in _DictionaryT select (T) Activator.CreateInstance(_TypesT.Value);

                }
                catch (Exception e)
                {
                    throw new Exception("RegisteredGraphFSs could not be created! " + e);
                }

            }
        }

        #endregion

        #region RegisteredImplementations

        /// <summary>
        /// Returns the names of the registered implementations of T
        /// </summary>
        public IEnumerable<String> RegisteredImplementationNames
        {
            get
            {
                return from _TypesT in _DictionaryT select _TypesT.Key;
            }
        }

        #endregion

        #endregion

        #region Constructor

        #region AutoDiscovery()

        /// <summary>
        /// This constructor will autodiscover all implementations of T
        /// </summary>
        public AutoDiscovery()
        {
            _DictionaryT = new Dictionary<String, Type>();
        }

        #endregion

        #endregion


        #region FindAndRegisterImplementations(myClear, myPaths, myFunc)

        /// <summary>
        /// Searches all .dll and .exe files at the given locations for classes implementing &lt;T&gt;
        /// </summary>
        /// <param name="myClear">Erase types before searching for new ones?</param>
        /// <param name="myPaths">An array of paths to search in</param>
        /// <param name="myType2Identification">A mapping from a found type to its identification string</param>
        public void FindAndRegisterImplementations(Boolean myClear, String[] myPaths, Func<Type, String> myType2Identification)
        {

            lock (this)
            {


                Assembly _Assembly = null;
                Type[] _AllTypes = null;

                if (myClear)
                    _DictionaryT.Clear();

                Debug.WriteLine("Started autodiscovering " + typeof(T).Name + "s! ");

                foreach (var _Path in myPaths)
                {

                    foreach (var _ActualFile in Directory.GetFiles(_Path))
                    {

                        var _FileInfo = new FileInfo(_ActualFile);

                        // Preliminary check, must be .dll or .exe
                        if (_FileInfo.Extension.Equals(".dll") || _FileInfo.Extension.Equals(".exe"))
                        {

                            try
                            {

                                _Assembly = Assembly.LoadFrom(_FileInfo.FullName);
                                _AllTypes = _Assembly.GetTypes();

                                foreach (var _ActualType in _AllTypes)
                                {

                                    if (!_ActualType.IsAbstract && _ActualType.IsPublic && _ActualType.IsVisible)
                                    {

                                        var _ActualTypeGetInterfaces = _ActualType.GetInterfaces();

                                        if (_ActualTypeGetInterfaces != null)
                                        {
                                            foreach (var _Interface in _ActualTypeGetInterfaces)
                                            {
                                                if (_Interface == typeof(T))
                                                {
                                                    try
                                                    {
                                                        var _ID = myType2Identification(_ActualType);
                                                        if (!_ID.IsNullOrEmpty())
                                                            _DictionaryT.Add(_ID, _ActualType);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Debug.WriteLine("Could not activate or register " + typeof(T).Name + " " + _ActualType.Name + ": " + e);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Autodiscovering " + typeof(T).Name + "s within file '" + _FileInfo.FullName + "' failed!" + Environment.NewLine + e);
                            }

                        }

                    }

                }

                Debug.WriteLine(typeof(T).Name + "s autodiscovering finished!");

            }

        }

        #endregion

        #region (protected) ActivateT_protected(myImplementationID)

        /// <summary>
        /// Activates a new instance of T based on the given name.
        /// </summary>
        /// <param name="myImplementationID">The identification string of the implementation to activate</param>
        /// <returns>An activated instance of T</returns>
        protected Exceptional<T> ActivateT_protected(String myImplementationID)
        {

            lock (this)
            {

                try
                {

                    Type _Type;

                    if (_DictionaryT.TryGetValue(myImplementationID, out _Type))
                        if (_Type != null)
                            return new Exceptional<T>( (T) Activator.CreateInstance(_Type));

                }
                catch (Exception e)
                {
                    return new Exceptional<T>(new AutoDiscoveryError(typeof(T).Name + " implementation '" + myImplementationID + "' could not be activated! " + e));
                }
                
                return new Exceptional<T>(new AutoDiscoveryError(typeof(T).Name + " implementation '" + myImplementationID + "' could not be activated!"));

            }

        }

        #endregion

    }

}
