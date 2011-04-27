using System.Collections.Generic;

namespace ShortestPathAlgorithms.BreathFirstSearch
{
    public sealed class Node
    {
        #region private members

        //unique identifier
        private long _Key;

        //are used to create the paths recursive
        private HashSet<Node> _Parents;

        //are used to create the paths recursive
        private HashSet<Node> _Children;

        //is used to check if the node is already in path
        private bool _AlreadyInPath;

        #endregion

        #region constructors

        public Node(long myObjectID)
        {
            _Key = myObjectID;
            
            _Parents = new HashSet<Node>();

            _Children = new HashSet<Node>();

            _AlreadyInPath = false;
        }

        public Node(long myObjectID, bool AlreadyInPath)
            : this(myObjectID)
        {
            _AlreadyInPath = AlreadyInPath;
        }

        public Node(long myObjectID, Node myParent)
            : this(myObjectID)
        {
            _Parents.Add(myParent);
        }

        #endregion

        #region getter/setter

        public long Key
        {
            get { return this._Key; }
            set { this._Key = value; }
        }

        public HashSet<Node> Parents
        {
            get { return this._Parents; }
            set { this._Parents = value; }
        }

        public HashSet<Node> Children
        {
            get { return this._Children; }
            set { this._Children = value; }
        }

        public bool AlreadyInPath
        {
            get { return this._AlreadyInPath; }
            set { this._AlreadyInPath = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Fügt dem Knoten ein Child hinzu, existiert dieser schon, werden die Parents und Children des existierenden aktualisiert.
        /// </summary>
        /// <param name="myChild">Child welches hinzugefügt werden soll.</param>
        /// <returns></returns>
        public bool addChild(Node myChild)
        {
            bool equal = false;

            foreach (var thisChild in _Children)
            {
                //check if the node wich should be added IS already existing
                if (thisChild.Equals(myChild))
                {
                    equal = true;

                    break;
                }
            }

            if (!equal)
            {
                return _Children.Add(myChild);
            }

            return false;
        }

        /// <summary>
        /// Fügt eine Liste von Children hinzu.
        /// </summary>
        /// <param name="myChildren">Liste von Children.</param>
        public void addChildren(HashSet<Node> myChildren)
        {
            foreach (var myChild in myChildren)
            {
                addChild(myChild);
            }
        }

        /// <summary>
        /// Fügt dem Knoten ein Parent hinzu, existiert dieser schon, werden die Parents und Children des existierenden aktualisiert.
        /// </summary>
        /// <param name="myParent">Parent welcher hinzugefügt werden soll.</param>
        /// <returns></returns>
        public bool addParent(Node myParent)
        {
            bool equal = false;

            foreach (var thisParent in _Parents)
            {
                //check if the node wich should be added IS already existing
                if (thisParent.Equals(myParent))
                {
                    //exists
                    equal = true;

                    break;
                }
            }

            //node is NOT already existing, add
            if (!equal)
            {
                return _Parents.Add(myParent);
            }

            return false;
        }

        /// <summary>
        /// Fügt eine Liste von Parents hinzu.
        /// </summary>
        /// <param name="myParents">Liste von Parents.</param>
        public void addParents(HashSet<Node> myParents)
        {
            foreach (var myParent in myParents)
            {
                addParent(myParent);
            }
        }

        /// <summary>
        /// Überprüft ob der angegebene Key bereits vorhanden ist.
        /// </summary>
        /// <param name="key">Key nach dem gesucht werden soll.</param>
        /// <returns></returns>
        public bool ChildrenContainsKey(long key)
        {
            foreach (var node in _Children)
            {
                if (node.Key.Equals(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Überprüft ob der angegebene Key bereits vorhanden ist.
        /// </summary>
        /// <param name="key">Key nach dem gesucht werden soll.</param>
        /// <returns></returns>
        public bool ParentsContainsKey(long key)
        {
            foreach (var node in _Parents)
            {
                if (node.Key.Equals(key))
                    return true;
            }

            return false;
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return _Key.GetHashCode();
        }

        /// <summary>
        /// Überprüft ob Nodes identisch sind.
        /// </summary>
        /// <param name="obj">Objekt der überprüft werden soll. Muss vom Typ "Node" sein.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return _Key.Equals((obj as Node).Key);
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
