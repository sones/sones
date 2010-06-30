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
using sones.Lib.NewFastSerializer;
using System.IO;
using System.Reflection;

namespace sones.Lib.FastSerializer
{
    public class FillFastSerialTypeSurrogateList
    {
        //NLOG: temporarily commented
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        #region constructor
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
            foreach (string fileOn in Directory.GetFiles("."))
            {
                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if ((file.Extension.Equals(".dll")) || (file.Extension.Equals(".exe")))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("{0} : ",fileOn));
                        Type[] allTypes = Assembly.LoadFrom(file.FullName).GetTypes();
                        foreach (Type type in allTypes)
                        {
                            if (type.IsAbstract)
                                continue;

                            if (type.IsInterface)
                                continue;

                            // Not a *_Accessor class created from the tests
                            // maybe we can get here a problem with derived types
                            if (type.BaseType.Name == "BaseShadow")
                                continue;

                            if (typeof(IFastSerializationTypeSurrogate).IsAssignableFrom(type))
                            {
                                IFastSerializationTypeSurrogate SurrogateType = (IFastSerializationTypeSurrogate)Activator.CreateInstance(type);

                                var existingType = SerializationWriter.findSurrogateForType(SurrogateType.TypeCode);
                                if (existingType != null && existingType.GetType() != type)
                                {
                                    throw new sones.Lib.Exceptions.FastSerializeSurrogateTypeCodeExistException(
                                        "Could not add [" + type.Name + "] SurrogateForType already exists with typeCode " + SurrogateType.TypeCode
                                        + "[" + SerializationWriter.findSurrogateForType(SurrogateType.TypeCode).GetType().Name + "]");
                                }

                                SerializationWriter.AddSurrogateType(SurrogateType);
                            }
                        }
                    }
                    catch (sones.Lib.Exceptions.FastSerializeSurrogateTypeCodeExistException fsstcee)
                    {
                        throw fsstcee;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(file.Name + " failed: " + ex.Message);
                    }
                }
            }
        }
    }
}
