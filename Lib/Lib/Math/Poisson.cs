/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Statistics
{
    public class Poisson
    {
        private Random random;

        public Poisson()
        {
            random = new Random();
        }

        public double PoissonSample(double lambda)
        {
            double L = System.Math.Exp(-1.0 * lambda);
            double k = 0;
            double p = 1;

            do
            {
                k++;
                double u = random.NextDouble();
                p = p * u;
            }
            while (p >= L);
            return k - 1;
        }
    }
}
