using System;
using System.Collections.Generic;
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Media;       // PixelFormats, Color
using System.Windows.Media.Imaging; // BitmapPalette; reqs ref to WindowsBase, PresentationCore
using Emgu.CV;
using Emgu.CV.Plot;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using MathNet.Numerics;
using Prism.Commands;
using Prism.Mvvm;

using Cryoview_ModuleMessages;
using Cryoview_Tools;
using LLE.Util;
using DB_Model;

namespace Target_Filling
{   
    public class PointD
    {
        public Double X;
        public Double Y;

        public PointD(Double x, Double y)
        {
            this.X = x;
            this.Y = y;
        }

    };
    public class TargetFilling : IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // asnyc notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        public delegate void UpdateGUIValues();
        public event UpdateGUIValues UpdateGUI;
        #endregion delegates, events

        #region backing vars
        private DBModel m_dbModel = null;
        private UMat MatImage = null;
        private Image<Gray, byte> img = null;
        private CircleF Shell = new CircleF();
        private RotatedRect RREllipse = new RotatedRect();
        private Double voxel2microliter = 1.0f;
        private Double pixel2micron = 0.0f;
        private Double micron2pixel = 0.0f;
        private Double viewingAngle = 0.0f;
        private Thread FillThread = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public TargetFilling()
        {
            IsFilling = false;
            Initialize();
            ImageDataMessageEvent.Instance.Subscribe(ImageDataMessageRecieved);
        }

        /// <summary>
        /// got here by user code; clean up any file handles
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param keyToRetrieve="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)  // managed resources here 
            {
                ImageDataMessageEvent.Instance.Unsubscribe(ImageDataMessageRecieved);
            }
            { } // unmanaged resources here
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        private void Initialize()
        {
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.
            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();
            m_dbModel = new DBModel();
            m_dbModel.RetrieveSettings(Location, "TargetFill", configParams);
            string s;
            configParams.TryGetValue("PixelToMicron", out s);
            pixel2micron = Convert.ToDouble(s);
            voxel2microliter = Math.Pow(pixel2micron / 10000, 3) * 1000;
            configParams.TryGetValue("MicronToPixel", out s);
            micron2pixel = Convert.ToDouble(s);
            configParams.TryGetValue("ViewingAngle", out s);
            viewingAngle = Convert.ToDouble(s);
        }

        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public WriteableBitmap bmp1 { get; set; }
        
        public WriteableBitmap bmp2 { get; set; }

        public Single IceThickness { get; set; }

        public bool IsFilling { get; set; }
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        private void CalibrationImage(int width)
        {
            CircleF[] circles = CvInvoke.HoughCircles(MatImage, HoughModes.Gradient, 1, width, 60, 50);
            MatImage = new UMat(img.Size, DepthType.Cv8U, 1);
            MatImage.SetTo(new MCvScalar(0));
            for (int i = 0; i < circles.Length; i++)
            {
                System.Drawing.Point p = new System.Drawing.Point((int)circles[i].Center.X, (int)circles[i].Center.Y);
                CvInvoke.Circle(MatImage, p, (int)circles[i].Radius, new Gray(250).MCvScalar, 2);
                CvInvoke.Circle(img, p, (int)circles[i].Radius, new Gray(250).MCvScalar, 2);
            }
        }

        private bool FindCircleAndEllipse(int width)
        {
            bool success = true;
            CircleF[] circles = CvInvoke.HoughCircles(MatImage, HoughModes.Gradient, 1, width, 100, 50);
            CvInvoke.Canny(MatImage, MatImage, 75, 50);

            if (circles.Length > 0)
            {
                Shell = circles[0];
                UMat Temp = new UMat(img.Size, DepthType.Cv8U, 1);
                Temp.SetTo(new MCvScalar(0));
                Mat mask = new Mat(img.Size, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(0));
                CvInvoke.Circle(mask, System.Drawing.Point.Round(circles[0].Center), (int)(circles[0].Radius - 10),
                    new Gray(255).MCvScalar, -1);
                MatImage.CopyTo(Temp, mask);
                MatImage = Temp;

                CircleF[] checkCircles = CvInvoke.HoughCircles(MatImage, HoughModes.Gradient, 1, width, 100, 50);
                if (checkCircles.Length > 0)
                {
                    if ((checkCircles[0].Radius * 1.1) >= circles[0].Radius)
                    {
                        Shell = checkCircles[0];
                        Temp = new UMat(img.Size, DepthType.Cv8U, 1);
                        Temp.SetTo(new MCvScalar(0));
                        mask = new Mat(img.Size, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(0));
                        CvInvoke.Circle(mask, System.Drawing.Point.Round(checkCircles[0].Center), (int)(checkCircles[0].Radius - 10),
                            new Gray(255).MCvScalar, -1);
                        MatImage.CopyTo(Temp, mask);
                        MatImage = Temp;
                    }
                }
            }
            //failed to find the circle
            else { return false; }

            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Cross, new System.Drawing.Size(3, 3), new System.Drawing.Point(1, 1));
            CvInvoke.MorphologyEx(MatImage, MatImage, MorphOp.Close, kernel, new System.Drawing.Point(-1, -1), 5, BorderType.Default, new MCvScalar(0));

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(MatImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);

