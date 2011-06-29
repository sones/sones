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
using sones.Library.VersionedPluginManager.ErrorHandling;

namespace sones.Library.VersionedPluginManager
{
    public class PluginManager
    {
        #region Data

        #region struct ActivatorInfo

        /// <summary>
        /// Just a wrapper to hold some information about the plugin which is going to be activated.
        /// </summary>
        private struct ActivatorInfo
        {
            public Type Type { get; set; }
            public Version MinVersion { get; set; }
            public Version MaxVersion { get; set; }
            public Object[] CtorArgs { get; set; }
            public Func<Type, Object> ActivateDelegate { get; set; }
        }

        #endregion

        /// <summary>
        /// This will store the plugin inherit type and the Activator info containing the compatible version and a list of 
        /// valid plugin instances
        /// </summary>
        private readonly Dictionary<Type, Tuple<ActivatorInfo, List<Object>>> _inheritTypeAndInstance;

        /// <summary>
        /// The locations to search for plugins
        /// </summary>
        private readonly String[] _lookupLocations;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a plugin was found and activated.
        /// </summary>
        public event PluginFoundEvent OnPluginFound;

        /// <summary>
        /// Occurs when a plugin was found but was not activated due to a incompatible version.
        /// </summary>
        public event PluginIncompatibleVersionEvent OnPluginIncompatibleVersion;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new instance of the PluginActivator which searches at the plugin folder of the executing assembly directory for valid plugins.
        /// </summary>
        public PluginManager()
        {
            Assembly assem = Assembly.GetEntryAssembly();

            // ensured the execution of the tests
            assem = assem ?? Assembly.GetExecutingAssembly();

            if (assem == null)
            {
                throw new FileNotFoundException("Executing Assembly could not founded");
            }
            
            //notice for the refactoring                 
            // todo: recursive search into depth starting from the plugin folder

            string location = Path.GetDirectoryName(assem.Location);
            _lookupLocations = new[] {location + Path.DirectorySeparatorChar + "plugins", location};
            if (_lookupLocations.IsNullOrEmpty())
            {
                _lookupLocations = new[] {Environment.CurrentDirectory};
            }

            _inheritTypeAndInstance = new Dictionary<Type, Tuple<ActivatorInfo, List<object>>>();
        }

        #endregion

        #region Register<T1>

        /// <summary>
        /// Register the <typeparamref name="T1"/> as plugin. This can be an interface, an abstract class or 
        /// a usual class which is a base class.
        /// </summary>
        /// <typeparam name="T1">This can be an interface, an abstract class or a usual class which is a base class.</typeparam>
        /// <param name="myMinVersion">The minimum allowed version.</param>
        /// <param name="myMaxVersion">The maximum allowed version. If null all version greater than <paramref name="myMinVersion"/> are valid.</param>
        /// <param name="myActivateDelegate">Using this delegate you can activate the type instance.</param>
        /// <param name="myCtorArgs">Optional constructor parameters which will be used at the activation time.</param>
        /// <returns>The same instance to register more types in a fluent way.</returns>
        public PluginManager Register<T1>(Version myMinVersion, Version myMaxVersion = null,
                                          Func<Type, Object> myActivateDelegate = null, params Object[] myCtorArgs)
        {
            if (_inheritTypeAndInstance.ContainsKey(typeof (T1)))
            {
                throw new Exception("Duplicate activator type '" + typeof (T1).Name + "'");
            }

            var activatorInfo = new ActivatorInfo
                                    {
                                        Type = typeof (T1),
                                        MinVersion = myMinVersion,
                                        MaxVersion = myMaxVersion,
                                        CtorArgs = myCtorArgs,
                                        ActivateDelegate = myActivateDelegate
                                    };
            _inheritTypeAndInstance.Add(typeof (T1),
                                        new Tuple<ActivatorInfo, List<object>>(activatorInfo, new List<object>()));

            return this;
        }

