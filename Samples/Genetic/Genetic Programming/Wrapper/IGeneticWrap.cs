using Accord.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{
    public interface IGeneticWrap
    {
        Population Population { get; set; }
        Result Evaluate();
    }




    public static class IGeneticWrapEx
    {

        public static Task RunMultipleEpochs(this IGeneticWrap wrap, int iterations, IProgress<KeyValuePair<int, Result>> progress = null)
        {
            return Task.Run(() =>
            {
                for (int i = 1; i < iterations + 1; i++)
                {
                    // run one epoch of genetic algorithm
                    wrap.Population.RunEpoch();
                    var t = wrap.Evaluate();
                    progress?.Report(new KeyValuePair<int, Result>(i, t));
                }

            });

        }


        public static Task RunMultipleEpochs(this IGeneticWrap wrap, int iterations, CancellationToken token, IProgress<KeyValuePair<int, Result>> progress = null)
        {
            return Task.Run(() =>
            {
                for (int i = 1; i < iterations + 1; i++)
                {

                    if (token.IsCancellationRequested == true) return;
                    // run one epoch of genetic algorithm
                    wrap.Population.RunEpoch();
                    var t = wrap.Evaluate();
                    progress?.Report(new KeyValuePair<int, Result>(i, t));
                }

            });


        }
    }
}
