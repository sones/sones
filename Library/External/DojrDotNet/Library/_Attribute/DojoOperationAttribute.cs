using System;
//+
namespace Jampad.Dojo.Rpc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class DojoOperationAttribute : Attribute
    {
        private String name;

        //+
        //- @Name -//
        public String Name
        {
            get
            {
                return name;
            }
        }

        //+
        //- @Ctor -//
        public DojoOperationAttribute(String name)
        {
            this.name = name;
        }
    }
}