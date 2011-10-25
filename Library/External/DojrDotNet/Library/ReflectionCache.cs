using System;
using System.Collections.Generic;
using Jampad.Dojo.Rpc.Description;
using MethodCatalog = System.Collections.Generic.Dictionary<System.String, Jampad.Dojo.Rpc.Method>;
//+
namespace Jampad.Dojo.Rpc
{
    internal static class ReflectionCache
    {
        private static Dictionary<String, MethodCatalog> MethodCatalog;

        //+
        //- $ServiceDescription -//
        private static Dictionary<String, DojoServiceDescription> ServiceDescription { get; set; }

        //+
        //- $ReflectionCache -//
        static ReflectionCache()
        {
            MethodCatalog = new Dictionary<String, MethodCatalog>();
            ServiceDescription = new Dictionary<String, DojoServiceDescription>();
        }

        //+
        //- @AddMethodCatalog -//
        public static void AddMethodCatalog(String type, MethodCatalog methodCatalog)
        {
            MethodCatalog.Add(type, methodCatalog);
        }

        //- @AddMethodToCatalog -//
        public static void AddMethodToCatalog(String type, String name, Method method)
        {
            if (!MethodCatalog.ContainsKey(type))
            {
                MethodCatalog.Add(type, new MethodCatalog());
            }
            MethodCatalog[type].Add(name, method);
        }

        //- @AddServiceDescription -//
        public static void AddServiceDescription(String type, DojoServiceDescription serviceDescription)
        {
            ServiceDescription.Add(type, serviceDescription);
        }

        //- @ContainsSpecificMethodCatalog -//
        public static Boolean ContainsSpecificMethodCatalog(String type)
        {
            return MethodCatalog.ContainsKey(type);
        }

        //- @ContainsSpecificServiceDescription -//
        public static Boolean ContainsSpecificServiceDescription(String type)
        {
            return ServiceDescription.ContainsKey(type);
        }

        //- @GetSpecificMethodCatalog -//
        public static MethodCatalog GetSpecificMethodCatalog(String type)
        {
            return MethodCatalog[type];
        }

        //- @GetSpecificServiceDescription -//
        public static DojoServiceDescription GetSpecificServiceDescription(String type)
        {
            return ServiceDescription[type];
        }
    }
}