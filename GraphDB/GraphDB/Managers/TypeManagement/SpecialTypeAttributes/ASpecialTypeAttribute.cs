
#region Usings

using System;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{

    public abstract class ASpecialTypeAttribute : TypeAttribute, IEquatable<ASpecialTypeAttribute>
    {

        /// <summary>
        /// The ShowSetting name
        /// </summary>
        public abstract String ShowSettingName { get; }

        /// <summary>
        /// This will change the <paramref name="myNewDBObject"/> dependend on the SpecialTypeAttribute implementation
        /// </summary>
        /// <param name="myNewDBObject">The DBObject</param>
        /// <param name="myValue">The values which should be assigned to the <paramref name="myNewDBObject"/></param>
        /// <param name="myOptionalParameters">Some optional parameters</param>
        public abstract Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, Object myValue, params object[] myOptionalParameters);

        /// <summary>
        /// Extracts the value dependend on the SpecialTypeAttribute from the <paramref name="dbObjectStream"/>
        /// </summary>
        /// <param name="dbObjectStream">The dbObjectStream</param>
        /// <param name="graphDBType"></param>
        /// <param name="sessionInfos"></param>
        /// <returns>The extracted value</returns>
        public abstract Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext);


        #region Operator overloading

        #region Operator == (myASpecialTypeAttribute1, myASpecialTypeAttribute2)

        public static Boolean operator == (ASpecialTypeAttribute myASpecialTypeAttribute1, ASpecialTypeAttribute myASpecialTypeAttribute2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(myASpecialTypeAttribute1, myASpecialTypeAttribute2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) myASpecialTypeAttribute1 == null) || ((Object) myASpecialTypeAttribute2 == null))
                return false;

            return myASpecialTypeAttribute1.Equals(myASpecialTypeAttribute2);

        }

        #endregion

        #region Operator != (myASpecialTypeAttribute1, myASpecialTypeAttribute2)

        public static Boolean operator != (ASpecialTypeAttribute myASpecialTypeAttribute1, ASpecialTypeAttribute myASpecialTypeAttribute2)
        {
            return !(myASpecialTypeAttribute1 == myASpecialTypeAttribute2);
        }

        #endregion

        #endregion

        #region IEquatable<ASpecialTypeAttribute> Members

        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {

            if (myObject == null)
                return false;

            var _Object = myObject as ASpecialTypeAttribute;
            if (_Object == null)
                return (Equals(_Object));

            return false;

        }

        #endregion

        #region Equals(myVertex)

        public Boolean Equals(ASpecialTypeAttribute myASpecialTypeAttribute)
        {

            if ((Object) myASpecialTypeAttribute == null)
            {
                return false;
            }

            return this.GetType().FullName == myASpecialTypeAttribute.GetType().FullName;

        }

        #endregion

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return this.GetType().FullName.GetHashCode();
        }

        #endregion

    }

}
