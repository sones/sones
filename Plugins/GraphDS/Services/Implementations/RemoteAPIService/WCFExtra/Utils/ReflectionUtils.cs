using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace WCFExtras.Utils
{
    static class ReflectionUtils
    {
        internal static object GetValue(object obj, string propertyName)
        {
            MemberInfo mi = obj.GetType().GetMember(propertyName, MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];
            if (mi.MemberType == MemberTypes.Field)
                return ((FieldInfo)mi).GetValue(obj);
            else
                return ((PropertyInfo)mi).GetValue(obj, null);
        }

        internal static void SetValue(object obj, string propertyName, object value)
        {
            MemberInfo mi = obj.GetType().GetMember(propertyName, MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];
            if (mi.MemberType == MemberTypes.Field)
                ((FieldInfo)mi).SetValue(obj, value);
            else
                ((PropertyInfo)mi).SetValue(obj, value, null);
        }

        internal static Dictionary<string, MemberInfo> GetEnumMembers(Type type)
        {
            bool hasDataContract = GetDataContractAttribute(type) != null;
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            Dictionary<string, MemberInfo> members = new Dictionary<string, MemberInfo>(fields.Length);
            foreach (MemberInfo memberInfo in fields)
            {
                if (hasDataContract)
                {
                    EnumMemberAttribute enumAttribute = GetAttribute<EnumMemberAttribute>(memberInfo);
                    if (enumAttribute != null)
                    {
                        string enumName = enumAttribute.Value;
                        if (String.IsNullOrEmpty(enumName))
                            enumName = memberInfo.Name;
                        members.Add(enumName, memberInfo);
                    }
                }
                else
                {
                    members.Add(memberInfo.Name, memberInfo);
                }
            }
            return members;
        }

        internal static DataContractAttribute GetDataContractAttribute(Type type)
        {
            return GetAttribute<DataContractAttribute>(type);
        }

        private static T GetAttribute<T>(MemberInfo memberInfo) where T : class
        {
            object[] customAttributes = memberInfo.GetCustomAttributes(typeof(T), false);
            if (customAttributes.Length > 0)
                return (T)customAttributes[0];
            return null;
        }
    }
}
