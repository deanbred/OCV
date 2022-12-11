<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
		Me.panelChannelStatus = New System.Windows.Forms.Panel()
		Me.labelMonitor2Unit = New System.Windows.Forms.Label()
		Me.labelOnline = New System.Windows.Forms.Label()
		Me.labelMonitor1Unit = New System.Windows.Forms.Label()
		Me.labelMonitor2 = New System.Windows.Forms.Label()
		Me.Monitor2Value = New System.Windows.Forms.Label()
		Me.labelMonitor1 = New System.Windows.Forms.Label()
		Me.Monitor1Value = New System.Windows.Forms.Label()
		Me.label9 = New System.Windows.Forms.Label()
		Me.label7 = New System.Windows.Forms.Label()
		Me.label5 = New System.Windows.Forms.Label()
		Me.cbChannels = New System.Windows.Forms.ComboBox()
		Me.label15 = New System.Windows.Forms.Label()
		Me.trackBarPulseWidth = New System.Windows.Forms.TrackBar()
		Me.statusStrip1 = New System.Windows.Forms.StatusStrip()
		Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
		Me.PnlBrightness = New System.Windows.Forms.Panel()
		Me.label3 = New System.Windows.Forms.Label()
		Me.trackBarBrightness = New System.Windows.Forms.TrackBar()
		Me.labelBrightness = New System.Windows.Forms.Label()
		Me.label2 = New System.Windows.Forms.Label()
		Me.panelCommand = New System.Windows.Forms.Panel()
		Me.BtnImportConfiguration = New System.Windows.Forms.Button()
		Me.BtnExportConfiguration = New System.Windows.Forms.Button()
		Me.BtnClose = New System.Windows.Forms.Button()
		Me.BtnOpen = New System.Windows.Forms.Button()
		Me.panelDirectCommand = New System.Windows.Forms.Panel()
		Me.textBoxResult = New System.Windows.Forms.TextBox()
		Me.btnSend = New System.Windows.Forms.Button()
		Me.textBoxCommand = New System.Windows.Forms.TextBox()
		Me.label10 = New System.Windows.Forms.Label()
		Me.PnlFocalPower = New System.Windows.Forms.Panel()
		Me.trackBarFocalPower = New System.Windows.Forms.TrackBar()
		Me.labelFocalPower = New System.Windows.Forms.Label()
		Me.labelFocalPowerTitle = New System.Windows.Forms.Label()
		Me.BtnOpenWebPage = New System.Windows.Forms.Button()
		Me.labelLength = New System.Windows.Forms.Label()
		Me.labelDelay = New System.Windows.Forms.Label()
		Me.BtnSearch = New System.Windows.Forms.Button()
		Me.label1 = New System.Windows.Forms.Label()
		Me.cbControllers = New System.Windows.Forms.ComboBox()
		Me.panelControllers = New System.Windows.Forms.Panel()
		Me.panelChannelCommands = New System.Windows.Forms.Panel()
		Me.label8 = New System.Windows.Forms.Label()
		Me.trackBarPulseDelay = New System.Windows.Forms.TrackBar()
		Me.label6 = New System.Windows.Forms.Label()
		Me.label4 = New System.Windows.Forms.Label()
		Me.cbMode = New System.Windows.Forms.ComboBox()
		Me.labelMonitor3Unit = New System.Windows.Forms.Label()
		Me.labelMonitor3 = New System.Windows.Forms.Label()
		Me.Monitor3Value = New System.Windows.Forms.Label()
		Me.panelChannelStatus.SuspendLayout()
		CType(Me.trackBarPulseWidth, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.statusStrip1.SuspendLayout()
		Me.PnlBrightness.SuspendLayout()
		CType(Me.trackBarBrightness, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.panelCommand.SuspendLayout()
		Me.panelDirectCommand.SuspendLayout()
		Me.PnlFocalPower.SuspendLayout()
		CType(Me.trackBarFocalPower, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.panelControllers.SuspendLayout()
		Me.panelChannelCommands.SuspendLayout()
		CType(Me.trackBarPulseDelay, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'panelChannelStatus
		'
		Me.panelChannelStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.panelChannelStatus.Controls.Add(Me.labelMonitor2Unit)
		Me.panelChannelStatus.Controls.Add(Me.labelOnline)
		Me.panelChannelStatus.Controls.Add(Me.labelMonitor1Unit)
		Me.panelChannelStatus.Controls.Add(Me.labelMonitor2)
		Me.panelChannelStatus.Controls.Add(Me.Monitor2Value)
		Me.panelChannelStatus.Controls.Add(Me.labelMonitor1)
		Me.panelChannelStatus.Controls.Add(Me.Monitor1Value)
		Me.panelChannelStatus.Controls.Add(Me.label9)
		Me.panelChannelStatus.Location = New System.Drawing.Point(9, 458)
		Me.panelChannelStatus.Margin = New System.Windows.Forms.Padding(2)
		Me.panelChannelStatus.Name = "panelChannelStatus"
		Me.panelChannelStatus.Size = New System.Drawing.Size(444, 81)
		Me.panelChannelStatus.TabIndex = 12
		'
		'labelMonitor2Unit
		'
		Me.labelMonitor2Unit.AutoSize = True
		Me.labelMonitor2Unit.Location = New System.Drawing.Point(312, 49)
		Me.labelMonitor2Unit.Name = "labelMonitor2Unit"
		Me.labelMonitor2Unit.Size = New System.Drawing.Size(10, 13)
		Me.labelMonitor2Unit.TabIndex = 25
		Me.labelMonitor2Unit.Text = "-"
		'
		'labelOnline
		'
		Me.labelOnline.BackColor = System.Drawing.Color.Gray
		Me.labelOnline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.labelOnline.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.labelOnline.Location = New System.Drawing.Point(356, 8)
		Me.labelOnline.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelOnline.Name = "labelOnline"
		Me.labelOnline.Size = New System.Drawing.Size(81, 61)
		Me.labelOnline.TabIndex = 16
		Me.labelOnline.Text = "OFF Line"
		Me.labelOnline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'labelMonitor1Unit
		'
		Me.labelMonitor1Unit.AutoSize = True
		Me.labelMonitor1Unit.Location = New System.Drawing.Point(312, 15)
		Me.labelMonitor1Unit.Name = "labelMonitor1Unit"
		Me.labelMonitor1Unit.Size = New System.Drawing.Size(10, 13)
		Me.labelMonitor1Unit.TabIndex = 24
		Me.labelMonitor1Unit.Text = "-"
		'
		'labelMonitor2
		'
		Me.labelMonitor2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.labelMonitor2.Location = New System.Drawing.Point(75, 45)
		Me.labelMonitor2.Name = "labelMonitor2"
		Me.labelMonitor2.Size = New System.Drawing.Size(166, 18)
		Me.labelMonitor2.TabIndex = 23
		Me.labelMonitor2.Text = "----"
		Me.labelMonitor2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Monitor2Value
		'
		Me.Monitor2Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.Monitor2Value.Location = New System.Drawing.Point(247, 42)
		Me.Monitor2Value.Name = "Monitor2Value"
		Me.Monitor2Value.Size = New System.Drawing.Size(60, 26)
		Me.Monitor2Value.TabIndex = 22
		Me.Monitor2Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'labelMonitor1
		'
		Me.labelMonitor1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.labelMonitor1.Location = New System.Drawing.Point(75, 11)
		Me.labelMonitor1.Name = "labelMonitor1"
		Me.labelMonitor1.Size = New System.Drawing.Size(166, 18)
		Me.labelMonitor1.TabIndex = 21
		Me.labelMonitor1.Text = "----"
		Me.labelMonitor1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Monitor1Value
		'
		Me.Monitor1Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.Monitor1Value.Location = New System.Drawing.Point(247, 8)
		Me.Monitor1Value.Name = "Monitor1Value"
		Me.Monitor1Value.Size = New System.Drawing.Size(60, 26)
		Me.Monitor1Value.TabIndex = 20
		Me.Monitor1Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'label9
		'
		Me.label9.AutoSize = True
		Me.label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.label9.Location = New System.Drawing.Point(13, 5)
		Me.label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label9.Name = "label9"
		Me.label9.Size = New System.Drawing.Size(43, 13)
		Me.label9.TabIndex = 15
		Me.label9.Text = "Status"
		'
		'label7
		'
		Me.label7.AutoSize = True
		Me.label7.Location = New System.Drawing.Point(416, 164)
		Me.label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label7.Name = "label7"
		Me.label7.Size = New System.Drawing.Size(20, 13)
		Me.label7.TabIndex = 19
		Me.label7.Text = "ms"
		'
		'label5
		'
		Me.label5.AutoSize = True
		Me.label5.Location = New System.Drawing.Point(416, 129)
		Me.label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label5.Name = "label5"
		Me.label5.Size = New System.Drawing.Size(20, 13)
		Me.label5.TabIndex = 18
		Me.label5.Text = "ms"
		'
		'cbChannels
		'
		Me.cbChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbChannels.FormattingEnabled = True
		Me.cbChannels.Items.AddRange(New Object() {"Continuous", "Pulse"})
		Me.cbChannels.Location = New System.Drawing.Point(115, 11)
		Me.cbChannels.Margin = New System.Windows.Forms.Padding(2)
		Me.cbChannels.Name = "cbChannels"
		Me.cbChannels.Size = New System.Drawing.Size(116, 21)
		Me.cbChannels.TabIndex = 16
		'
		'label15
		'
		Me.label15.AutoSize = True
		Me.label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.label15.Location = New System.Drawing.Point(14, 13)
		Me.label15.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label15.Name = "label15"
		Me.label15.Size = New System.Drawing.Size(53, 13)
		Me.label15.TabIndex = 15
		Me.label15.Text = "Channel"
		'
		'trackBarPulseWidth
		'
		Me.trackBarPulseWidth.Location = New System.Drawing.Point(107, 164)
		Me.trackBarPulseWidth.Margin = New System.Windows.Forms.Padding(2)
		Me.trackBarPulseWidth.Maximum = 100
		Me.trackBarPulseWidth.Name = "trackBarPulseWidth"
		Me.trackBarPulseWidth.Size = New System.Drawing.Size(260, 45)
		Me.trackBarPulseWidth.TabIndex = 14
		Me.trackBarPulseWidth.TickFrequency = 10
		'
		'statusStrip1
		'
		Me.statusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
		Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
		Me.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
		Me.statusStrip1.Location = New System.Drawing.Point(0, 549)
		Me.statusStrip1.Name = "statusStrip1"
		Me.statusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 10, 0)
		Me.statusStrip1.Size = New System.Drawing.Size(462, 22)
		Me.statusStrip1.SizingGrip = False
		Me.statusStrip1.TabIndex = 14
		Me.statusStrip1.Text = "statusStrip1"
		'
		'toolStripStatusLabel1
		'
		Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
		Me.toolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
		Me.toolStripStatusLabel1.Spring = True
		'
		'PnlBrightness
		'
		Me.PnlBrightness.BackColor = System.Drawing.Color.Transparent
		Me.PnlBrightness.Controls.Add(Me.label3)
		Me.PnlBrightness.Controls.Add(Me.trackBarBrightness)
		Me.PnlBrightness.Controls.Add(Me.labelBrightness)
		Me.PnlBrightness.Controls.Add(Me.label2)
		Me.PnlBrightness.Location = New System.Drawing.Point(10, 37)
		Me.PnlBrightness.Name = "PnlBrightness"
		Me.PnlBrightness.Size = New System.Drawing.Size(421, 49)
		Me.PnlBrightness.TabIndex = 26
		'
		'label3
		'
		Me.label3.AutoSize = True
		Me.label3.Location = New System.Drawing.Point(406, 3)
		Me.label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(15, 13)
		Me.label3.TabIndex = 21
		Me.label3.Text = "%"
		'
		'trackBarBrightness
		'
		Me.trackBarBrightness.Location = New System.Drawing.Point(97, 3)
		Me.trackBarBrightness.Margin = New System.Windows.Forms.Padding(2)
		Me.trackBarBrightness.Maximum = 999
		Me.trackBarBrightness.Name = "trackBarBrightness"
		Me.trackBarBrightness.Size = New System.Drawing.Size(260, 45)
		Me.trackBarBrightness.TabIndex = 20
		Me.trackBarBrightness.TickFrequency = 10
		'
		'labelBrightness
		'
		Me.labelBrightness.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.labelBrightness.Location = New System.Drawing.Point(347, 0)
		Me.labelBrightness.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelBrightness.Name = "labelBrightness"
		Me.labelBrightness.Size = New System.Drawing.Size(54, 19)
		Me.labelBrightness.TabIndex = 19
		Me.labelBrightness.Text = "0"
		Me.labelBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'label2
		'
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(4, 3)
		Me.label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(56, 13)
		Me.label2.TabIndex = 18
		Me.label2.Text = "Brightness"
		'
		'panelCommand
		'
		Me.panelCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.panelCommand.Controls.Add(Me.BtnImportConfiguration)
		Me.panelCommand.Controls.Add(Me.BtnExportConfiguration)
		Me.panelCommand.Controls.Add(Me.BtnClose)
		Me.panelCommand.Controls.Add(Me.BtnOpen)
		Me.panelCommand.Location = New System.Drawing.Point(9, 82)
		Me.panelCommand.Margin = New System.Windows.Forms.Padding(2)
		Me.panelCommand.Name = "panelCommand"
		Me.panelCommand.Size = New System.Drawing.Size(444, 48)
		Me.panelCommand.TabIndex = 15
		'
		'BtnImportConfiguration
		'
		Me.BtnImportConfiguration.Enabled = False
		Me.BtnImportConfiguration.Location = New System.Drawing.Point(300, 8)
		Me.BtnImportConfiguration.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnImportConfiguration.Name = "BtnImportConfiguration"
		Me.BtnImportConfiguration.Size = New System.Drawing.Size(136, 32)
		Me.BtnImportConfiguration.TabIndex = 7
		Me.BtnImportConfiguration.Text = "Import Configuration..."
		Me.BtnImportConfiguration.UseVisualStyleBackColor = True
		'
		'BtnExportConfiguration
		'
		Me.BtnExportConfiguration.Enabled = False
		Me.BtnExportConfiguration.Location = New System.Drawing.Point(160, 8)
		Me.BtnExportConfiguration.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnExportConfiguration.Name = "BtnExportConfiguration"
		Me.BtnExportConfiguration.Size = New System.Drawing.Size(136, 32)
		Me.BtnExportConfiguration.TabIndex = 6
		Me.BtnExportConfiguration.Text = "Export Configuration..."
		Me.BtnExportConfiguration.UseVisualStyleBackColor = True
		'
		'BtnClose
		'
		Me.BtnClose.Enabled = False
		Me.BtnClose.Location = New System.Drawing.Point(76, 8)
		Me.BtnClose.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnClose.Name = "BtnClose"
		Me.BtnClose.Size = New System.Drawing.Size(67, 32)
		Me.BtnClose.TabIndex = 5
		Me.BtnClose.Text = "Close"
		Me.BtnClose.UseVisualStyleBackColor = True
		'
		'BtnOpen
		'
		Me.BtnOpen.Location = New System.Drawing.Point(5, 8)
		Me.BtnOpen.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnOpen.Name = "BtnOpen"
		Me.BtnOpen.Size = New System.Drawing.Size(64, 32)
		Me.BtnOpen.TabIndex = 4
		Me.BtnOpen.Text = "Open"
		Me.BtnOpen.UseVisualStyleBackColor = True
		'
		'panelDirectCommand
		'
		Me.panelDirectCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.panelDirectCommand.Controls.Add(Me.textBoxResult)
		Me.panelDirectCommand.Controls.Add(Me.btnSend)
		Me.panelDirectCommand.Controls.Add(Me.textBoxCommand)
		Me.panelDirectCommand.Controls.Add(Me.label10)
		Me.panelDirectCommand.Enabled = False
		Me.panelDirectCommand.Location = New System.Drawing.Point(9, 342)
		Me.panelDirectCommand.Name = "panelDirectCommand"
		Me.panelDirectCommand.Size = New System.Drawing.Size(444, 111)
		Me.panelDirectCommand.TabIndex = 16
		'
		'textBoxResult
		'
		Me.textBoxResult.Location = New System.Drawing.Point(92, 33)
		Me.textBoxResult.Multiline = True
		Me.textBoxResult.Name = "textBoxResult"
		Me.textBoxResult.ReadOnly = True
		Me.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me.textBoxResult.Size = New System.Drawing.Size(263, 63)
		Me.textBoxResult.TabIndex = 19
		Me.textBoxResult.WordWrap = False
		'
		'btnSend
		'
		Me.btnSend.Location = New System.Drawing.Point(361, 3)
		Me.btnSend.Margin = New System.Windows.Forms.Padding(2)
		Me.btnSend.Name = "btnSend"
		Me.btnSend.Size = New System.Drawing.Size(76, 28)
		Me.btnSend.TabIndex = 18
		Me.btnSend.Text = "Send"
		Me.btnSend.UseVisualStyleBackColor = True
		'
		'textBoxCommand
		'
		Me.textBoxCommand.Location = New System.Drawing.Point(92, 7)
		Me.textBoxCommand.Name = "textBoxCommand"
		Me.textBoxCommand.Size = New System.Drawing.Size(263, 20)
		Me.textBoxCommand.TabIndex = 17
		'
		'label10
		'
		Me.label10.AutoSize = True
		Me.label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.label10.Location = New System.Drawing.Point(14, 10)
		Me.label10.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label10.Name = "label10"
		Me.label10.Size = New System.Drawing.Size(61, 13)
		Me.label10.TabIndex = 16
		Me.label10.Text = "Command"
		'
		'PnlFocalPower
		'
		Me.PnlFocalPower.BackColor = System.Drawing.Color.Transparent
		Me.PnlFocalPower.Controls.Add(Me.trackBarFocalPower)
		Me.PnlFocalPower.Controls.Add(Me.labelFocalPower)
		Me.PnlFocalPower.Controls.Add(Me.labelFocalPowerTitle)
		Me.PnlFocalPower.Location = New System.Drawing.Point(10, 37)
		Me.PnlFocalPower.Name = "PnlFocalPower"
		Me.PnlFocalPower.Size = New System.Drawing.Size(421, 49)
		Me.PnlFocalPower.TabIndex = 23
		'
		'trackBarFocalPower
		'
		Me.trackBarFocalPower.Location = New System.Drawing.Point(97, 3)
		Me.trackBarFocalPower.Margin = New System.Windows.Forms.Padding(2)
		Me.trackBarFocalPower.Maximum = 999
		Me.trackBarFocalPower.Name = "trackBarFocalPower"
		Me.trackBarFocalPower.Size = New System.Drawing.Size(260, 45)
		Me.trackBarFocalPower.TabIndex = 25
		Me.trackBarFocalPower.TickFrequency = 10
		'
		'labelFocalPower
		'
		Me.labelFocalPower.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.labelFocalPower.Location = New System.Drawing.Point(347, 0)
		Me.labelFocalPower.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelFocalPower.Name = "labelFocalPower"
		Me.labelFocalPower.Size = New System.Drawing.Size(54, 19)
		Me.labelFocalPower.TabIndex = 24
		Me.labelFocalPower.Text = "0.00"
		Me.labelFocalPower.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'labelFocalPowerTitle
		'
		Me.labelFocalPowerTitle.AutoSize = True
		Me.labelFocalPowerTitle.Location = New System.Drawing.Point(4, 3)
		Me.labelFocalPowerTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelFocalPowerTitle.Name = "labelFocalPowerTitle"
		Me.labelFocalPowerTitle.Size = New System.Drawing.Size(63, 13)
		Me.labelFocalPowerTitle.TabIndex = 23
		Me.labelFocalPowerTitle.Text = "FocalPower"
		'
		'BtnOpenWebPage
		'
		Me.BtnOpenWebPage.Location = New System.Drawing.Point(9, 54)
		Me.BtnOpenWebPage.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnOpenWebPage.Name = "BtnOpenWebPage"
		Me.BtnOpenWebPage.Size = New System.Drawing.Size(444, 22)
		Me.BtnOpenWebPage.TabIndex = 13
		Me.BtnOpenWebPage.UseVisualStyleBackColor = True
		Me.BtnOpenWebPage.Visible = False
		'
		'labelLength
		'
		Me.labelLength.Location = New System.Drawing.Point(358, 161)
		Me.labelLength.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelLength.Name = "labelLength"
		Me.labelLength.Size = New System.Drawing.Size(54, 19)
		Me.labelLength.TabIndex = 13
		Me.labelLength.Text = "100"
		Me.labelLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'labelDelay
		'
		Me.labelDelay.Location = New System.Drawing.Point(357, 126)
		Me.labelDelay.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.labelDelay.Name = "labelDelay"
		Me.labelDelay.Size = New System.Drawing.Size(55, 19)
		Me.labelDelay.TabIndex = 10
		Me.labelDelay.Text = "100"
		Me.labelDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'BtnSearch
		'
		Me.BtnSearch.Location = New System.Drawing.Point(374, 5)
		Me.BtnSearch.Margin = New System.Windows.Forms.Padding(2)
		Me.BtnSearch.Name = "BtnSearch"
		Me.BtnSearch.Size = New System.Drawing.Size(62, 27)
		Me.BtnSearch.TabIndex = 3
		Me.BtnSearch.Text = "Search"
		Me.BtnSearch.UseVisualStyleBackColor = True
		'
		'label1
		'
		Me.label1.AutoSize = True
		Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.label1.Location = New System.Drawing.Point(5, 9)
		Me.label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(67, 13)
		Me.label1.TabIndex = 0
		Me.label1.Text = "Controllers"
		'
		'cbControllers
		'
		Me.cbControllers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbControllers.FormattingEnabled = True
		Me.cbControllers.Location = New System.Drawing.Point(75, 8)
		Me.cbControllers.Margin = New System.Windows.Forms.Padding(2)
		Me.cbControllers.Name = "cbControllers"
		Me.cbControllers.Size = New System.Drawing.Size(288, 21)
		Me.cbControllers.TabIndex = 1
		'
		'panelControllers
		'
		Me.panelControllers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.panelControllers.Controls.Add(Me.BtnSearch)
		Me.panelControllers.Controls.Add(Me.label1)
		Me.panelControllers.Controls.Add(Me.cbControllers)
		Me.panelControllers.Location = New System.Drawing.Point(9, 10)
		Me.panelControllers.Margin = New System.Windows.Forms.Padding(2)
		Me.panelControllers.Name = "panelControllers"
		Me.panelControllers.Size = New System.Drawing.Size(444, 38)
		Me.panelControllers.TabIndex = 10
		'
		'panelChannelCommands
		'
		Me.panelChannelCommands.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.panelChannelCommands.Controls.Add(Me.labelMonitor3Unit)
		Me.panelChannelCommands.Controls.Add(Me.PnlBrightness)
		Me.panelChannelCommands.Controls.Add(Me.labelMonitor3)
		Me.panelChannelCommands.Controls.Add(Me.Monitor3Value)
		Me.panelChannelCommands.Controls.Add(Me.PnlFocalPower)
		Me.panelChannelCommands.Controls.Add(Me.label7)
		Me.panelChannelCommands.Controls.Add(Me.label5)
		Me.panelChannelCommands.Controls.Add(Me.cbChannels)
		Me.panelChannelCommands.Controls.Add(Me.label15)
		Me.panelChannelCommands.Controls.Add(Me.trackBarPulseWidth)
		Me.panelChannelCommands.Controls.Add(Me.labelLength)
		Me.panelChannelCommands.Controls.Add(Me.label8)
		Me.panelChannelCommands.Controls.Add(Me.trackBarPulseDelay)
		Me.panelChannelCommands.Controls.Add(Me.labelDelay)
		Me.panelChannelCommands.Controls.Add(Me.label6)
		Me.panelChannelCommands.Controls.Add(Me.label4)
		Me.panelChannelCommands.Controls.Add(Me.cbMode)
		Me.panelChannelCommands.Enabled = False
		Me.panelChannelCommands.Location = New System.Drawing.Point(9, 134)
		Me.panelChannelCommands.Margin = New System.Windows.Forms.Padding(2)
		Me.panelChannelCommands.Name = "panelChannelCommands"
		Me.panelChannelCommands.Size = New System.Drawing.Size(444, 203)
		Me.panelChannelCommands.TabIndex = 11
		'
		'label8
		'
		Me.label8.AutoSize = True
		Me.label8.Location = New System.Drawing.Point(14, 164)
		Me.label8.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label8.Name = "label8"
		Me.label8.Size = New System.Drawing.Size(64, 13)
		Me.label8.TabIndex = 12
		Me.label8.Text = "Pulse Width"
		'
		'trackBarPulseDelay
		'
		Me.trackBarPulseDelay.Location = New System.Drawing.Point(107, 129)
		Me.trackBarPulseDelay.Margin = New System.Windows.Forms.Padding(2)
		Me.trackBarPulseDelay.Maximum = 100
		Me.trackBarPulseDelay.Name = "trackBarPulseDelay"
		Me.trackBarPulseDelay.Size = New System.Drawing.Size(260, 45)
		Me.trackBarPulseDelay.TabIndex = 11
		Me.trackBarPulseDelay.TickFrequency = 10
		'
		'label6
		'
		Me.label6.AutoSize = True
		Me.label6.Location = New System.Drawing.Point(14, 129)
		Me.label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label6.Name = "label6"
		Me.label6.Size = New System.Drawing.Size(63, 13)
		Me.label6.TabIndex = 9
		Me.label6.Text = "Pulse Delay"
		'
		'label4
		'
		Me.label4.AutoSize = True
		Me.label4.Location = New System.Drawing.Point(14, 95)
		Me.label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
		Me.label4.Name = "label4"
		Me.label4.Size = New System.Drawing.Size(34, 13)
		Me.label4.TabIndex = 8
		Me.label4.Text = "Mode"
		'
		'cbMode
		'
		Me.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbMode.FormattingEnabled = True
		Me.cbMode.Items.AddRange(New Object() {"Continuous", "Pulse"})
		Me.cbMode.Location = New System.Drawing.Point(115, 93)
		Me.cbMode.Margin = New System.Windows.Forms.Padding(2)
		Me.cbMode.Name = "cbMode"
		Me.cbMode.Size = New System.Drawing.Size(116, 21)
		Me.cbMode.TabIndex = 7
		'
		'labelMonitor3Unit
		'
		Me.labelMonitor3Unit.AutoSize = True
		Me.labelMonitor3Unit.Location = New System.Drawing.Point(426, 99)
		Me.labelMonitor3Unit.Name = "labelMonitor3Unit"
		Me.labelMonitor3Unit.Size = New System.Drawing.Size(10, 13)
		Me.labelMonitor3Unit.TabIndex = 27
		Me.labelMonitor3Unit.Text = "-"
		'
		'labelMonitor3
		'
		Me.labelMonitor3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.labelMonitor3.Location = New System.Drawing.Point(247, 95)
		Me.labelMonitor3.Name = "labelMonitor3"
		Me.labelMonitor3.Size = New System.Drawing.Size(108, 18)
		Me.labelMonitor3.TabIndex = 26
		Me.labelMonitor3.Text = "----"
		Me.labelMonitor3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Monitor3Value
		'
		Me.Monitor3Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.Monitor3Value.Location = New System.Drawing.Point(361, 92)
		Me.Monitor3Value.Name = "Monitor3Value"
		Me.Monitor3Value.Size = New System.Drawing.Size(60, 26)
		Me.Monitor3Value.TabIndex = 25
		Me.Monitor3Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(462, 571)
		Me.Controls.Add(Me.panelChannelStatus)
		Me.Controls.Add(Me.statusStrip1)
		Me.Controls.Add(Me.panelCommand)
		Me.Controls.Add(Me.panelDirectCommand)
		Me.Controls.Add(Me.BtnOpenWebPage)
		Me.Controls.Add(Me.panelControllers)
		Me.Controls.Add(Me.panelChannelCommands)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Margin = New System.Windows.Forms.Padding(2)
		Me.MaximizeBox = False
		Me.Name = "MainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Triniti Controller API VB Sample"
		Me.panelChannelStatus.ResumeLayout(False)
		Me.panelChannelStatus.PerformLayout()
		CType(Me.trackBarPulseWidth, System.ComponentModel.ISupportInitialize).EndInit()
		Me.statusStrip1.ResumeLayout(False)
		Me.statusStrip1.PerformLayout()
		Me.PnlBrightness.ResumeLayout(False)
		Me.PnlBrightness.PerformLayout()
		CType(Me.trackBarBrightness, System.ComponentModel.ISupportInitialize).EndInit()
		Me.panelCommand.ResumeLayout(False)
		Me.panelDirectCommand.ResumeLayout(False)
		Me.panelDirectCommand.PerformLayout()
		Me.PnlFocalPower.ResumeLayout(False)
		Me.PnlFocalPower.PerformLayout()
		CType(Me.trackBarFocalPower, System.ComponentModel.ISupportInitialize).EndInit()
		Me.panelControllers.ResumeLayout(False)
		Me.panelControllers.PerformLayout()
		Me.panelChannelCommands.ResumeLayout(False)
		Me.panelChannelCommands.PerformLayout()
		CType(Me.trackBarPulseDelay, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents panelChannelStatus As System.Windows.Forms.Panel
    Private WithEvents labelMonitor2Unit As System.Windows.Forms.Label
    Private WithEvents labelOnline As System.Windows.Forms.Label
    Private WithEvents labelMonitor1Unit As System.Windows.Forms.Label
    Private WithEvents labelMonitor2 As System.Windows.Forms.Label
    Private WithEvents Monitor2Value As System.Windows.Forms.Label
    Private WithEvents labelMonitor1 As System.Windows.Forms.Label
    Private WithEvents Monitor1Value As System.Windows.Forms.Label
    Private WithEvents label9 As System.Windows.Forms.Label
    Private WithEvents label7 As System.Windows.Forms.Label
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents cbChannels As System.Windows.Forms.ComboBox
    Private WithEvents label15 As System.Windows.Forms.Label
    Private WithEvents trackBarPulseWidth As System.Windows.Forms.TrackBar
    Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
    Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents PnlBrightness As System.Windows.Forms.Panel
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents trackBarBrightness As System.Windows.Forms.TrackBar
    Private WithEvents labelBrightness As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents panelCommand As System.Windows.Forms.Panel
    Private WithEvents BtnImportConfiguration As System.Windows.Forms.Button
    Private WithEvents BtnExportConfiguration As System.Windows.Forms.Button
    Private WithEvents BtnClose As System.Windows.Forms.Button
    Private WithEvents BtnOpen As System.Windows.Forms.Button
    Private WithEvents panelDirectCommand As System.Windows.Forms.Panel
    Private WithEvents textBoxResult As System.Windows.Forms.TextBox
    Private WithEvents btnSend As System.Windows.Forms.Button
    Private WithEvents textBoxCommand As System.Windows.Forms.TextBox
    Private WithEvents label10 As System.Windows.Forms.Label
    Private WithEvents PnlFocalPower As System.Windows.Forms.Panel
    Private WithEvents trackBarFocalPower As System.Windows.Forms.TrackBar
    Private WithEvents labelFocalPower As System.Windows.Forms.Label
    Private WithEvents labelFocalPowerTitle As System.Windows.Forms.Label
    Private WithEvents BtnOpenWebPage As System.Windows.Forms.Button
    Private WithEvents labelLength As System.Windows.Forms.Label
    Private WithEvents labelDelay As System.Windows.Forms.Label
    Private WithEvents BtnSearch As System.Windows.Forms.Button
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents cbControllers As System.Windows.Forms.ComboBox
    Private WithEvents panelControllers As System.Windows.Forms.Panel
    Private WithEvents panelChannelCommands As System.Windows.Forms.Panel
    Private WithEvents label8 As System.Windows.Forms.Label
    Private WithEvents trackBarPulseDelay As System.Windows.Forms.TrackBar
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents cbMode As System.Windows.Forms.ComboBox
	Private WithEvents labelMonitor3Unit As Label
	Private WithEvents labelMonitor3 As Label
	Private WithEvents Monitor3Value As Label
End Class
