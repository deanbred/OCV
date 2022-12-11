using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;   // MouseButtonEventArgs
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Optical_View_Model;
using Cryoview_ModuleMessages;
namespace Optical_View
{
    public partial class PopoutImageView : Window, IDisposable
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        private bool m_bUserControlLoaded = false;
        private bool markerBeingMoved = false;
        private bool canMoveMarker = true;
        private OpticalViewModel m_OpticalViewModel = null;
        private Point MarkerLocation = new Point(0, 0);
        private string ID = string.Empty;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// </summary>
        public PopoutImageView(OpticalViewModel model, string id)
        {
            m_OpticalViewModel = model;
            ID = id;
            InitializeComponent();
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
                FocusROIMessageEvent.Instance.Unsubscribe(FocusROIMessageRecieved);
                FocusROIMarkerMessageEvent.Instance.Unsubscribe(FocusROIMarkerMessageRecieved);
                FocusROIMarkerMovableMessageEvent.Instance.Unsubscribe(FocusROIMarkerMovableMessageRecieved);
                this.Close();
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
        private void PopoutImageView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                this.DataContext = m_OpticalViewModel;

                MarkerTransform.X = 0;
                MarkerTransform.Y = 0;
                Grid.SetZIndex(Marker, 80);
                Grid.SetZIndex(LiveImage, 70);
                Marker.Visibility = Visibility.Collapsed;

                FocusROIMessageEvent.Instance.Subscribe(FocusROIMessageRecieved);
                FocusROIMarkerMessageEvent.Instance.Subscribe(FocusROIMarkerMessageRecieved);
                FocusROIMarkerMovableMessageEvent.Instance.Subscribe(FocusROIMarkerMovableMessageRecieved);
            }
        }

        private void cmdZoomIn_Click(object sender, RoutedEventArgs e)
        {
            controlZoomPan.ContentScale += 0.1;
        }

        private void cmdZoomOut_Click(object sender, RoutedEventArgs e)
        {
            controlZoomPan.ContentScale -= 0.1;
        }


        private void ImgMouseMove(object sender, MouseEventArgs e)
        {
            if (markerBeingMoved && e.LeftButton == MouseButtonState.Pressed)
            {
                MarkerLocation = e.GetPosition(LiveImage);
                Point MarkerP = e.GetPosition(controlZoomPan);
                Point centerP = new Point(controlZoomPan.ActualWidth / 2, controlZoomPan.ActualHeight / 2);
                MarkerTransform.X = MarkerP.X- centerP.X -5;    // in wpf we apply a transform to paint an ellipse
                MarkerTransform.Y = MarkerP.Y- centerP.Y -5;    // in a new location. we do NOT redraw it.
                e.Handled = true;
            }
        }

        private void marker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (canMoveMarker) { markerBeingMoved = true; }
        }

        private void marker_MouseUp(object sender, MouseButtonEventArgs e)
        {
            markerBeingMoved = false;
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
        private void FocusROIMessageRecieved(FocusROIMessage msg)
        {
            if(msg.ViewAxis == ID)
            {
                if (msg.ROIisSet)
                {
                    Marker.Visibility = Visibility.Visible;
                }
                else
                {
                    Marker.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void FocusROIMarkerMessageRecieved( FocusROIMarkerMessage msg)
        {
            if(msg.IsRequest && msg.ViewAxis == ID)
            {
                FocusROIMarkerMessageEvent.Instance.Publish(new FocusROIMarkerMessage
                {
                    ViewAxis = ID,
                    IsRequest = false,
                    MarkerPoint = MarkerLocation
                });
            }
        }

        private void FocusROIMarkerMovableMessageRecieved(FocusROIMarkerMovableMessage msg)
        {
            if(msg.ViewAxis == ID)
            {
                canMoveMarker = msg.CanMoveMarker;
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
