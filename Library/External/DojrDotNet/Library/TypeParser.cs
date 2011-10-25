using System;
using System.Collections.Generic;
using System.Reflection;
using MethodCatalog = System.Collections.Generic.Dictionary<System.String, Jampad.Dojo.Rpc.Method>;
//+
namespace Jampad.Dojo.Rpc
{
    internal static class TypeScanner
    {
        //- ~SetMappedMethods -//
        internal static void SetMappedMethods(Type type)
        {
            foreach (MethodInfo m in type.GetMethods())
            {
                DojoOperationAttribute[] doa = (DojoOperationAttribute[])Attribute.GetCustomAttributes(m, typeof(DojoOperationAttribute), false);
                if (doa != null && doa.Length > 0)
                {
                    Method md = new Method(m.Name);
                    md.MethodInfo = m;
                    ParameterInfo[] ps = m.GetParameters();
                    if (ps != null && ps.Length > 0)
                    {
                        foreach (ParameterInfo p in ps)
                        {
                            md.Parameters.Add(p.Name, p.ParameterType.Name);
                        }
                    }
                    ReflectionCache.AddMethodToCatalog(type.FullName, doa[0].Name, md);
                }
            }
        }

        //- ~MatchProperties -//
        internal static Object[] MatchProperties(Method method, String[] values)
        {
            List<Object> p = new List<Object>();
            if (method.Parameters.Count != values.Length)
            {
                throw new DojoRpcException("Invalid number of parameters");
            }
            Int32 i = 0;
            foreach (String key in method.Parameters.Keys)
            {
                Object val = GetValue(key, method.Parameters[key], values[i]);
                p.Add(val);
                i++;
            }
            //+
            return p.ToArray();
        }

        //- $GetValue -//
        private static Object GetValue(String name, String type, String value)
        {
            Object o = null;
            switch (type.ToLower())
            {
                case "int16":
                    return Convert.ToInt16(value);
                case "int32":
                    return Convert.ToInt32(value);
                case "int64":
                    return Convert.ToInt64(value);
                case "single":
                    return Convert.ToSingle(value);
                case "double":
                    return Convert.ToDouble(value);
                case "decimal":
                    return Convert.ToDecimal(value);
                case "string":
                    return Convert.ToString(value);
                case "object":
                    o = value;
                    break;
            }
            //+
            return o;
        }

        //- ~GetMethod -//
        internal static Method GetMethod(Type type, String name)
        {
            MethodCatalog mc = ReflectionCache.GetSpecificMethodCatalog(type.FullName);
            if (mc.ContainsKey(name))
            {
                return mc[name];
            }
            //+
            return null;
        }
    }
}