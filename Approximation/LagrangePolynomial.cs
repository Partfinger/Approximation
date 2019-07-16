using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Approximation
{
    class LagrangePolynomial : Polynomial
    {
        public LagrangePolynomial( List<Vector2> points)
        {
            degree = points.Count;
            Polynomial polynomials = new Polynomial(0);
            for (int i = 0; i < degree; i++)
            {
                polynomials += GetLagrangePolinomForNum(ref points, i);
            }
            coefficientVector = polynomials.CoefficientVector;
        }

        Polynomial GetLagrangePolinomForNum(ref List<Vector2> points, int num)
        {
            Polynomial result = new Polynomial(1);
            double koef = 1;
            for (int i = 0; i < points.Count; i++)
            {
                if (i != num)
                {
                    result *= new Polynomial(-points[i].X, 1);
                    koef *= points[num].X - points[i].X;
                }
            }
            koef = points[num].Y / koef;
            result *= koef;
            return result;
        }
    }
}
