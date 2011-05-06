using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class IndexRemoveException : AGraphDBIndexException
    {
        public String Name { get; private set; }
        public String Edition { get; private set; }


        public IndexRemoveException(String myName, String myEdition, string myInfo)
        {
            Name = myName;
            Edition = myEdition;
            _msg = String.Format("Could not remove the index with name {0} and edition {1}. {2}", Name, Edition, myInfo);
        }
    }
}

