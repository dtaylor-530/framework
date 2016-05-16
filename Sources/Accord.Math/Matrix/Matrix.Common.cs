﻿// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Math
{
    using System;
    using Accord.Math.Decompositions;
    using Accord.Math.Comparers;
    using System.Collections.Generic;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Reflection;

    public static partial class Matrix
    {

        #region Comparison

        /// <summary>
        ///   Determines whether a number is an integer, given a tolerance threshold.
        /// </summary>
        /// 
        /// <param name="x">The value to be compared.</param>
        /// <param name="threshold">The maximum that the number can deviate from its closest integer number.</param>
        /// 
        /// <returns>True if the number if an integer, false otherwise.</returns>
        /// 
        public static bool IsInteger(this double x, double threshold)
        {
            double a = Math.Round(x);
            double b = x;

            if (a == b)
                return true;

            double limit = Math.Abs(a) * threshold;
            double delta = Math.Abs(a - b);

            return delta <= limit;
        }

        /// <summary>
        ///   Compares two values for equality, considering a relative acceptance threshold.
        /// </summary>
        /// 
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Obsolete("Use IsEqual(a, b, rtol) with the named parameter rtol instead.")]
        public static bool IsRelativelyEqual(this double a, double b, double threshold)
        {
            return a.IsEqual(b, rtol: threshold);
        }



        /// <summary>
        ///   Compares two matrices for equality.
        /// </summary>
        /// 
        public static bool IsEqual<T>(this T[][] objA, T[][] objB)
        {
            // TODO: Make this method recursive, or create
            // a enumerator to read all values from jagged
            // matrices sequentially
            if (objA == objB)
                return true;

            if (objA == null)
                throw new ArgumentNullException("objA");

            if (objB == null)
                throw new ArgumentNullException("objB");

            if (objA.Length != objB.Length)
                return false;

            for (int i = 0; i < objA.Length; i++)
            {
                if (objA[i] == objB[i])
                    continue;

                if (objA[i] == null || objB[i] == null)
                    return false;

                if (objA[i].Length != objB[i].Length)
                    return false;

                for (int j = 0; j < objA[i].Length; j++)
                {
                    var elemA = objA[i][j];
                    var elemB = objB[i][j];

                    if (!elemA.Equals(elemB))
                        return false;
                }
            }
            return true;
        }

        private static readonly Dictionary<Tuple<Type, Type>, MethodInfo> equalsCache =
            new Dictionary<Tuple<Type, Type>, MethodInfo>();

        /// <summary>Compares two matrices for equality.</summary>
        public static bool IsEqual(this Array objA, Array objB, double atol = 0, double rtol = 0)
        {
            if (objA == objB)
                return true;

            if (objA == null)
                throw new ArgumentNullException("objA");

            if (objB == null)
                throw new ArgumentNullException("objB");

            if (!objA.GetLength().IsEqual(objB.GetLength()))
                return false;
            
            // TODO: Implement this cache mechanism here
            // http://blog.slaks.net/2015-06-26/code-snippets-fast-property-access-reflection/

            // Base case: arrays contain values, not other arrays
            if (!objA.GetType().GetElementType().IsArray)
            {
                var typeA = objA.GetType();
                var typeB = objB.GetType();
                var equals = typeof(Matrix).GetMethod("IsEqual", new Type[]
                {
                    typeA, typeB, typeof(double), typeof(double) 
                });

                var _this = typeof(Matrix).GetMethod("IsEqual", new Type[] {
                    typeof(Array), typeof(Array), typeof(double), typeof(double)
                });

                if (equals != _this)
                {
                    return (bool)equals.Invoke(null, new object[] { objA, objB, atol, rtol });
                }
                if (atol == 0 && rtol == 0)
                {
                    var a = objA.GetEnumerator();
                    var b = objB.GetEnumerator();

                    while (a.MoveNext() && b.MoveNext())
                    {
                        if (a.Current == b.Current)
                            continue;

                        Array arrA = a.Current as Array;
                        Array arrB = b.Current as Array;
                        if (arrA != null && arrB != null && IsEqual(arrA, arrB))
                            continue;

                        if (!Object.Equals(a.Current, b.Current))
                            return false;
                    }

                    return true;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                var a = objA.GetEnumerator();
                var b = objB.GetEnumerator();

                while (a.MoveNext() && b.MoveNext())
                {
                    if (a.Current == b.Current)
                        continue;

                    Array arrA = a.Current as Array;
                    Array arrB = b.Current as Array;
                    if (arrA != null && arrB != null && IsEqual(arrA, arrB))
                        continue;

                    if (!Object.Equals(arrA, arrB))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        ///   This method should not be called. Use Matrix.IsEqual instead.
        /// </summary>
        /// 
        public static new bool Equals(object value)
        {
            throw new NotSupportedException("Use Matrix.IsEqual instead.");
        }

        /// <summary>
        ///   Checks whether two arrays have the same dimensions.
        /// </summary>
        /// 
        public static bool DimensionEquals(this Array a, Array b)
        {
            if (a.Rank != b.Rank)
                return false;
            for (int i = 0; i < a.Rank; i++)
                if (a.GetLength(i) != b.GetLength(i))
                    return false;
            return true;
        }

        /// <summary>
        ///   Compares two enumerables for set equality. Two
        ///   enumerables are set equal if they contain the
        ///   same elements, but not necessarily in the same
        ///   order.
        /// </summary>
        /// 
        /// <typeparam name="T">The element type.</typeparam>
        /// 
        /// <param name="list1">The first set.</param>
        /// <param name="list2">The first set.</param>
        /// 
        /// <returns>
        ///   True if the two sets contains the same elements, false otherwise.
        /// </returns>
        /// 
        public static bool SetEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();

            foreach (var s in list1)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else cnt.Add(s, 1);
            }

            foreach (var s in list2)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else return false;
            }

            foreach (var s in cnt)
            {
                if (s.Value != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsNaN(e)) return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsNaN(e))
                    return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN(this double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Double.IsNaN(matrix[i][j]))
                        return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains infinity values, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsInfinity(e))
                    return true;
            return false;
        }


        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value within a given tolerance.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="tolerance">The relative tolerance that a value must be in
        ///   order to be considered equal to the value being searched.</param>
        /// 
        /// <returns>True if the matrix contains the value, false otherwise.</returns>
        /// 
        public static bool Has(this double[,] matrix, double value, double tolerance = 0.0)
        {
            foreach (var e in matrix)
                if (Math.Abs(e - value) <= Math.Abs(e) * tolerance)
                    return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value within a given tolerance.
        /// </summary>
        /// 
        /// <param name="matrix">A single-precision multidimensional matrix.</param>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="tolerance">The relative tolerance that a value must be in
        ///   order to be considered equal to the value being searched.</param>
        /// 
        /// <returns>True if the matrix contains the value, false otherwise.</returns>
        /// 
        public static bool Has(this float[,] matrix, float value, double tolerance = 0.0)
        {
            foreach (var e in matrix)
                if (Math.Abs(e - value) <= Math.Abs(e) * tolerance)
                    return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[] matrix)
        {
            foreach (var e in matrix)
                if (Double.IsInfinity(e))
                    return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a infinity value.
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        /// 
        public static bool HasInfinity(this double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Double.IsInfinity(matrix[i][j]))
                        return true;
            return false;
        }

        #endregion


        #region Transpose

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[,] Transpose<T>(this T[,] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///   Gets the transpose of a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <param name="inPlace">True to store the transpose over the same input
        ///   <paramref name="matrix"/>, false otherwise. Default is false.</param>
        ///   
        /// <returns>The transpose of the given matrix.</returns>
        /// 
        public static T[,] Transpose<T>(this T[,] matrix, bool inPlace)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = i; j < cols; j++)
                    {
                        T element = matrix[j, i];
                        matrix[j, i] = matrix[i, j];
                        matrix[i, j] = element;
                    }
                }

                return matrix;
            }
            else
            {
                T[,] result = new T[cols, rows];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        result[j, i] = matrix[i, j];

                return result;
            }
        }



        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// 
        /// <param name="rowVector">A row vector.</param>
        /// 
        /// <returns>The transpose of the given vector.</returns>
        /// 
        public static T[,] Transpose<T>(this T[] rowVector)
        {
            var result = new T[rowVector.Length, 1];
            for (int i = 0; i < rowVector.Length; i++)
                result[i, 0] = rowVector[i];
            return result;
        }

        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// 
        /// <param name="rowVector">A row vector.</param>
        /// <param name="result">The matrix where to store the transpose.</param>
        ///
        /// <returns>The transpose of the given vector.</returns>
        /// 
        public static T[,] Transpose<T>(this T[] rowVector, out T[,] result)
        {
            result = new T[rowVector.Length, 1];
            for (int i = 0; i < rowVector.Length; i++)
                result[i, 0] = rowVector[i];
            return result;
        }

        /// <summary>
        ///   Gets the transpose of a row vector.
        /// </summary>
        /// 
        /// <param name="rowVector">A row vector.</param>
        /// <param name="result">The matrix where to store the transpose.</param>
        /// 
        /// <returns>The transpose of the given vector.</returns>
        /// 
        public static T[,] Transpose<T>(this T[] rowVector, T[,] result)
        {
            for (int i = 0; i < rowVector.Length; i++)
                result[i, 0] = rowVector[i];
            return result;
        }


        /// <summary>
        ///   Gets the generalized transpose of a tensor.
        /// </summary>
        /// 
        /// <param name="array">A tensor.</param>
        /// <param name="order">The new order for the tensor's dimensions.</param>
        /// 
        /// <returns>The transpose of the given tensor.</returns>
        /// 
        public static Array Transpose(this Array array, int[] order)
        {
            return transpose(array, order);
        }

        /// <summary>
        ///   Gets the generalized transpose of a tensor.
        /// </summary>
        /// 
        /// <param name="array">A tensor.</param>
        /// <param name="order">The new order for the tensor's dimensions.</param>
        /// 
        /// <returns>The transpose of the given tensor.</returns>
        /// 
        public static T Transpose<T>(this T array, int[] order)
            where T : class, ICloneable, IList
        {
            Array arr = array as Array;

            if (arr == null)
                throw new ArgumentException("The given object must inherit from System.Array.", "array");

            return transpose(arr, order) as T;
        }

        private static Array transpose(Array array, int[] order)
        {
            if (array.Length == 1 || array.Length == 0)
                return array;

            // Get the number of samples at each dimension
            int[] size = new int[array.Rank];
            for (int i = 0; i < size.Length; i++)
                size[i] = array.GetLength(i);

            Array r = Array.CreateInstance(array.GetType().GetElementType(), size.Submatrix(order));

            // Generate all indices for accessing the matrix 
            foreach (int[] pos in Combinatorics.Sequences(size, true))
            {
                int[] newPos = pos.Submatrix(order);
                object value = array.GetValue(pos);
                r.SetValue(value, newPos);
            }

            return r;
        }

        #endregion


        #region Matrix Characteristics

        /// <summary>
        ///   Gets the number of rows in a multidimensional matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of rows must be computed.</param>
        /// 
        /// <returns>The number of rows in the matrix.</returns>
        /// 
        public static int Rows<T>(this T[,] matrix)
        {
            return matrix.GetLength(0);
        }

        /// <summary>
        ///   Gets the number of columns in a multidimensional matrix.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// 
        /// <returns>The number of columns in the matrix.</returns>
        /// 
        public static int Columns<T>(this T[,] matrix)
        {
            return matrix.GetLength(1);
        }


        /// <summary>
        ///   Returns true if a vector of real-valued observations
        ///   is ordered in ascending or descending order.
        /// </summary>
        /// 
        /// <param name="values">An array of values.</param>
        /// <param name="direction">The sort order direction.</param>
        /// 
        public static bool IsSorted<T>(this T[] values, ComparerDirection direction) where T : IComparable<T>
        {
            if (direction == ComparerDirection.Ascending)
            {
                for (int i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) > 0)
                        return false;
            }
            else
            {
                for (int i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) < 0)
                        return false;
            }

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is square.
        /// </summary>
        public static bool IsSquare<T>(this T[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return matrix.GetLength(0) == matrix.GetLength(1);
        }

        /// <summary>
        ///   Returns true if a matrix is symmetric.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsSymmetric<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (matrix[i, j].CompareTo(matrix[j, i]) != 0)
                            return false;
                    }
                }
                return true;
            }
            return false;
        }



        /// <summary>
        ///   Returns true if a matrix is upper triangular.
        /// </summary>
        public static bool IsUpperTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < i; j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is lower triangular.
        /// </summary>
        public static bool IsLowerTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = i + 1; j < matrix.GetLength(1); j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Returns true if a matrix is diagonal
        /// </summary>
        public static bool IsDiagonal<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            T zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (i != j && matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///   Gets the trace of a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   The trace of an n-by-n square matrix A is defined to be the sum of the
        ///   elements on the main diagonal (the diagonal from the upper left to the
        ///   lower right) of A.
        /// </remarks>
        /// 
        public static double Trace(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            double trace = 0.0;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///   Gets the trace of a matrix product.
        /// </summary>
        /// 
        public static double Trace(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.Length != matrixB.Length)
                throw new DimensionMismatchException("matrixB", "Matrices must have the same length.");

            int length = matrixA.Length;

            unsafe
            {
                fixed (double* ptrA = matrixA)
                fixed (double* ptrB = matrixB)
                {
                    double* a = ptrA;
                    double* b = ptrB;

                    double trace = 0.0;
                    for (int i = 0; i < length; i++)
                        trace += (*a++) * (*b++);
                    return trace;
                }
            }
        }

        /// <summary>
        ///   Gets the trace of a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   The trace of an n-by-n square matrix A is defined to be the sum of the
        ///   elements on the main diagonal (the diagonal from the upper left to the
        ///   lower right) of A.
        /// </remarks>
        /// 
        public static int Trace(this int[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            int trace = 0;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///   Gets the trace of a matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   The trace of an n-by-n square matrix A is defined to be the sum of the
        ///   elements on the main diagonal (the diagonal from the upper left to the
        ///   lower right) of A.
        /// </remarks>
        /// 
        public static float Trace(this float[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);

            float trace = 0.0f;
            for (int i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///   Gets the diagonal vector from a matrix.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// 
        /// <returns>The diagonal vector from the given matrix.</returns>
        /// 
        public static T[] Diagonal<T>(this T[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var r = new T[matrix.GetLength(0)];
            for (int i = 0; i < r.Length; i++)
                r[i] = matrix[i, i];

            return r;
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static double Determinant(this double[,] matrix)
        {
            // Assume the most general case
            return Determinant(matrix, false);
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static double Determinant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, robust: true, lowerTriangular: true);

                if (!chol.PositiveDefinite)
                {
                    throw new ArgumentException("The matrix could not be decomposed using " +
                        " a robust Cholesky decomposition. Please specify symmetric as false " +
                        " and provide a full matrix to be decomposed.", "matrix");
                }

                return chol.Determinant;
            }

            return new LuDecomposition(matrix).Determinant;
        }

        /// <summary>
        ///   Gets the log-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogDeterminant(this double[,] matrix)
        {
            // Assume the most general case
            return LogDeterminant(matrix, false);
        }

        /// <summary>
        ///   Gets the log-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogDeterminant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, robust: true, lowerTriangular: true);

                if (!chol.PositiveDefinite)
                {
                    throw new ArgumentException("The matrix could not be decomposed using " +
                        " a robust Cholesky decomposition. Please specify symmetric as false " +
                        " and provide a full matrix to be decomposed.", "matrix");
                }

                return chol.LogDeterminant;
            }

            return new LuDecomposition(matrix).LogDeterminant;
        }

        /// <summary>
        ///   Gets the pseudo-determinant of a matrix.
        /// </summary>
        /// 
        public static double PseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).PseudoDeterminant;
        }

        /// <summary>
        ///   Gets the log of the pseudo-determinant of a matrix.
        /// </summary>
        /// 
        public static double LogPseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).LogPseudoDeterminant;
        }



        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static int Rank(this double[,] matrix)
        {
            return new SingularValueDecomposition(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).Rank;
        }

        /// <summary>
        ///   Gets the determinant of a matrix.
        /// </summary>
        /// 
        public static int Rank(this float[,] matrix)
        {
            return new SingularValueDecompositionF(matrix,
                computeLeftSingularVectors: false, computeRightSingularVectors: false,
                autoTranspose: true, inPlace: false).Rank;
        }

        /// <summary>
        ///    Gets whether a matrix is singular.
        /// </summary>
        /// 
        public static bool IsSingular(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            return new SingularValueDecomposition(matrix).IsSingular;
        }

        /// <summary>
        ///    Gets whether a matrix is positive definite.
        /// </summary>
        /// 
        public static bool IsPositiveDefinite(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return new CholeskyDecomposition(matrix).PositiveDefinite;
        }

        #endregion


        #region Summation
        /// <summary>Calculates the matrix Sum vector.</summary>
        /// 
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// 
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        /// 
        public static float[] Sum(this float[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[] Sum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[] sum;

            if (dimension == -1)
            {
                sum = new double[1];
                foreach (double a in matrix)
                    sum[0] += a;
            }
            else if (dimension == 0)
            {
                sum = new double[cols];

                for (int j = 0; j < cols; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new double[rows];

                for (int j = 0; j < rows; j++)
                {
                    double s = 0.0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static float[] Sum(this float[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            float[] sum;

            if (dimension == -1)
            {
                sum = new float[1];
                foreach (float a in matrix)
                    sum[0] += a;
            }
            else if (dimension == 0)
            {
                sum = new float[cols];

                for (int j = 0; j < cols; j++)
                {
                    float s = 0.0f;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new float[rows];

                for (int j = 0; j < rows; j++)
                {
                    float s = 0.0f;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }


        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(this int[,] matrix)
        {
            return Sum(matrix, 0);
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the sum will be calculated. Default is 0.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static int[] Sum(this int[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int[] sum;

            if (dimension == 0)
            {
                sum = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    int s = 0;
                    for (int i = 0; i < rows; i++)
                        s += matrix[i, j];
                    sum[j] = s;
                }
            }
            else if (dimension == 1)
            {
                sum = new int[rows];
                for (int j = 0; j < rows; j++)
                {
                    int s = 0;
                    for (int i = 0; i < cols; i++)
                        s += matrix[j, i];
                    sum[j] = s;
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        /// 
        public static double Sum(this double[] vector)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            double sum = 0.0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        /// 
        public static float Sum(this float[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            float sum = 0.0f;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>
        ///   Gets the sum of all elements in a vector.
        /// </summary>
        public static int Sum(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int sum = 0;
            for (int i = 0; i < vector.Length; i++)
                sum += vector[i];
            return sum;
        }

        /// <summary>Calculates a vector cumulative sum.</summary>
        public static double[] CumulativeSum(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            if (vector.Length == 0)
                return new double[0];

            double[] sum = new double[vector.Length];

            sum[0] = vector[0];
            for (int i = 1; i < vector.Length; i++)
                sum[i] += sum[i - 1] + vector[i];
            return sum;
        }

        /// <summary>Calculates the matrix Sum vector.</summary>
        /// <param name="matrix">A matrix whose sums will be calculated.</param>
        /// <param name="dimension">The dimension in which the cumulative sum will be calculated.</param>
        /// <returns>Returns a vector containing the sums of each variable in the given matrix.</returns>
        public static double[][] CumulativeSum(this double[,] matrix, int dimension)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            double[][] sum;

            if (dimension == 1)
            {
                sum = new double[matrix.GetLength(0)][];
                sum[0] = matrix.GetRow(0);

                // for each row
                for (int i = 1; i < matrix.GetLength(0); i++)
                {
                    sum[i] = new double[matrix.GetLength(1)];

                    // for each column
                    for (int j = 0; j < matrix.GetLength(1); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[i, j];
                }
            }
            else if (dimension == 0)
            {
                sum = new double[matrix.GetLength(1)][];
                sum[0] = matrix.GetColumn(0);

                // for each column
                for (int i = 1; i < matrix.GetLength(1); i++)
                {
                    sum[i] = new double[matrix.GetLength(0)];

                    // for each row
                    for (int j = 0; j < matrix.GetLength(0); j++)
                        sum[i][j] += sum[i - 1][j] + matrix[j, i];
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension", "dimension");
            }

            return sum;
        }
        #endregion

        #region Product
        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static double Product(this double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double product = 1.0;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }

        /// <summary>
        ///   Gets the product of all elements in a vector.
        /// </summary>
        public static int Product(this int[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            int product = 1;
            for (int i = 0; i < vector.Length; i++)
                product *= vector[i];
            return product;
        }
        #endregion


        #region Operation Mapping (Apply)





        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Length]);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, int, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Length]);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, TResult> func, TResult[] result)
        {
            for (int i = 0; i < vector.Length; i++)
                result[i] = func(vector[i]);
            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, int, TResult> func, TResult[] result)
        {
            for (int i = 0; i < vector.Length; i++)
                result[i] = func(vector[i], i);
            return result;
        }



        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, TResult> func)
        {
            return Apply(matrix, func, Matrix.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, int, int, TResult> func)
        {
            return Apply(matrix, func, Matrix.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, TResult> func, TResult[,] result)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = func(matrix[i, j]);
            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, int, int, TResult> func, TResult[,] result)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = func(matrix[i, j], i, j);
            return result;
        }





        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        [Obsolete("Please use Apply passing a result parameter instead.")]
        public static void ApplyInPlace<T>(this T[] vector, Func<T, T> func)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            for (int i = 0; i < vector.Length; i++)
                vector[i] = func(vector[i]);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        /// 
        [Obsolete("Please use Apply passing a result parameter instead.")]
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, T> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j]);
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        [Obsolete("Please use Apply passing a result parameter instead.")]
        public static void ApplyInPlace<T>(this T[,] matrix, Func<T, int, int, T> func)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = func(matrix[i, j], i, j);
        }



        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        [Obsolete("Please use Apply passing a result parameter instead.")]
        public static TResult[] ApplyWithIndex<TData, TResult>(this TData[] vector, Func<TData, int, TResult> func)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            TResult[] result = new TResult[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                result[i] = func(vector[i], i);

            return result;
        }

        /// <summary>
        ///   Applies a function to every element of a matrix.
        /// </summary>
        /// 
        [Obsolete("Please use Apply passing a result parameter instead.")]
        public static TResult[,] ApplyWithIndex<TData, TResult>(this TData[,] matrix, Func<TData, int, int, TResult> func)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            TResult[,] result = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = func(matrix[i, j], i, j);

            return result;
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TData, TResult>(this IList<TData> vector, Func<TData, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Count]);
        }

        /// <summary>
        ///   Applies a function to every element of the array.
        /// </summary>
        /// 
        public static TResult[] Apply<TData, TResult>(this IList<TData> vector, Func<TData, TResult> func, TResult[] result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = func(vector[i]);
            return result;
        }
        #endregion


        #region Rounding and discretization
        /// <summary>
        ///   Rounds a double-precision floating-point matrix to a specified number of fractional digits.
        /// </summary>
        /// 
        public static double[,] Round(this double[,] matrix, int decimals = 0)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Round(matrix[i, j], decimals);

            return result;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        /// 
        public static double[,] Floor(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Floor(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the matrix.
        /// </summary>
        public static double[,] Ceiling(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = System.Math.Ceiling(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Rounds a double-precision floating-point number array to a specified number of fractional digits.
        /// </summary>
        public static double[] Round(double[] vector, int decimals = 0)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Round(vector[i], decimals);
            return result;
        }

        /// <summary>
        ///   Returns the largest integer less than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Floor(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Floor(vector[i]);
            return result;
        }

        /// <summary>
        ///   Returns the largest integer greater than or equal than to the specified 
        ///   double-precision floating-point number for each element of the array.
        /// </summary>
        public static double[] Ceiling(double[] vector)
        {
            if (vector == null) throw new ArgumentNullException("vector");

            double[] result = new double[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Math.Ceiling(vector[i]);
            return result;
        }

        #endregion


        #region Morphological operations

        /// <summary>
        ///   Transforms a matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static T[] Reshape<T>(this T[,] matrix, MatrixOrder order = MatrixOrder.Default)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            return Reshape(matrix, new T[rows * cols], order);
        }

        /// <summary>
        ///   Transforms a matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="matrix">A matrix.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static T[] Reshape<T>(this T[,] matrix, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (order == MatrixOrder.CRowMajor)
            {
                int k = 0;
                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        result[k++] = matrix[j, i];
            }
            else
            {
                int k = 0;
                for (int i = 0; i < cols; i++)
                    for (int j = 0; j < rows; j++)
                        result[k++] = matrix[j, i];
            }

            return result;
        }


        /// <summary>
        ///   Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="array">A jagged array.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static T[] Reshape<T>(this T[][] array, MatrixOrder order = MatrixOrder.Default)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
                count += array[i].Length;
            return Reshape(array, new T[count], order);
        }

        /// <summary>
        ///   Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// 
        /// <param name="array">A jagged array.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">The direction to perform copying. Pass
        ///   1 to perform a copy by reading the matrix in row-major order.
        ///   Pass 0 to perform a copy in column-major copy. Default is 1 
        ///   (row-major, c-style order).</param>
        /// 
        public static T[] Reshape<T>(this T[][] array, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            if (order == MatrixOrder.CRowMajor)
            {
                int k = 0;
                for (int j = 0; j < array.Length; j++)
                    for (int i = 0; i < array[j].Length; i++)
                        result[k++] = array[j][i];
            }
            else
            {
                int maxCols = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Length > maxCols)
                        maxCols = array[i].Length;
                }

                for (int i = 0, k = 0; i < maxCols; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (i < array[j].Length)
                            result[k++] = array[j][i];
                    }
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        ///   Convolves an array with the given kernel.
        /// </summary>
        /// 
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        /// 
        public static double[] Convolve(this double[] a, double[] kernel)
        {
            return Convolve(a, kernel, false);
        }

        /// <summary>
        /// Convolves an array with the given kernel.
        /// </summary>
        /// 
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        /// <param name="trim">
        ///   If <c>true</c> the resulting array will be trimmed to
        ///   have the same length as the input array. Default is false.</param>
        ///   
        public static double[] Convolve(this double[] a, double[] kernel, bool trim)
        {
            double[] result;
            int m = (int)System.Math.Ceiling(kernel.Length / 2.0);

            if (trim)
            {
                result = new double[a.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (int j = 0; j < kernel.Length; j++)
                    {
                        int k = i - j + m - 1;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }
            else
            {
                result = new double[a.Length + m];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (int j = 0; j < kernel.Length; j++)
                    {
                        int k = i - j;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }

            return result;
        }


        /// <summary>
        ///   Creates a memberwise copy of a multidimensional matrix. Matrix elements
        ///   themselves are copied only in a shallowed manner (i.e. not cloned).
        /// </summary>
        /// 
        public static T[,] MemberwiseClone<T>(this T[,] a)
        {
            return (T[,])a.Clone();
        }

        /// <summary>
        ///   Copies the content of an array to another array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// 
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// 
        public static void CopyTo<T>(this T[,] matrix, T[,] destination)
        {
            Array.Copy(matrix, 0, destination, 0, matrix.Length);
        }


        /// <summary>
        ///   Copies the content of an array to another array.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// 
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// 
        public static void CopyTo<T>(this T[,] matrix, T[][] destination)
        {
            for (int i = 0; i < destination.Length; i++)
                for (int j = 0; j < destination[i].Length; j++)
                    destination[i][j] = matrix[i, j];
        }

        /// <summary>
        ///   Replaces one value by another in a matrix of any dimensions.
        ///   This is not an optimized operation.
        /// </summary>
        /// 
        /// <param name="array">The array where elements will be replaced.</param>
        /// <param name="from">The values which should be replaced.</param>
        /// <param name="to">The value to put in place of <paramref name="from"/>.</param>
        /// 
        /// <returns>A new array where all instances of <paramref name="from"/>
        ///   have been replaced with <paramref name="to"/>.</returns>
        /// 
        public static T Replace<T>(this T array, object from, object to)
            where T : class
        {
            Array ar = array as Array;
            Array result = (Array)ar.Clone();
            foreach (var dim in ar.GetIndices())
                if (result.GetValue(dim) == from)
                    result.SetValue(to, dim);
            return result as T;
        }
    }
}

