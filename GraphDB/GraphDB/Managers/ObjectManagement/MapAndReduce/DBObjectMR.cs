

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.ObjectManagement
{


    /// <summary>
    /// The DBObject carries the myAttributes of a database object.
    /// </summary>
    public class DBObjectMR
    {

        private ObjectUUID _ObjectUUID;

        public ObjectUUID ObjectUUID
        {
            get
            {
                return _ObjectUUID;
            }
        }

        private Dictionary<String, Object> _Attributes;

        public IDictionary<String, Object> Attributes
        {
            get
            {
                return _Attributes;
            }
        }


        #region Constructors

        #region DBObjectMR()

        public DBObjectMR()
        {
        }

        #endregion

        #region DBObjectMR(myDBObjectMR)

        public DBObjectMR(DBObjectMR myDBObjectMR)
        {
            _ObjectUUID = myDBObjectMR.ObjectUUID;
            _Attributes = new Dictionary<String, Object>(myDBObjectMR.Attributes);
        }

        #endregion

        #region DBObjectMR(myObjectUUID, myAttributes)

        public DBObjectMR(ObjectUUID myObjectUUID, IDictionary<String, Object> myAttributes)
        {
            _ObjectUUID = myObjectUUID;
            _Attributes = new Dictionary<String, Object>(myAttributes);
        }

        #endregion

        #region (myDBObject, myTypeManager)

        public DBObjectMR(DBObjectStream myDBObject, GraphDBType myDBTypeStream, DBContext myTypeManager)
        {

            _ObjectUUID = myDBObject.ObjectUUID;
            _Attributes = new Dictionary<String, Object>();

            foreach (var _Attribute in myDBTypeStream.Attributes)
            {
                _Attributes.Add(_Attribute.Value.Name, myDBObject.GetAttribute(_Attribute.Key));
            }

        }

        #endregion

        #endregion



        #region ToString()

        public override String ToString()
        {

            var retVal = new StringBuilder("Attributes[" + _Attributes.Count + "] ");

            foreach (var attr in _Attributes)
                retVal.Append(attr.Key + " = " + attr.Value + ", ");

            retVal.Length = retVal.Length - 2;

            return retVal.ToString();

        }

        #endregion


    }

}
