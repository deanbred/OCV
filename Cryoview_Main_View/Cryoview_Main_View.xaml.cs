using RuntimeComponent = ICryoview.ICryoviewWindow; // for those times when the namespace and class name are identical,
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Diagnostics;

using Cryoview_Main_ViewModel;
using Cryoview_ModuleMessages;
using XRay_View;
using Optical_View;
using Image_Basler_Model;
using XRay_Interface;
using XRay_Physical_Model;
using XRay_Virtual_Model;
using Cryoview_Tools;       // logging
using LLE.Util;             // LogLevel
using DB_Model;


namespace Cryoview_Main_View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        private EventLog m_eventLog = null;
        private ConcurrentDictionary<string, string> m_appProperties = null;

        [Import(typeof(RuntimeComponent))]
        // we want MEF to find user controls that satisfy this specification
        public RuntimeComponent m_runtimePart { get; set; }
        IEnumerable<RuntimeComponent> m_runtimeParts = null;   // all the parts that MEF brings in

        bool m_bUserControlLoaded = false; //Only load once
        DBModel m_db = null;
        private CryoviewMainViewModel m_cryoviewViewModel = null;

        ImageBaslerModel CameraX = null;
        ImageBaslerModel CameraY = null;
        XRayInterface CameraZ = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        ///  designer needs a public ctor() if view is to be loaded as a user control.
        ///  The main window will not used in that circumstance.
        /// </summary>
        public MainWindow() { }

        /// <summary>
        /// </summary>
        /// <param name="eventLog">Event Viewer for catastrophic conditions</param>
        public MainWindow(EventLog eventLog)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            m_eventLog = eventLog;
            m_appProperties = new ConcurrentDictionary<string, string>();
            // transfer cmd line params from the app non-generic HybridDictionary to the generic dictionary
            foreach (DictionaryEntry de in Application.Current.Properties)
            {
                m_appProperties.TryAdd(de.Key.ToString(), de.Value.ToString());
                CryoviewTools.LogMessage(LogLevel.Info, " App properties: " + de.Key.ToString() + " / " + de.Value.ToString());
            }

            try
            {   
                // catch any xaml parse errors
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Current.Shutdown();
            }

            //m_cryoviewViewModel = new CryoviewMainViewModel();
            //m_cryoviewViewModel.ExceptionRaised += m_cryoviewViewModel_ExceptionRaised;   // something unexpected happened
            //m_cryoviewViewModel.Initialize();
            new Thread(
                    delegate ()
                    {
                        CreateViewModel();  // can't put in thread as datacontext and Initialize need an object
                        m_cryoviewViewModel.Initialize();
                    }
                ).Start();

            LoadMef(); // Load UI components
            m_db = new DBModel();
            ConnectCameras();
            LoadImageTabs();
            this.DataContext = m_cryoviewViewModel;
            m_cryoviewViewModel.SendMCMObject();
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// user requested close
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");        
            Dispose(true);
            GC.SuppressFinalize(this);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            if (disposing)
            {
                foreach (TabItem tab in tabFunctionView.Items)
                {
                    (new Thread(() =>
                    {
                        Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                RuntimeComponent part = (RuntimeComponent)tab.Content;
                                part.Close();    // calls the Close() method implemented in the code behind of the user control, per the IOcvWindow contract
                                tabFunctionView.Items.Remove(tab);
                                Thread.Sleep(500);
                            }
                            )
                        );
                    })).Start();
                }
                foreach (TabItem tab in tabImageViewOne.Items)
                {
                    (new Thread(() =>
                    {
                        Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                if (tab.Content.GetType() == typeof(XRayView))
                                {
                                    XRayView xrayTab = (XRayView)tab.Content;
                                    xrayTab.Dispose();
                                }
                                else if(tab.Content.GetType() == typeof(OpticalView))
                                {
                                    OpticalView opticalTab = (OpticalView)tab.Content;
                                    opticalTab.Dispose();
                                }
                                Thread.Sleep(500);
                            }
                            )
                        );
                    })).Start();

                }
                foreach (TabItem tab in tabImageViewTwo.Items)
                {
                    (new Thread(() =>
                    {
                        Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                if (tab.Content.GetType() == typeof(XRayView))
                                {
                                    XRayView xrayTab = (XRayView)tab.Content;
                                    xrayTab.Dispose();
                                }
                                else if (tab.Content.GetType() == typeof(OpticalView))
                                {
                                    OpticalView opticalTab = (OpticalView)tab.Content;
                                    opticalTab.Dispose();
                                }
                                Thread.Sleep(500);
                            }
                            )
                        );
                    })).Start();
                }

                if(CameraX != null)
                {
                    CameraX.ExceptionRaised -= CameraX_ExceptionRaised;
                    CameraX?.Dispose();
                }
                if (CameraY != null)
                {
                    CameraY.ExceptionRaised -= CameraY_ExceptionRaised;
                    CameraY?.Dispose();
                }

                m_db.Dispose();
                m_cryoviewViewModel.ExceptionRaised -= m_cryoviewViewModel_ExceptionRaised;
                m_cryoviewViewModel.Dispose();
                m_eventLog?.Dispose();
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            m_cryoviewViewModel = new CryoviewMainViewModel();
            m_cryoviewViewModel.ExceptionRaised += m_cryoviewViewModel_ExceptionRaised;   // something unexpected happened
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectCameras()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();

            //retrieve camera settings from database
            m_db.RetrieveSettings(Location, "XRayCamera", configParams);
            string model = "";
            configParams.TryGetValue("Model", out model);

            CameraX = new ImageBaslerModel(Location + "X");
            CameraX.ExceptionRaised += CameraX_ExceptionRaised;
            CameraY = new ImageBaslerModel(Location + "Y");
            CameraY.ExceptionRaised += CameraY_ExceptionRaised;

            if(model == "Physical")
            {
                string device = ""; string plugin = "";
                configParams.TryGetValue("Device", out device);
                configParams.TryGetValue("Plugin", out plugin);
                CameraZ = new XRayPhysicalModel(device, plugin);
            }
            else
            {
                CameraZ = new XRayVirtualModel();
            }
            
            bool success = CameraZ.Initialize();
            if (!success)
            {
                MessageBox.Show("Failed to initialize X-Ray camera", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { CameraZ.ExceptionRaised += CameraZ_ExceptionRaised; }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        //The image tabs do not use MEF since we need to have multiple instances of each class.
        private void LoadImageTabs()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            XRayView xrayTab1 = new XRayView("XRayView1", "XRayView2");
            XRayView xrayTab2 = new XRayView("XRayView2", "XRayView1");
            OpticalView opticalTab1 = new OpticalView("OpticalView1", "OpticalView2");
            OpticalView opticalTab2 = new OpticalView("OpticalView2", "OpticalView1");
            OpticalView opticalTab3 = new OpticalView("OpticalView3", "OpticalView4");
            OpticalView opticalTab4 = new OpticalView("OpticalView4", "OpticalView3");
            
            TabItem tab = new TabItem();
            tab.Header = "X-Ray Image";
            tab.Content = xrayTab1;
            tabImageViewOne.Items.Add(tab);
            
            tab = new TabItem();
            tab.Header = "X-Ray Image";
            tab.Content = xrayTab2;
            tabImageViewTwo.Items.Add(tab);

            tab = new TabItem();
            tab.Header = "Optical Image One";
            tab.Content = opticalTab1;
            tabImageViewOne.Items.Add(tab);

            tab = new TabItem();
            tab.Header = "Optical Image One";
            tab.Content = opticalTab2;
            tabImageViewTwo.Items.Add(tab);

            tab = new TabItem();
            tab.Header = "Optical Image Two";
            tab.Content = opticalTab3;
            tabImageViewOne.Items.Add(tab);

            tab = new TabItem();
            tab.Header = "Optical Image Two";
            tab.Content = opticalTab4;
            tabImageViewTwo.Items.Add(tab);
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        #endregion initialization

        #region windows events
        /// <summary>
        /// user wants to exit app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileExit_Click(object sender, RoutedEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            this.Close(); // raises the Closing() event
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// <summary>
        /// Displays the About window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuHelpAbout_Click(object sender, RoutedEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// initialized; layout has run; data is bound and ready for rendering; before ContentRendered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks> fires because of the MainWindow.Show() method in the app startup event</remarks>
        private void CryoviewMainView_Loaded(object sender, RoutedEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            m_bUserControlLoaded = true;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// occurs after all content is rendered; after _loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CryoviewMainView_ContentRendered(object sender, EventArgs e)
        { }

        /// <summary>
        /// element created and properties set; fires on children before the parent is initialized; no rendering performed.
        /// Actual width not calculated yet; data binding not occurred yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>fires because of the InitializeComponent() method in the ctor </remarks>
        private void CryoviewMainView_Initialized(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///  Chance to clean up Main Window resources.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CryoviewMainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");    
            Dispose();
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// By this point we are on the way out of the app. No turnaround now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CryoviewMainView_Closed(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Button clicked to save all open HDF files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdHDF_Save_All_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Button clicked to set the illuminator to the high preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSet_Illuminator_CH1_High_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");         
            CommandMessageEvent.Instance.Publish(new CommandMessage()
            {
                ID = "Illuminator",
                Command = "SetCH1High"
            });
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// Button clicked to set the illuminator to the low preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSet_Illuminator_CH1_Low_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");      
            CommandMessageEvent.Instance.Publish(new CommandMessage()
            {
                ID = "Illuminator",
                Command = "SetCH1Low"
            });
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// Button clicked to set the illuminator to the high preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void cmdSet_Illuminator_CH2_High_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            CommandMessageEvent.Instance.Publish(new CommandMessage()
            {
                ID = "Illuminator",
                Command = "SetCH2High"
            });
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// Button clicked to set the illuminator to the low preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSet_Illuminator_CH2_Low_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");          
            CommandMessageEvent.Instance.Publish(new CommandMessage()
            {
                ID = "Illuminator",
                Command = "SetCH2Low"
            });
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// Button clicked to clear the Application Status box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdClear_App_Status_Click(object sender, EventArgs e)
        {

        }
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        /// <summary>
        /// This is where the user interface (main m_runtimePart) is built at runtime.
        /// MEF looks at each dll in the exec dir. If it matches the contract specified in OCV_Cryoview_IWindow, then the 
        /// dll is loaded into mem as a part. When MEF is finished, we will add each part to the tab control on the main m_runtimePart.
        /// </summary>
        private void LoadMef()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            string status = "";
            LoadMefParts(ref m_runtimeParts, ref status);
            if (!status.Equals("")) { MessageBox.Show(status, "Contact SDG!"); Process.GetCurrentProcess().Kill(); }
            foreach (RuntimeComponent win in m_runtimeParts)
            {
                TabItem tab = new TabItem();
                tab.Header = win.ServiceName;
                tab.Content = win;
                tabFunctionView.Items.Add(tab);
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// MEF allows building the app according to what dlls are found in the execution dir. Moves building the app from compile time to runtime.
        /// </summary>
        /// <param name="m_runtimeParts"> the dlls found which match the MEF contract </param>
        /// <param name="status"></param>
        private void LoadMefParts(ref IEnumerable<RuntimeComponent> windows, ref string status)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
           
            try
            {       // let MEF build the app out of discovered parts
                var catalog = new AggregateCatalog();   // var type can not be init'd to null
                // BadFormatImageException occurs as a first chance exception if attempting to load a flat dll
                // as they are w/o a manifest, e.g. hd421md.dll, szlib.dll. It can be safely ignored. 
                // The code will continue to load .net dlls relevant to using the Managed Extension Framework (MEF).
                catalog = new AggregateCatalog(new DirectoryCatalog("."),
                        new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                var container = new CompositionContainer(catalog);
                windows = container.GetExportedValues<RuntimeComponent>();  // ctors of the individual user components for the views called here
            }
            catch (ReflectionTypeLoadException ex)
            {
                string msg = "";
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    msg += exSub.Message + Environment.NewLine;
                }
                status = msg;
            }
            catch (Exception ex)
            {
                status = ex.ToString();
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

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

        /// <summary>
        /// Not really expecting exceptions to make it back to client. Something went very wrong, e.g. viewmodel initialization.
        /// Really shouldn't continue.
        /// </summary>
        /// <param name="msg"></param>
        private void m_cryoviewViewModel_ExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            if (msg.Contains("Fatal Configuration Error"))
            {
                MessageBox.Show(msg + "\r\n SHUTTING DOWN!", "Contact SDG!");
                Process.GetCurrentProcess().Kill();
            }
            MessageBox.Show(msg, "Contact SDG!");
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void CameraX_ExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            CryoviewTools.LogMessage(LogLevel.Alert, msg);
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            m_cryoviewViewModel.StatusMessage = msg;
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void CameraY_ExceptionRaised (string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            CryoviewTools.LogMessage(LogLevel.Alert, msg);
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            m_cryoviewViewModel.StatusMessage = msg;
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void CameraZ_ExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            CryoviewTools.LogMessage(LogLevel.Alert, msg);
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            m_cryoviewViewModel.StatusMessage = msg;
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion event sinks
    }
}
