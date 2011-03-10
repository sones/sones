using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace sones.Library.VersionedPluginManager
{
    public static class Extensions
    {
        internal static Boolean IsNullOrEmpty<T1>(this IEnumerable<T1> myEnumeration)
        {
            return myEnumeration == null || !myEnumeration.Any();
        }

        /// <summary>
        /// Checks whether the type <paramref name="myBaseType"/> is a basetype of <paramref name="myType"/>
        /// </summary>
        /// <param name="myBaseType"></param>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static Boolean IsBaseType(this Type myBaseType, Type myType)
        {
            Type curType = myType;
            while (curType != null)
            {
                if (curType.BaseType == myBaseType)
                {
                    return true;
                }
                curType = curType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks whether thy type <paramref name="myType"/> has any basetype with the fullname <paramref name="myBaseType"/>
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myBaseType"></param>
        /// <returns></returns>
        public static Boolean HasBaseType(this Type myType, String myBaseType)
        {
            Type curType = myType;
            while (curType != null && curType.BaseType != null)
            {
                if (curType.BaseType.FullName == myBaseType)
                {
                    return true;
                }
                curType = curType.BaseType;
            }

            return false;
        }

        public static Boolean IsInterfaceOf(this Type myInterfaceType, Type myType)
        {
            return (myInterfaceType.IsAssignableFrom(myType));
        }

        public static String ErrorsToString(this CompilerErrorCollection myCompilerErrorCollection)
        {
            if (myCompilerErrorCollection == null || myCompilerErrorCollection.Count == 0)
            {
                return string.Empty;
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Errors(" + myCompilerErrorCollection.Count + "): ");
            foreach (object error in myCompilerErrorCollection)
            {
                strBuilder.AppendLine("[" + error + "]");
            }

            return strBuilder.ToString();
        }

        public static AssemblyName GetReferencedAssembly(this Assembly myAssembly, String myTargetAssemblyName)
        {
            #region Check whether the current assembly is already the target assembly

            if (myAssembly.GetName().Name == myTargetAssemblyName)
            {
                return myAssembly.GetName(); // return current assembly instead following the referenced asse
            }

            #endregion

            var referencesToCheck = new Queue<Assembly>();
            var assemblyAlreadyChecked = new HashSet<string>();

            referencesToCheck.Enqueue(myAssembly);

            Assembly curAssembly;
            int depth = 0;
            while (referencesToCheck.Count > 0)
            {
                curAssembly = referencesToCheck.Dequeue();

                #region Check the ReferencedAssemblies whether thy have the myTargetAssemblyName

                AssemblyName assembly = curAssembly.GetReferencedAssemblies()
                    .Where(an => an.Name == myTargetAssemblyName).FirstOrDefault();

                if (assembly != null)
                {
                    return assembly;
                }

                #endregion

                assemblyAlreadyChecked.Add(curAssembly.FullName);

                //if (depth > 2) continue;

                #region Load all referenced assemblies to check their referenced assemblies as well

                foreach (AssemblyName refAss in curAssembly.GetReferencedAssemblies())
                {
                    if (!assemblyAlreadyChecked.Contains(refAss.FullName))
                    {
                        Assembly loadedAssembly = null;

                        #region Load the referenced assembly

                        try
                        {
                            loadedAssembly = Assembly.Load(refAss.FullName);
                        }
                        catch (FileNotFoundException)
                        {
                            #region Try to load from the same path at the source assembly

                            try
                            {
                                loadedAssembly =
                                    Assembly.LoadFrom(new DirectoryInfo(curAssembly.Location).Parent.Name +
                                                      Path.DirectorySeparatorChar + refAss.Name + ".dll");
                            }
                            catch
                            {
                            }

                            #endregion
                        }

                        #endregion

                        #region Assembly could not be load, proceed

                        if (loadedAssembly == null)
                        {
                            continue;
                        }

                        #endregion

                        referencesToCheck.Enqueue(loadedAssembly);
                    }
                    depth++;
                }

                #endregion
            }

            return null;
        }
    }
}