using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyGeneration
{
    public class Ball
    {
        private static readonly Random Rand = new Random();
        public static int MaxSpeed = 6;

        private bool _hasHitWallSinceExpire;
        private Point _location;
        private readonly Control _control;
        
        public Brush Brush;
        public bool IsParticle;
        public int Radius = 8;
        public int OffsetX = 1;
        public int OffsetY = 1;
        public int LifeInMoves;
        public int MaxLifeInMoves = -1;

        public Ball(Control c, Pen pen)
        {
            _control = c;
            Brush = pen.Brush;
            GenerateOffsets();
            GenerateLocation();
        }

        public Ball(Control c)
        {
            _control = c;
            GenerateOffsets();
            GenerateLocation();
            GenerateColor();
        }

        public Ball(Control c, Point location)
        {
            _control = c;
            _location = location;
            GenerateOffsets();
            GenerateLocation();
            GenerateColor();
        }

        public Ball(Control c, Point location, int offsetX, int offsetY, Pen pen)
        {
            _control = c;
            _location = location;
            Brush = pen.Brush;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        private Ball(Control c, Point location, int radius, Brush brush, int maxLives)
        {
            _control = c;
            _location = location;
            Brush = brush;
            Radius = radius;
            MaxLifeInMoves = maxLives;
            if (radius <= 2) IsParticle = true;
            GenerateOffsets();
        }

        public Ball[] Explode()
        {
            var maxLives = 40;
            if (MaxLifeInMoves <= maxLives && MaxLifeInMoves > 0) maxLives = MaxLifeInMoves / 2;
            return new[]
                   {
                       new Ball(_control, _location, Radius / 2, Brush, maxLives),
                       new Ball(_control, _location, Radius / 2, Brush, maxLives),
                       new Ball(_control, _location, Radius / 2, Brush, maxLives),
                       new Ball(_control, _location, Radius / 2, Brush, maxLives)
                   };
        }

        public bool IsExpired
        {
            get
            {
                if (IsParticle)
                {
                    return _hasHitWallSinceExpire && IsLifeExpired;
                }
                return IsLifeExpired;
            }
        }

        private bool IsLifeExpired
        {
            get
            {
                if (MaxLifeInMoves == -1) return false;
                return LifeInMoves > MaxLifeInMoves;
            }
        }

        private void GenerateColor()
        {
            var r = Rand.Next(255);
            var b = Rand.Next(255 - r) + r;
            var a = Rand.Next(128) + 127;
            var p = new Pen(Color.FromArgb(a, r, r, b));
            Brush = p.Brush;
        }

        private void GenerateLocation()
        {
            var x = Rand.Next(_control.Width - Radius * 2) + Radius;
            var y = Rand.Next(_control.Height - Radius * 2) + Radius;

            _location = new Point(x, y);
        }

        private void GenerateOffsets()
        {
            int ox = 0, oy = 0;

            while (ox == 0) ox = Rand.Next(MaxSpeed * 2) - MaxSpeed;
            while (oy == 0) oy = Rand.Next(MaxSpeed * 2) - MaxSpeed;

            OffsetX = ox;
            OffsetY = oy;
        }

        public void Move()
        {
            LifeInMoves++;

            var x = OffsetX + _location.X;
            var y = OffsetY + _location.Y;

            if (x < 0 && OffsetX < 0 || x > (_control.ClientSize.Width - Radius) && OffsetX > 0)
            {
                OffsetX = -1 * OffsetX;
                x = OffsetX + _location.X;
                if (IsParticle && IsLifeExpired) _hasHitWallSinceExpire = true;
            }

            if (y < 0 && OffsetY < 0 || y > (_control.ClientSize.Height - Radius) && OffsetY > 0)
            {
                OffsetY = -1 * OffsetY;
                y = OffsetY + _location.Y;
                if (IsParticle && IsLifeExpired) _hasHitWallSinceExpire = true;
            }

            _location.X = x;
            _location.Y = y;
        }

        public void Paint(Graphics g)
        {
            if (IsExpired) return;
            g.FillEllipse(Brush,
                          new Rectangle(_location.X - Radius,
                                        _location.Y - Radius,
                                        Radius * 2,
                                        Radius * 2));
        }
    }
}
