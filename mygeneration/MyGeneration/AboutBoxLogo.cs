using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyGeneration
{
    public partial class AboutBoxLogo : Control
    {
        private static Random rand = new Random();
        private List<Ball> balls = new List<Ball>();
        private Point ip;
        private int countMoves = 0;

        public AboutBoxLogo()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public void Start()
        {
            balls.Add(new Ball(this, new Point(rand.Next(this.Width - 10) + 5, rand.Next(this.Height - 10) + 5), rand.Next(4) + 1, rand.Next(4) + 1, new Pen(Color.Red)));
            balls.Add(new Ball(this, new Point(rand.Next(this.Width - 10) + 5, rand.Next(this.Height - 10) + 5), (rand.Next(4) + 1) * -1, rand.Next(4) + 1, new Pen(Color.White)));
            balls.Add(new Ball(this, new Point(rand.Next(this.Width - 10) + 5, rand.Next(this.Height - 10) + 5), rand.Next(4) + 1, (rand.Next(4) + 1) * -1, new Pen(Color.Blue)));
            
            ip = new Point((this.ClientSize.Width - Properties.Resources.mygenlogo1.Width) / 2,
                (this.ClientSize.Height - Properties.Resources.mygenlogo1.Height) / 2);

            this.timerRepaint.Interval = 25;
            this.timerRepaint.Enabled = true;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.timerRepaint.Enabled = false;

            Graphics g = pe.Graphics;

            foreach (Ball b in balls)
            {
                b.Paint(g);
            }

            g.DrawImage(Properties.Resources.mygenlogo1, ip);

            // Calling the base class OnPaint
            base.OnPaint(pe);

            this.timerRepaint.Enabled = true;
        }

        private void timerRepaint_Tick(object sender, EventArgs e)
        {
            countMoves++;
            foreach (Ball b in balls)
                b.Move();

            if (countMoves == 200 && this.balls.Count < 100)
            {
                countMoves = 0;
                balls.Add(new Ball(this, new Point(rand.Next(this.Width - 10) + 5, rand.Next(this.Height - 10) + 5), rand.Next(12) - 6, rand.Next(12) - 6, new Pen(Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)))));
            }

            this.Invalidate();
        }

        /// <summary>
        /// Inner class Ball.
        /// </summary>
        public class Ball
        {
            public Ball(Control c, Point location, int offsetX, int offsetY, Pen pen)
            {
                this.Control = c;
                this.Location = location;
                this.Brush = pen.Brush;
                this.OffsetX = offsetX;
                this.OffsetY = offsetY;
            }

            public void Move()
            {

                int x = OffsetX + Location.X;
                int y = OffsetY + Location.Y;

                if ((x < 0 && OffsetX < 0) || (x > (this.Control.ClientSize.Width - Radius) && OffsetX > 0))
                {
                    OffsetX = -1 * OffsetX;
                    x = OffsetX + Location.X;
                }

                if ((y < 0 && OffsetY < 0) || (y > (this.Control.ClientSize.Height - Radius) && OffsetY > 0))
                {
                    OffsetY = -1 * OffsetY;
                    y = OffsetY + Location.Y;
                }

                Location.X = x;
                Location.Y = y;
            }

            public void Paint(Graphics g)
            {
                g.FillEllipse(this.Brush, new Rectangle(Location.X - Radius,
                    Location.Y - Radius,
                    Radius * 2,
                    Radius * 2));
            }


            public Point Location;
            public int Radius = 5;
            public Brush Brush;
            public int OffsetX = 1;
            public int OffsetY = 1;
            public Control Control;
        }

        private void AboutBoxLogo_MouseUp(object sender, MouseEventArgs e)
        {
            balls.Add(new Ball(this, e.Location, rand.Next(12) - 6, rand.Next(12) - 6, new Pen(Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)))));
        }
    }
}
