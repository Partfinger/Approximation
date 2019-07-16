using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Approximation
{
    class LeastSquaresPolynomial : Polynomial
    {
        public LeastSquaresPolynomial(List<Vector2> points, int degree)
        {
            this.degree = degree + 1;
            double[,] A = CreateMatrix(ref points, ref degree);
            double[] b = GetFreeMembers(ref points, ref degree);
            coefficientVector = GaussSolve(A, b);
        }

        double[,] CreateMatrix(ref List<Vector2> points, ref int degree)
        {
            double[,] matrix = new double[degree + 1, degree + 1];
            for (int i = 0; i < degree + 1; i++)
            {
                for (int j = 0; j < degree + 1; j++)
                {
                    matrix[i,j] = 0;
                    for (int k = 0; k < points.Count; k++)
                    {
                        matrix[i, j] += Math.Pow(points[k].X, i + j);
                    }
                }
            }
            return matrix;
        }

        double[] GetFreeMembers(ref List<Vector2> points, ref int degree)
        {
            double[] coeffs = new double[degree + 1];
            for (int i = 0; i < degree + 1; i++)
            {
                coeffs[i] = 0;
                for (int k = 0; k < points.Count; k++)
                {
                    coeffs[i] += Math.Pow(points[k].X, i) * points[k].Y;
                }
            }
            return coeffs;
        }

        private double[] GaussSolve(double[,] A, double[] b)
        {
            int[] index = InitIndex(b.Length);
            if (GaussForwardStroke(index, ref A, ref b) == 1)
            {
                return GaussBackwardStroke(index, ref A, ref b);
            }
            else
            {
                return null;
            }
        }

        private int[] InitIndex(int size)
        {
            int[] index = new int[size];
            for (int i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        private byte GaussForwardStroke(int[] index, ref double[,] A, ref double[] b)
        {
            for (int i = 0; i < b.Length; ++i)
            {
                double r = FindR(i, index, ref A, ref b);
                if (double.IsNaN(r))
                {
                    return 0;
                }

                for (int j = 0; j < b.Length; ++j)
                    A[i, j] /= r;

                b[i] /= r;

                for (int k = i + 1; k < b.Length; ++k)
                {
                    double p = A[k, index[i]];
                    for (int j = i; j < b.Length; ++j)
                        A[k, index[j]] -= A[i, index[j]] * p;
                    b[k] -= b[i] * p;
                    A[k, index[i]] = 0.0;
                }
            }
            return 1;
        }

        private double FindR(int row, int[] index, ref double[,] A, ref double[] b)
        {
            int max_index = row;
            double max = A[row, index[max_index]];
            double max_abs = Math.Abs(max);

            for (int cur_index = row + 1; cur_index < b.Length; ++cur_index)
            {
                double cur = A[row, index[cur_index]];
                double cur_abs = Math.Abs(cur);
                if (cur_abs > max_abs)
                {
                    max_index = cur_index;
                    max = cur;
                    max_abs = cur_abs;
                }
            }

            if (max_abs < 0.000001)
            {
                if (Math.Abs(b[row]) > 0.000001)
                {
                    DialogResult dialog = MessageBox.Show("Не вдалося знайти корені системи лінійних рівнянь методом Гауса:\n Система рівнянь несумісна!", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return double.NaN;
                }
                else
                {
                    DialogResult dialog = MessageBox.Show("Не вдалося знайти корені системи лінійних рівнянь методом Гауса:\n Система має безліч коренів!", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return double.NaN;
                }
            }

            int temp = index[row] * 1;
            index[row] = index[max_index] * 1;
            index[max_index] = temp;

            return max;
        }

        private double[] GaussBackwardStroke(int[] index, ref double[,] A, ref double[] b)
        {
            double[] coeffs = new double[b.Length];
            for (int i = b.Length - 1; i >= 0; --i)
            {
                double x_i = b[i];

                for (int j = i + 1; j < b.Length; ++j)
                    x_i -= coeffs[index[j]] * A[i, index[j]];
                coeffs[index[i]] = x_i;
            }
            return coeffs;
        }
    }
}
