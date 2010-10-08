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
