using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WFC
{
    public partial class ProgressBarCircularControl : UserControl
    {
        private int currentValueAction = 0;
        
        /*
         * Cicles
         */
        private int circleCount = 8;
        [Browsable(true), Category("Appearance")]
        [DefaultValue(8)]
        public int CircleCount
        {
            get { return circleCount; }
            set { circleCount = value; Invalidate(); }
        }

        private Color circleColor = Color.CadetBlue;
        [Browsable(true), Category("Appearance")]
        public Color CircleColor
        {
            get { return circleColor; }
            set { circleColor = value; Invalidate(); }
        }

        private bool inAction = false;
        public bool InAction
        {
            get { return inAction; }
            set { inAction = value; }
        }

        private int interval = 250;
        [Browsable(true), Category("Action")]
        [DefaultValue(250)]
        public int Interval
        {
            get { return interval; }
            set { interval = value; if (this.InAction) this.timer.Interval = this.interval; }
        }

        /*
         * Progress
         */
        private bool showProgress = true;
        [Browsable(true), Category("Progress")]
        [DefaultValue(true)]
        public bool ShowProgress
        {
            get { return showProgress; }
            set { showProgress = value; Invalidate(); }
        }

        private double progressMin = 0.0;
        [Browsable(true), Category("Progress")]
        [DefaultValue(0.0)]
        public double ProgressMin
        {
            get { return progressMin; }
            set { progressMin = value; Invalidate(); }
        }

        private double progressMax = 100.0;
        [Browsable(true), Category("Progress")]
        [DefaultValue(100.0)]
        public double ProgressMax
        {
            get { return progressMax; }
            set { progressMax = value; Invalidate(); }
        }

        private double progressValue = 0.0;
        [Browsable(true), Category("Progress")]
        [DefaultValue(0.0)]
        public double ProgressValue
        {
            get { return progressValue; }
            set 
            {
                if (this.ProgressValue != value)
                {
                    if (value < this.ProgressMin)
                        this.progressValue = this.ProgressMin;
                    else if (value > this.ProgressMax)
                        this.progressValue = this.ProgressMax;
                    else
                        progressValue = value;
                    Invalidate();
                }
            }
        }

        private string progressValueFormat = "{0}%";
        [Browsable(true), Category("Progress")]
        [DefaultValue("{0}%")]
        public string ProgressValueFormat
        {
            get { return progressValueFormat; }
            set { progressValueFormat = value; Invalidate(); }
        }

        Font progressFont = new Font("Arial", 8F, System.Drawing.FontStyle.Regular);
        [Browsable(true), Category("Progress")]
        public Font ProgressFont
        {
            get { return this.progressFont; }
            set { this.progressFont = value; Invalidate(); }
        }

        Color progressTextColor = Color.Black;
        [Browsable(true), Category("Progress")]
        [DefaultValue("Color.Black")]
        public Color ProgressTextColor 
        {
            get { return this.progressTextColor; }
            set { this.progressTextColor = value; Invalidate(); }
        }

        /*
         * Debug
         */
        bool inDebug = false;
        public bool InDebug
        {
            get { return inDebug; }
            set { inDebug = value; }
        }


        /*
         * Construction
         */
        public ProgressBarCircularControl()
        {
            InitializeComponent();

            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            ResizeRedraw = true;
        }

        /*
         * Action operations
         */
        public void Start()
        {
            if (!InAction)
            {
                this.InAction = true;
                this.timer.Interval = this.Interval;
                this.timer.Start();
                Invalidate();
            }
        }

        public void Stop()
        {
            if (InAction)
            {
                this.InAction = false;
                this.timer.Stop();
                Invalidate();
            }

        }

        public void Increment()
        {
            if (!this.DesignMode)
            {
                this.currentValueAction++;
                if (this.currentValueAction >= this.circleCount)
                    this.currentValueAction = 0;
                Invalidate();

                //if (this.ShowProgress)
                //    this.ProgressValue++;
            }
        }

        /*
         * Protected and private method
         */
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //this.BackColor = Color.FromArgb(50, Color.Gray);
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Gray)), 0, 0, this.Width, this.Height);
           
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float angle = (float)(2 * Math.PI / circleCount);

            GraphicsState oldState = e.Graphics.Save();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //if (this.InDebug)
            //    e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Width, this.Height);

            float w = (this.Width / 2) - this.Margin.All;
            PointF p1 = new PointF((float)(Math.Cos(0) * w), (float)(Math.Sin(0) * w));
            PointF p2 = new PointF((float)(Math.Cos(angle) * w), (float)(Math.Sin(angle) * w));
            double dist = Utils.Distance(p1, p2);
            double smallRadius = dist / 2.5f;

            //System.Diagnostics.Trace.WriteLine("Printout " + (this.inAction ? " OUI" : "Non"));

            double length = this.Width / 2 - this.Margin.All - smallRadius;
            for (int i = 0; i < circleCount; i++)
            {
                PointF p = new PointF((float)(Math.Cos(i * angle) * length), (float)(Math.Sin(i * angle) * length));

                int no = 0;
                if (i <= this.currentValueAction)
                    no = circleCount + (i - this.currentValueAction);
                else
                    no = i - this.currentValueAction;

                int alpha = this.InAction ? (int)(255.0F * (no / (float)circleCount)) : (int)(255.0F * (1.0F / (float)circleCount));

                //System.Diagnostics.Trace.WriteLine("\ti=" + i.ToString() + " : No = " + no.ToString() + "; current = " + this.currentValue.ToString() + ", alpha=" + alpha.ToString());
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, CircleColor)))
                {
                    float x = this.Width / 2 + p.X;
                    float y = this.Height / 2 + p.Y;
                    e.Graphics.FillEllipse(brush, (float)(x - smallRadius / 2), (float)(y - smallRadius / 2), (float)smallRadius, (float)smallRadius);

                    //if (InDebug)
                    //{
                    //    e.Graphics.DrawLine(new Pen(Color.Black), this.Width / 2, this.Height / 2, this.Width / 2 + p.X, this.Height / 2 + p.Y);
                    //    e.Graphics.DrawRectangle(new Pen(Color.Black), (float)(x - smallRadius / 2), (float)(y - smallRadius / 2), (float)smallRadius, (float)smallRadius);
                    //}
                }
            }

            if (this.ShowProgress)
            {
                double pourcent = (this.ProgressValue - this.ProgressMin) / (this.ProgressMax - this.ProgressMin)*100;

                string str = string.Format(this.ProgressValueFormat, pourcent);

                SizeF sizeText = e.Graphics.MeasureString(str, this.ProgressFont);
                PointF textPos = new PointF((this.Width - sizeText.Width) / 2.0f, (this.Height - sizeText.Height) / 2.0f);
                e.Graphics.DrawString(str, this.ProgressFont, new SolidBrush(this.ProgressTextColor), textPos);
            }

            e.Graphics.Restore(oldState);
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            Size = new Size(Math.Max(Width, Height), Math.Max(Width, Height));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Size = new Size(Math.Max(Width, Height), Math.Max(Width, Height));
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Increment();
        }
    }
}
