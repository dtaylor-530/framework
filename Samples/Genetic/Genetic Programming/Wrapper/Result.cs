using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public struct Result
    {

        public string BestSolution { get; }

        public double[,]Output { get; }

        public Result(double[,] output,string solution)
        {
  

            BestSolution = solution;
            Output = output;
        }


    }
}
