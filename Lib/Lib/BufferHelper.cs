using System;
using System.Collections.Generic;
using System.Text;

namespace sones.Lib
{
    public static class BufferHelper
    {

        public static int AlignBufferLength(int mySize, int myAlign)
        {

            if (mySize % myAlign == 0) return mySize;
            else return ((mySize / myAlign) + 1 ) * myAlign;

        }

    }

}
