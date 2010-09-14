/*
 * GraphFSStreamUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class GraphFSStreamUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 230; } }

        #endregion

        #region Constructors

        #region GraphFSStreamUUID()

        public GraphFSStreamUUID()
            : base()
        {
        }

        #endregion

        #region GraphFSStreamUUID(myUInt64)

        public GraphFSStreamUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region GraphFSStreamUUID(myString)

        public GraphFSStreamUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region GraphFSStreamUUID(mySerializedData)

        public GraphFSStreamUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static GraphFSStreamUUID NewUUID
        {
            get
            {
                return new GraphFSStreamUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


    }

}
