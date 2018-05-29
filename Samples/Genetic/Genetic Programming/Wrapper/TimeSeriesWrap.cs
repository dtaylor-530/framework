using Accord;
using Accord.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{

    
    public class TimeSeriesWrap : IGeneticWrap
    {

        readonly double[] constants = new double[10] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };

        public Population Population { get; set; }

        private int solutionSize;
        private double[,] solution;
        private double[] input;

        IGPGene gene;
        TimeSeriesPredictionFitness fitness;
        double[] _data;
        int _predictionSize;
        int _populationSize;
        int _windowSize;


        public TimeSeriesWrap(double[] data, int windowSize, int predictionSize, int populationSize, int headLength, int functionsSet, int geneticMethod, int selectionMethod)
        {

            this._predictionSize = predictionSize;
            this._windowSize = windowSize;
            this._populationSize = populationSize;
            this._data = data;

            // create fitness function
            fitness = new TimeSeriesPredictionFitness(data, windowSize, predictionSize, constants);
            // create gene function
            gene = (functionsSet == 0) ? (IGPGene)new SimpleGeneFunction(windowSize + constants.Length) : (IGPGene)new ExtendedGeneFunction(windowSize + constants.Length);
            // create population
            Population = new Population(populationSize,
                (geneticMethod == 0) ? (IChromosome)new GPTreeChromosome(gene) : (IChromosome)new GEPChromosome(gene, headLength),
                fitness,
                (selectionMethod == 0) ? (ISelectionMethod)new EliteSelection() : (selectionMethod == 1) ? (ISelectionMethod)new RankSelection() : (ISelectionMethod)new RouletteWheelSelection());


            solutionSize = _data.Length - _windowSize;
            solution = new double[solutionSize, 2];
            input = new double[_windowSize + constants.Length];

            for (int j = 0; j < solutionSize; j++)
            {
                solution[j, 0] = j + _windowSize;
            }
            // prepare input
            Array.Copy(constants, 0, input, _windowSize, constants.Length);
        }






        public Result Evaluate()
        {
            // get best solution
            string bestFunction = Population.BestChromosome.ToString();

            // go through all the data
            for (int j = 0, n = _data.Length - _windowSize; j < n; j++)
            {
                // put values from current window as variables
                for (int k = 0, b = j + _windowSize - 1; k < _windowSize; k++)
                    input[k] = _data[b - k];

                // evaluate the function
                solution[j, 1] = PolishExpression.Evaluate(bestFunction, input);

            }

            return new Result(solution, bestFunction);
        }




        public Error EvaluateError()
        {
            // calculate prediction/learning error
            double learningError = 0.0;
            double predictionError = 0.0;

            for (int j = 0, n = _data.Length - _windowSize; j < n; j++)
            {

                // calculate prediction error
                if (j < n - _predictionSize)
                    learningError += Math.Abs(solution[j, 1] - _data[_windowSize + j]);
                else
                    predictionError += Math.Abs(solution[j, 1] - _data[_windowSize + j]);
            }

            return new Error(learningError, predictionError);

        }






    }








}
