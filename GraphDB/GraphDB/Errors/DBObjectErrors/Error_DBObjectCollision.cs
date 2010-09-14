using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;

namespace sones.GraphDB.Errors
{
    public class Error_DBObjectCollision : GraphDBObjectError
    {

        public DBObjectStream Object { get; private set; }

        public Error_DBObjectCollision(DBObjectStream myOBject)
        {
            Object = myOBject;
        }

        public override string ToString()
        {
            return String.Format("A DBObject collision occurred. The DBObject with attributes \"{0}\" has been inserted with a UUID that already exists!", Object);
        }

    }
}
