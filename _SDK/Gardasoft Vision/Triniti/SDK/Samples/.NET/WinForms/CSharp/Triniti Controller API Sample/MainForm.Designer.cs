namespace Gardasoft.Controller.API.CSharp.Sample.WinForms.Sample_1
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.label1 = new System.Windows.Forms.Label();
			this.cbControllers = new System.Windows.Forms.ComboBox();
			this.BtnSearch = new System.Windows.Forms.Button();
			this.panelControllers = new System.Windows.Forms.Panel();
			this.labelOnline = new System.Windows.Forms.Label();
			this.BtnExportConfiguration = new System.Windows.Forms.Button();
			this.BtnClose = new System.Windows.Forms.Button();
			this.panelChannelCommands = new System.Windows.Forms.Panel();
			this.PnlBrightness = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.trackBarBrightness = new System.Windows.Forms.TrackBar();
			this.labelBrightness = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.PnlFocalPower = new System.Windows.Forms.Panel();
			this.trackBarFocalPower = new System.Windows.Forms.TrackBar();
			this.labelFocalPower = new System.Windows.Forms.Label();
			this.labelFocalPowerTitle = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cbChannels = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.trackBarPulseWidth = new System.Windows.Forms.TrackBar();
			this.labelLength = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.trackBarPulseDelay = new System.Windows.Forms.TrackBar();
			this.labelDelay = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cbMode = new System.Windows.Forms.ComboBox();
			this.panelChannelStatus = new System.Windows.Forms.Panel();
			this.labelMonitor2Unit = new System.Windows.Forms.Label();
			this.labelMonitor1Unit = new System.Windows.Forms.Label();
			this.labelMonitor2 = new System.Windows.Forms.Label();
			this.Monitor2Value = new System.Windows.Forms.Label();
			this.labelMonitor1 = new System.Windows.Forms.Label();
			this.Monitor1Value = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.panelCommand = new System.Windows.Forms.Panel();
			this.BtnImportConfiguration = new System.Windows.Forms.Button();
			this.BtnOpen = new System.Windows.Forms.Button();
			this.panelDirectCommand = new System.Windows.Forms.Panel();
			this.textBoxResult = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.textBoxCommand = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.BtnOpenWebPage = new System.Windows.Forms.Button();
			this.Monitor3Value = new System.Windows.Forms.Label();
			this.labelMonitor3 = new System.Windows.Forms.Label();
			this.labelMonitor3Unit = new System.Windows.Forms.Label();
			this.panelControllers.SuspendLayout();
			this.panelChannelCommands.SuspendLayout();
			this.PnlBrightness.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
			this.PnlFocalPower.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarFocalPower)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarPulseWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarPulseDelay)).BeginInit();
			this.panelChannelStatus.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.panelCommand.SuspendLayout();
			this.panelDirectCommand.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(5, 9);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Controllers";
			// 
			// cbControllers
			// 
			this.cbControllers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbControllers.FormattingEnabled = true;
			this.cbControllers.Location = new System.Drawing.Point(75, 8);
			this.cbControllers.Margin = new System.Windows.Forms.Padding(2);
			this.cbControllers.Name = "cbControllers";
			this.cbControllers.Size = new System.Drawing.Size(288, 21);
			this.cbControllers.TabIndex = 1;
			this.cbControllers.SelectedIndexChanged += new System.EventHandler(this.cbControllers_SelectedIndexChanged);
			// 
			// BtnSearch
			// 
			this.BtnSearch.Location = new System.Drawing.Point(374, 5);
			this.BtnSearch.Margin = new System.Windows.Forms.Padding(2);
			this.BtnSearch.Name = "BtnSearch";
			this.BtnSearch.Size = new System.Drawing.Size(62, 27);
			this.BtnSearch.TabIndex = 3;
			this.BtnSearch.Text = "Search";
			this.BtnSearch.UseVisualStyleBackColor = true;
			this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
			// 
			// panelControllers
			// 
			this.panelControllers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelControllers.Controls.Add(this.BtnSearch);
			this.panelControllers.Controls.Add(this.label1);
			this.panelControllers.Controls.Add(this.cbControllers);
			this.panelControllers.Location = new System.Drawing.Point(9, 10);
			this.panelControllers.Margin = new System.Windows.Forms.Padding(2);
			this.panelControllers.Name = "panelControllers";
			this.panelControllers.Size = new System.Drawing.Size(444, 38);
			this.panelControllers.TabIndex = 4;
			// 
			// labelOnline
			// 
			this.labelOnline.BackColor = System.Drawing.Color.Gray;
			this.labelOnline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOnline.Location = new System.Drawing.Point(356, 8);
			this.labelOnline.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelOnline.Name = "labelOnline";
			this.labelOnline.Size = new System.Drawing.Size(81, 61);
			this.labelOnline.TabIndex = 16;
			this.labelOnline.Text = "OFF Line";
			this.labelOnline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BtnExportConfiguration
			// 
			this.BtnExportConfiguration.Enabled = false;
			this.BtnExportConfiguration.Location = new System.Drawing.Point(160, 8);
			this.BtnExportConfiguration.Margin = new System.Windows.Forms.Padding(2);
			this.BtnExportConfiguration.Name = "BtnExportConfiguration";
			this.BtnExportConfiguration.Size = new System.Drawing.Size(136, 32);
			this.BtnExportConfiguration.TabIndex = 6;
			this.BtnExportConfiguration.Text = "Export Configuration...";
			this.BtnExportConfiguration.UseVisualStyleBackColor = true;
			this.BtnExportConfiguration.Click += new System.EventHandler(this.BtnExportConfiguration_Click);
			// 
			// BtnClose
			// 
			this.BtnClose.Enabled = false;
			this.BtnClose.Location = new System.Drawing.Point(76, 8);
			this.BtnClose.Margin = new System.Windows.Forms.Padding(2);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(67, 32);
			this.BtnClose.TabIndex = 5;
			this.BtnClose.Text = "Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
			// 
			// panelChannelCommands
			// 
			this.panelChannelCommands.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelChannelCommands.Controls.Add(this.labelMonitor3Unit);
			this.panelChannelCommands.Controls.Add(this.labelMonitor3);
			this.panelChannelCommands.Controls.Add(this.Monitor3Value);
			this.panelChannelCommands.Controls.Add(this.PnlBrightness);
			this.panelChannelCommands.Controls.Add(this.PnlFocalPower);
			this.panelChannelCommands.Controls.Add(this.label7);
			this.panelChannelCommands.Controls.Add(this.label5);
			this.panelChannelCommands.Controls.Add(this.cbChannels);
			this.panelChannelCommands.Controls.Add(this.label15);
			this.panelChannelCommands.Controls.Add(this.trackBarPulseWidth);
			this.panelChannelCommands.Controls.Add(this.labelLength);
			this.panelChannelCommands.Controls.Add(this.label8);
			this.panelChannelCommands.Controls.Add(this.trackBarPulseDelay);
			this.panelChannelCommands.Controls.Add(this.labelDelay);
			this.panelChannelCommands.Controls.Add(this.label6);
			this.panelChannelCommands.Controls.Add(this.label4);
			this.panelChannelCommands.Controls.Add(this.cbMode);
			this.panelChannelCommands.Enabled = false;
			this.panelChannelCommands.Location = new System.Drawing.Point(9, 134);
			this.panelChannelCommands.Margin = new System.Windows.Forms.Padding(2);
			this.panelChannelCommands.Name = "panelChannelCommands";
			this.panelChannelCommands.Size = new System.Drawing.Size(444, 203);
			this.panelChannelCommands.TabIndex = 5;
			// 
			// PnlBrightness
			// 
			this.PnlBrightness.BackColor = System.Drawing.Color.Transparent;
			this.PnlBrightness.Controls.Add(this.label3);
			this.PnlBrightness.Controls.Add(this.trackBarBrightness);
			this.PnlBrightness.Controls.Add(this.labelBrightness);
			this.PnlBrightness.Controls.Add(this.label2);
			this.PnlBrightness.Location = new System.Drawing.Point(10, 37);
			this.PnlBrightness.Name = "PnlBrightness";
			this.PnlBrightness.Size = new System.Drawing.Size(421, 49);
			this.PnlBrightness.TabIndex = 26;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(404, 4);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 13);
			this.label3.TabIndex = 21;
			this.label3.Text = "%";
			// 
			// trackBarBrightness
			// 
			this.trackBarBrightness.Location = new System.Drawing.Point(97, 3);
			this.trackBarBrightness.Margin = new System.Windows.Forms.Padding(2);
			this.trackBarBrightness.Maximum = 999;
			this.trackBarBrightness.Name = "trackBarBrightness";
			this.trackBarBrightness.Size = new System.Drawing.Size(260, 45);
			this.trackBarBrightness.TabIndex = 20;
			this.trackBarBrightness.TickFrequency = 10;
			this.trackBarBrightness.ValueChanged += new System.EventHandler(this.trackBarBrightness_ValueChanged);
			// 
			// labelBrightness
			// 
			this.labelBrightness.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelBrightness.Location = new System.Drawing.Point(347, 1);
			this.labelBrightness.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelBrightness.Name = "labelBrightness";
			this.labelBrightness.Size = new System.Drawing.Size(54, 19);
			this.labelBrightness.TabIndex = 19;
			this.labelBrightness.Text = "0";
			this.labelBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 16);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 18;
			this.label2.Text = "Brightness";
			// 
			// PnlFocalPower
			// 
			this.PnlFocalPower.BackColor = System.Drawing.Color.Transparent;
			this.PnlFocalPower.Controls.Add(this.trackBarFocalPower);
			this.PnlFocalPower.Controls.Add(this.labelFocalPower);
			this.PnlFocalPower.Controls.Add(this.labelFocalPowerTitle);
			this.PnlFocalPower.Location = new System.Drawing.Point(10, 37);
			this.PnlFocalPower.Name = "PnlFocalPower";
			this.PnlFocalPower.Size = new System.Drawing.Size(421, 49);
			this.PnlFocalPower.TabIndex = 23;
			// 
			// trackBarFocalPower
			// 
			this.trackBarFocalPower.Location = new System.Drawing.Point(97, 3);
			this.trackBarFocalPower.Margin = new System.Windows.Forms.Padding(2);
			this.trackBarFocalPower.Maximum = 999;
			this.trackBarFocalPower.Name = "trackBarFocalPower";
			this.trackBarFocalPower.Size = new System.Drawing.Size(260, 45);
			this.trackBarFocalPower.TabIndex = 25;
			this.trackBarFocalPower.TickFrequency = 10;
			this.trackBarFocalPower.ValueChanged += new System.EventHandler(this.trackBarFocalPower_ValueChanged);
			// 
			// labelFocalPower
			// 
			this.labelFocalPower.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelFocalPower.Location = new System.Drawing.Point(347, 0);
			this.labelFocalPower.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelFocalPower.Name = "labelFocalPower";
			this.labelFocalPower.Size = new System.Drawing.Size(54, 19);
			this.labelFocalPower.TabIndex = 24;
			this.labelFocalPower.Text = "0.00";
			this.labelFocalPower.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFocalPowerTitle
			// 
			this.labelFocalPowerTitle.AutoSize = true;
			this.labelFocalPowerTitle.Location = new System.Drawing.Point(4, 3);
			this.labelFocalPowerTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelFocalPowerTitle.Name = "labelFocalPowerTitle";
			this.labelFocalPowerTitle.Size = new System.Drawing.Size(63, 13);
			this.labelFocalPowerTitle.TabIndex = 23;
			this.labelFocalPowerTitle.Text = "FocalPower";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(416, 164);
			this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(20, 13);
			this.label7.TabIndex = 19;
			this.label7.Text = "ms";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(416, 129);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(20, 13);
			this.label5.TabIndex = 18;
			this.label5.Text = "ms";
			// 
			// cbChannels
			// 
			this.cbChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbChannels.FormattingEnabled = true;
			this.cbChannels.Items.AddRange(new object[] {
            "Continuous",
            "Pulse"});
			this.cbChannels.Location = new System.Drawing.Point(115, 11);
			this.cbChannels.Margin = new System.Windows.Forms.Padding(2);
			this.cbChannels.Name = "cbChannels";
			this.cbChannels.Size = new System.Drawing.Size(116, 21);
			this.cbChannels.TabIndex = 16;
			this.cbChannels.SelectedIndexChanged += new System.EventHandler(this.cbChannels_SelectedIndexChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(14, 13);
			this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(53, 13);
			this.label15.TabIndex = 15;
			this.label15.Text = "Channel";
			// 
			// trackBarPulseWidth
			// 
			this.trackBarPulseWidth.Location = new System.Drawing.Point(107, 164);
			this.trackBarPulseWidth.Margin = new System.Windows.Forms.Padding(2);
			this.trackBarPulseWidth.Maximum = 100;
			this.trackBarPulseWidth.Name = "trackBarPulseWidth";
			this.trackBarPulseWidth.Size = new System.Drawing.Size(260, 45);
			this.trackBarPulseWidth.TabIndex = 14;
			this.trackBarPulseWidth.TickFrequency = 10;
			this.trackBarPulseWidth.ValueChanged += new System.EventHandler(this.trackBarPulseWidth_ValueChanged);
			// 
			// labelLength
			// 
			this.labelLength.Location = new System.Drawing.Point(358, 161);
			this.labelLength.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelLength.Name = "labelLength";
			this.labelLength.Size = new System.Drawing.Size(54, 19);
			this.labelLength.TabIndex = 13;
			this.labelLength.Text = "100";
			this.labelLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(14, 164);
			this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 13);
			this.label8.TabIndex = 12;
			this.label8.Text = "Pulse Width";
			// 
			// trackBarPulseDelay
			// 
			this.trackBarPulseDelay.Location = new System.Drawing.Point(107, 129);
			this.trackBarPulseDelay.Margin = new System.Windows.Forms.Padding(2);
			this.trackBarPulseDelay.Maximum = 100;
			this.trackBarPulseDelay.Name = "trackBarPulseDelay";
			this.trackBarPulseDelay.Size = new System.Drawing.Size(260, 45);
			this.trackBarPulseDelay.TabIndex = 11;
			this.trackBarPulseDelay.TickFrequency = 10;
			this.trackBarPulseDelay.ValueChanged += new System.EventHandler(this.trackBarPulseDelay_ValueChanged);
			// 
			// labelDelay
			// 
			this.labelDelay.Location = new System.Drawing.Point(357, 126);
			this.labelDelay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelDelay.Name = "labelDelay";
			this.labelDelay.Size = new System.Drawing.Size(55, 19);
			this.labelDelay.TabIndex = 10;
			this.labelDelay.Text = "100";
			this.labelDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 129);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(63, 13);
			this.label6.TabIndex = 9;
			this.label6.Text = "Pulse Delay";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 95);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Mode";
			// 
			// cbMode
			// 
			this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMode.FormattingEnabled = true;
			this.cbMode.Items.AddRange(new object[] {
            "Continuous",
            "Pulse"});
			this.cbMode.Location = new System.Drawing.Point(115, 93);
			this.cbMode.Margin = new System.Windows.Forms.Padding(2);
			this.cbMode.Name = "cbMode";
			this.cbMode.Size = new System.Drawing.Size(116, 21);
			this.cbMode.TabIndex = 7;
			this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
			// 
			// panelChannelStatus
			// 
			this.panelChannelStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelChannelStatus.Controls.Add(this.labelMonitor2Unit);
			this.panelChannelStatus.Controls.Add(this.labelOnline);
			this.panelChannelStatus.Controls.Add(this.labelMonitor1Unit);
			this.panelChannelStatus.Controls.Add(this.labelMonitor2);
			this.panelChannelStatus.Controls.Add(this.Monitor2Value);
			this.panelChannelStatus.Controls.Add(this.labelMonitor1);
			this.panelChannelStatus.Controls.Add(this.Monitor1Value);
			this.panelChannelStatus.Controls.Add(this.label9);
			this.panelChannelStatus.Location = new System.Drawing.Point(9, 458);
			this.panelChannelStatus.Margin = new System.Windows.Forms.Padding(2);
			this.panelChannelStatus.Name = "panelChannelStatus";
			this.panelChannelStatus.Size = new System.Drawing.Size(444, 81);
			this.panelChannelStatus.TabIndex = 6;
			// 
			// labelMonitor2Unit
			// 
			this.labelMonitor2Unit.AutoSize = true;
			this.labelMonitor2Unit.Location = new System.Drawing.Point(312, 49);
			this.labelMonitor2Unit.Name = "labelMonitor2Unit";
			this.labelMonitor2Unit.Size = new System.Drawing.Size(10, 13);
			this.labelMonitor2Unit.TabIndex = 25;
			this.labelMonitor2Unit.Text = "-";
			// 
			// labelMonitor1Unit
			// 
			this.labelMonitor1Unit.AutoSize = true;
			this.labelMonitor1Unit.Location = new System.Drawing.Point(312, 15);
			this.labelMonitor1Unit.Name = "labelMonitor1Unit";
			this.labelMonitor1Unit.Size = new System.Drawing.Size(10, 13);
			this.labelMonitor1Unit.TabIndex = 24;
			this.labelMonitor1Unit.Text = "-";
			// 
			// labelMonitor2
			// 
			this.labelMonitor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMonitor2.Location = new System.Drawing.Point(75, 45);
			this.labelMonitor2.Name = "labelMonitor2";
			this.labelMonitor2.Size = new System.Drawing.Size(166, 18);
			this.labelMonitor2.TabIndex = 23;
			this.labelMonitor2.Text = "----";
			this.labelMonitor2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Monitor2Value
			// 
			this.Monitor2Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Monitor2Value.Location = new System.Drawing.Point(247, 42);
			this.Monitor2Value.Name = "Monitor2Value";
			this.Monitor2Value.Size = new System.Drawing.Size(60, 26);
			this.Monitor2Value.TabIndex = 22;
			this.Monitor2Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMonitor1
			// 
			this.labelMonitor1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMonitor1.Location = new System.Drawing.Point(75, 11);
			this.labelMonitor1.Name = "labelMonitor1";
			this.labelMonitor1.Size = new System.Drawing.Size(166, 18);
			this.labelMonitor1.TabIndex = 21;
			this.labelMonitor1.Text = "----";
			this.labelMonitor1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// Monitor1Value
			// 
			this.Monitor1Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Monitor1Value.Location = new System.Drawing.Point(247, 8);
			this.Monitor1Value.Name = "Monitor1Value";
			this.Monitor1Value.Size = new System.Drawing.Size(60, 26);
			this.Monitor1Value.TabIndex = 20;
			this.Monitor1Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(13, 5);
			this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(43, 13);
			this.label9.TabIndex = 15;
			this.label9.Text = "Status";
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip1.Location = new System.Drawing.Point(0, 549);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			this.statusStrip1.Size = new System.Drawing.Size(462, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 7;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			this.toolStripStatusLabel1.Spring = true;
			// 
			// panelCommand
			// 
			this.panelCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelCommand.Controls.Add(this.BtnImportConfiguration);
			this.panelCommand.Controls.Add(this.BtnExportConfiguration);
			this.panelCommand.Controls.Add(this.BtnClose);
			this.panelCommand.Controls.Add(this.BtnOpen);
			this.panelCommand.Location = new System.Drawing.Point(9, 82);
			this.panelCommand.Margin = new System.Windows.Forms.Padding(2);
			this.panelCommand.Name = "panelCommand";
			this.panelCommand.Size = new System.Drawing.Size(444, 48);
			this.panelCommand.TabIndex = 8;
			// 
			// BtnImportConfiguration
			// 
			this.BtnImportConfiguration.Enabled = false;
			this.BtnImportConfiguration.Location = new System.Drawing.Point(300, 8);
			this.BtnImportConfiguration.Margin = new System.Windows.Forms.Padding(2);
			this.BtnImportConfiguration.Name = "BtnImportConfiguration";
			this.BtnImportConfiguration.Size = new System.Drawing.Size(136, 32);
			this.BtnImportConfiguration.TabIndex = 7;
			this.BtnImportConfiguration.Text = "Import Configuration...";
			this.BtnImportConfiguration.UseVisualStyleBackColor = true;
			this.BtnImportConfiguration.Click += new System.EventHandler(this.BtnImportConfiguration_Click);
			// 
			// BtnOpen
			// 
			this.BtnOpen.Location = new System.Drawing.Point(5, 8);
			this.BtnOpen.Margin = new System.Windows.Forms.Padding(2);
			this.BtnOpen.Name = "BtnOpen";
			this.BtnOpen.Size = new System.Drawing.Size(64, 32);
			this.BtnOpen.TabIndex = 4;
			this.BtnOpen.Text = "Open";
			this.BtnOpen.UseVisualStyleBackColor = true;
			this.BtnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
			// 
			// panelDirectCommand
			// 
			this.panelDirectCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelDirectCommand.Controls.Add(this.textBoxResult);
			this.panelDirectCommand.Controls.Add(this.btnSend);
			this.panelDirectCommand.Controls.Add(this.textBoxCommand);
			this.panelDirectCommand.Controls.Add(this.label10);
			this.panelDirectCommand.Enabled = false;
			this.panelDirectCommand.Location = new System.Drawing.Point(9, 342);
			this.panelDirectCommand.Name = "panelDirectCommand";
			this.panelDirectCommand.Size = new System.Drawing.Size(444, 111);
			this.panelDirectCommand.TabIndex = 9;
			// 
			// textBoxResult
			// 
			this.textBoxResult.Location = new System.Drawing.Point(92, 33);
			this.textBoxResult.Multiline = true;
			this.textBoxResult.Name = "textBoxResult";
			this.textBoxResult.ReadOnly = true;
			this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxResult.Size = new System.Drawing.Size(263, 63);
			this.textBoxResult.TabIndex = 19;
			this.textBoxResult.WordWrap = false;
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(361, 3);
			this.btnSend.Margin = new System.Windows.Forms.Padding(2);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(76, 28);
			this.btnSend.TabIndex = 18;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// textBoxCommand
			// 
			this.textBoxCommand.Location = new System.Drawing.Point(92, 7);
			this.textBoxCommand.Name = "textBoxCommand";
			this.textBoxCommand.Size = new System.Drawing.Size(263, 20);
			this.textBoxCommand.TabIndex = 17;
			this.textBoxCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCommand_KeyPress);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(14, 10);
			this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(61, 13);
			this.label10.TabIndex = 16;
			this.label10.Text = "Command";
			// 
			// BtnOpenWebPage
			// 
			this.BtnOpenWebPage.Location = new System.Drawing.Point(9, 54);
			this.BtnOpenWebPage.Margin = new System.Windows.Forms.Padding(2);
			this.BtnOpenWebPage.Name = "BtnOpenWebPage";
			this.BtnOpenWebPage.Size = new System.Drawing.Size(444, 22);
			this.BtnOpenWebPage.TabIndex = 6;
			this.BtnOpenWebPage.UseVisualStyleBackColor = true;
			this.BtnOpenWebPage.Visible = false;
			this.BtnOpenWebPage.Click += new System.EventHandler(this.BtnOpenWebPage_Click);
			// 
			// Monitor3Value
			// 
			this.Monitor3Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Monitor3Value.Location = new System.Drawing.Point(356, 91);
			this.Monitor3Value.Name = "Monitor3Value";
			this.Monitor3Value.Size = new System.Drawing.Size(60, 26);
			this.Monitor3Value.TabIndex = 21;
			this.Monitor3Value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMonitor3
			// 
			this.labelMonitor3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMonitor3.Location = new System.Drawing.Point(236, 95);
			this.labelMonitor3.Name = "labelMonitor3";
			this.labelMonitor3.Size = new System.Drawing.Size(114, 18);
			this.labelMonitor3.TabIndex = 22;
			this.labelMonitor3.Text = "----";
			this.labelMonitor3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMonitor3Unit
			// 
			this.labelMonitor3Unit.AutoSize = true;
			this.labelMonitor3Unit.Location = new System.Drawing.Point(422, 98);
			this.labelMonitor3Unit.Name = "labelMonitor3Unit";
			this.labelMonitor3Unit.Size = new System.Drawing.Size(10, 13);
			this.labelMonitor3Unit.TabIndex = 25;
			this.labelMonitor3Unit.Text = "-";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(462, 571);
			this.Controls.Add(this.BtnOpenWebPage);
			this.Controls.Add(this.panelDirectCommand);
			this.Controls.Add(this.panelCommand);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.panelChannelStatus);
			this.Controls.Add(this.panelChannelCommands);
			this.Controls.Add(this.panelControllers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Triniti Controller API C# Sample";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.panelControllers.ResumeLayout(false);
			this.panelControllers.PerformLayout();
			this.panelChannelCommands.ResumeLayout(false);
			this.panelChannelCommands.PerformLayout();
			this.PnlBrightness.ResumeLayout(false);
			this.PnlBrightness.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
			this.PnlFocalPower.ResumeLayout(false);
			this.PnlFocalPower.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarFocalPower)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarPulseWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarPulseDelay)).EndInit();
			this.panelChannelStatus.ResumeLayout(false);
			this.panelChannelStatus.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panelCommand.ResumeLayout(false);
			this.panelDirectCommand.ResumeLayout(false);
			this.panelDirectCommand.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbControllers;
        private System.Windows.Forms.Button BtnSearch;
        private System.Windows.Forms.Panel panelControllers;
        private System.Windows.Forms.Button BtnExportConfiguration;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Label labelOnline;
        private System.Windows.Forms.Panel panelChannelCommands;
        private System.Windows.Forms.ComboBox cbChannels;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackBarPulseWidth;
        private System.Windows.Forms.Label labelLength;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar trackBarPulseDelay;
        private System.Windows.Forms.Label labelDelay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.Panel panelChannelStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label labelMonitor2;
        private System.Windows.Forms.Label Monitor2Value;
        private System.Windows.Forms.Label labelMonitor1;
        private System.Windows.Forms.Label Monitor1Value;
        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.Button BtnImportConfiguration;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelMonitor2Unit;
        private System.Windows.Forms.Label labelMonitor1Unit;
        private System.Windows.Forms.Button BtnOpen;
        private System.Windows.Forms.Panel panelDirectCommand;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Button BtnOpenWebPage;
        private System.Windows.Forms.Panel PnlBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel PnlFocalPower;
        private System.Windows.Forms.TrackBar trackBarFocalPower;
        private System.Windows.Forms.Label labelFocalPower;
        private System.Windows.Forms.Label labelFocalPowerTitle;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelMonitor3;
		private System.Windows.Forms.Label Monitor3Value;
		private System.Windows.Forms.Label labelMonitor3Unit;
	}
}

