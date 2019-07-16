using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Approximation
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*Polynomial a = new Polynomial(-1);
            Polynomial b = new Polynomial(-3);
            Polynomial c = new Polynomial(-4);
            Polynomial d = a * b * c;
            Polynomial e = new Polynomial(28.8, 14.7, -8.91, 2.22, -0.192);
            string res = e.GetInCanonForm();

            Vector2[] points = new Vector2[]
            {
                new Vector2(1,36.6),
                new Vector2(2,37.2),
                new Vector2(3,37),
                new Vector2(4,37.8),
                new Vector2(5,36.8),
            };

            Polynomial L = new LeastSquaresPolynomial(points, 3);

            string res = L.GetInCanonForm();*/

            Application.Run(new Form1());
        }


    }
}