        /// <summary>
        /// Uses the AssemblyVersionCompatibilityAttribute to determine the min and max assembly version
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="myCtorArgs"></param>
        /// <returns></returns>
        public PluginManager Register<T1>(params Object[] myCtorArgs)
        {
            if (_inheritTypeAndInstance.ContainsKey(typeof (T1)))
            {
                throw new Exception("Duplicate activator type '" + typeof (T1).Name + "'");
            }

            Assembly assembly = Assembly.GetCallingAssembly();
            object[] assemblyVersionCompatibilityAttributes =
                assembly.GetCustomAttributes(typeof (AssemblyVersionCompatibilityAttribute), false);
            AssemblyVersionCompatibilityAttribute assemblyVersionCompatibilityAttribute = null;

            if (assemblyVersionCompatibilityAttributes.Length > 0)
            {
                assemblyVersionCompatibilityAttribute =
                    assemblyVersionCompatibilityAttributes.Where(
                        avc => ((AssemblyVersionCompatibilityAttribute) avc).PluginName == typeof (T1).Name).
                        FirstOrDefault() as AssemblyVersionCompatibilityAttribute;
            }

            if (assemblyVersionCompatibilityAttribute != null)
            {
                return Register<T1>(assemblyVersionCompatibilityAttribute.MinVersion,
                                    assemblyVersionCompatibilityAttribute.MaxVersion, null, myCtorArgs);
            }
            
            Version version = Assembly.GetAssembly(typeof (T1)).GetName().Version;
            return Register<T1>(version, version, null, myCtorArgs);
        }

        #endregion

        #region Discover

        /// <summary>
        /// Activate all plugins of the previously registered types. 
        /// All newly registered types need to be activated again!
        /// </summary>
        /// <returns></returns>
        public void Discover(Boolean myThrowExceptionOnIncompatibleVersion = true, Boolean myPublicOnly = true)
        {
            #region Clean up old plugins

            foreach (var kv in _inheritTypeAndInstance)
            {
                _inheritTypeAndInstance[kv.Key].Item2.Clear();
            }

            #endregion

            foreach (string folder in _lookupLocations)
            {                
                DiscoverPath(myThrowExceptionOnIncompatibleVersion, myPublicOnly, folder);
            }
        }

        private void DiscoverPath(Boolean myThrowExceptionOnIncompatibleVersion, Boolean myPublicOnly, String myPath)
        {
            #region Get all files in the _LookupLocations

            if (!Directory.Exists(myPath)) return;

            IEnumerable<string> files = Directory.EnumerateFiles(myPath, "*.dll")
                .Union(Directory.EnumerateFiles(myPath, "*.exe"));

            #endregion

            foreach (string file in files)
            {
                DiscoverFile(myThrowExceptionOnIncompatibleVersion, myPublicOnly, file);
            }
        }

        private void DiscoverFile(Boolean myThrowExceptionOnIncompatibleVersion, Boolean myPublicOnly, String myFile)
        {
            Assembly loadedPluginAssembly;

            #region Try to load assembly from the filename

            #region Load assembly

            try
            {
                loadedPluginAssembly = Assembly.LoadFrom(myFile);
            }
            catch (Exception)
            {
                throw new CouldNotLoadAssemblyException(myFile);                
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
                #region Do we have a conflict of an plugin implementation?

                // Check all referenced assembly of this failed loadedPluginAssembly.GetTypes() and find all matching assemblies with 
                // all types in _InheritTypeAndInstance

                //TODO: check more than only one reference depth...

                //var matchingAssemblies = new List<Tuple<AssemblyName, AssemblyName>>();
                foreach (AssemblyName assembly in loadedPluginAssembly.GetReferencedAssemblies())
                {
                    IEnumerable<KeyValuePair<Type, Tuple<ActivatorInfo, List<object>>>> matchings =
                        _inheritTypeAndInstance.Where(kv => Assembly.GetAssembly(kv.Key).GetName().Name == assembly.Name);
                    
                    if (matchings != null)
                    {
                        foreach (var matchAss in matchings)
                        {
                            //matchingAssemblies.Add(new Tuple<AssemblyName, AssemblyName>(Assembly.GetAssembly(matchAss.Key).GetName(), assembly));

                            CheckVersion(myThrowExceptionOnIncompatibleVersion, loadedPluginAssembly,
                                         Assembly.GetAssembly(matchAss.Key).GetName(), assembly, matchAss.Value.Item1);
                        }
                    }
                }

                #endregion
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

                    if (!type.IsPublic && myPublicOnly)
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

                    FindAndActivateTypes(myThrowExceptionOnIncompatibleVersion, loadedPluginAssembly, type);
                }
            }
            catch 
            {
                //if we can't load a dll, so we drop this
            }