            MatImage.SetTo(new MCvScalar(0));

            if (contours.Size > 0)
            {
                int numPoints = 0;
                for (int i = 0; i < contours.Size; i++)
                {
                    if (contours[i].Size > 100) { numPoints += contours[i].Size; }
                }
                PointF[] Points = new PointF[numPoints];
                numPoints = 0;
                for (int i = 0; i < contours.Size; i++)
                {
                    if (contours[i].Size > 100)
                    {
                        //CvInvoke.DrawContours(MatImage, contours, i, new Gray(255).MCvScalar, 2);
                        for (int a = 0; a < contours[i].Size; a++)
                        {
                            Points[numPoints] = contours[i][a];
                            numPoints++;
                        }
                    }
                }

                if (Points.Length > 5)
                {
                    VectorOfPointF f = new VectorOfPointF();
                    f.Push(Points);
                    RREllipse = CvInvoke.FitEllipse(f);
                    RREllipse.Angle -= 90;
                    CvInvoke.Ellipse(MatImage, RREllipse, new Gray(150).MCvScalar, 2);
                    CvInvoke.Ellipse(img, RREllipse, new Gray(0).MCvScalar, 2);
                    System.Drawing.Point p = new System.Drawing.Point((int)Shell.Center.X, (int)Shell.Center.Y);
                    CvInvoke.Circle(MatImage, p, (int)Shell.Radius, new Gray(150).MCvScalar, 2);
                    CvInvoke.Circle(img, p, (int)Shell.Radius, new Gray(0).MCvScalar, 2);
                }
                //failed to find the ellipse
                else { success = false; }
            }
            return success;
        }

        private VectorOfPointF FindIntersections(int height) 
        {
            int interval = 1000;
            Double[] t = Linespace(0, 2 * Math.PI, interval);
            Double[] cx = new Double[interval];
            Double[] cy = new Double[interval];
            Double[] ex = new Double[interval];
            Double[] ey = new Double[interval];
            Double MajorAxis = RREllipse.Size.Height/2;
            Double MinorAxis = RREllipse.Size.Width/2;
            VectorOfPointF IntersectionPoints = new VectorOfPointF();
            double ang = RREllipse.Angle * (Math.PI/180);

            for (int i = 0; i < interval; i++)
            {
                Double val = Shell.Center.X + Shell.Radius * Math.Cos(t[i]);
                cx.SetValue(val, i);

                val = Shell.Center.Y + Shell.Radius * Math.Sin(t[i]);
                cy.SetValue(val, i);

                val = MajorAxis * Math.Sin(ang) * Math.Sin(t[i])
                    - MinorAxis * Math.Cos(ang) * Math.Cos(t[i]) + RREllipse.Center.Y; 
                ey.SetValue(val, i);

                val = MajorAxis * Math.Cos(ang) * Math.Sin(t[i])
                    + MinorAxis * Math.Sin(ang) * Math.Cos(t[i])
                    + RREllipse.Center.X;
                ex.SetValue(val, i);
            }

            for(int i = 0; i < interval; i++)
            {
                PointD p1 = new PointD(cx[i], cy[i]);
                PointD q1;
                if (i == (interval - 1)) { q1 = new PointD(cx[0], cy[0]); }
                else { q1 = new PointD(cx[i + 1], cy[i + 1]);}
                
                for( int j = 0; j < interval; j++)
                {
                    PointD p2 = new PointD(ex[j], ey[j]);
                    PointD q2;
                    if (j == (interval - 1)) { q2 = new PointD(ex[0], ey[0]); }
                    else { q2 = new PointD(ex[j + 1], ey[j + 1]); }
                    bool hasIntersection = false;

                    // Find the four orientations needed for general and 
                    // special cases 
                    int o1 = Orientation(p1, q1, p2);
                    int o2 = Orientation(p1, q1, q2);
                    int o3 = Orientation(p2, q2, p1);
                    int o4 = Orientation(p2, q2, q1);

                    // General case 
                    if (o1 != o2 && o3 != o4) { hasIntersection = true; }

                    // Special Cases 
                    // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
                    else if (o1 == 0 && onSegment(p1, p2, q1)) { hasIntersection = true; }

                    // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
                    else if (o2 == 0 && onSegment(p1, q2, q1)) { hasIntersection = true; }

                    // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
                    else if (o3 == 0 && onSegment(p2, p1, q2)) { hasIntersection = true; }

                    // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
                    else if (o4 == 0 && onSegment(p2, q1, q2)) { hasIntersection = true; }

                    if (hasIntersection)
                    {
                        System.Drawing.PointF[] p = new System.Drawing.PointF[4];
                        p[0] = new System.Drawing.PointF((Single)p1.X, (Single)p1.Y);
                        p[1] = new System.Drawing.PointF((Single)q1.X, (Single)q1.Y);
                        p[2] = new System.Drawing.PointF((Single)p2.X, (Single)p2.Y);
                        p[3] = new System.Drawing.PointF((Single)q2.X, (Single)q2.Y);
                        IntersectionPoints.Push(p);
                    }
                }                
            }
            return IntersectionPoints;
        }

        private Single CalcVolMethod1()
        {
            Double MinorAxis = RREllipse.Size.Width / 2;
            IceThickness = (Single)((((MinorAxis / Shell.Radius) + (0.0104 * Math.Cos(2 * viewingAngle)) - 1.02)
                            / (0.0004 * Math.Cos(2 * viewingAngle) - 0.00332)) * pixel2micron);
            return IceThickness;
        }

        private Single CalcVolMethod2(int x1, int x2, int y1, int y2)
        {
            Double MajorAxis = RREllipse.Size.Height / 2;
            Double MinorAxis = RREllipse.Size.Width / 2;
            
            Double Vol_Sphere = ((4 / 3) * Math.PI * Math.Pow(Shell.Radius, 3)) * voxel2microliter;
            Double Vol_Ellipse = ((4 / 3) * Math.PI * MinorAxis * MinorAxis * MajorAxis) * voxel2microliter;
            
            int c = Math.Abs(x1 - x2) / 2;
            Double h = ((y1 + y2) / 2) - (Shell.Center.Y - Shell.Radius);
            Double Vol_SphereCap = ((Math.PI * h * (3 * Math.Pow(c, 2) + Math.Pow(h, 2))) / 6) * voxel2microliter;

            h = ((y1 + y2) / 2) - (RREllipse.Center.Y - MajorAxis);
            Double Vol_EllipseCap = ((Math.PI * MinorAxis * MinorAxis * Math.Pow(h, 2) * (3 * MajorAxis - h)) / (3 * Math.Pow(MajorAxis, 2))) * voxel2microliter;

            Double Vol_InEllipse = Vol_Ellipse - Vol_EllipseCap + Vol_SphereCap;
            Double Vol_Liquid = Vol_Sphere - Vol_InEllipse;
            Double Vol_Solid = Vol_Liquid * 0.879646f;

            Double a = (Vol_Solid / voxel2microliter) * 3;
            Double b = Math.Pow(Shell.Radius, 3) - a / (4 * Math.PI);
            Double d = Math.Ceiling(Math.Pow(b, (double)1 / 3));
            Single FillLevel = (Single)((Shell.Radius - d) * pixel2micron);

            return FillLevel;
        }

        private Single CalcVolMethod3(int height, int y1, int y2)
        {
            Double YMax = height * pixel2micron;
            Double YOC = YMax - (Shell.Center.Y * pixel2micron);
            Double YOE = YMax - (RREllipse.Center.Y * pixel2micron);
            Double R = Shell.Radius * pixel2micron;
            Double A = (RREllipse.Size.Height / 2) * pixel2micron;
            Double B = (RREllipse.Size.Width / 2) * pixel2micron;
            Double YZero = 0.0f;
            Double YTrans = YOE - B;
            Double YFinal = YMax - (((y1 + y2) /2) * pixel2micron);

            int interval = 10000;
            Double[] LI1 = Linespace(YZero, YTrans, interval);
            Double[] LI2 = Linespace(YTrans, YFinal, interval);
            Double[] RS1Left = new Double[interval - 1];
            Double[] RS1Right = new Double[interval - 1];
            Double[] RS2Left = new Double[interval - 1];
            Double[] RS2Right = new Double[interval - 1];
            Double DeltaLI1 = (YTrans - YZero) / interval;
            Double DeltaLI2 = (YFinal - YTrans) / interval;

            Double val = Math.Pow(R, 2) - Math.Pow(YZero - YOC, 2);
            if (Double.IsNaN(val) || val < 0) { RS1Right[0] = 0; }
            else { RS1Right[0] = val; }

            for (int i = 1; i < (interval - 1); i++)
            {
                val = Math.Pow(R, 2) - Math.Pow(LI1[i] - YOC, 2);
                if (Double.IsNaN(val) || val < 0) { val = 0; }
                RS1Left[i - 1] = val;
                RS1Right[i] = val;
            }

            val = Math.Pow(R, 2) - Math.Pow(YTrans - YOC, 2);
            if (Double.IsNaN(val) || val < 0) { RS1Left[interval - 2] = 0; }
            else { RS1Left[interval - 2] = val; }
            val = val - (Math.Pow(A, 2) * (1 - (Math.Pow(YTrans - YOE, 2) / Math.Pow(B, 2))));
            if (Double.IsNaN(val) || val < 0) { RS2Right[0] = 0; }
            else { RS2Right[0] = val; }

            for (int i = 1; i < (interval - 1); i++)
            {
                val = (Math.Pow(R, 2) - Math.Pow(LI2[i] - YOC, 2)) - (Math.Pow(A, 2) * (1 - (Math.Pow(LI2[i] - YOE, 2) / Math.Pow(B, 2))));
                if (Double.IsNaN(val) || val < 0) { val = 0; }
                RS2Left[i - 1] = val;
                RS2Right[i] = val;
            }

            val = (Math.Pow(R, 2) - Math.Pow(YFinal - YOC, 2)) - (Math.Pow(A, 2) * (1 - (Math.Pow(YFinal - YOE, 2) / Math.Pow(B, 2))));
            if (Double.IsNaN(val) || val < 0) { RS2Left[interval - 2] = 0; }
            else { RS2Left[interval - 2] = val; }

            val = 0.0f;
            for( int i = 0; i < (interval - 1); i++)
            {
                val += (RS1Left[i] * DeltaLI1) + (RS2Left[i] * DeltaLI2);
            }
            Double LeftSum = val;

            val = 0.0f;
            for(int i = 0; i < (interval - 1); i++)
            {
                val += (RS1Right[i] * DeltaLI1) + (RS2Right[i] * DeltaLI2);
            }
            Double RightSum = val;

            Double AverageSum = Math.PI * ((LeftSum + RightSum) / 2);

            Double MassDD = .174 * AverageSum;
            Double PDD = .1971;
            val = (3 * MassDD) / (4 * Math.PI * PDD);
            val = Math.Pow(R, 3) - val;
            val = Math.Pow(val, (Double)1 / 3);
            val = R - val;

            return (Single)val;
        }

        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        private void ImageDataMessageRecieved(object state)
        {
            ImageDataMessage msg = (ImageDataMessage)state;
            if (msg.Id == "XRay" && IsFilling)
            {
                int width = msg.Width;
                int height = msg.Height;
                int adjustment = width % 4;
                if(adjustment != 0) { width -= adjustment; }

                byte[] byteImage = new byte[width * height];
                int k = 0;
                int l = 0;

                switch (msg.BitsPerPixel)
                {
                    case 16:
                        ushort[] workingImage = new ushort[msg.Width * msg.Height];
                        Array.Copy(msg.Image, workingImage, msg.Width * msg.Height);

                        for(int i = 0; i < workingImage.Length; i++)
                        {
                            if (k < width)
                            {
                                byte b = (byte)(workingImage[i] / 256);
                                byteImage[l] = b;
                                l++;
                            }
                            k++;
                            if (k == msg.Width) { k = 0; }
                        }
                        break;
                    case 8:
                        ushort[] workingImage2 = new ushort[msg.Width * msg.Height];
                        Array.Copy(msg.Image, workingImage2, msg.Width * msg.Height);
                        for(int j = 0; j < workingImage2.Length; j++)
                        {
                            if(k < width)
                            {
                                byte c = Convert.ToByte(workingImage2[j]);
                                byteImage[l] = c;
                                l++;
                            }
                            k++;
                            if(k == msg.Width) { k = 0; }
                        }
                        break;
                }

                img = new Image<Gray, byte>(width, height);
                img.Bytes = byteImage;
                CvInvoke.GaussianBlur(img, img, new System.Drawing.Size(9, 9), 1);
                MatImage = img.ToUMat();

                //CalibrationImage(width); //for test use only

                bool success = FindCircleAndEllipse(width);

                if (success)
                {
                    VectorOfPointF IntersectionPoints = FindIntersections(height);
                    if (IntersectionPoints.Size == 8)
                    {
                        int x1 = (int)((IntersectionPoints[0].X + IntersectionPoints[1].X + IntersectionPoints[2].X + IntersectionPoints[3].X) / 4);
                        int x2 = (int)((IntersectionPoints[4].X + IntersectionPoints[5].X + IntersectionPoints[6].X + IntersectionPoints[7].X) / 4);
                        int y1 = (int)((IntersectionPoints[0].Y + IntersectionPoints[1].Y + IntersectionPoints[2].Y + IntersectionPoints[3].Y) / 4);
                        int y2 = (int)((IntersectionPoints[4].Y + IntersectionPoints[5].Y + IntersectionPoints[6].Y + IntersectionPoints[7].Y) / 4);

                        CvInvoke.Circle(MatImage, new System.Drawing.Point(x1, y1), 15, new Gray(250).MCvScalar, 4);
                        CvInvoke.Circle(MatImage, new System.Drawing.Point(x2, y2), 15, new Gray(250).MCvScalar, 4);
                        CvInvoke.Circle(img, new System.Drawing.Point(x1, y1), 15, new Gray(250).MCvScalar, 4);
                        CvInvoke.Circle(img, new System.Drawing.Point(x2, y2), 15, new Gray(250).MCvScalar, 4);

                        Single Thickness1 = CalcVolMethod1();
                        Single Thickness2 = CalcVolMethod2(x1, x2, y1, y2);
                        Single Thickness3 = CalcVolMethod3(height, y1, y2);
                    }

                    else
                    {
                        //something went wrong
                        OnExceptionRaised("Unable to calculate fill level and ice thickness");
                    }
                }

                byteImage = img.Bytes;
                Int32Rect rect = new Int32Rect(0, 0, width, height);
                WriteableBitmap bmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                bmap.WritePixels(rect, byteImage, width, 0);
                bmap.Freeze();
                bmp1 = bmap;

                img = MatImage.ToImage<Gray, Byte>();
                byteImage = img.Bytes;
                bmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                bmap.WritePixels(rect, byteImage, width, 0);
                bmap.Freeze();
                bmp2 = bmap;
                OnUpdateRaised();
            }
        }

        private Double[] Linespace(Double start, Double end, int interval)
        {
            Double[] parameterVals = new Double[interval];
            Double increment = Math.Abs(start - end) / (interval - 1);
            Double nextValue = start;
            for (int i = 0; i < interval; i++)
            {
                parameterVals.SetValue(nextValue, i);
                nextValue = nextValue + increment;
            }
            return parameterVals;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private int Orientation(PointD p, PointD q, PointD r)
        {
            Double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // colinear 

            if(Math.Abs(val) < 0.0000000001)
            {
                return 0; // colinear
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // Given three colinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        private bool onSegment(PointD p, PointD q, PointD r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) 
                && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
            { return true; }

            return false;
        }

        private void OnExceptionRaised(string msg)
        {
            if (ExceptionRaised != null)    // check for subscribers
            {
                ExceptionRaised(msg);   // pass it on (to the view)
            }
        }

        private void OnUpdateRaised()
        {
            if (UpdateGUI != null)    // check for subscribers
            {
                UpdateGUI();   // pass it on (to the view)
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
