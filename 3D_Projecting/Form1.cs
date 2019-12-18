using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Projecting
{
    struct Point3D
    {
        public double x, y, z;
        public Point3D(double a, double b, double c)
        {
            this.x = a;
            this.y = b;
            this.z = c;
        }
    }

    

    public partial class Form1 : Form
    {
        public int x = 75, y = 75;
        Point3D[] points;
        Point[] toDrawPoints;
        double[,] projectionMatrix, RotationX, RotationY, RotationZ;
        double angle = 0;
        double distance = 5;
        double angleX = 0;
        double angleY = 0;

        bool toRotate = false;

        public Form1()
        {
            InitializeComponent();
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            double label_angle_x = ((angleX * 180) / Math.PI) % 180;
            Xlabel.Text = "X axis: " + label_angle_x;
            double label_angle_Y = ((angleY * 180) / Math.PI) % 180;
            Ylabel.Text = "Y axis: " + label_angle_Y;

            double distance = -((points[0].z + points[4].z) / 2);
            label1.Text = "Distance : " + distance;

            //update to rotation matrixes
            //RotationX = new double[,] { { 1, 0, 0 }, { 0, Math.Cos(angle), -Math.Sin(angle) }, { 0, Math.Sin(angle), Math.Cos(angle) } };

            //RotationY = new double[,] { { Math.Cos(angle), 0, Math.Sin(angle) }, { 0, 1, 0 }, { -Math.Sin(angle), 0, Math.Cos(angle) } };

            //RotationZ = new double[,] { { Math.Cos(angle), -Math.Sin(angle), 0 }, { Math.Sin(angle), Math.Cos(angle), 0 }, { 0, 0, 1 } };



            //loop all over the points and to the rotation & projection transformation

            for (int i = 0; i < points.Length; i++)
            {
                double[] vector = {points[i].x, points[i].y, points[i].z};
                double[] projected;

               
                double[] rotated = MatMulVector(RotationX, vector);
                rotated = MatMulVector(RotationY, rotated);
                rotated = MatMulVector(RotationZ, rotated);
                projected = MatMulVector(projectionMatrix, rotated);
                    

               

                toDrawPoints[i].X = (int)(projected[0]);
                toDrawPoints[i].Y = (int)(projected[1]);
            }

            if(toRotate)
                angle = angle + 0.007;
            normalize();
            Canvas.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //initialize points array

            points = new Point3D[8];
            points[0] = new Point3D(75, 75, 75);
            points[1] = new Point3D(-75, 75, 75);
            points[3] = new Point3D(75, -75, 75);
            points[2] = new Point3D(-75, -75, 75);
            points[4] = new Point3D(75, 75, -75);
            points[5] = new Point3D(-75, 75, -75);
            points[7] = new Point3D(75, -75, -75);
            points[6] = new Point3D(-75, -75, -75);

            toDrawPoints = new Point[points.Length];
            toDrawPoints[0] = new Point(0, 0);
            toDrawPoints[1] = new Point(0, 0);
            toDrawPoints[2] = new Point(0, 0);
            toDrawPoints[3] = new Point(0, 0);
            toDrawPoints[4] = new Point(0, 0);
            toDrawPoints[5] = new Point(0, 0);
            toDrawPoints[6] = new Point(0, 0);
            toDrawPoints[7] = new Point(0, 0);

            //initialize projecting matrix

            projectionMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

            //initialize rotation matrixes 

            RotationX = new double[,] { { 1, 0, 0 }, { 0, Math.Cos(angle), -Math.Sin(angle) }, { 0, Math.Sin(angle), Math.Cos(angle) } };

            RotationY = new double[,] { { Math.Cos(angle), 0, Math.Sin(angle) }, { 0, 1, 0}, { -Math.Sin(angle), 0, Math.Cos(angle) } };

            RotationZ = new double[,] { { Math.Cos(angle), -Math.Sin(angle), 0 }, { Math.Sin(angle), Math.Cos(angle), 0 }, { 0, 0, 1 } };


        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = Canvas.CreateGraphics();
            Pen pen = new Pen(Color.White);

            for (int i = 0; i < toDrawPoints.Length; i++)
            {
                g.DrawEllipse(pen, toDrawPoints[i].X, toDrawPoints[i].Y, 5, 5);
            }

            //Draw lines between cube points to create the cube 

            g.DrawLine(pen, toDrawPoints[0], toDrawPoints[1]);
            g.DrawLine(pen, toDrawPoints[1], toDrawPoints[2]);
            g.DrawLine(pen, toDrawPoints[2], toDrawPoints[3]);
            g.DrawLine(pen, toDrawPoints[3], toDrawPoints[0]);

            g.DrawLine(pen, toDrawPoints[1], toDrawPoints[5]);
            g.DrawLine(pen, toDrawPoints[5], toDrawPoints[4]);
            g.DrawLine(pen, toDrawPoints[4], toDrawPoints[0]);
            g.DrawLine(pen, toDrawPoints[0], toDrawPoints[1]);

            g.DrawLine(pen, toDrawPoints[5], toDrawPoints[6]);
            g.DrawLine(pen, toDrawPoints[6], toDrawPoints[7]);
            g.DrawLine(pen, toDrawPoints[7], toDrawPoints[4]);
            g.DrawLine(pen, toDrawPoints[4], toDrawPoints[5]);

            g.DrawLine(pen, toDrawPoints[0], toDrawPoints[4]);
            g.DrawLine(pen, toDrawPoints[4], toDrawPoints[7]);
            g.DrawLine(pen, toDrawPoints[7], toDrawPoints[3]);
            g.DrawLine(pen, toDrawPoints[4], toDrawPoints[0]);

            g.DrawLine(pen, toDrawPoints[2], toDrawPoints[6]);


        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
          
            if (e.KeyCode == Keys.Space)
            {
                if (toRotate)
                    toRotate = false;
                else
                    toRotate = true;
                    
            }

            if (e.KeyCode == Keys.Down)
            {
                angleX += 0.05;
                RotationX = new double[,] { { 1, 0, 0 }, { 0, Math.Cos(angleX), -Math.Sin(angleX) }, { 0, Math.Sin(angleX), Math.Cos(angleX) } };
            }

            if (e.KeyCode == Keys.Up)
            {
                angleX -= 0.05;
                RotationX = new double[,] { { 1, 0, 0 }, { 0, Math.Cos(angleX), -Math.Sin(angleX) }, { 0, Math.Sin(angleX), Math.Cos(angleX) } };
            }

            if (e.KeyCode == Keys.Left)
            {
                angleY -= 0.05;
                RotationY = new double[,] { { Math.Cos(angleY), 0, Math.Sin(angleY) }, { 0, 1, 0 }, { -Math.Sin(angleY), 0, Math.Cos(angleY) } };
            }

            if (e.KeyCode == Keys.Right)
            {
                angleY += 0.05;
                RotationY = new double[,] { { Math.Cos(angleY), 0, Math.Sin(angleY) }, { 0, 1, 0 }, { -Math.Sin(angleY), 0, Math.Cos(angleY) } };
            }

            if (e.KeyCode == Keys.A)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].x -= 5;
                    angleY -= 0.001;
                    RotationY = new double[,] { { Math.Cos(angleY), 0, Math.Sin(angleY) }, { 0, 1, 0 }, { -Math.Sin(angleY), 0, Math.Cos(angleY) } };

                }
            }

            if (e.KeyCode == Keys.D)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].x += 5;
                    angleY += 0.001;
                    RotationY = new double[,] { { Math.Cos(angleY), 0, Math.Sin(angleY) }, { 0, 1, 0 }, { -Math.Sin(angleY), 0, Math.Cos(angleY) } };


                }
            }

            if (e.KeyCode == Keys.W)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].z--;
                    if (points[i].x < 0)
                    {
                        points[i].x += 1;
                        if (points[i].y < 0)
                            points[i].y++;
                        else
                            points[i].y--;
                     
                    }
                    if (points[i].x > 0)
                    {
                        points[i].x--;
                        if (points[i].y < 0)
                            points[i].y++;
                        else
                            points[i].y--;
                    }

                }
            }

            if (e.KeyCode == Keys.S)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].z++;

                    if (points[i].x < 0)
                    {
                        points[i].x -= 1;
                        if (points[i].y < 0)
                            points[i].y--;
                        else
                            points[i].y++;

                    }
                    if (points[i].x > 0)
                    {
                        points[i].x++;
                        if (points[i].y < 0)
                            points[i].y--;
                        else
                            points[i].y++;
                    }

                }
            }


        }

        private double[] MatMulVector(Double[,] matrix,double[] vector)
        {
            int length = matrix.GetLength(0);

            double[] ret = new double[vector.Length];

            for (int i = 0; i < length; i++)
            {
                double sum = 0;
                for (int j = 0; j < length; j++)
                {
                    sum += matrix[i, j] * vector[j];
                }
                ret[i] = sum;
            }

            return ret;

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void normalize()
        {
            for (int i = 0; i < toDrawPoints.Length;i++)
            {
                toDrawPoints[i].X = this.Width / 2 + toDrawPoints[i].X;
                toDrawPoints[i].Y = this.Height / 2 - toDrawPoints[i].Y;
            }
        }
    }
}
