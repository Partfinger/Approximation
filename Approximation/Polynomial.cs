using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Approximation
{
    /// <summary>
    /// Полином
    /// </summary>
    class Polynomial
    {
        protected double[] coefficientVector;
        protected int degree;

        public double[] CoefficientVector { get { return coefficientVector; } }
        public int Degree { get { return degree; } }

        public Polynomial(double[] coefficientVector, int degree)
        {
            this.coefficientVector = coefficientVector;
            this.degree = degree;
        }

        /// <summary>
        /// [0] + [1]x + [2]x^2 + ...
        /// </summary>
        /// <param name="integers"></param>
        public Polynomial(params double[] coeffs)
        {
            this.coefficientVector = coeffs; 
            this.degree = coeffs.Length;
        }

        public Polynomial(double b)
        {
            this.coefficientVector = new double[] { b };
            this.degree = 1;
        }

        public static Polynomial operator +(Polynomial A, Polynomial B)
        {
            int D1 = Math.Max(A.degree, B.degree);
            double[] M1 = new double[D1];
            Polynomial C = (D1 == A.degree) ? new Polynomial(A.coefficientVector, D1) : new Polynomial(B.coefficientVector, D1);
            for (int i = 0; i < D1; i++)
            {
                try
                {
                    C.coefficientVector[i] = A.coefficientVector[i] + B.coefficientVector[i];
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }

            return C;
        }

        public static Polynomial operator -(Polynomial A, Polynomial B)
        {
            int D1 = A.degree;
            double[] M1 = new double[D1 + 1];
            Polynomial C = new Polynomial(M1, D1);
            for (int i = 0; i < A.degree + 1; i++)
            {
                C.coefficientVector[i] = A.coefficientVector[i] - B.coefficientVector[i];
            }

            return C;
        }

        public static Polynomial operator +(Polynomial A, double k)
        {
            Polynomial C = new Polynomial(A.coefficientVector, A.degree);
            C.coefficientVector[0] += k;
            return C;
        }

        public static Polynomial operator -(Polynomial A, double k)
        {
            Polynomial C = new Polynomial(A.coefficientVector, A.degree);
            C.coefficientVector[0] -= k;
            return C;
        }

        public static Polynomial operator *(Polynomial A, Polynomial B)
        {
            int newDegree = A.degree + B.degree - 1;
            double[] coeffs = new double[newDegree];
            for (int i = 0; i < A.degree; ++i)
                for (int j = 0; j < B.degree; ++j)
                    coeffs[i + j] += A.coefficientVector[i] * B.coefficientVector[j];
            return new Polynomial(coeffs, newDegree);
        }

        public static Polynomial operator *(Polynomial A, double d)
        {
            double[] coeffs = new double[A.degree];
            for (int i = 0; i < A.degree; ++i)
            {
                coeffs[i] = d * A.coefficientVector[i];
            }
            return new Polynomial(coeffs);
        }

        public static Polynomial operator /(Polynomial A, double d)
        {
            double[] coeffs = new double[A.degree];
            for (int i = 0; i < A.degree; ++i)
            {
                coeffs[i] = A.coefficientVector[i] / d;
            }
            return new Polynomial(coeffs);
        }

        public string ToString()
        {
            string result = (coefficientVector[degree - 1] > 0) ? "" : "- ";
            for (int i= degree - 1; i > 1 ;i--)
            {
                result += string.Format("{0}*x^{1} {2} ", Math.Abs(Math.Round(coefficientVector[i], 2)), i, (coefficientVector[i-1] > 0) ? "+" : "-");
            }
            result += string.Format("{0}x {1} ", Math.Abs(Math.Round(coefficientVector[1], 2)), (coefficientVector[0] > 0) ? "+" : "-");
            result += string.Format("{0}", Math.Abs(Math.Round(coefficientVector[0], 2)));
            return result;
        }

        public double Solve(double x)
        {
            double result = 0.0;
            for(int p = 0; p < degree; p++)
            {
                result += Math.Pow(x, p) * coefficientVector[p];
            }
            return result;
        }
    }
}
