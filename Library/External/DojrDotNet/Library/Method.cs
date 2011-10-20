using System;
using System.Collections.Generic;
using System.Reflection;
//+
namespace Jampad.Dojo.Rpc
{
    internal class Method
    {
        //- @Parameters -//
        public Dictionary<String, String> Parameters { get; set; }

        //- @Name -//
        public String Name { get; set; }

        //- @MethodInfo -//
        public MethodInfo MethodInfo { get; set; }

        //- @Method -//
        public Method(String name)
        {
            this.Name = name;
            this.Parameters = new Dictionary<String, String>();
        }
    }
}