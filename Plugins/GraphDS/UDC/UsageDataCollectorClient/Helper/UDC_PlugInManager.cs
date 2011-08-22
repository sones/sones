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
using System.IO;
using System.Linq;
using System.Reflection;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDS.UDC.Helper
{
    /// <summary>
    /// the UDC Plug-In Manager iterates through all available assemblies and extracts the available Plug-In information
    /// like name, shortname and version
    /// </summary>
    public class UDCPluginManager
    {
        /// <summary>
        /// The locations to search for plugins
        /// </summary>
        private readonly String[] _lookupLocations;

        private List<String> ListOfAvailablePlugIns = null;

        #region Ctor
        public UDCPluginManager()
        {
            try
            {
                ListOfAvailablePlugIns = new List<string>();
                Assembly assem = Assembly.GetEntryAssembly();

                // ensured the execution of the tests
                assem = assem ?? Assembly.GetExecutingAssembly();

                if (assem == null)
                {
                    throw new FileNotFoundException("Executing Assembly could not founded");
                }

                string location = Path.GetDirectoryName(assem.Location);
                _lookupLocations = new[] { location + Path.DirectorySeparatorChar + "plugins", location };
                if (_lookupLocations.IsNullOrEmpty())
                {
                    _lookupLocations = new[] { Environment.CurrentDirectory };
                }

                foreach (string folder in _lookupLocations)
                {
                    #region Get all files in the _LookupLocations
                    if (Directory.Exists(folder))
                    {
                        IEnumerable<string> files = Directory.EnumerateFiles(folder, "*.dll")
                            .Union(Directory.EnumerateFiles(folder, "*.exe"));


                        foreach (string file in files)
                        {
                            #region actually discover
                            Assembly loadedPluginAssembly;

                            #region Try to load assembly from the filename
                            #region Load assembly
                            try
                            {
                                loadedPluginAssembly = Assembly.LoadFrom(file);
                            }
                            catch (Exception)
                            {
                                throw new Exception();
                            }
                            #endregion

                            #region Check all types of the assembly - this might throw a ReflectionTypeLoadException if the plugin definition does no longer match the plugin implementation
                            try
                            {
                                if (loadedPluginAssembly.GetTypes().IsNullOrEmpty())
                                {
                                    return;
                                }
                            }
                            catch (ReflectionTypeLoadException)
                            {
                            }
                            #endregion
                            #endregion

                            #region Get all types of the assembly

                            try
                            {
                                foreach (Type type in loadedPluginAssembly.GetTypes())
                                {
                                    #region Type validation

                                    if (!type.IsClass || type.IsAbstract)
                                    {
                                        continue;
                                    }

                                    if (!type.IsPublic && true)
                                    {
                                        continue;
                                    }

                                    #region Skip _Accessor classes

                                    if (type.HasBaseType("Microsoft.VisualStudio.TestTools.UnitTesting.BaseShadow"))
                                    {
                                        continue;
                                    }

                                    #endregion

                                    //The plugin has to implement IPluginable so that we are able to initialize/distinguish them
                                    if (!typeof(IPluginable).IsInterfaceOf(type))
                                    {
                                        continue;
                                    }

                                    //The plugin has to have an empty constructor
                                    if (type.GetConstructor(Type.EmptyTypes) == null)
                                    {
                                        continue;
                                    }
                                    #endregion

                                    ListOfAvailablePlugIns.Add(type.AssemblyQualifiedName.Replace(", Culture=neutral, PublicKeyToken=null", ""));
                                }
                            }
                            catch
                            {
                                //if we can't load a dll, so we drop this
                            }

                            #endregion
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region GetAllPlugins
        /// <summary>
        /// Get all plugins regardless of their types and their version
        /// </summary>
        /// <returns>The plugins.</returns>
        public List<String> GetAllPlugins()
        {
            return ListOfAvailablePlugIns;
        }

        #endregion
    }
}
