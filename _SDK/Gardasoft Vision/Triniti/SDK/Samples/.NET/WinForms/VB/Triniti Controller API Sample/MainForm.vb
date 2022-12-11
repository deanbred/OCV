'-----------------------------------------------------------------------
' <copyright file="MainForm.vb" company="Gardasoft Products Ltd">
'    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
' </copyright>
' <author>Steve Cronk, Dharmesh Mistry</author>
'-----------------------------------------------------------------------
Imports System.IO
Imports Gardasoft.Controller.API.Exceptions
Imports Gardasoft.Controller.API.Managers
Imports Gardasoft.Controller.API.EventsArgs
Imports Gardasoft.Controller.API.Model
Imports Gardasoft.Controller.API.Interfaces
Imports Gardasoft.Controller.API.VB.Sample.WinForms.Sample_1.My

Public Class MainForm

#Region "Properties"
    ''' <summary>
    ''' The current instance of the ControllerManager
    ''' </summary>
    Private _controllerManager As ControllerManager
    Private _activeChannel As IChannel
    Private _activeController As IController

    Private Const Monitor1 As String = "ControllerTemperature"
	Private Const Monitor2 As String = "ChannelLoadPower"
	Private Const Monitor3 As String = "LensTemperature"
	Private Const FocalPowerText As String = "Focal Power"
    Private Const FocalPowerUnits As String = "(Diopters)"

#End Region

#Region "Delegates"
    Private Delegate Sub VoidDelegate()
    Private Delegate Sub UpdateStatusStripDelegate(e As EventArgs)
    Private Delegate Sub UpdateCommandStatusDelegate(e As ControllerCommandStatusChangedEventArgs)
    Private Delegate Sub PropertyChangedDelegate(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
#End Region



#Region "Construction/Initialisation"

    ''' <summary>
    ''' Handles the Load event of the MainForm control.
    ''' Initialise the Controller Manager and update form controls
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _controllerManager = ControllerManager.Instance()
        ' Display API version in caption
        Text += " v" + _controllerManager.APIInfo.Version
        _controllerManager.DiscoverControllers()

        cbMode.DataSource = [Enum].GetNames(GetType(Register.ChannelMode))

        PopulateControllerList()
        ResetForm()
        LensControlUIReset()
        LensControlUIToggle(False)

    End Sub

    ''' <summary>
    ''' Populates the controller list with available controllers
    ''' </summary>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub PopulateControllerList()
        cbControllers.Items.Clear()

        If _controllerManager.Controllers.Count > 0 Then
            ' Populate controller ComboBox
            For Each controller As IController In _controllerManager.Controllers
                cbControllers.Items.Add(controller)
                cbControllers.SelectedIndex = 0
            Next
            BtnOpen.Enabled = True
        Else
            BtnOpen.Enabled = False
        End If
    End Sub


#End Region

#Region "Implementation"
    ''' <summary>
    ''' Handles the Click event of the BtnSearch control.
    ''' Search for controllers and update the form
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>

    Private Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        _controllerManager.DiscoverControllers()
        PopulateControllerList()
    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnOpen control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click

        _activeController = _controllerManager.Controllers(cbControllers.SelectedIndex)

		Try
            AddHandler _activeController.StatusChanged, AddressOf controller_StatusChanged
            AddHandler _activeController.ConnectionStatusChanged, AddressOf controller_ConnectionStatusChanged

            _activeController.Open()
            If (_activeController.IsTrinitiController) Then
                ' Subscribe to register value changes to demonstrate monitoring dynamic registers
                If _activeController.Channels.Count > 0 Then
                    If _activeController.Registers.Contains(Monitor1) Then
                        AddHandler _activeController.Registers(Monitor1).PropertyChanged, AddressOf MainFormMonitor1_PropertyChanged
                        labelMonitor1.Text = _activeController.Registers(Monitor1).Caption
                        labelMonitor1Unit.Text = _activeController.Registers(Monitor1).UserUnits.Caption
                    End If

					If _activeController.Channels(0).Registers.Contains(Monitor2) Then
						AddHandler _activeController.Channels(0).Registers(Monitor2).PropertyChanged, AddressOf MainFormMonitor2_PropertyChanged
						labelMonitor2.Text = _activeController.Channels(0).Registers(Monitor2).Caption
						labelMonitor2Unit.Text = _activeController.Channels(0).Registers(Monitor2).UserUnits.Caption
					End If

					If _activeController.Channels(0).Registers.Contains(Monitor3) Then
						AddHandler _activeController.Channels(0).Registers(Monitor3).PropertyChanged, AddressOf MainFormMonitor3_PropertyChanged
						labelMonitor3.Text = _activeController.Channels(0).Registers(Monitor3).Caption
						labelMonitor3Unit.Text = _activeController.Channels(0).Registers(Monitor3).UserUnits.Caption
					End If

				End If

                ' Populate list of channels
                cbChannels.Items.Clear()
                For Each channel As IChannel In _activeController.Channels
                    cbChannels.Items.Add(channel)
                Next
                If cbChannels.Items.Count > 0 Then
                    ' Select the first channel, which then updates the controls on the form
                    cbChannels.SelectedIndex = 0
                End If

                BtnExportConfiguration.Enabled = True
                BtnImportConfiguration.Enabled = True
                BtnOpen.Enabled = False
                BtnClose.Enabled = True
                cbChannels.Enabled = True
                panelChannelCommands.Enabled = True
                panelDirectCommand.Enabled = True
                panelControllers.Enabled = False
            Else
                BtnOpen.Enabled = False
                BtnClose.Enabled = True
                panelControllers.Enabled = False

            End If

        Catch ex As FailedToOpenControllerGardasoftException
            RemoveHandler _activeController.ConnectionStatusChanged, AddressOf controller_ConnectionStatusChanged
            RemoveHandler _activeController.StatusChanged, AddressOf controller_StatusChanged
            ResetForm()
            LensControlUIReset()
            MessageBox.Show(ex.Message, Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub



    ''' <summary>
    ''' Handles the PropertyChanged event of the MainForm control.
    ''' Notification that the value of one of the registers we are monitoring has changed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub MainFormMonitor1_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormMonitor1_PropertyChanged), sender, e)
        Else
            Monitor1Value.Text = CSng(_activeController.Registers(Monitor1).CurrentValue).ToString("0.00")
        End If

    End Sub

    ''' <summary>
    ''' Handles the PropertyChanged event of the MainForm control.
    ''' Notification that the value of one of the registers we are monitoring has changed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub MainFormMonitor2_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormMonitor2_PropertyChanged), sender, e)
        Else
            Monitor2Value.Text = CSng(_activeChannel.Registers(Monitor2).CurrentValue).ToString("0.00")
        End If
    End Sub
	''' <summary>
	''' Handles the PropertyChanged event of the MainForm control.
	''' Notification that the value of one of the registers we are monitoring has changed
	''' </summary>
	''' <param name="sender">The source of the event.</param>
	''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
	''' <exception cref="System.NotImplementedException"></exception>
	Private Sub MainFormMonitor3_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
		If InvokeRequired Then
			BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormMonitor3_PropertyChanged), sender, e)
		Else
			Monitor3Value.Text = CSng(_activeChannel.Registers(Monitor3).CurrentValue).ToString("0.00")
		End If
	End Sub
	''' <summary>
	''' Handles the PropertyChanged event of the MainForm control.
	''' Notification that the value of one of the registers we are monitoring has changed
	''' </summary>
	''' <param name="sender">The source of the event.</param>
	''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
	''' <exception cref="System.NotImplementedException"></exception>
	Private Sub MainFormBrightness_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormBrightness_PropertyChanged), sender, e)
        Else

            If _activeChannel IsNot Nothing Then
                Try
                    labelBrightness.Text = CSng(_activeChannel.Registers("Brightness").CurrentValue).ToString("0.00")
                    trackBarBrightness.Value = Convert.ToInt32(_activeChannel.Registers("Brightness").CurrentValue)
                Catch ex As Exception
                    _activeChannel.Registers("Brightness").CurrentValue = 0
                End Try 
            End If
        End If

    End Sub
    ''' <summary>
    ''' Handles the PropertyChanged event of the MainForm control.
    ''' Notification that the value of one of the registers we are monitoring has changed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub MainFormPulseDelay_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormPulseDelay_PropertyChanged), sender, e)
        Else

            If _activeChannel IsNot Nothing Then
                Try
                    labelDelay.Text = CSng(_activeChannel.Registers("PulseDelay").CurrentValue).ToString("0.00")
                    trackBarPulseDelay.Value = Convert.ToInt32(_activeChannel.Registers("PulseDelay").CurrentValue)
                Catch ex As Exception
                    _activeChannel.Registers("PulseDelay").CurrentValue = 0
                End Try
                
            End If
        End If

    End Sub

    ''' <summary>
    ''' Handles the PropertyChanged event of the MainForm control.
    ''' Notification that the value of one of the registers we are monitoring has changed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub MainFormPulseWidth_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormPulseWidth_PropertyChanged), sender, e)
        Else
            If _activeChannel IsNot Nothing Then
                Try
                    labelLength.Text = CSng(_activeChannel.Registers("PulseWidth").CurrentValue).ToString("0.00")
                    trackBarPulseWidth.Value = Convert.ToInt32(_activeChannel.Registers("PulseWidth").CurrentValue)
                Catch ex As Exception
                    _activeChannel.Registers("PulseWidth").CurrentValue = 0
                End Try
                
            End If
        End If

    End Sub

    ''' <summary>
    ''' Handles the PropertyChanged event of the MainForm control.
    ''' Notification that the value of one of the registers we are monitoring has changed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub MainFormChannelMode_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormChannelMode_PropertyChanged), sender, e)
        Else
            If _activeChannel IsNot Nothing Then
                cbMode.SelectedIndex = CInt(_activeChannel.Registers("ChannelMode").CurrentValue)
            End If
        End If

    End Sub


	''' <summary>
	''' Handles the ConnectionStatusChanged event of the controller control.
	''' </summary>
	''' <param name="sender">The source of the event.</param>
	''' <param name="e">The <see cref="Gardasoft.Controller.API.EventsArgs.ControllerConnectionStatusChangedEventArgs"/> instance containing the event data.</param>
	''' <exception cref="System.NotImplementedException"></exception>
	Private Sub controller_ConnectionStatusChanged(sender As Object, e As ControllerConnectionStatusChangedEventArgs)
        UpdateStatusStrip(e)
    End Sub

    Private Sub UpdateStatusStrip(e As EventArgs)
        If InvokeRequired Then
            BeginInvoke(New UpdateStatusStripDelegate(AddressOf UpdateStatusStrip), (e))
        Else
            Dim ea As ControllerConnectionStatusChangedEventArgs = TryCast(e, ControllerConnectionStatusChangedEventArgs)

            If ea IsNot Nothing Then
                ' Update Status strip
                toolStripStatusLabel1.Text = ea.ControllerCaption + " Controller Connection " + ea.ConnectionStatus.ToString()

                Dim enable As Boolean
				If (ea.ConnectionStatus = ControllerConnectionStatus.Fault) Then
					enable = False
					labelOnline.BackColor = Color.Yellow
					labelOnline.Text = "OFF Line"

				Else
					enable = True
					labelOnline.BackColor = Color.LawnGreen
					labelOnline.Text = "ON Line"

				End If

				panelChannelCommands.Enabled = enable And _activeController.IsTrinitiController
                panelDirectCommand.Enabled = enable
                panelChannelStatus.Enabled = enable

                Windows.Forms.Application.DoEvents()
            Else
                Dim controllerStatusChangedEventArgs As ControllerStatusChangedEventArgs = TryCast(e, ControllerStatusChangedEventArgs)
                If controllerStatusChangedEventArgs IsNot Nothing Then
                    ' Update Status strip
                    toolStripStatusLabel1.Text = controllerStatusChangedEventArgs.ControllerCaption + " Controller " + controllerStatusChangedEventArgs.ControllerStatus.ToString()

                    Select Case controllerStatusChangedEventArgs.ControllerStatus
                        Case ControllerStatus.Connected
							labelOnline.BackColor = Color.LawnGreen
							labelOnline.Text = "ON Line"
							Exit Select
                        Case ControllerStatus.Connecting
							labelOnline.BackColor = Color.Yellow
							labelOnline.Text = "OFF Line"
							Exit Select
                        Case ControllerStatus.Disconnected
							labelOnline.BackColor = Color.DarkGray
							labelOnline.Text = "OFF Line"
							Exit Select
                    End Select


                    Windows.Forms.Application.DoEvents()
                End If
            End If
        End If
    End Sub


    ''' <summary>
    ''' Handles the StatusChanged event of the controller control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="Gardasoft.Controller.API.EventsArgs.ControllerStatusChangedEventArgs"/> instance containing the event data.</param>
    ''' <exception cref="System.NotImplementedException"></exception>
    Private Sub controller_StatusChanged(sender As Object, e As ControllerStatusChangedEventArgs)
        UpdateStatusStrip(e)
    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnClose control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        textBoxResult.Text = ""
        Try

            If _activeChannel IsNot Nothing Then
                If _activeChannel.Registers("Brightness") IsNot Nothing Then
                    RemoveHandler _activeChannel.Registers("Brightness").PropertyChanged, AddressOf MainFormBrightness_PropertyChanged
                End If
                If _activeChannel.Registers("PulseDelay") IsNot Nothing Then
                    RemoveHandler _activeChannel.Registers("PulseDelay").PropertyChanged, AddressOf MainFormPulseDelay_PropertyChanged
                End If
                If _activeChannel.Registers("PulseWidth") IsNot Nothing Then
                    RemoveHandler _activeChannel.Registers("PulseWidth").PropertyChanged, AddressOf MainFormPulseWidth_PropertyChanged
                End If
                If _activeChannel.Registers("ChannelMode") IsNot Nothing Then
                    RemoveHandler _activeChannel.Registers("ChannelMode").PropertyChanged, AddressOf MainFormChannelMode_PropertyChanged
                End If
            End If
            ' Unhook from the register we were monitoring
            If _activeController.Channels.Count > 0 Then
                If _activeController.Registers.Contains(Monitor1) Then
                    RemoveHandler _activeController.Registers(Monitor1).PropertyChanged, AddressOf MainFormMonitor1_PropertyChanged
                End If
				If _activeController.Channels(0).Registers.Contains(Monitor2) Then
					RemoveHandler _activeController.Channels(0).Registers(Monitor2).PropertyChanged, AddressOf MainFormMonitor2_PropertyChanged
				End If

				If _activeController.Channels(0).Registers.Contains(Monitor3) Then
					RemoveHandler _activeController.Channels(0).Registers(Monitor3).PropertyChanged, AddressOf MainFormMonitor3_PropertyChanged
				End If
			End If
            _activeController.Close()

        Catch ex As FailedToCloseControllerGardasoftException
            MessageBox.Show(ex.Message, Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


        RemoveHandler _activeController.ConnectionStatusChanged, AddressOf controller_ConnectionStatusChanged
        RemoveHandler _activeController.StatusChanged, AddressOf controller_StatusChanged

        _activeController = Nothing
        _activeChannel = Nothing

        panelControllers.Enabled = True
        BtnClose.Enabled = False
        BtnOpen.Enabled = True

        ResetForm()
        LensControlUIReset()

    End Sub

    Private Sub ResetForm()

        ' Clear databinding
        trackBarBrightness.DataBindings.Clear()
        labelBrightness.DataBindings.Clear()
        trackBarPulseDelay.DataBindings.Clear()
        labelDelay.DataBindings.Clear()
        trackBarPulseWidth.DataBindings.Clear()
        labelLength.DataBindings.Clear()

        Monitor1Value.Text = ""
        Monitor2Value.Text = ""
		Monitor3Value.Visible = False


		trackBarBrightness.Visible = False
		trackBarPulseDelay.Visible = False
		trackBarPulseWidth.Visible = False
		trackBarFocalPower.Visible = False

		trackBarBrightness.Value = 0
        trackBarPulseDelay.Value = 0
        trackBarPulseWidth.Value = 0

        labelBrightness.Text = "0"
        labelDelay.Text = "100"
        labelLength.Text = "100"

        labelMonitor1Unit.Text = "-"
        labelMonitor1.Text = "-----"

        labelMonitor2Unit.Text = "-"
        labelMonitor2.Text = "-----"

        cbMode.SelectedIndex = 0
        cbChannels.Items.Clear()

        BtnExportConfiguration.Enabled = False
        BtnImportConfiguration.Enabled = False
        BtnClose.Enabled = False
        panelChannelCommands.Enabled = False
        panelDirectCommand.Enabled = False
    End Sub


#End Region



    ''' <summary>
    ''' Handles the Click event of the BtnExportConfiguration control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub BtnExportConfiguration_Click(sender As Object, e As EventArgs) Handles BtnExportConfiguration.Click
        Dim sfd As New SaveFileDialog

        If MySettings.Default.LastXMLFile <> String.Empty Then
            sfd.InitialDirectory = Path.GetDirectoryName(MySettings.Default.LastXMLFile)
            sfd.FileName = Path.GetFileName(MySettings.Default.LastXMLFile)
        End If

        sfd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
        sfd.FilterIndex = 0


        If sfd.ShowDialog() = DialogResult.OK Then
            MySettings.Default.LastXMLFile = sfd.FileName
            MySettings.Default.Save()

            Try
                _activeController.ExportConfiguration(sfd.FileName)

                If MessageBox.Show("View File?", "Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Process.Start(sfd.FileName)
                End If

            Catch ex As Exception
                MessageBox.Show("Error when saving XML " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

    End Sub

    ''' <summary>
    ''' Handles the Click event of the BtnImportConfiguration control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    Private Sub BtnImportConfiguration_Click(sender As Object, e As EventArgs) Handles BtnImportConfiguration.Click

        Dim ofd As New OpenFileDialog()
        If MySettings.Default.LastXMLFile <> String.Empty Then
            ofd.InitialDirectory = Path.GetDirectoryName(MySettings.Default.LastXMLFile)
            ofd.FileName = Path.GetFileName(MySettings.Default.LastXMLFile)
        End If

        ofd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
        ofd.FilterIndex = 0

        If ofd.ShowDialog() = DialogResult.OK Then
            MySettings.Default.LastXMLFile = ofd.FileName
            MySettings.Default.Save()
            Try
                _activeController.ImportConfiguration(ofd.FileName)
            Catch ex As Exception
                MessageBox.Show(ex.Message, Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

    End Sub

    ''' <summary>
    ''' Handles the SelectedIndexChanged event of the cbChannels control.
    ''' User has changed channels so update the forms controls
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub cbChannels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbChannels.SelectedIndexChanged
        If _activeChannel IsNot Nothing Then
            If _activeChannel.Registers("Brightness") IsNot Nothing Then
                RemoveHandler _activeChannel.Registers("Brightness").PropertyChanged, AddressOf MainFormBrightness_PropertyChanged
            End If
            If _activeChannel.Registers("PulseDelay") IsNot Nothing Then
                RemoveHandler _activeChannel.Registers("PulseDelay").PropertyChanged, AddressOf MainFormPulseDelay_PropertyChanged
            End If
            If _activeChannel.Registers("PulseWidth") IsNot Nothing Then
                RemoveHandler _activeChannel.Registers("PulseWidth").PropertyChanged, AddressOf MainFormPulseWidth_PropertyChanged
            End If
            If _activeChannel.Registers("ChannelMode") IsNot Nothing Then
                RemoveHandler _activeChannel.Registers("ChannelMode").PropertyChanged, AddressOf MainFormChannelMode_PropertyChanged
            End If
            If _activeChannel.Registers("FocalPowerValue") IsNot Nothing Then
                RemoveHandler _activeChannel.Registers("FocalPowerValue").PropertyChanged, AddressOf MainFormFocalPowerValue_PropertyChanged
            End If
        End If

        _activeChannel = TryCast(cbChannels.SelectedItem, IChannel)
        If _activeChannel IsNot Nothing Then

            Dim isLensControl As Boolean = False
            Dim register As Register = _activeChannel.Registers("ChannelType")

            ' If the register exists the check to see if a lens channel or lighting channel
            If (register IsNot Nothing) Then

                ' If the channel is a Lens channel then subscribe to focal power and enable lens control
                If (Convert.ToInt32(register.CurrentValue) = 1) Then
                    isLensControl = True
                End If
            End If

            If (Not isLensControl) Then
                LensControlUIReset()
                LensControlUIToggle(False)

                If _activeChannel.Registers.Contains("Brightness") Then

                    AddHandler _activeChannel.Registers("Brightness").PropertyChanged, AddressOf MainFormBrightness_PropertyChanged
                    AddHandler _activeChannel.Registers("PulseWidth").PropertyChanged, AddressOf MainFormPulseWidth_PropertyChanged
                    AddHandler _activeChannel.Registers("PulseDelay").PropertyChanged, AddressOf MainFormPulseDelay_PropertyChanged
                    AddHandler _activeChannel.Registers("ChannelMode").PropertyChanged, AddressOf MainFormChannelMode_PropertyChanged

                End If
            Else
                ' Is a lens channel. Now  configure GUI and subscribe to focal power property changed
                LensControlUIToggle(True)
            End If
            'Force update of register values
            _activeChannel.Registers.Refresh()
        End If
    End Sub

    Private Sub LensControlUIReset()
        labelFocalPowerTitle.Text = FocalPowerText + Environment.NewLine + FocalPowerUnits
        trackBarFocalPower.Minimum = 0
        trackBarFocalPower.Maximum = 1500
        trackBarFocalPower.TickFrequency = 100
        trackBarFocalPower.TickStyle = TickStyle.BottomRight
        trackBarFocalPower.Value = 0
        labelFocalPower.Text = "0.00"
    End Sub

    Private Sub LensControlUIToggle(isEnabled As Boolean)
		PnlFocalPower.Visible = isEnabled
		Monitor3Value.Visible = isEnabled
		labelMonitor3.Visible = isEnabled
		labelMonitor3Unit.Visible = isEnabled
		trackBarFocalPower.Visible = isEnabled
		ToggleLightingControls(Not isEnabled)

        ' Get focal power min and max values
        If (_activeChannel IsNot Nothing And isEnabled) Then
            Dim minValue As Single = Convert.ToSingle(_activeChannel.Registers("FocalPowerMin").CurrentValue) * 100.0F
            trackBarFocalPower.Minimum = Convert.ToInt32(minValue)
            Dim maxValue As Single = Convert.ToSingle(_activeChannel.Registers("FocalPowerMax").CurrentValue) * 100.0F
            trackBarFocalPower.Maximum = Convert.ToInt32(maxValue)
            AddHandler _activeChannel.Registers("FocalPowerValue").PropertyChanged, AddressOf MainFormFocalPowerValue_PropertyChanged
            labelFocalPowerTitle.Text = FocalPowerText + Environment.NewLine + FocalPowerUnits + Environment.NewLine + "[" + (minValue / 100.0F).ToString("F") + " to " + (maxValue / 100.0F).ToString("F") + "]"
        End If
    End Sub

    Private Sub ToggleLightingControls(isEnabled As Boolean)
        PnlBrightness.Visible = isEnabled
        label2.Visible = isEnabled
        label3.Visible = isEnabled
        label4.Visible = isEnabled
        label5.Visible = isEnabled
        label6.Visible = isEnabled
        label7.Visible = isEnabled
        label8.Visible = isEnabled

        trackBarBrightness.Visible = isEnabled
        trackBarPulseDelay.Visible = isEnabled
        trackBarPulseWidth.Visible = isEnabled

        labelBrightness.Visible = isEnabled
        labelDelay.Visible = isEnabled
        labelLength.Visible = isEnabled

        If (isEnabled) Then
            cbMode_SelectedIndexChanged(Me, Nothing)
        End If

        cbMode.Visible = isEnabled
    End Sub

    Private Sub MainFormFocalPowerValue_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        If InvokeRequired Then
            BeginInvoke(New PropertyChangedDelegate(AddressOf MainFormFocalPowerValue_PropertyChanged), sender, e)
        Else

            If _activeChannel IsNot Nothing Then
                labelFocalPower.Text = (Convert.ToSingle(_activeChannel.Registers("FocalPowerValue").CurrentValue)).ToString("0.00")
                Dim fpValue As Single = Convert.ToSingle(_activeChannel.Registers("FocalPowerValue").CurrentValue) * 100.0F

                ' If the focal power is out of range then set back to 0
                If (fpValue < trackBarFocalPower.Minimum Or fpValue > trackBarFocalPower.Maximum) Then
                    fpValue = 0.0F
                    _activeChannel.Registers("FocalPowerValue").CurrentValue = fpValue
                End If
                trackBarFocalPower.Value = Convert.ToInt32(fpValue)
            End If
            End If
    End Sub

    ''' <summary>
    ''' Handles the ValueChanged event of the trackBarBrightness control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub trackBarBrightness_ValueChanged(sender As Object, e As EventArgs) Handles trackBarBrightness.ValueChanged
        If _activeChannel IsNot Nothing Then
            If _activeChannel.Registers("Brightness") IsNot Nothing Then
                _activeChannel.Registers("Brightness").CurrentValue = trackBarBrightness.Value
            End If
        End If

    End Sub


    ''' <summary>
    ''' Handles the ValueChanged event of the trackBarPulseDelay control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub trackBarPulseDelay_ValueChanged(sender As Object, e As EventArgs) Handles trackBarPulseDelay.ValueChanged

        If _activeChannel IsNot Nothing Then
            If _activeChannel.Registers("PulseDelay") IsNot Nothing Then
                _activeChannel.Registers("PulseDelay").CurrentValue = trackBarPulseDelay.Value
            End If
        End If
    End Sub


    ''' <summary>
    ''' Handles the ValueChanged event of the trackBarPulseWidth control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    Private Sub trackBarPulseWidth_ValueChanged(sender As Object, e As EventArgs) Handles trackBarPulseWidth.ValueChanged

        If _activeChannel IsNot Nothing Then
            If _activeChannel.Registers("PulseWidth") IsNot Nothing Then
                _activeChannel.Registers("PulseWidth").CurrentValue = trackBarPulseWidth.Value
            End If
        End If
    End Sub



    ''' <summary>
    ''' Handles the SelectedIndexChanged event of the cbMode control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub cbMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbMode.SelectedIndexChanged

        Try
            If DirectCast(cbMode.SelectedItem, String) = "Pulse" Then
                label5.Enabled = True
                label6.Enabled = True
                label7.Enabled = True
                label8.Enabled = True
                labelDelay.Enabled = True
                labelLength.Enabled = True
                trackBarPulseDelay.Enabled = True
                trackBarPulseWidth.Enabled = True
            Else
                label5.Enabled = False
                label6.Enabled = False
                label7.Enabled = False
                label8.Enabled = False
                labelDelay.Enabled = False
                labelLength.Enabled = False
                trackBarPulseDelay.Enabled = False
                trackBarPulseWidth.Enabled = False
            End If

            trackBarBrightness.Maximum = 999

            If _activeChannel IsNot Nothing Then
                If _activeChannel.Registers("ChannelMode") IsNot Nothing Then
                    If DirectCast(cbMode.SelectedItem, String) = "Continuous" Then
                        _activeChannel.Registers("ChannelMode").CurrentValue = Register.ChannelMode.Continuous
                        trackBarBrightness.Maximum = 100
                    ElseIf DirectCast(cbMode.SelectedItem, String) = "Pulse" Then
                        _activeChannel.Registers("ChannelMode").CurrentValue = Register.ChannelMode.Pulse
                    ElseIf DirectCast(cbMode.SelectedItem, String) = "Switched" Then
                        _activeChannel.Registers("ChannelMode").CurrentValue = Register.ChannelMode.Switched
                    ElseIf DirectCast(cbMode.SelectedItem, String) = "Selected" Then
                        _activeChannel.Registers("ChannelMode").CurrentValue = Register.ChannelMode.Selected
                    End If

                    _activeChannel.Registers.Refresh()
                End If
            End If

        Catch exception As Exception
            Trace.WriteLine(exception.Message)

        End Try
    End Sub


    ''' <summary>
    ''' Handles the KeyPress event of the textBoxCommand control.
    ''' Send command to the controller on enter pressed
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
    Private Sub textBoxCommand_KeyPress(sender As Object, e As KeyPressEventArgs)
        If e.KeyChar = CChar(ChrW(Keys.Return)) Then
            e.Handled = True
            btnSend_Click(Me, New EventArgs())
        End If

    End Sub

    ''' <summary>
    ''' Handles the Click event of the btnSend control.
    ''' Sends the command to the controller
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        If _activeController IsNot Nothing Then
            textBoxResult.Text = ""
            Try
                Dim result As String = _activeController.SendCommand(textBoxCommand.Text)
                textBoxResult.Text = result
            Catch ex As Exception
                MessageBox.Show(ex.Message, Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End Try
        End If

    End Sub

    Private Sub trackBarFocalPower_ValueChanged(sender As Object, e As EventArgs) Handles trackBarFocalPower.ValueChanged

        If _activeChannel IsNot Nothing Then
            If _activeChannel.Registers("FocalPowerValue") IsNot Nothing Then
                Dim fpValue As Single = Convert.ToSingle(trackBarFocalPower.Value) / 100.0F
                _activeChannel.Registers("FocalPowerValue").CurrentValue = fpValue
            End If
        End If
    End Sub

    Private Sub BtnOpenWebPage_Click(sender As Object, e As EventArgs) Handles BtnOpenWebPage.Click

        If _activeController IsNot Nothing Then
            Process.Start("http://" + _activeController.IPAddress)
        End If
    End Sub

    Private Sub cbControllers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbControllers.SelectedIndexChanged
        _activeController = _controllerManager.Controllers(cbControllers.SelectedIndex)

        If _activeController IsNot Nothing And Not _activeController.IPAddress.Contains("COM") Then
            ' Set VisualStyleElement.Window.Caption of open web page button and make visible
            BtnOpenWebPage.Text = String.Format("Open web page for {0} with IP {1}", _activeController.Caption, _activeController.IPAddress)
            BtnOpenWebPage.Visible = True
        Else
            BtnOpenWebPage.Text = String.Empty
            BtnOpenWebPage.Visible = False
        End If
    End Sub
End Class
