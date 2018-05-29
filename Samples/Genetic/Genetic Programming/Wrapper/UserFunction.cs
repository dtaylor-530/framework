using Accord;
using Accord.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{

    public class UserFunction : OptimizationFunction1D
    {
        public UserFunction() : base(new Range(0, 255)) { }

        public override double OptimizationFunction(double x)
        {
            return Math.Cos(x / 23) * Math.Sin(x / 50) + 2;
        }
    }

}
