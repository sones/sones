using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using sones.ErrorHandling;
using sones.VersionedPluginManager.ErrorHandling;
using sones.VersionedPluginManager.ErrorHandling.Events;

namespace sones.VersionedPluginManager
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
            else
            {
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
            else
            {
                Version version = Assembly.GetAssembly(typeof (T1)).GetName().Version;
                return Register<T1>(version, version, null, myCtorArgs);
            }
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
                //retVal.PushIExceptional(DiscoverPath(myThrowExceptionOnIncompatibleVersion, myPublicOnly, folder));

                DiscoverPath(myThrowExceptionOnIncompatibleVersion, myPublicOnly, folder);
            }
        }

        private void DiscoverPath(Boolean myThrowExceptionOnIncompatibleVersion, Boolean myPublicOnly, String myPath)
        {
            #region Get all files in the _LookupLocations

            if (Directory.Exists(myPath))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(myPath, "*.dll")
                    .Union(Directory.EnumerateFiles(myPath, "*.exe"));

                #endregion

                foreach (string file in files)
                {
                    //retVal.PushIExceptional(DiscoverFile(myThrowExceptionOnIncompatibleVersion, myPublicOnly, file));

                    DiscoverFile(myThrowExceptionOnIncompatibleVersion, myPublicOnly, file);
                }
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
            catch (CouldNotLoadAssemblyException)
            {
                throw new CouldNotLoadAssemblyException(myFile);

                //Console.WriteLine(e.ToString());
                //return retVal.PushIError(new Error_CouldNotLoadAssembly(myFile));
            }

            #endregion

            #region Check all types of the assembly - this might throw a ReflectionTypeLoadException if the plugin definition des no longer match the plugin implementation

            try
            {
                if (loadedPluginAssembly.GetTypes().IsNullOrEmpty())
                {
                    return;
                    //return retVal;
                }
            }
            catch (ReflectionTypeLoadException e)
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

                #endregion

                FindAndActivateTypes(myThrowExceptionOnIncompatibleVersion, loadedPluginAssembly, type);
                //retVal.PushIExceptional(FindAndActivateTypes(myThrowExceptionOnIncompatibleVersion, loadedPluginAssembly, type));
            }

            #endregion
        }

        /// <summary>
        /// Will seach all registered type whether it is an plugin definition of <paramref name="myCurrentPluginType"/>.
        /// </summary>
        /// <param name="myThrowExceptionOnIncompatibleVersion"></param>
        /// <param name="myLoadedPluginAssembly">The assembly from which the <paramref name="myCurrentPluginType"/> comes from.</param>
        /// <param name="myCurrentPluginType">The current plugin (or not).</param>
        private void FindAndActivateTypes(bool myThrowExceptionOnIncompatibleVersion, Assembly myLoadedPluginAssembly,
                                          Type myCurrentPluginType)
        {
            IEnumerable<KeyValuePair<Type, Tuple<ActivatorInfo, List<object>>>> validBaseTypes =
                _inheritTypeAndInstance.Where(
                    kv => kv.Key.IsBaseType(myCurrentPluginType) || kv.Key.IsInterfaceOf(myCurrentPluginType));

            #region Take each baseType which is valid (either base or interface) and verify version and add

            foreach (var baseType in validBaseTypes)
            {
                ActivatorInfo activatorInfo = _inheritTypeAndInstance[baseType.Key].Item1;

                #region Get baseTypeAssembly and plugin referenced assembly

                AssemblyName baseTypeAssembly = Assembly.GetAssembly(baseType.Key).GetName();
                AssemblyName pluginReferencedAssembly =
                    myLoadedPluginAssembly.GetReferencedAssembly(baseTypeAssembly.Name);

                #endregion

                try
                {
                    CheckVersion(myThrowExceptionOnIncompatibleVersion, myLoadedPluginAssembly, baseTypeAssembly,
                                 pluginReferencedAssembly, activatorInfo);
                }

                catch (Exception e)
                {
                    continue;
                }

                #region Create instance and add to lookup dict

                try
                {
                    Object instance;
                    if (activatorInfo.ActivateDelegate != null)
                    {
                        instance = activatorInfo.ActivateDelegate(myCurrentPluginType);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(myCurrentPluginType, activatorInfo.CtorArgs);
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
                    //retVal.PushIError(new GeneralError(ex.ToString()));
                }

                #endregion
            }

            #endregion
        }

        private void CheckVersion(bool myThrowExceptionOnIncompatibleVersion, Assembly myPluginAssembly,
                                  AssemblyName myBaseTypeAssembly, AssemblyName myPluginReferencedAssembly,
                                  ActivatorInfo myActivatorInfo)
        {
            #region Check version

            if (myBaseTypeAssembly.Version != myPluginReferencedAssembly.Version)
            {
                //Console.WriteLine("Assembly version does not match! Expected '{0}' but current is '{1}'", myLoadedPluginAssembly.GetName().Version, pluginReferencedAssembly.Version);
                if (myActivatorInfo.MaxVersion != null)
                {
                    #region Compare min and max version

                    if (myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MinVersion) < 0
                        || myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MaxVersion) > 0)
                    {
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
                        //retVal.PushIError(new IncompatiblePluginVersion(myPluginAssembly, myPluginReferencedAssembly.Version, myActivatorInfo.MinVersion, myActivatorInfo.MaxVersion));                        
                    }
                    else
                    {
                        // valid version
                    }

                    #endregion
                }
                else
                {
                    #region Compare min version

                    if (myPluginReferencedAssembly.Version.CompareTo(myActivatorInfo.MinVersion) < 0)
                    {
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
                        //retVal.PushIError(new IncompatiblePluginVersion(myPluginAssembly, myPluginReferencedAssembly.Version, myActivatorInfo.MinVersion, null));
                    }
                    else
                    {
                        // valid version
                    }

                    #endregion
                }
            }

            #endregion
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
            else
            {
                return _inheritTypeAndInstance[typeof (T1)].Item2.Any(o => mySelector((T1) o));
            }
        }

        #endregion
    }
}