            #endregion
        }

        private static Type DeGenerification(Type mySearchType, Type myGenericType)
        {
            if (mySearchType.GetGenericArguments().Length > 0)
            {
                if (myGenericType.ContainsGenericParameters)
                {
                    try
                    {
                        var generics = mySearchType.GetGenericArguments();
                        return myGenericType.MakeGenericType(generics);
                    }
                    catch
                    {
                    }
                }
            }
            return myGenericType;

        }

        /// <summary>
        /// Will seach all registered type whether it is an plugin definition of <paramref name="myCurrentPluginType"/>.
        /// </summary>
        /// <param name="myThrowExceptionOnIncompatibleVersion">Truth value of throw an exception</param>
        /// <param name="myLoadedPluginAssembly">The assembly from which the <paramref name="myCurrentPluginType"/> comes from.</param>
        /// <param name="myCurrentPluginType">The current plugin (or not).</param>
        private void FindAndActivateTypes(bool myThrowExceptionOnIncompatibleVersion, Assembly myLoadedPluginAssembly,
                                          Type myCurrentPluginType)
        {
            
            IEnumerable<KeyValuePair<Type, Tuple<ActivatorInfo, List<object>>>> validBaseTypes =
                _inheritTypeAndInstance.Where(kv => 
                {
                    Type realType = DeGenerification(kv.Key, myCurrentPluginType);
                    return kv.Key.IsBaseType(realType) || kv.Key.IsInterfaceOf(realType);
                }
            );

            #region Take each baseType which is valid (either base or interface) and verify version and add

            foreach (var baseType in validBaseTypes)
            {
                ActivatorInfo activatorInfo = _inheritTypeAndInstance[baseType.Key].Item1;

                #region Get baseTypeAssembly and plugin referenced assembly

                AssemblyName baseTypeAssembly = Assembly.GetAssembly(baseType.Key).GetName();
                AssemblyName pluginReferencedAssembly = myLoadedPluginAssembly.GetReferencedAssembly(baseTypeAssembly.Name);

                #endregion

                Boolean _validVersion = false;

                try
                {
                    if (CheckVersion(myThrowExceptionOnIncompatibleVersion, myLoadedPluginAssembly, baseTypeAssembly, pluginReferencedAssembly, activatorInfo))
                        _validVersion = true;
                }

                catch (Exception)
                {
                    continue;
                }

                #region Create instance and add to lookup dict

                if (_validVersion)
                {
                    try
                    {
                        Object instance;
                        Type realType = DeGenerification(baseType.Key, myCurrentPluginType);
                        if (activatorInfo.ActivateDelegate != null)
                        {
                            instance = activatorInfo.ActivateDelegate(realType);
                        }
                        else
                        {
                            instance = Activator.CreateInstance(realType, activatorInfo.CtorArgs);
                        }

                        if (instance != null)
                        {
                            _inheritTypeAndInstance[baseType.Key].Item2.Add(instance);

                            if (OnPluginFound != null)
                            {
                                OnPluginFound(this, new PluginFoundEventArgs(myCurrentPluginType, instance));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new UnknownException(e);                        
                    }
                }

                #endregion
            }

            #endregion
        }

        private Boolean CheckVersion(bool myThrowExceptionOnIncompatibleVersion, Assembly myPluginAssembly,
                                  AssemblyName myBaseTypeAssembly, AssemblyName myPluginReferencedAssembly,
                                  ActivatorInfo myActivatorInfo)
        {
            Boolean _validVersion = false;

            #region Check version

            //if (myBaseTypeAssembly.Version != myPluginReferencedAssembly.Version)
            
                //Console.WriteLine("Assembly version does not match! Expected '{0}' but current is '{1}'", myLoadedPluginAssembly.GetName().Version, pluginReferencedAssembly.Version);
                if (myActivatorInfo.MaxVersion != null)
                {
                    #region Compare min and max version

                    if (myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MinVersion) < 0
                        || myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MaxVersion) > 0)
                    {
                        _validVersion = false;

                        if (OnPluginIncompatibleVersion != null)
                        {
                            OnPluginIncompatibleVersion(this,
                                                        new PluginIncompatibleVersionEventArgs(myPluginAssembly,
                                                                                               myPluginReferencedAssembly
                                                                                                   .Version,
                                                                                               myActivatorInfo.
                                                                                                   MinVersion,
                                                                                               myActivatorInfo.
                                                                                                   MaxVersion,
                                                                                               myActivatorInfo.Type));
                        }
                        if (myThrowExceptionOnIncompatibleVersion)
                        {
                            throw new IncompatiblePluginVersionException(myPluginAssembly,
                                                                         myPluginReferencedAssembly.Version,
                                                                         myActivatorInfo.MinVersion,
                                                                         myActivatorInfo.MaxVersion);
                        }                                               
                    }
                    else
                    {                        
                        _validVersion = true;
                    }

                    #endregion
                }
                else
                {
                    #region Compare min version

                    if (myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MinVersion) < 0)
                    {
                        _validVersion = false;

                        if (OnPluginIncompatibleVersion != null)
                        {
                            OnPluginIncompatibleVersion(this,
                                                        new PluginIncompatibleVersionEventArgs(myPluginAssembly,
                                                                                               myPluginReferencedAssembly
                                                                                                   .Version,
                                                                                               myActivatorInfo.
                                                                                                   MinVersion,
                                                                                               myActivatorInfo.
                                                                                                   MaxVersion,
                                                                                               myActivatorInfo.Type));
                        }
                        if (myThrowExceptionOnIncompatibleVersion)
                        {
                            throw new IncompatiblePluginVersionException(myPluginAssembly,
                                                                         myPluginReferencedAssembly.Version,
                                                                         myActivatorInfo.MinVersion);
                        }                        
                    }
                    else
                    {                        
                        _validVersion = true;
                    }

                    #endregion
                }            

