using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Collections.ObjectModel;
//using System.Data.Odbc;     // read palettes in from file; reqs ref to System.Data
using System.IO;
using System.Reflection;
using System.Windows;   // Visibility
using System.Windows.Input; // for ICommand; requires ref to PresentationCore
using System.Threading;
using Prism.Commands;
using Prism.Mvvm;
using System.Runtime.CompilerServices;
using Emgu.CV;
using Emgu.CV.Plot;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Quality;
using System.Drawing;

using Cryoview_Tools;   // logging
using LLE.Util; // logging
using Cryoview_ModuleMessages;

namespace Focus_ViewModel
{
    public class FocusViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private bool View1ROI_Set = false;
        private bool View2ROI_Set = false;
        private bool ProcessNextView1Img = false;
        private bool ProcessNextView2Img = false;
        private bool View1ImgObtained = false;
        private bool View2ImgObtained = false;
        private UMat img1Data = null;
        private int img1Width = 0;
        private UMat img2Data = null;
        private int img2Width = 0;
        private bool View1MarkerObtained = false;
        private bool View2MarkerObtained = false;
        private PointF Marker1Point = new PointF(0, 0);
        private PointF Marker2Point = new PointF(0, 0);
        private int ObjectiveFocus1Limit = 0;
        private int ObjectiveFocus2Limit = 0;
        private Thread FocusThread = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public FocusViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating view model for Focus");

