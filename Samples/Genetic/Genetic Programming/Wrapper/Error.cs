using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public struct Error
    {
        public double Learning { get; }
        public double Prediction { get; }

        public Error(double learningError, double predictionError)
        {
            Learning = learningError;
            Prediction = predictionError;

        }
    }
}
