﻿#pragma checksum "..\..\Cryoview_Main_View.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0580197BE2542C647EFB3A8F4738A384D69B7A8CBB6C1A08F6F2B47E8C72498C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Converters;
using Cryoview_Main_ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Cryoview_Main_View {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Cryoview_Main_View.MainWindow mainWindow;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabImageViewOne;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabImageViewTwo;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ToolBarTray tlbrtrayMain;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LSTemplbl;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label CFETemplbl;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label TIDlbl;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdHDFSaveAll;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdSetIlluminatorCH1High;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdSetIlluminatorCH1Low;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdSetIlluminatorCH2High;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdSetIlluminatorCH2Low;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Tasklbl;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label NetworkConnectionlbl;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabFunctionView;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\Cryoview_Main_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Statuslbl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Cryoview_Main_View;component/cryoview_main_view.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Cryoview_Main_View.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mainWindow = ((Cryoview_Main_View.MainWindow)(target));
            
            #line 10 "..\..\Cryoview_Main_View.xaml"
            this.mainWindow.Loaded += new System.Windows.RoutedEventHandler(this.CryoviewMainView_Loaded);
            
            #line default
            #line hidden
            
            #line 11 "..\..\Cryoview_Main_View.xaml"
            this.mainWindow.Initialized += new System.EventHandler(this.CryoviewMainView_Initialized);
            
            #line default
            #line hidden
            
            #line 12 "..\..\Cryoview_Main_View.xaml"
            this.mainWindow.ContentRendered += new System.EventHandler(this.CryoviewMainView_ContentRendered);
            
            #line default
            #line hidden
            
            #line 13 "..\..\Cryoview_Main_View.xaml"
            this.mainWindow.Closing += new System.ComponentModel.CancelEventHandler(this.CryoviewMainView_Closing);
            
            #line default
            #line hidden
            
            #line 14 "..\..\Cryoview_Main_View.xaml"
            this.mainWindow.Closed += new System.EventHandler(this.CryoviewMainView_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 39 "..\..\Cryoview_Main_View.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuFileExit_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 42 "..\..\Cryoview_Main_View.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.mnuHelpAbout_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.tabImageViewOne = ((System.Windows.Controls.TabControl)(target));
            return;
            case 5:
            this.tabImageViewTwo = ((System.Windows.Controls.TabControl)(target));
            return;
            case 6:
            this.tlbrtrayMain = ((System.Windows.Controls.ToolBarTray)(target));
            return;
            case 7:
            this.LSTemplbl = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.CFETemplbl = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.TIDlbl = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.cmdHDFSaveAll = ((System.Windows.Controls.Button)(target));
            
            #line 73 "..\..\Cryoview_Main_View.xaml"
            this.cmdHDFSaveAll.Click += new System.Windows.RoutedEventHandler(this.cmdHDF_Save_All_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.cmdSetIlluminatorCH1High = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\Cryoview_Main_View.xaml"
            this.cmdSetIlluminatorCH1High.Click += new System.Windows.RoutedEventHandler(this.cmdSet_Illuminator_CH1_High_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.cmdSetIlluminatorCH1Low = ((System.Windows.Controls.Button)(target));
            
            #line 78 "..\..\Cryoview_Main_View.xaml"
            this.cmdSetIlluminatorCH1Low.Click += new System.Windows.RoutedEventHandler(this.cmdSet_Illuminator_CH1_Low_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.cmdSetIlluminatorCH2High = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\Cryoview_Main_View.xaml"
            this.cmdSetIlluminatorCH2High.Click += new System.Windows.RoutedEventHandler(this.cmdSet_Illuminator_CH2_High_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.cmdSetIlluminatorCH2Low = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\Cryoview_Main_View.xaml"
            this.cmdSetIlluminatorCH2Low.Click += new System.Windows.RoutedEventHandler(this.cmdSet_Illuminator_CH2_Low_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.Tasklbl = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.NetworkConnectionlbl = ((System.Windows.Controls.Label)(target));
            return;
            case 17:
            this.tabFunctionView = ((System.Windows.Controls.TabControl)(target));
            return;
            case 18:
            
            #line 101 "..\..\Cryoview_Main_View.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.cmdClear_App_Status_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.Statuslbl = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

