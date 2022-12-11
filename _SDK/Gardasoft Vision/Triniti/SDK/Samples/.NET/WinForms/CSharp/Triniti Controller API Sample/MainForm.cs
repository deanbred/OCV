//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Gardasoft.Controller.API.Exceptions;
using Gardasoft.Controller.API.Interfaces;
using Gardasoft.Controller.API.Managers;
using Gardasoft.Controller.API.Model;
using Gardasoft.Controller.API.EventsArgs;
using System.IO;

namespace Gardasoft.Controller.API.CSharp.Sample.WinForms.Sample_1
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainForm : Form
    {
        #region Properties
        /// <summary>
        /// The current instance of the ControllerManager
        /// </summary>
        private ControllerManager _controllerManager;
        private IChannel _activeChannel;
        private IController _activeController;

        private const string Monitor1 = "ControllerTemperature";
        private string Monitor2 = "ChannelLoadPower";
		private const string Monitor3 = "LensTemperature";
        private const string FocalPowerText = "Focal Power";
        private const string FocalPowerUnits = "(Diopters)";

        #endregion

        #region Delegates
        private delegate void UpdateStatusStripDelegate(EventArgs e);
        private delegate void PropertyChangedDelegate(object sender, System.ComponentModel.PropertyChangedEventArgs e);
        #endregion

        #region Construction/Initialisation
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// Initialise the Controller Manager and update form controls
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            _controllerManager = ControllerManager.Instance();
            // Display API version in caption
            Text += @" v" + _controllerManager.APIInfo.Version;
            _controllerManager.DiscoverControllers();

			cbMode.DataSource = Enum.GetNames(typeof(Register.ChannelMode));

            PopulateControllerList();
            ResetForm();
            LensControlUIReset();
            LensControlUIToggle(false);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Populates the controller list with available controllers
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void PopulateControllerList()
        {
            cbControllers.Items.Clear();

            if (_controllerManager.Controllers.Count > 0)
            {   // Populate controller ComboBox
                foreach (IController controller in _controllerManager.Controllers)
                {
                    cbControllers.Items.Add(controller);
                    cbControllers.SelectedIndex = 0;
                }
                BtnOpen.Enabled = true;
            }
            else
            {
                BtnOpen.Enabled = false;
            }
        }


        /// <summary>
        /// Handles the Click event of the BtnSearch control.
        /// Search for controllers and update the form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            _controllerManager.DiscoverControllers();
            PopulateControllerList();
        }


        /// <summary>
        /// Handles the Click event of the BtnOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnOpen_Click(object sender, EventArgs e)
        {
			_activeController = _controllerManager.Controllers[cbControllers.SelectedIndex];

			try
            {
                _activeController.StatusChanged += ActiveControllerStatusChanged;
                _activeController.ConnectionStatusChanged += ActiveControllerConnectionStatusChanged;

                _activeController.Open();

                
                if (_activeController.IsTrinitiController)
                {

                    // Subscribe to register value changes to demonstrate monitoring dynamic registers
                    if (_activeController.Channels.Count > 0)
                    {
                        if (_activeController.Registers.Contains(Monitor1))
                        {
                            _activeController.Registers[Monitor1].PropertyChanged += MainFormMonitor1_PropertyChanged;
                            labelMonitor1.Text = _activeController.Registers[Monitor1].Caption;
                            labelMonitor1Unit.Text = _activeController.Registers[Monitor1].UserUnits.Caption;
                        }

						if (_activeController.Channels[0].Registers.Contains(Monitor2))
                        {
                            //Monitor2Value.DataBindings.Clear(); // Use databinding to monitor value
                            //Monitor2Value.DataBindings.Add("Value", controller.Channels[0].Registers[monitor2], "CurrentValue");

                            _activeController.Channels[0].Registers[Monitor2].PropertyChanged += MainFormMonitor2_PropertyChanged;
                            labelMonitor2.Text = _activeController.Channels[0].Registers[Monitor2].Caption;
                            labelMonitor2Unit.Text = _activeController.Channels[0].Registers[Monitor2].UserUnits.Caption;
                        }


						if (_activeController.Channels[0].Registers.Contains(Monitor3))
						{
							//Monitor2Value.DataBindings.Clear(); // Use databinding to monitor value
							//Monitor2Value.DataBindings.Add("Value", controller.Channels[0].Registers[monitor2], "CurrentValue");

							_activeController.Channels[0].Registers[Monitor3].PropertyChanged += MainFormMonitor3_PropertyChanged;
							labelMonitor3.Text = _activeController.Channels[0].Registers[Monitor3].Caption;
							labelMonitor3Unit.Text = _activeController.Channels[0].Registers[Monitor3].UserUnits.Caption;
						}

					}

                    // Populate list of channels
                    cbChannels.Items.Clear();
                    foreach (IChannel channel in _activeController.Channels)
                    {
                        cbChannels.Items.Add(channel);
                    }
                    if (cbChannels.Items.Count > 0)
                        // Select the first channel, which then updates the controls on the form
                        cbChannels.SelectedIndex = 0;

                    cbChannels.Enabled = true;
                    BtnImportConfiguration.Enabled = true;
                    BtnExportConfiguration.Enabled = true;
                    BtnOpen.Enabled = false;
                    BtnClose.Enabled = true;

                    panelChannelCommands.Enabled = true;
                    panelDirectCommand.Enabled = true;
                    panelControllers.Enabled = false;
                }
                else
                {
                    BtnOpen.Enabled = false;
                    BtnClose.Enabled = true;
                    panelControllers.Enabled = false;
                    panelChannelCommands.Enabled = false;
                }
            }
            catch (FailedToOpenControllerGardasoftException exception)
            {
                _activeController.ConnectionStatusChanged -= ActiveControllerConnectionStatusChanged;
                _activeController.StatusChanged -= ActiveControllerStatusChanged;
                ResetForm();
                LensControlUIReset();
                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }


        /// <summary>
        /// Handles the PropertyChanged event of the MainForm control.
        /// Notification that the value of one of the registers we are monitoring has changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void MainFormMonitor1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormMonitor1_PropertyChanged), sender, e);
            }
            else
            {
                Monitor1Value.Text =
                        ((float)_activeController.Registers[Monitor1].CurrentValue).ToString("0.00");
            }

        }

        /// <summary>
        /// Handles the PropertyChanged event of the MainForm control.
        /// Notification that the value of one of the registers we are monitoring has changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void MainFormMonitor2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormMonitor2_PropertyChanged), sender, e);
            }
            else
            {
                Monitor2Value.Text =
                        ((float)_activeChannel.Registers[Monitor2].CurrentValue).ToString("0.00000");
            }
        }
		/// <summary>
		/// Handles the PropertyChanged event of the MainForm control.
		/// Notification that the value of one of the registers we are monitoring has changed
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		void MainFormMonitor3_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new PropertyChangedDelegate(MainFormMonitor3_PropertyChanged), sender, e);
			}
			else
			{
				Monitor3Value.Text =
						((float)_activeChannel.Registers[Monitor3].CurrentValue).ToString("0.00");
			}
		}
		/// <summary>
		/// Handles the PropertyChanged event of the MainForm control.
		/// Notification that the value of one of the registers we are monitoring has changed
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		void MainFormBrightness_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormBrightness_PropertyChanged), sender, e);
            }
            else
            {

                if (_activeChannel != null)
                {
                    try
                    {
                        labelBrightness.Text = ((float)_activeChannel.Registers["Brightness"].CurrentValue).ToString("0.00");
                        trackBarBrightness.Value = Convert.ToInt32(_activeChannel.Registers["Brightness"].CurrentValue);
                        }
                    catch (Exception)
                    {
                        _activeChannel.Registers["Brightness"].CurrentValue = 0;
                    }
 
                }
            }

        }
        /// <summary>
        /// Handles the PropertyChanged event of the MainForm control.
        /// Notification that the value of one of the registers we are monitoring has changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void MainFormPulseDelay_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormPulseDelay_PropertyChanged), sender, e);
            }
            else
            {

                if (_activeChannel != null)
                {
                    try
                    {
                        labelDelay.Text =
                            ((float)_activeChannel.Registers["PulseDelay"].CurrentValue).ToString("0.00");
                        trackBarPulseDelay.Value = Convert.ToInt32(_activeChannel.Registers["PulseDelay"].CurrentValue);
                    }
                    catch (Exception)
                    {
                        _activeChannel.Registers["PulseDelay"].CurrentValue = 10F;
                    }

                }
            }

        }

        /// <summary>
        /// Handles the PropertyChanged event of the MainForm control.
        /// Notification that the value of one of the registers we are monitoring has changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void MainFormPulseWidth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormPulseWidth_PropertyChanged), sender, e);
            }
            else
            {
                if (_activeChannel != null)
                {
                    try
                    {
                        labelLength.Text =
                            ((float)_activeChannel.Registers["PulseWidth"].CurrentValue).ToString("0.00");
                        trackBarPulseWidth.Value = Convert.ToInt32(_activeChannel.Registers["PulseWidth"].CurrentValue);

                    }
                    catch (Exception)
                    {
                        _activeChannel.Registers["PulseWidth"].CurrentValue = 10F;
                        
                    }
                }
            }

        }

        /// <summary>
        /// Handles the ConnectionStatusChanged event of the controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Gardasoft.Controller.API.EventsArgs.ControllerConnectionStatusChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void ActiveControllerConnectionStatusChanged(object sender, ControllerConnectionStatusChangedEventArgs e)
        {
            UpdateStatusStrip(e);
        }

        /// <summary>
        /// Updates the status strip.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UpdateStatusStrip(EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new UpdateStatusStripDelegate(UpdateStatusStrip), (e));
            else
            {
                ControllerConnectionStatusChangedEventArgs ea = e as ControllerConnectionStatusChangedEventArgs;

                if (ea != null)
                {
                    // Update Status strip
                    toolStripStatusLabel1.Text = ea.ControllerCaption + @" Controller Connection " + ea.ConnectionStatus;

                    bool enable = ea.ConnectionStatus != ControllerConnectionStatus.Fault;


					if (enable)
					{
						labelOnline.BackColor = Color.LawnGreen;
						labelOnline.Text = "ON Line";
					}
					else
					{
						labelOnline.BackColor = Color.Yellow;
						labelOnline.Text = "OFF Line";
					}

					panelChannelCommands.Enabled = enable & _activeController.IsTrinitiController;
                    panelDirectCommand.Enabled = enable;
                    panelChannelStatus.Enabled = enable;



                    Application.DoEvents();
                }
                else
                {
                    ControllerStatusChangedEventArgs controllerStatusChangedEventArgs = e as ControllerStatusChangedEventArgs;
                    if (controllerStatusChangedEventArgs != null)
                    {
                        // Update Status strip
                        toolStripStatusLabel1.Text = controllerStatusChangedEventArgs.ControllerCaption + @" Controller " + controllerStatusChangedEventArgs.ControllerStatus;
                        switch (controllerStatusChangedEventArgs.ControllerStatus)
                        {
                            case ControllerStatus.Connected:
                                labelOnline.BackColor = Color.LawnGreen;
								labelOnline.Text = "ON Line";

								break;
                            case ControllerStatus.Connecting:
                                labelOnline.BackColor = Color.Yellow;
								labelOnline.Text = "OFF Line";

								break;
                            case ControllerStatus.Disconnected:
                                labelOnline.BackColor = Color.DarkGray;
								labelOnline.Text = "OFF Line";

								break;
                        }

                        Application.DoEvents();
                    }
                }
            }
        }


        /// <summary>
        /// Handles the StatusChanged event of the controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Gardasoft.Controller.API.EventsArgs.ControllerStatusChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void ActiveControllerStatusChanged(object sender, ControllerStatusChangedEventArgs e)
        {
            UpdateStatusStrip(e);

        }

        /// <summary>
        /// Handles the Click event of the BtnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            textBoxResult.Text = "";
            try
            {
                if (_activeChannel != null)
                {
                    if (_activeChannel.Registers["Brightness"] != null)
                    {
                        _activeChannel.Registers["Brightness"].PropertyChanged -= MainFormBrightness_PropertyChanged;
                    }
                    if (_activeChannel.Registers["PulseDelay"] != null)
                    {
                        _activeChannel.Registers["PulseDelay"].PropertyChanged -= MainFormPulseDelay_PropertyChanged;
                    }
                    if (_activeChannel.Registers["PulseWidth"] != null)
                    {
                        _activeChannel.Registers["PulseWidth"].PropertyChanged -= MainFormPulseWidth_PropertyChanged;
                    }
                    if (_activeChannel.Registers["ChannelMode"] != null)
                    {
                        _activeChannel.Registers["ChannelMode"].PropertyChanged -= MainFormChannelMode_PropertyChanged;
                    }
                    if (_activeChannel.Registers["FocalPowerValue"] != null)
                    {
                        _activeChannel.Registers["FocalPowerValue"].PropertyChanged -= MainFormFocalPowerValue_PropertyChanged;
                    }
                }
                // Unhook from the register we were monitoring
                if (_activeController.Channels.Count > 0)
                {
                    if (_activeController.Registers.Contains(Monitor1))
                        _activeController.Registers[Monitor1].PropertyChanged -= MainFormMonitor1_PropertyChanged;
                    if (_activeController.Channels[0].Registers.Contains(Monitor2))
                        _activeController.Channels[0].Registers[Monitor2].PropertyChanged -= MainFormMonitor2_PropertyChanged;
					if (_activeController.Channels[0].Registers.Contains(Monitor3))
						_activeController.Channels[0].Registers[Monitor3].PropertyChanged -= MainFormMonitor3_PropertyChanged;
				}

                _activeController.Close();
				

            }
            catch (FailedToCloseControllerGardasoftException exception)
            {
                MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            _activeController.ConnectionStatusChanged -= ActiveControllerConnectionStatusChanged;
            _activeController.StatusChanged -= ActiveControllerStatusChanged;
			
            _activeChannel = null;
            _activeController = null;


            ResetForm();
            LensControlUIReset();

            panelControllers.Enabled = true;
            BtnClose.Enabled = false;
            BtnOpen.Enabled = true;
			_controllerManager.DiscoverControllers();
			PopulateControllerList();
			LensControlUIToggle(false);
		}

        private void ResetForm()
        {
            // Clear databinding
            trackBarBrightness.DataBindings.Clear();
            labelBrightness.DataBindings.Clear();
            trackBarPulseDelay.DataBindings.Clear();
            labelDelay.DataBindings.Clear();
            trackBarPulseWidth.DataBindings.Clear();
            labelLength.DataBindings.Clear();


            Monitor1Value.Text = "";
            Monitor2Value.Text = "";
			Monitor3Value.Text = "";
			Monitor3Value.Visible = false;


			trackBarBrightness.Visible = false;
            trackBarPulseDelay.Visible = false;
            trackBarPulseWidth.Visible = false;
			trackBarFocalPower.Visible = false;

			trackBarBrightness.Value = 0;
			trackBarPulseDelay.Value = 0;
			trackBarPulseWidth.Value = 0;
			trackBarFocalPower.Value = 0;

			labelBrightness.Text = @"0";
            labelDelay.Text = @"100";
            labelLength.Text = @"100";

            labelMonitor1Unit.Text = @"-";
            labelMonitor1.Text = @"-----";

            labelMonitor2Unit.Text = @"-";
            labelMonitor2.Text = @"-----";

            cbMode.SelectedIndex = 0;
            cbChannels.Items.Clear();

            BtnImportConfiguration.Enabled = false;
            BtnExportConfiguration.Enabled = false;
            BtnClose.Enabled = false;
            panelChannelCommands.Enabled = false;
            panelDirectCommand.Enabled = false;
        }

        /// <summary>
        /// Handles the Click event of the BtnExportConfiguration control.
        /// Export open controller configuration to a user specified XML file
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnExportConfiguration_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = @"XML files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 0
            };

            if (Properties.Settings.Default.LastXMLFile != string.Empty)
            {
                sfd.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.LastXMLFile);
                sfd.FileName = Path.GetFileName(Properties.Settings.Default.LastXMLFile);
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastXMLFile = sfd.FileName;
                Properties.Settings.Default.Save();


                try
                {
                    _activeController.ExportConfiguration(sfd.FileName);

                    if (MessageBox.Show(@"View File?", @"Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        Process.Start(sfd.FileName);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(@"Error when saving XML " + exception.Message, @"Exception", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

        }

        /// <summary>
        /// Handles the Click event of the BtnImportConfiguration control.
        /// Import configuration from user specified XML file and send to the currently open controller 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnImportConfiguration_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = @"XML files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 0
            };

            if (Properties.Settings.Default.LastXMLFile != string.Empty)
            {
                ofd.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.LastXMLFile);
                ofd.FileName = Path.GetFileName(Properties.Settings.Default.LastXMLFile);
            }

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastXMLFile = ofd.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    _activeController.ImportConfiguration(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbChannels control.
        /// User has changed channels so update the forms controls
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cbChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_activeChannel != null)
            {
                if (_activeChannel.Registers["Brightness"] != null)
                {
                    _activeChannel.Registers["Brightness"].PropertyChanged -= MainFormBrightness_PropertyChanged;
                }
                if (_activeChannel.Registers["PulseDelay"] != null)
                {
                    _activeChannel.Registers["PulseDelay"].PropertyChanged -= MainFormPulseDelay_PropertyChanged;
                }
                if (_activeChannel.Registers["PulseWidth"] != null)
                {
                    _activeChannel.Registers["PulseWidth"].PropertyChanged -= MainFormPulseWidth_PropertyChanged;
                }
                if (_activeChannel.Registers["ChannelMode"] != null)
                {
                    _activeChannel.Registers["ChannelMode"].PropertyChanged -= MainFormChannelMode_PropertyChanged;
                }
                if (_activeChannel.Registers["FocalPowerValue"] != null)
                {
                    _activeChannel.Registers["FocalPowerValue"].PropertyChanged -= MainFormFocalPowerValue_PropertyChanged;
                }
            }

            _activeChannel = cbChannels.SelectedItem as IChannel;
            if (_activeChannel != null)
            {

	//			  bool isLensChannel = false;
	//            Register register = _activeChannel.Registers["ChannelType"];

	//            // If the register exists the check to see if a lens channel or lighting channel
	//            if (register != null)
	//            {
	//                // If the channel is a Lens channel then subscribe to focal power and enable lens control
	//                if (Convert.ToInt32(register.CurrentValue) == 1)
	//                {
	//                    isLensChannel = true;
	//                }
	//            }
	//                if (!isLensChannel)


				if (!_activeController.IsLensChannel)
                {
                    LensControlUIReset();
                    LensControlUIToggle(false);
                    if (_activeChannel.Registers.Contains("Brightness"))
                    {

                        _activeChannel.Registers["Brightness"].PropertyChanged += MainFormBrightness_PropertyChanged;
                        _activeChannel.Registers["PulseWidth"].PropertyChanged += MainFormPulseWidth_PropertyChanged;
                        _activeChannel.Registers["PulseDelay"].PropertyChanged += MainFormPulseDelay_PropertyChanged;
                        _activeChannel.Registers["ChannelMode"].PropertyChanged += MainFormChannelMode_PropertyChanged;

                    }
                }
                // Is a lens channel. Now  configure GUI and subscribe to focal power property changed
                else
                {
                    LensControlUIToggle(true);
                }
                // Force immediate update of values 
                _activeChannel.Registers.Refresh();
            }
        }

        private void LensControlUIReset()
        {
            labelFocalPowerTitle.Text = FocalPowerText + Environment.NewLine + FocalPowerUnits;
            trackBarFocalPower.Minimum = 0;
            trackBarFocalPower.Maximum = 1500;
            trackBarFocalPower.TickFrequency = 100;
            trackBarFocalPower.TickStyle = TickStyle.BottomRight;
            trackBarFocalPower.Value = 0;
            labelFocalPower.Text = @"0.00";
        }

        private void LensControlUIToggle(bool isEnabled)
        {
            PnlFocalPower.Visible = isEnabled;
			Monitor3Value.Visible = isEnabled;
			labelMonitor3.Visible = isEnabled;
			labelMonitor3Unit.Visible = isEnabled;
			trackBarFocalPower.Visible = isEnabled;
			ToggleLightingControls(!isEnabled);

            // Get focal power min and max values
            if (_activeChannel != null && isEnabled)
            {
                float minValue = Convert.ToSingle(_activeChannel.Registers["FocalPowerMin"].CurrentValue) * 100.0F;
                trackBarFocalPower.Minimum = Convert.ToInt32(minValue);
                float maxValue = Convert.ToSingle(_activeChannel.Registers["FocalPowerMax"].CurrentValue) * 100.0F;
                trackBarFocalPower.Maximum = Convert.ToInt32(maxValue);
                _activeChannel.Registers["FocalPowerValue"].PropertyChanged += MainFormFocalPowerValue_PropertyChanged;
                labelFocalPowerTitle.Text = FocalPowerText + Environment.NewLine + FocalPowerUnits + Environment.NewLine + @"[" + (minValue / 100.0F).ToString("F") + @" to " + (maxValue / 100.0F).ToString("F") + @"]";
				
			}
        }

        private void ToggleLightingControls(bool isEnabled)
        {
            PnlBrightness.Visible = isEnabled;
            label2.Visible = isEnabled;
            label3.Visible = isEnabled;
            label4.Visible = isEnabled;
            label5.Visible = isEnabled;
            label6.Visible = isEnabled;
            label7.Visible = isEnabled;
            label8.Visible = isEnabled;

            trackBarBrightness.Visible = isEnabled;
            trackBarPulseDelay.Visible = isEnabled;
            trackBarPulseWidth.Visible = isEnabled;

            labelBrightness.Visible = isEnabled;
            labelDelay.Visible = isEnabled;
            labelLength.Visible = isEnabled;

            if (isEnabled)
            {
                cbMode_SelectedIndexChanged(this, null);
            }
            cbMode.Visible = isEnabled;
        }

        private void MainFormFocalPowerValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormFocalPowerValue_PropertyChanged), sender, e);
            }
            else
            {

                if (_activeChannel != null)
                {
                    labelFocalPower.Text = ((float)_activeChannel.Registers["FocalPowerValue"].CurrentValue).ToString("0.00");
                    float fpValue = Convert.ToSingle(_activeChannel.Registers["FocalPowerValue"].CurrentValue) * 100.0F;

                    // If the focal power is out of range then set back to 0
                    if (fpValue < trackBarFocalPower.Minimum || fpValue > trackBarFocalPower.Maximum)
                    {
                        fpValue = 0.0F;
                        _activeChannel.Registers["FocalPowerValue"].CurrentValue = fpValue;
                    }
                    trackBarFocalPower.Value = Convert.ToInt32(fpValue);
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ChannelMode register.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void MainFormChannelMode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PropertyChangedDelegate(MainFormChannelMode_PropertyChanged), sender, e);
            }
            else
            {
                if (_activeChannel != null)
                {
                    cbMode.SelectedIndex = (int)_activeChannel.Registers["ChannelMode"].CurrentValue;
                }
            }
        }


        /// <summary>
        /// Handles the ValueChanged event of the trackBarBrightness control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void trackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            if (_activeChannel != null && _activeChannel.Registers["Brightness"] != null)
                _activeChannel.Registers["Brightness"].CurrentValue = trackBarBrightness.Value;
        }


        /// <summary>
        /// Handles the ValueChanged event of the trackBarPulseDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void trackBarPulseDelay_ValueChanged(object sender, EventArgs e)
        {
            if (_activeChannel != null && _activeChannel.Registers["PulseDelay"] != null)
                _activeChannel.Registers["PulseDelay"].CurrentValue = trackBarPulseDelay.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of the trackBarPulseWidth control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void trackBarPulseWidth_ValueChanged(object sender, EventArgs e)
        {
            if (_activeChannel != null && _activeChannel.Registers["PulseWidth"] != null)
                _activeChannel.Registers["PulseWidth"].CurrentValue = trackBarPulseWidth.Value;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cbMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((string) cbMode.SelectedItem == "Pulse")
                {
                    label5.Enabled = true;
                    label6.Enabled = true;
                    label7.Enabled = true;
                    label8.Enabled = true;
                    labelDelay.Enabled = true;
                    labelLength.Enabled = true;
                    trackBarPulseDelay.Enabled = true;
                    trackBarPulseWidth.Enabled = true;
                }
                else
                {
                    label5.Enabled = false;
                    label6.Enabled = false;
                    label7.Enabled = false;
                    label8.Enabled = false;
                    labelDelay.Enabled = false;
                    labelLength.Enabled = false;
                    trackBarPulseDelay.Enabled = false;
                    trackBarPulseWidth.Enabled = false;
                }

                trackBarBrightness.Maximum = 999;

                if (_activeChannel != null && _activeChannel.Registers["ChannelMode"] != null)
                {

                    if ((string) cbMode.SelectedItem == "Continuous")
                    {
                        _activeChannel.Registers["ChannelMode"].CurrentValue = Register.ChannelMode.Continuous;
                        trackBarBrightness.Maximum = 100;
                    }
                    else if ((string) cbMode.SelectedItem == "Pulse")
                        _activeChannel.Registers["ChannelMode"].CurrentValue = Register.ChannelMode.Pulse;
                    else if ((string) cbMode.SelectedItem == "Switched")
                        _activeChannel.Registers["ChannelMode"].CurrentValue = Register.ChannelMode.Switched;
                    else if ((string) cbMode.SelectedItem == "Selected")
                        _activeChannel.Registers["ChannelMode"].CurrentValue = Register.ChannelMode.Selected;


                    _activeChannel.Registers.Refresh();
                }
            }
            catch (GardasoftException exception)
            {
             Trace.WriteLine(exception.Message);   
            }
        }


        /// <summary>
        /// Handles the KeyPress event of the textBoxCommand control.
        /// If teh enter key is pressed send the command
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        private void textBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                btnSend_Click(this, new EventArgs());
            }
        }


        /// <summary>
        /// Handles the Click event of the btnSend control.
        /// Send the command to the connected controller
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_activeController != null)
            {
                textBoxResult.Text = "";
                try
                {
                    string result = _activeController.SendCommand(textBoxCommand.Text);
                    textBoxResult.Text = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        /// <summary>
        /// Handles the Click event of the BtnOpenWebPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BtnOpenWebPage_Click(object sender, EventArgs e)
        {
            if (_activeController != null)
            {
                Process.Start("http://" + _activeController.IPAddress);
            }
        }
        #endregion

        private void cbControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _activeController = _controllerManager.Controllers[cbControllers.SelectedIndex];

            if (_activeController != null && !_activeController.IPAddress.Contains("COM"))
            {
                // Set caption of open web page button and make visible
                BtnOpenWebPage.Text = string.Format("Open web page for {0} with IP {1}", _activeController.Caption, _activeController.IPAddress);
                BtnOpenWebPage.Visible = true;
            }
            else
            {
                BtnOpenWebPage.Text = string.Empty;
                BtnOpenWebPage.Visible = false;
            }
        }

        private void trackBarFocalPower_ValueChanged(object sender, EventArgs e)
        {
            if (_activeChannel != null && _activeChannel.Registers["FocalPowerValue"] != null)
            {
                float fpValue = Convert.ToSingle(trackBarFocalPower.Value) / 100.0F;
                _activeChannel.Registers["FocalPowerValue"].CurrentValue = fpValue;
            }
        }
    }
}
