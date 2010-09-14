using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using sones.Lib.NewFastSerializer;

namespace sones.Lib.FastSerializer
{

    public class FillFastSerialTypeSurrogateList
    {

        #region Constructor(s)

        public FillFastSerialTypeSurrogateList()
        {
        }

        #endregion

        public void FillList()
        {
            FindAllTypes();
        }
        
        private void FindAllTypes()
        {

            var files = new List<String>();
            files.AddRange(Directory.GetFiles(".", "*.dll"));
            files.AddRange(Directory.GetFiles(".", "*.exe"));

            foreach (var actualFile in files)
            {

                var fileInfo = new FileInfo(actualFile);
                Type[] allTypes = null;

                //Debug.WriteLine(String.Format("{0} : ", actualFile));
                try
                {
                    allTypes = Assembly.LoadFrom(fileInfo.FullName).GetTypes();
                }
                catch (ReflectionTypeLoadException rtlex)
                {
                    Console.WriteLine("Could not get types from file [" + fileInfo.Name + "]: " + rtlex.LoaderExceptions.ToAggregatedString(e => e.ToString(true), Environment.NewLine + Environment.NewLine));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not get types from file [" + fileInfo.Name + "]: " + ex.ToString(true));
                }

                foreach (var _Type in allTypes)
                {

                    try
                    {

                        if (_Type.IsAbstract)
                            continue;

                        if (_Type.IsInterface)
                            continue;

                        // Not a *_Accessor class created from the tests
                        // maybe we can get here a problem with derived types
                        if (_Type.Module.ScopeName == "GraphDB_Accessor")
                            continue;

                        if (typeof(IFastSerializationTypeSurrogate).IsAssignableFrom(_Type))
                        {

                            IFastSerializationTypeSurrogate SurrogateType = (IFastSerializationTypeSurrogate)Activator.CreateInstance(_Type);

                            var existingType = SerializationWriter.findSurrogateForType(SurrogateType.TypeCode);
                            if (existingType != null && existingType.GetType() != _Type)
                            {
                                throw new sones.Lib.Exceptions.FastSerializeSurrogateTypeCodeExistException(
                                    "Could not add [" + _Type.Name + "] SurrogateForType already exists with typeCode " + SurrogateType.TypeCode
                                    + "[" + SerializationWriter.findSurrogateForType(SurrogateType.TypeCode).GetType().Name + "]");
                            }

                            SerializationWriter.AddSurrogateType(SurrogateType);

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(fileInfo.Name + " failed for type [" + _Type + "]: " + ex.Message);
                    }

                }

            }

        }

    }

}
