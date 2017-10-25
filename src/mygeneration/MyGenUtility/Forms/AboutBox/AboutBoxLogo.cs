using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyGeneration
{
    public sealed partial class AboutBoxLogo : Control
    {
        private const int MAX_NUMBER_OF_BALLS = 300;
        
        private static readonly Random Rand = new Random();
        private readonly List<Ball> _balls = new List<Ball>();
        private Point _myGenerationLogoPoint;
        private int _moveCounter;

        public AboutBoxLogo()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void Start()
        {
            for (var i = 0; i < 5; i++)
            {
                var b = new Ball(this) {MaxLifeInMoves = Rand.Next(250) + 250};
                _balls.Add(b);
            }

            _myGenerationLogoPoint = new Point((ClientSize.Width - Properties.Resources.mygenlogo1.Width) / 2,
                                               (ClientSize.Height - Properties.Resources.mygenlogo1.Height) / 2);

            RepaintTimer.Interval = 25;
            RepaintTimer.Enabled = true;
        }

        private void AboutBoxLogo_MouseUp(object sender, MouseEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            RepaintTimer.Enabled = false;

            Graphics graphics = pe.Graphics;

            foreach (Ball ball in _balls)
            {
                ball.Paint(graphics);
            }

            graphics.DrawImage(Properties.Resources.mygenlogo1, _myGenerationLogoPoint);

            base.OnPaint(pe);

            RepaintTimer.Enabled = true;
        }

        private void RepaintTimer_OnClicked(object sender, EventArgs e)
        {
            _moveCounter++;
            List<Ball> ballsToAdd = null;
            List<Ball> ballsToRemove = null;

            foreach (Ball b in _balls)
            {
                if (b.IsExpired)
                {
                    if (ballsToRemove == null) 
                        ballsToRemove = new List<Ball>();

                    ballsToRemove.Add(b);

                    if (b.IsParticle) continue;

                    if (ballsToAdd == null) ballsToAdd = new List<Ball>();
                    ballsToAdd.AddRange(b.Explode());
                }
                else
                {
                    b.Move();
                }
            }

            if (ballsToRemove != null)
            {
                foreach (Ball b in ballsToRemove) _balls.Remove(b);
            }

            if (ballsToAdd != null)
            {
                foreach (Ball b in ballsToAdd) _balls.Add(b);
            }

            if (_moveCounter == 50 && _balls.Count < MAX_NUMBER_OF_BALLS)
            {
                _moveCounter = 0;
                var b = new Ball(this) {MaxLifeInMoves = Rand.Next(500) + 100};
                _balls.Add(b);
            }

            Invalidate();
        }
    }
}
