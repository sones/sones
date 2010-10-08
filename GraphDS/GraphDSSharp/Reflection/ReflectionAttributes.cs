/*
 * sones GraphDS API - ReflectionAttributes
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;

using sones.GraphDB.NewAPI;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDS.API.CSharp.Reflection
{

    #region Mandatory

    /// <summary>
    /// Mandatory attributes have to be set to a value within
    /// an insert query.
    /// </summary>
    public class Mandatory : Attribute
    {
    }

    #endregion

    #region Default

    /// <summary>
    /// The default value of an attribute.
    /// </summary>
    public class Default : Attribute
    {

        public Object DefaultValue { get; private set; }

        public Default(Object myDefaultValue)
        {
            DefaultValue = myDefaultValue;
        }

    }

    #endregion

    #region Indexed

    /// <summary>
    /// Attributes may be indexed for a faster lookup.
    /// </summary>
    public class Indexed : Attribute
    {

        #region Properties (or named parameters)

        public String IndexName { get; set; }
        public String IndexType { get; set; }
        public String IndexOrder { get; set; }

        #endregion

        #region Constructors

        public Indexed()
        {
            IndexName = "";
            IndexOrder = "";
            IndexType = "";
        }

        public Indexed(String myIndexType)
        {
            IndexName = "";
            IndexOrder = "";
            IndexType = myIndexType;
        }

        public Indexed(DBIndexTypes myDBIndexType)
        {
            IndexName = "";
            IndexOrder = "";
            IndexType = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        public Indexed(String myIndexName, String myIndexType)
        {
            IndexName = myIndexName;
            IndexOrder = "";
            IndexType = myIndexType;
        }

        public Indexed(String myIndexName, DBIndexTypes myDBIndexType)
        {
            IndexName = myIndexName;
            IndexOrder = "";
            IndexType = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        public Indexed(String myIndexName, String myIndexOrder, String myIndexType)
        {
            IndexName = myIndexName;
            IndexOrder = myIndexOrder;
            IndexType = myIndexType;
        }

        public Indexed(String myIndexName, String myIndexOrder, DBIndexTypes myDBIndexType)
        {
            IndexName = myIndexName;
            IndexOrder = myIndexOrder;
            IndexType = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        #endregion

    }

    #endregion

    #region Edgetype

    //public class Edgetype : Attribute
    //{

    //    #region Properties (or Named Parameters)

    //    public Type DBObject { get; set; }

    //    #endregion

    //    #region Constructors

    //    public Edgetype(Type myType)
    //    {

    //        Type _Type = myType;

    //        while (!_Type.IsAssignableFrom(typeof(DBObject)))
    //        {

    //            _Type = _Type.BaseType;

    //            if (_Type == typeof(Object))
    //                throw new Exception("'" + myType.ToString() + "' has to be assignable from DBObject!");

    //        }

    //        DBObject = myType;

    //    }

    //    #endregion

    //}

    #endregion

    #region BackwardEdge

    /// <summary>
    /// Backwardedges are a hidden and automagically maintained feature
    /// of the database, but they may be accessed as normal attributes
    /// on request.
    /// </summary>
    public class BackwardEdge : Attribute
    {

        #region Properties (or Named Parameters)

        public String ReferencedAttributeName { get; set; }

        #endregion

        #region Constructors

        public BackwardEdge()
        {
            ReferencedAttributeName = "";
        }

        public BackwardEdge(String myReferencedAttributeName)
        {
            ReferencedAttributeName = myReferencedAttributeName;
        }

        #endregion

    }

    #endregion


    #region Temporary

    /// <summary>
    /// The values of temporary attributes will never be persistet
    /// by the database and thus removed at the given moment.
    /// </summary>
    public class Temporary : Attribute
    {

        public TemporaryType TemporaryType { get; private set; }

        public Temporary(TemporaryType myTemporaryType)
        {
            TemporaryType = myTemporaryType;
        }

    }

    #endregion

}
