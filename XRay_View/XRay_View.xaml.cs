﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using XRay_View_Model;
using Cryoview_ModuleMessages;

namespace XRay_View
{
    public partial class XRayView : UserControl, IDisposable
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars

        bool m_bUserControlLoaded = false;
        XRayViewModel m_XRayViewModel = null;
        string ID = "";
        string TwinID = "";

        private bool m_SetROIOnNextClick = false;
        private bool m_IsDrag = false;
        private Point m_RectStartPoint = new Point(0, 0);
        private Point m_RectEndPoint = new Point(0, 0);

        PopoutXRayView m_Popout = null;

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// </summary>
        public XRayView(string id, string twindID)
        {
            ID = id;
            TwinID = twindID;
            InitializeComponent();
            m_XRayViewModel = new XRayViewModel(ID, TwinID);
        }
        /// <summary>
        /// Called by user-code. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Called by either user-code or the runtime. If runtime, disposing = false;
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {       // managed resources here
                m_Popout?.Dispose();
                m_XRayViewModel.Dispose();
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        #endregion initialization

        #region windows events
        /// <summary>
        /// This event fires when the focus is moved back to the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XRayView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;

                //set up the rectable for the ROI
                fidTTROI.X = 0;
                fidTTROI.Y = 0;
                Grid.SetZIndex(fidRectROI, 100);
                fidRectROI.Visibility = Visibility.Collapsed;

                m_XRayViewModel.Initialize();
                this.DataContext = m_XRayViewModel;
                if(ID == "XRayView1")
                {
                    m_Popout = new PopoutXRayView(m_XRayViewModel);
                    m_Popout.Show();
                }       
            }
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_IsDrag)
            {
                //Need to find the upper left corner
                int X = Math.Min((int)m_RectStartPoint.X, (int)m_RectEndPoint.X);
                int Y = Math.Min((int)m_RectStartPoint.Y, (int)m_RectEndPoint.Y);

                int Width = (int)m_RectEndPoint.X - (int)m_RectStartPoint.X;
                int Height = (int)m_RectEndPoint.Y - (int)m_RectStartPoint.Y;
                if (Width == 0 || Height == 0) //not a valid ROI size
                {
                    MessageBox.Show("ROI can not have a width or height of 0. ROI not set.", "Invalid ROI", MessageBoxButton.OK, MessageBoxImage.Error);
                    fidRectROI.Visibility = Visibility.Collapsed;
                    m_IsDrag = m_SetROIOnNextClick = false;
                    m_XRayViewModel.ROISetIndicator = "Not Set";
                    return;
                }

                //Make sure the width and height are not negative
                if (Width < 0) { Width *= -1; }
                if (Height < 0) { Height *= -1; }

                //Add a 10 pixel padding to the ROI 
                m_XRayViewModel.ROIRect = new Int32Rect(X - 10, Y - 10, Width + 20, Height + 20);

                m_IsDrag = m_SetROIOnNextClick = false;
                m_XRayViewModel.ROISetIndicator = "ROI Set";
                m_XRayViewModel.ROISet = true;
                fidRectROI.Visibility = Visibility.Collapsed;
                m_XRayViewModel.ChangeROICurrentImage();
            }
        }

        //Same as the other mouse up method, this is here in the event that a bug causes the view not to recognize when the mouse button is released
        private void img_MouseUp()
        {
            if (m_IsDrag)
            {
                //Need to find the upper left corner
                int X = Math.Min((int)m_RectStartPoint.X, (int)m_RectEndPoint.X);
                int Y = Math.Min((int)m_RectStartPoint.Y, (int)m_RectEndPoint.Y);

                int Width = (int)m_RectEndPoint.X - (int)m_RectStartPoint.X;
                int Height = (int)m_RectEndPoint.Y - (int)m_RectStartPoint.Y;
                if (Width == 0 || Height == 0)//not a valid ROI size
                {
                    MessageBox.Show("ROI can not have a width or height of 0. ROI not set.", "Invalid ROI", MessageBoxButton.OK, MessageBoxImage.Error);
                    fidRectROI.Visibility = Visibility.Collapsed;
                    m_IsDrag = m_SetROIOnNextClick = false;
                    m_XRayViewModel.ROISetIndicator = "Not Set";
                    return;
                }

                //Make sure the width and height are not negative
                if (Width < 0) { Width *= -1; }
                if (Height < 0) { Height *= -1; }

                //Add a 10 pixel padding to the ROI 
                m_XRayViewModel.ROIRect = new Int32Rect(X - 10, Y - 10, Width + 20, Height + 20);

                m_IsDrag = m_SetROIOnNextClick = false;
                m_XRayViewModel.ROISetIndicator = "ROI Set";
                m_XRayViewModel.ROISet = true;
                fidRectROI.Visibility = Visibility.Collapsed;
                m_XRayViewModel.ChangeROICurrentImage();
            }
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_SetROIOnNextClick && e.LeftButton == MouseButtonState.Pressed)
            {
                m_RectEndPoint = m_RectStartPoint = e.GetPosition(XRayImg);
                m_IsDrag = true;
            }
        }

        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsDrag)
            {
                if (e.LeftButton != MouseButtonState.Pressed) { img_MouseUp(); return; } //preventing a bug with ROI being set from any corner other than the top left.
                Point center = new Point((XRayImg.ActualWidth / 2), (XRayImg.ActualHeight / 2));
                m_RectEndPoint = e.GetPosition(XRayImg);

                //need to find the upper left corner
                Point pt = new Point(Math.Min(m_RectEndPoint.X, m_RectStartPoint.X), Math.Min(m_RectEndPoint.Y, m_RectStartPoint.Y));
                double Width = Math.Max(m_RectStartPoint.X, m_RectEndPoint.X) - pt.X;
                double Height = Math.Max(m_RectStartPoint.Y, m_RectEndPoint.Y) - pt.Y;

                //redraw the ROI rectagle on the screen
                fidTTROI.X = (pt.X + Width / 2) - center.X;
                fidTTROI.Y = (pt.Y + Height / 2) - center.Y;
                fidRectROI.Width = Width + 20;
                fidRectROI.Height = Height + 20;
                fidRectROI.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Button clicked set the Region of Interest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSetROI_Click(object sender, EventArgs e)
        {
            if (m_XRayViewModel.ROISet) //Can not set ROI again if ROI is already set
            {
                MessageBox.Show("ROI is already set. Remove ROI and try again", "ROI Already Set", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            m_SetROIOnNextClick = true;
        }

        /// <summary>
        /// Button clicked to remove the Region of Interest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdRemoveROI_Click(object sender, EventArgs e)
        {
            m_XRayViewModel.ROISet = false;
            m_XRayViewModel.ROIAdjusted = false;
            m_XRayViewModel.ROISetIndicator = "Not Set";
            ImageViewROIMessageEvent.Instance.Publish(new ImageViewROIMessage()
            {
                ID = this.ID,
                ROISetIndicator = "Not Set",
                ROISet = false,
                ROIRect = new Int32Rect(),
                ROIAdjusted = false
            });
            m_XRayViewModel.ChangeROICurrentImage();
        }

        /// <summary>
        /// Button clicked to increase the Gain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdIncrease_Gain_Click(object sender, EventArgs e)
        {
            if (sender == GainIncreasebtn ) { m_XRayViewModel.Gain = m_XRayViewModel.Gain + 1; }
            else if (sender == DigitalGainIncreasebtn ) { m_XRayViewModel.DigitalGain = m_XRayViewModel.DigitalGain + 1; }
        }
        /// <summary>
        /// Button clicked to decrease the Gain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDecrease_Gain_Click(object sender, EventArgs e)
        {
            if (sender == GainDecreasebtn) { m_XRayViewModel.Gain = m_XRayViewModel.Gain - 1; }
            else if (sender == DigitalGainDecreasebtn ) { m_XRayViewModel.DigitalGain = m_XRayViewModel.DigitalGain - 1; }
        }
        /// <summary>
        /// Button clicked to take a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdTake_Image_Click(object sender, EventArgs e)
        {
            CommandMessageEvent.Instance.Publish(new CommandMessage
            {
                ID = "xray",
                Command = "Take Image"
            });
        }
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
