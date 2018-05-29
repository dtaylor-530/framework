using Accord;
using Accord.Genetic;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SampleApp
{
    public class Optimisation2DWrap //: IGeneticWrap
    {
        ////public Population Population { get; set; }
        ////bool _showOnlyBest;
        ////double[,] _solution;
        ////OptimizationFunction2D _userFunction;
        ////private int _populationSize;

        ////public Optimisation2DWrap(int populationSize, int chromosomeLength, OptimizationFunction2D userFunction, int selectionMethod, int optimizationMode, bool showOnlyBest)
        ////{
        ////    _showOnlyBest = showOnlyBest;
        ////    _userFunction = userFunction;
        ////    _populationSize = populationSize;
        ////    // create population
        ////    Population = new Population(populationSize,
        ////         new BinaryChromosome(chromosomeLength),
        ////         userFunction,
        ////         (selectionMethod == 0) ? (ISelectionMethod)new EliteSelection() : (selectionMethod == 1) ? (ISelectionMethod)new RankSelection() : (ISelectionMethod)new RouletteWheelSelection()
        ////         );
        ////    // set optimization mode
        ////    userFunction.Mode = (optimizationMode == 0) ? OptimizationFunction2D.Modes.Maximization : OptimizationFunction2D.Modes.Minimization;

        ////    _solution = new double[(showOnlyBest) ? 1 : populationSize, 2];


        ////}



        ////public Result Evaluate()
        ////{

        ////    // show current solution
        ////    if (_showOnlyBest)
        ////    {
        ////        _solution[0, 0] = _userFunction.Translate(Population.BestChromosome);
        ////        _solution[0, 1] = _userFunction.OptimizationFunction(_solution[0, 0]);
        ////    }
        ////    else
        ////    {
        ////        for (int j = 0; j < _populationSize; j++)
        ////        {
        ////            _solution[j, 0] = _userFunction.Translate(Population[j]);
        ////            _solution[j, 1] = _userFunction.OptimizationFunction(_solution[j, 0]);
        ////        }
        ////    }

        ////    return new Result(_solution, _userFunction.Translate(Population.BestChromosome).ToString("F3"));
        ////}

    }
}
