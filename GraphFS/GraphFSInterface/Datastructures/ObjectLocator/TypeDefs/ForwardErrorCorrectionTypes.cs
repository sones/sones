/* ForwardErrorCorrectionTypes
 * (c) Achim Friedland, 2008
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphFS.DataStructures
{

    
    public enum ForwardErrorCorrectionTypes : ushort
    {

        UNUSED,

        CONVOLUTION_1_2,
        CONVOLUTION_2_3,
        CONVOLUTION_3_4,
        CONVOLUTION_4_5,
        CONVOLUTION_5_6,
        CONVOLUTION_6_7,

        SCRAMBLEDCONVOLUTION_1_2,
        SCRAMBLEDCONVOLUTION_2_3,
        SCRAMBLEDCONVOLUTION_3_4,
        SCRAMBLEDCONVOLUTION_4_5,
        SCRAMBLEDCONVOLUTION_5_6,
        SCRAMBLEDCONVOLUTION_6_7,

        REEDSOLOMON,
        SCRAMBLEDREEDSOLOMON,

    }

}
