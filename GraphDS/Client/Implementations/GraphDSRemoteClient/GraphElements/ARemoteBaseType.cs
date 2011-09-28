using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    public abstract class ARemoteBaseType : IBaseType
    {
        private long _ID;
        private String _Name;
        private String _Comment;
        private Boolean _IsUserDefined;

        internal ARemoteBaseType(ServiceBaseType myServiceBaseType)
        {
            _ID = myServiceBaseType.ID;
            _Name = myServiceBaseType.Name;
            _Comment = myServiceBaseType.Comment;
            _IsUserDefined = myServiceBaseType.IsUserDefined;
        }

        long ID
        {
            get { return _ID; }
        }

        string Name
        {
            get { return _Name; }
        }

        string Comment
        {
            get { return _Comment; }
        }

        bool IsUserDefined
        {
            get { return _IsUserDefined; }
        }
    }
}
