using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Target_Filling_ViewModel;

namespace Target_Filling_View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IDisposable
    {
        #region backing vars
        bool m_bUserControlLoaded = false;
        TargetFillingViewModel m_ViewModel = null;
        #endregion backing vars

        public Window1(TargetFillingViewModel model)
        {
            InitializeComponent();
            m_ViewModel = model;
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
                this.Close();
            }
        }

        /// <summary>
        /// This event fires when the focus is moved back to the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                this.DataContext = m_ViewModel;
            }
        }
    }
}