            #endregion

                return _validVersion;
        }

        #endregion

        #region GetPlugins

        /// <summary>
        /// Get all plugins of type <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the plugin.</typeparam>
        /// <param name="mySelector">An optional selector to narrow down the result.</param>
        /// <returns>The plugins.</returns>
        public IEnumerable<T1> GetPlugins<T1>(Func<T1, Boolean> mySelector = null)
        {
            if (_inheritTypeAndInstance.ContainsKey(typeof (T1)))
            {
                foreach (object instance in _inheritTypeAndInstance[typeof (T1)].Item2)
                {
                    if (mySelector == null || (mySelector != null && mySelector((T1) instance)))
                    {
                        yield return (T1) instance;
                    }
                }
            }

            yield break;
        }

        #endregion

        #region HasPlugins

        /// <summary>
        /// Returns true if there are any plugins of type <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the plugins.</typeparam>
        /// <param name="mySelector">An optional selector to narrow down the plugins.</param>
        /// <returns>True if any plugin exists.</returns>
        public Boolean HasPlugins<T1>(Func<T1, Boolean> mySelector = null)
        {
            if (!_inheritTypeAndInstance.ContainsKey(typeof (T1)))
            {
                return false;
            }

            if (mySelector == null)
            {
                return !_inheritTypeAndInstance[typeof (T1)].Item2.IsNullOrEmpty();
            }
            
            return _inheritTypeAndInstance[typeof (T1)].Item2.Any(o => mySelector((T1) o));
        }

        #endregion
    }
}