            CryoviewTools.LogMessage(LogLevel.Debug7, "Focus view model created");
        }

        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Disposing view model for Focus");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {       // managed resources here
                if(FocusThread != null) { UserCancelV1 = true; UserCancelV2 = true; FocusThread.Join(); }
                ImageDataMessageEvent.Instance.Unsubscribe(HandleNewImage);
                FocusROIMarkerMessageEvent.Instance.Unsubscribe(FocusROIMarkerMessageRecieved);
                FocusSettingLimitsMessageEvent.Instance.Unsubscribe(GetObjectiveLimits);
            }
            // unmanaged resources here
            {
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        public void Initialize()
        {
            CurrentFocusView1 = -1.0f;
            CurrentFocusView2 = -1.0f;
            AutofocusRunningV1 = false;
            AutofocusRunningV2 = false;
            ImageDataMessageEvent.Instance.Subscribe(HandleNewImage);
            FocusROIMarkerMessageEvent.Instance.Subscribe(FocusROIMarkerMessageRecieved);
            FocusSettingLimitsMessageEvent.Instance.Subscribe(GetObjectiveLimits);
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        #endregion properties

        #region bindable properties
        public Single CurrentFocusView1 { get; private set; }
        public Single CurrentFocusView2 { get; private set; }
        public string IsRIOSet1 { get; private set; }
        public string IsRIOSet2 { get; private set; }
        public bool AutofocusRunningV1 { get; private set; }
        public bool AutofocusRunningV2 { get; private set; }
        public bool UserCancelV1 { get; set; }
        public bool UserCancelV2 { get; set; }
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        public void MeasureFocus(int v, bool isFocusing)
        {
            UMat FocusImage;
            if (v == 1)
            {
                View1ImgObtained = false;
                ProcessNextView1Img = true;
                while (!View1ImgObtained)
                {
                    Thread.Sleep(100);
                }

                if (View1ROI_Set)
                {
                    CircleF[] circles = CvInvoke.HoughCircles(img1Data, HoughModes.Gradient, 1, img1Width, 100, 50);

                    if (circles.Length > 0)
                    {
                        View1MarkerObtained = false;
                        FocusROIMarkerMessageEvent.Instance.Publish(new FocusROIMarkerMessage
                        {
                            ViewAxis = "OpticalView1",
                            IsRequest = true
                        });
                        while (!View1MarkerObtained)
                        {
                            Thread.Sleep(100);
                        }

                        double x = (double)(circles[0].Center.X - Marker1Point.X);
                        double y = (double)(circles[0].Center.Y - Marker1Point.Y);
                        int radius = Convert.ToInt32(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));

                        UMat Temp = new UMat(img1Data.Size, DepthType.Cv8U, 1);
                        Temp.SetTo(new MCvScalar(0));
                        Mat mask = new Mat(img1Data.Size, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(0));

                        CvInvoke.Circle(mask, System.Drawing.Point.Round(circles[0].Center), radius + 15,
                            new Gray(255).MCvScalar, -1);
                        CvInvoke.Circle(mask, System.Drawing.Point.Round(circles[0].Center), radius - 15,
                            new Gray(0).MCvScalar, -1);

                        img1Data.CopyTo(Temp, mask);
                        img1Data = Temp;
                    }
                    else if (!isFocusing)
                    {
                        OnExceptionRaised("Unable to find outer shell. Try focusing first.");
                    }
                }
                FocusImage = img1Data;
            }
            else
            {
                View2ImgObtained = false;
                ProcessNextView2Img = true;
                while (!View2ImgObtained)
                {
                    Thread.Sleep(100);
                }

                if (View2ROI_Set)
                {
                    CircleF[] circles = CvInvoke.HoughCircles(img2Data, HoughModes.Gradient, 1, img2Width, 100, 50);

                    if (circles.Length > 0)
                    {
                        View2MarkerObtained = false;
                        FocusROIMarkerMessageEvent.Instance.Publish(new FocusROIMarkerMessage
                        {
                            ViewAxis = "OpticalView3",
                            IsRequest = true
                        });
                        while (!View2MarkerObtained)
                        {
                            Thread.Sleep(100);
                        }

                        double x = (double)(circles[0].Center.X - Marker2Point.X);
                        double y = (double)(circles[0].Center.Y - Marker2Point.Y);
                        int radius = Convert.ToInt32(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));

                        UMat Temp = new UMat(img2Data.Size, DepthType.Cv8U, 1);
                        Temp.SetTo(new MCvScalar(0));
                        Mat mask = new Mat(img2Data.Size, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(0));

                        CvInvoke.Circle(mask, System.Drawing.Point.Round(circles[0].Center), radius + 15,
                            new Gray(255).MCvScalar, -1);
                        CvInvoke.Circle(mask, System.Drawing.Point.Round(circles[0].Center), radius - 15,
                            new Gray(0).MCvScalar, -1);

                        img2Data.CopyTo(Temp, mask);
                        img2Data = Temp;
                    }
                    else if (!isFocusing)
                    {
                        OnExceptionRaised("Unable to find outer shell. Try focusing first.");
                    }
                }
                FocusImage = img2Data;
            }

            MCvScalar mean = new MCvScalar();
            MCvScalar StdDev = new MCvScalar();
            using (var lap = new UMat())
            {
                CvInvoke.Laplacian(FocusImage, lap, FocusImage.Depth);
                CvInvoke.MeanStdDev(lap, ref mean, ref StdDev);
            }

            if (v == 1)
            {
                CurrentFocusView1 = (Single)Math.Pow(StdDev.V0, 2);
                RaisePropertyChanged(nameof(CurrentFocusView1));
            }
            else
            {
                CurrentFocusView2 = (Single)Math.Pow(StdDev.V0, 2);
                RaisePropertyChanged(nameof(CurrentFocusView2));
            }

        }

        //AutoFocus() is not working properly
        public void AutoFocus(object V)
        {
            int v = Convert.ToInt32(V);
            if(v == 1)
            {
                AutofocusRunningV1 = true;
                UserCancelV1 = false;
            }
            else
            {
                AutofocusRunningV2 = true;
                UserCancelV2 = false;
            }
            string view = v == 1 ? "OpticalView1" : "OpticalView3";
            FocusROIMarkerMovableMessageEvent.Instance.Publish(new FocusROIMarkerMovableMessage
            {
                ViewAxis = view,
                CanMoveMarker = false
            });
            EnableUserObjectiveControlMessageEvent.Instance.Publish(new EnableUserObjectiveControlMessage
            {
                ViewAxis = view,
                Enabled = false
            });
            if (ObjectiveFocus1Limit == 0 || ObjectiveFocus2Limit == 0)
            {
                FocusSettingLimitsMessageEvent.Instance.Publish(new FocusSettingLimitsMessage
                {
                    IsRequest = true
                });
            }

            int SetFocus;
            int StepSize;
            if (v == 1 && ObjectiveFocus1Limit > 0) { SetFocus = ObjectiveFocus1Limit / 2; StepSize = ObjectiveFocus1Limit / 10; }
            else if (v == 2 && ObjectiveFocus2Limit > 0) { SetFocus = ObjectiveFocus2Limit / 2; StepSize = ObjectiveFocus2Limit / 10; }
            else { OnExceptionRaised("Unable to get the limit for Navitar focus motor."); return; }
            FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
            {
                ViewAxis = view,
                focus = SetFocus
            });
            Thread.Sleep(20000); //Give the Navitar time to move to set focus

            int PreviousFocus = 0;
            int Direction = 1; //positive means moving up, negative means moving down
            bool InFocus = false;

            while (!InFocus)
            {
                if((v == 1 && UserCancelV1) ||(v == 2 && UserCancelV2))
                {
                    break;
                }

                MeasureFocus(v, true);
                if (v == 1)
                {
                    int currentFocus = Convert.ToInt32(CurrentFocusView1);
                    if (currentFocus > PreviousFocus)
                    {
                        PreviousFocus = currentFocus;
                        SetFocus += StepSize * Direction;
                        if(SetFocus < 0) { SetFocus = 0; }
                        else if(SetFocus > ObjectiveFocus1Limit) { SetFocus = ObjectiveFocus1Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                    }
                    else if (currentFocus <= PreviousFocus && StepSize > 10)
                    {
                        PreviousFocus = currentFocus;
                        Direction *= -1;
                        StepSize /= 2;
                        SetFocus += StepSize * Direction;
                        if (SetFocus < 0) { SetFocus = 0; }
                        else if (SetFocus > ObjectiveFocus1Limit) { SetFocus = ObjectiveFocus1Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                    }
                    else if (currentFocus < PreviousFocus)
                    {
                        Direction *= -1;
                        SetFocus += StepSize * Direction;
                        if (SetFocus < 0) { SetFocus = 0; }
                        else if (SetFocus > ObjectiveFocus1Limit) { SetFocus = ObjectiveFocus1Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                        InFocus = true;
                    }
                    else
                    {
                        InFocus = true;
                    }
                }
                else
                {
                    int currentFocus = Convert.ToInt32(CurrentFocusView2);
                    if (currentFocus > PreviousFocus)
                    {
                        PreviousFocus = currentFocus;
                        SetFocus += StepSize * Direction;
                        if (SetFocus < 0) { SetFocus = 0; }
                        else if (SetFocus > ObjectiveFocus2Limit) { SetFocus = ObjectiveFocus2Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                    }
                    else if (currentFocus <= PreviousFocus && StepSize > 10)
                    {
                        PreviousFocus = currentFocus;
                        Direction *= -1;
                        StepSize /= 2;
                        SetFocus += StepSize * Direction;
                        if (SetFocus < 0) { SetFocus = 0; }
                        else if (SetFocus > ObjectiveFocus2Limit) { SetFocus = ObjectiveFocus2Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                    }
                    else if (currentFocus < PreviousFocus)
                    {
                        Direction *= -1;
                        SetFocus += StepSize * Direction;
                        if (SetFocus < 0) { SetFocus = 0; }
                        else if (SetFocus > ObjectiveFocus2Limit) { SetFocus = ObjectiveFocus2Limit; }
                        FocusSettingMessageEvent.Instance.Publish(new FocusSettingMessage
                        {
                            ViewAxis = view,
                            focus = SetFocus
                        });
                        InFocus = true;
                    }
                    else
                    {
                        InFocus = true;
                    }
                }

                if (Direction == 1) { Thread.Sleep(2000); }
                else if (Direction == -1) { Thread.Sleep(20000); }
                else { Thread.Sleep(500); }
            }

            if (v == 1) { AutofocusRunningV1 = false; }
            else { AutofocusRunningV2 = false; }

            FocusROIMarkerMovableMessageEvent.Instance.Publish(new FocusROIMarkerMovableMessage
            {
                ViewAxis = view,
                CanMoveMarker = true
            });

            EnableUserObjectiveControlMessageEvent.Instance.Publish(new EnableUserObjectiveControlMessage
            {
                ViewAxis = view,
                Enabled = true
            });
        }
        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        public void StartAutoFocus(int v)
        {
            if(!AutofocusRunningV1 && !AutofocusRunningV2)
            {
                FocusThread = new Thread(new ParameterizedThreadStart(AutoFocus));
                FocusThread.SetApartmentState(ApartmentState.STA);
                FocusThread.Start(v);
            }
        }
        public void SetROI(int v)
        {
            if (v == 1)
            {
                View1ROI_Set = true;
                IsRIOSet1 = "ROI In Use";
                RaisePropertyChanged(nameof(IsRIOSet1));
                FocusROIMessageEvent.Instance.Publish(new FocusROIMessage
                {
                    ViewAxis = "OpticalView1",
                    ROIisSet = true
                });
            }
            else
            {
                View2ROI_Set = true;
                IsRIOSet2 = "ROI In Use";
                RaisePropertyChanged(nameof(IsRIOSet2));
                FocusROIMessageEvent.Instance.Publish(new FocusROIMessage
                {
                    ViewAxis = "OpticalView3",
                    ROIisSet = true
                });
            }
        }
        public void RemoveROI(int v)
        {
            if (v == 1)
            {
                View1ROI_Set = false;
                IsRIOSet1 = string.Empty;
                RaisePropertyChanged(nameof(IsRIOSet1));
                FocusROIMessageEvent.Instance.Publish(new FocusROIMessage
                {
                    ViewAxis = "OpticalView1",
                    ROIisSet = false
                });
            }
            else
            {
                View2ROI_Set = false;
                IsRIOSet2 = string.Empty;
                RaisePropertyChanged(nameof(IsRIOSet2));
                FocusROIMessageEvent.Instance.Publish(new FocusROIMessage
                {
                    ViewAxis = "OpticalView3",
                    ROIisSet = false
                });
            }
        }
        private void HandleNewImage(ImageDataMessage msg)
        {
            if ((msg.Id != "XRay" && msg.Id.Contains("X") && ProcessNextView1Img) || (msg.Id.Contains("Y") && ProcessNextView2Img))
            {
                if (msg.Id.Contains("X")) { ProcessNextView1Img = false; }
                else { ProcessNextView2Img = false; }

                int width = msg.Width;
                int adjustment = width % 4;
                if (adjustment != 0) { width -= adjustment; }

                byte[] byteImage = new byte[width * msg.Height];
                int k = 0;
                int l = 0;

                switch (msg.BitsPerPixel)
                {
                    case 12:
                        ushort[] workingImage = new ushort[msg.Width * msg.Height];
                        Array.Copy(msg.Image, workingImage, msg.Width * msg.Height);
                        for (int i = 0; i < workingImage.Length; i++) { ushort tempVal = (ushort)(workingImage[i]); workingImage[i] = (ushort)(tempVal << 4); }

                        for (int i = 0; i < workingImage.Length; i++)
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
                        for (int j = 0; j < workingImage2.Length; j++)
                        {
                            if (k < width)
                            {
                                byte c = Convert.ToByte(workingImage2[j]);
                                byteImage[l] = c;
                                l++;
                            }
                            k++;
                            if (k == msg.Width) { k = 0; }
                        }
                        break;
                }

                Image<Gray, byte> img = new Image<Gray, byte>(width, msg.Height);
                img.Bytes = byteImage;
                //CvInvoke.GaussianBlur(img, img, new System.Drawing.Size(9, 9), 1);

                if (msg.Id.Contains("X"))
                {
                    img1Data = img.ToUMat();
                    img1Width = width;
                    View1ImgObtained = true;
                }
                else
                {
                    img2Data = img.ToUMat();
                    img2Width = width;
                    View2ImgObtained = true;
                }
            }
        }
        private void FocusROIMarkerMessageRecieved(FocusROIMarkerMessage msg)
        {
            if (!msg.IsRequest)
            {
                if(msg.ViewAxis == "OpticalView1")
                {
                    Marker1Point.X = (Single)msg.MarkerPoint.X;
                    Marker1Point.Y = (Single)msg.MarkerPoint.Y;
                    View1MarkerObtained = true;
                }
                else
                {
                    Marker2Point.X = (Single)msg.MarkerPoint.X;
                    Marker2Point.Y = (Single)msg.MarkerPoint.Y;
                    View2MarkerObtained = true;
                }
            }
        }
        private void GetObjectiveLimits(FocusSettingLimitsMessage msg)
        {
            if (!msg.IsRequest)
            {
                ObjectiveFocus1Limit = msg.focusLimit1;
                ObjectiveFocus2Limit = msg.focusLimit2;
            }
        }
        private void OnExceptionRaised(string msg)
        {
            if(ExceptionRaised != null)
            {
                ExceptionRaised(msg);
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks

    }
}
