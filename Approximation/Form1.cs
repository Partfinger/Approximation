using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Approximation
{
    public partial class Form1 : Form
    {
        int size;
        int nx = 7, ny = 10;
        int min, max;
        int pointRadius = 10;
        Bitmap background;
        bool selected;
        int selectedNumInList = -1;
        Vector2 selectedVector;

        List<Vector2> points = new List<Vector2>();

        List<bool> bools = new List<bool>();
        List<Polynomial> polynomials = new List<Polynomial>();
        List<Color> colors = new List<Color>();

        public Form1()
        {
            InitializeComponent();
            size = Field.Width;
            min = size / 20;
            max = size - min;
            Vector2.kx = 3;
            Vector2.ky = 2;
            Vector2.max = max;
            Vector2.min = min;
            bools.Add(false);
            bools.Add(false);
            polynomials.Add(new Polynomial(0));
            polynomials.Add(new Polynomial(0));
            colors.Add(panel3.BackColor);
            colors.Add(panel4.BackColor);
            background = GetCartesianCoordinates();
            Update();
        }

        private void Field_MouseDown(object sender, MouseEventArgs e)
        {
            if ((selectedVector = GetPointOnField(e.X, e.Y)) == null)
            {
                Vector2 point = new Vector2(e.X, e.Y);
                points.Add(point);
                selectedNumInList = -1;
                selected = false;
            }
            else if (ModifierKeys == Keys.Shift)
            {
                points.Remove(selectedVector);
                selectedNumInList = -1;
                selected = false;
            }
            else
            {
                selected = true;
            }
            Update();
        }

        void AddPoint(double x, double y)
        {
            Vector2 v = Vector2.GetVector2(x, y);
            if (GetPointOnField(v.Point.X, v.Point.Y) == null)
            {
                points.Add(v);
            }
        }

        void Update()
        {
            Bitmap back = new Bitmap(background);
            Field.Image = GetPoints(back);
        }

        Vector2 GetPointOnField(int x, int y)
        {
            Vector2 s = null;
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i].Point;
                if ((Math.Abs(p.X - x) + Math.Abs(p.Y - y)) < pointRadius + 3) 
                {
                    if (selectedVector != null && points[i] == selectedVector)
                    {
                        s = points[i];
                        continue;
                    }
                    selectedNumInList = i;
                    return points[i];
                }
            }
            return s;
        }

        void UpdateTogetherSpline()
        {
            Bitmap back = new Bitmap(background);
            selectedVector = null;
            selectedNumInList = -1;
            Field.Image = GetSplines(GetPoints(back));
        }

        Bitmap GetPoints(Bitmap back)
        {
            Graphics g = Graphics.FromImage(back);
            if (selectedNumInList > -1)
            {
                g.FillEllipse(Brushes.Green, selectedVector.Point.X - pointRadius - 2, selectedVector.Point.Y - pointRadius - 2, 2 * pointRadius + 4, 2 * pointRadius + 4);
            }
            foreach (Vector2 point in points)
            {
                g.FillEllipse(Brushes.Red, point.Point.X - pointRadius, point.Point.Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
            }
            return back;
        }

        private void Panel3_MouseDown(object sender, MouseEventArgs e)
        {
            colorDialog1.AllowFullOpen = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = textBox1.ForeColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[0] = colorDialog1.Color;
                panel3.BackColor = colorDialog1.Color;
            }
        }

        private void Panel4_MouseDown(object sender, MouseEventArgs e)
        {
            colorDialog1.AllowFullOpen = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = textBox1.ForeColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[1] = colorDialog1.Color;
                panel4.BackColor = colorDialog1.Color;
            }
        }

        private void Button1_Click(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked)
            {
                bools[0] = true;
                polynomials[0] = new LagrangePolynomial(points);
            }
            else
            {
                bools[0] = false;
            }
            if (checkBox2.Checked)
            {
                int degree = 0;
                try
                {
                    degree = Convert.ToInt32(textBox1.Text);
                }
                catch (FormatException)
                {
                    DialogResult dialog = MessageBox.Show("Лабуда якась записана в полі порядку апроксимації: \n " + textBox1.Text, "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (degree < points.Count)
                {
                    polynomials[1] = new LeastSquaresPolynomial(points, degree);
                    if (polynomials[1].CoefficientVector == null)
                    {
                        bools[1] = false;
                    }
                    else
                    {
                        bools[1] = true;
                    }
                }
                else
                {
                    DialogResult dialog = MessageBox.Show("Порядок полінома для апроксимації більше або рівний кількості заданих точок!", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bools[1] = false;
            }
            if (points.Count > 0 && (bools[0] || bools[1] ))
            {
                UpdateTogetherSpline();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int degree = -1;
            try
            {
                degree = Convert.ToInt32(textBox3.Text);
            }
            catch(FormatException)
            {
                DialogResult dialog = MessageBox.Show("Лабуда якась записана в полі порядку екстраполяції: \n " + textBox3.Text, "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<int> nums;
            if ((nums = GetPointForX(0)).Count != 0)
            {
                foreach (int i in nums)
                {
                    points.RemoveAt(i);
                }
            }
            if ((nums = GetPointForX(6)).Count != 0)
            {
                foreach (int i in nums)
                {
                    points.RemoveAt(i);
                }
            }
            if (degree < points.Count)
            {
                Polynomial extra = new LeastSquaresPolynomial(points, degree);
                AddPoint(0, extra.Solve(0));
                AddPoint(6, extra.Solve(6));
                Update();
            }
            else
            {
                DialogResult dialog = MessageBox.Show("Порядок полінома для ектраполяції більше або рівний кількості заданих точок!", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        List<int> GetPointForX(int x)
        {
            List<int> reslt = new List<int>();
            for (int i = 0; i < points.Count; i++) 
            {
                if (points[i].X == x)
                {
                    reslt.Add(i);
                }
            }
            return reslt;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            foreach (Vector2 p in points)
            {
                p.RoundX();
            }
            Update();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            points.Clear();
            Update();
        }

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected)
            {
                selectedVector = new Vector2(e.X, e.Y);
                points[selectedNumInList] = selectedVector;
                Update();
            }
        }

        private void Field_MouseUp(object sender, MouseEventArgs e)
        {
            selected = false;
            Update();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("ЛКМ - поставить/вибрати точку\n вибрану точку можна рухати зажатим ЛКМ по графіку.\n shift+ЛКМ - видалити вибрану точку.\n Після екстраполяції можливо, що точок не буде на графіку - вони знаходятся за межами данної площини.", "Інструкція", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string text = "";
            if (bools[0])
            {
                text += string.Format("Лагранж:\n {0}\n", polynomials[0].ToString());
            }
            if (bools[1])
            {
                text += string.Format("МНК, {0} порядок:\n {1}", polynomials[1].Degree - 1, polynomials[1].ToString());
            }
            DialogResult dialog = MessageBox.Show(text, "Іформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        Bitmap GetCartesianCoordinates()
        {
            Bitmap coor = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(coor);
            g.Clear(Color.White);
            Pen pen = new Pen(Color.Black);
            pen.EndCap = LineCap.ArrowAnchor;
            pen.Width = 4;
            g.DrawLine(pen, new Point(min, max), new Point(max, max));
            g.DrawLine(pen, new Point(min, max), new Point(min, min));
            pen = new Pen(Color.Black);
            pen.Width = 2;
            for (int i = 1; i < nx; i++)
            {
                g.DrawLine(pen, new Point(min + min * i * 3, max - 15), new Point(min + min * i * 3, max));
                pen.DashStyle = DashStyle.Dash;
                pen.Color = Color.Gray;
                g.DrawLine(pen, new Point(min + min * i * 3, max - 15), new Point(min + min * i * 3, min));
                pen.Color = Color.Black;
                pen.DashStyle = DashStyle.Solid;
                g.DrawString(i.ToString(), new Font("Arial", 12), Brushes.Black, new Point(min + min * i * 3, max + 4));
            }
            for (int i = 0; i < ny; i++)
            {
                g.DrawLine(pen, new Point(min , max - min * i * 2), new Point(min + 15, max - min * i * 2));
                g.DrawString(i.ToString(), new Font("Arial", 12), Brushes.Black, new Point(min - 20, max - min * i * 2));
            }
            
            return coor;
        }

        Bitmap GetSplines(Bitmap bitmap)
        {
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Brushes.White);
            pen.Width = 3;
            for (int i = 0; i < polynomials.Count; i++)
            {
                if (bools[i])
                {
                    
                    pen.Color = colors[i];
                    int j = 0;
                    Point[] p = new Point[61];
                    for (double x = 0; x < 6; x += 0.1, j++)
                    {
                        p[j] = Vector2.GetVector2(x, polynomials[i].Solve(x)).Point;
                    }
                    try
                    {
                        g.DrawCurve(pen, p, 0);
                    }
                    catch(OverflowException)
                    {
                        DialogResult dialog = MessageBox.Show("Переповнення стеку під час малювання сплайнів.\n Причини: занадто близько розташовані точки (накладання), або більше одної точки на прямій паралельній Oy ( x1 = x2 = x3 = ...)", "Помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return bitmap;
                    }
                }
            }
            return bitmap;
        }
    }
}
