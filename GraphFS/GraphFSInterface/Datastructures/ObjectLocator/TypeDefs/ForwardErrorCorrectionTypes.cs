/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* ForwardErrorCorrectionTypes
 * Achim Friedland, 2008
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
