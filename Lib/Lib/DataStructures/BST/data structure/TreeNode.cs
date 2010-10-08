using System;
using System.Collections.Generic;
using System.Text;

namespace sones.Lib.BTree
{

    /// <summary>
    /// The Node class represents the base concept of a Node for a tree or graph.  It contains
    /// a data item of type T, and a list of neighbors.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the Node.</typeparam>
    /// <remarks>None of the classes in the sones.Graph.Storage.BTree namespace use the Node class directly;
    /// they all derive from this class, adding necessary functionality specific to each data structure.</remarks>

    

    public class Node<T>
    {


        #region Private Member Variables

        private T data;

        private NodeList<T> neighbors = null;

        #endregion


        #region Constructors

        public Node() { }
        
        public Node(T data) : this(data, null) { }
        
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The element stored in this tree
        /// </summary>
        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        /// <summary>
        /// the neighbors of this tree node
        /// </summary>
        protected NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }

        #endregion


    }

}
