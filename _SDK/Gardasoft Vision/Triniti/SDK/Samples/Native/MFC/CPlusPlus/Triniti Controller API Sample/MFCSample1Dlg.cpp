//-----------------------------------------------------------------------------------------------
// <copyright file="MFCSample1Dlg.cpp" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------

#include "stdafx.h"
#include "MFCSample1.h"
#include "MFCSample1Dlg.h"
#include "afxdialogex.h"

#include "TrinitiAPI.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace std;
using namespace GardasoftControllerManagementAPINativeWrapper;

static UINT BASED_CODE indicators[] =
{
	ID_STATUSPANEMESSAGE
};

/// <summary>
/// Initializes a new instance of the <see cref="CMFCSample1Dlg"/> class.
/// </summary>
/// <param name="pParent">The p parent.</param>
CMFCSample1Dlg::CMFCSample1Dlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CMFCSample1Dlg::IDD, pParent)
	
{
	m_hIcon = AfxGetApp()->LoadIcon(IDI_TRINITI);
	activeController = NULL;
	controllerManager = NULL;
	activeChannel = NULL;
	brightness = NULL;
	pulseLength = NULL;
	pulseDelay = NULL;
	mControllerStatus = CS_DISCONNECTED;
	mHealthy = false;
}

void CMFCSample1Dlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBOCONTROLLER, cbControllerList);
	DDX_Control(pDX, IDC_COMBOMODE, cbChannelMode);
	DDX_Control(pDX, IDC_COMBOCHANNEL, cbChannelList);
	DDX_Control(pDX, IDC_STATICONLINE, labelOnline);
	DDX_Control(pDX, IDC_SLIDERBRIGHTNESS, sliderBrightness);
	DDX_Control(pDX, IDC_SLIDERPULSEDELAY, sliderPulseDelay);
	DDX_Control(pDX, IDC_SLIDERPULSELENGTH, sliderPulseLength);
	DDX_Control(pDX, IDC_STATICBRIGHTNESS, labelBrightness);
	DDX_Control(pDX, IDC_STATICPULSEDELAY, labelPulseDelay);
	DDX_Control(pDX, IDC_STATICPULSELENGTH, labelPulseLength);
	DDX_Control(pDX, IDC_STATICMONITORVALUE1, labelControllerTemperature);
	DDX_Control(pDX, IDC_STATICMONITORVALUE2, labelChannelLoadPower);
	DDX_Control(pDX, IDC_STATICMONITOR1, labelMonitor1);
	DDX_Control(pDX, IDC_STATICMONITOR2, labelMonitor2);
	DDX_Control(pDX, IDC_STATICMONITORUNIT1, labelMonitor1Unit);
	DDX_Control(pDX, IDC_STATICMONITORUNIT2, labelMointor2Unit);
	
}

BEGIN_MESSAGE_MAP(CMFCSample1Dlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_MESSAGE( WM_APP,OnUser)
	ON_BN_CLICKED(IDC_MFCBUTTONSEARCH, &CMFCSample1Dlg::OnBnClickedMfcbuttonsearch)
	ON_BN_CLICKED(IDC_MFCBUTTONOPEN, &CMFCSample1Dlg::OnBnClickedMfcbuttonopen)
	ON_BN_CLICKED(IDC_MFCBUTTONEXPORT, &CMFCSample1Dlg::OnBnClickedMfcbuttonexport)
	ON_BN_CLICKED(IDC_MFCBUTTONIMPORT, &CMFCSample1Dlg::OnBnClickedMfcbuttonimport)
	ON_WM_CTLCOLOR()
	ON_BN_CLICKED(IDC_MFCBUTTONCLOSE, &CMFCSample1Dlg::OnBnClickedMfcbuttonclose)
	ON_CBN_SELCHANGE(IDC_COMBOMODE, &CMFCSample1Dlg::OnCbnSelchangeCombomode)
	ON_CBN_SELCHANGE(IDC_COMBOCHANNEL, &CMFCSample1Dlg::OnCbnSelchangeCombochannel)
	ON_WM_DESTROY()
	ON_BN_CLICKED(IDC_MFCBUTTONSEND, &CMFCSample1Dlg::OnBnClickedMfcbuttonsend)
	ON_WM_HSCROLL()
	ON_CBN_SELCHANGE(IDC_COMBOCONTROLLER, &CMFCSample1Dlg::OnCbnSelchangeCombocontroller)
	ON_BN_CLICKED(IDC_MFCBTNWEBPAGE, &CMFCSample1Dlg::OnBnClickedMfcbtnwebpage)
END_MESSAGE_MAP()


#pragma region Message Handlers

/// <summary>
/// Called to initialise the application dialog
/// </summary>
/// <returns></returns>
BOOL CMFCSample1Dlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Create and configure the status bar
	statusBar.Create(this);
	statusBar.GetStatusBarCtrl().SetBkColor(RGB(180, 180, 180));
	statusBar.SetIndicators(indicators, 1);
	statusBar.SetPaneInfo(0, ID_STATUSPANEMESSAGE, SBPS_STRETCH, 0);
	RepositionBars(AFX_IDW_CONTROLBAR_FIRST, AFX_IDW_CONTROLBAR_LAST,
		ID_STATUSPANEMESSAGE);

	// Create some brushes for the online status indicator
	DWORD d = GetSysColor(COLOR_BTNFACE);
	COLORREF normal = RGB(GetRValue(d), GetGValue(d), GetBValue(d));
	mYellowBrush = (HBRUSH)CreateSolidBrush(RGB(255, 255, 0));
	mGreenBrush = (HBRUSH)CreateSolidBrush(RGB(0, 255, 0));
	mNormalBrush = (HBRUSH)CreateSolidBrush(normal);

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// Set the items for the mode selection dialog
	int index = cbChannelMode.AddString(_T("Continuous"));
	cbChannelMode.SetItemData(index, CM_CONTINUOUS);

	index = cbChannelMode.AddString(_T("Pulse"));
	cbChannelMode.SetItemData(index, CM_PULSE);

	index = cbChannelMode.AddString(_T("Switched"));
	cbChannelMode.SetItemData(index, CM_SWITCHED);

	index = cbChannelMode.AddString(_T("Selected"));
	cbChannelMode.SetItemData(index, CM_SELECTED);


	GetDlgItem(IDC_EDITCOMMAND)->SetWindowText(_T(""));


	sliderPulseLength.SetTicFreq(10);
	sliderPulseDelay.SetTicFreq(10);
	sliderBrightness.SetTicFreq(10);

	// Initialise Triniti API

	controllerManager = new CControllerManager();
	controllerManager->Initialise();
	controllerManager->AddNotificationHandler(this);

	// Update caption bar to show version info
	CString captionString;
	captionString.LoadString(AFX_IDS_APP_TITLE);
	CString versionString (controllerManager->GetAPIInfo()->version);
	captionString.Append(_T(" v"));
	captionString.Append(versionString);

	SetWindowText(captionString);

	DiscoverControllers();

	UpdateGUI();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMFCSample1Dlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Centre icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMFCSample1Dlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



/// <summary>
/// Called when search button clicked.
/// Search for controllers and populate the comboBox
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonsearch()
{
	DiscoverControllers();
	UpdateGUI();
}

/// <summary>
/// Called when controller open button clicked.
/// Opens the currently selected controller
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonopen()
{
	if (cbControllerList.GetCount() > 0)
	{
		CWaitCursor waitCursor;

		activeController = (CController*)cbControllerList.GetItemDataPtr(cbControllerList.GetCurSel());

		if (activeController != NULL)
		{
			activeController->AddNotificationHandler(this);
			if (activeController->Open())
			{

				if (activeController->GetIsTriniti())
				{	// Only intitialise these controls if it is a triniti  controller
					// Populate the list of available channels
					for (int i = 0; i < activeController->GetChannelCount(); ++i)
					{

						CString caption;
						caption.Format(_T("Channel %d"), (i + 1));
						int index = cbChannelList.AddString(caption);
						cbChannelList.SetItemData(index, i);
					}

					if (cbChannelList.GetCount() > 0)
					{
						TRACE("SELECT CHANNEL\n");
						cbChannelList.SetCurSel(0);			// Set first channel as default
						OnCbnSelchangeCombochannel();
					}
					else
						TRACE("!!NO CHANNELS\n");


					// Set up controller register that we want to monitor
					controllerTemperature = (CRegisterFloat*)activeController->GetRegisterByName(_T("ControllerTemperature"), TRT_FLOAT);
					channelLoadPower = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("ChannelLoadPower"), TRT_FLOAT);

					controllerTemperature->AddNotificationHandler(this);
					labelMonitor1.SetWindowTextW(controllerTemperature->GetCaption());
					labelMonitor1Unit.SetWindowTextW(controllerTemperature->GetUserUnit()->GetCaption());
					UpdateControllerTemperatureControls();
				}
				TRACE("CONTROLLER OPEN\n");
			}
			else
			{
				LastError *error = activeController->GetLastError();
				AfxMessageBox(error->message, MB_ICONERROR | MB_OK);
				CloseController();
			}
		}
	}
	UpdateGUI();
}


/// <summary>
/// Called when export button clicked.
/// Allows the user to specify a destination file and save the controller configuration
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonexport()
{
	if (activeController != NULL)
	{
		char strFilter[] = { "Configuration Files (*.xml)|*.xml|" };


		CFileDialog FileDlg(FALSE, CString(".xml"), NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, CString(strFilter));

		if (FileDlg.DoModal() == IDOK)
		{
			bool success = activeController->ExportConfiguration((LPTSTR)(LPCTSTR)FileDlg.GetPathName());
			if (!success)
			{
				LastError* lastError = activeController->GetLastError();
				AfxMessageBox(lastError->message, MB_ICONERROR | MB_OK);

			}
		}
	}
}

/// <summary>
/// Called when import button clicked].
/// Allows the user to load a configuration file and load into the currently open controller
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonimport()
{
	if (activeController != NULL)
	{
		char strFilter[] = { "Configuration Files (*.xml)|*.xml|" };


		CFileDialog FileDlg(TRUE, CString(".xml"), NULL, OFN_HIDEREADONLY, CString(strFilter));

		if (FileDlg.DoModal() == IDOK)
		{
			bool success = activeController->ImportConfiguration((LPTSTR)(LPCTSTR)FileDlg.GetPathName());

			if (!success)
			{
				LastError* lastError = activeController->GetLastError();
				AfxMessageBox(lastError->message, MB_ICONERROR | MB_OK);
			}

		}
	}
}

/// <summary>
/// Overridden to allow us to set background colour of controls
/// </summary>
/// <param name="pDC">The p dc.</param>
/// <param name="pWnd">The p WND.</param>
/// <param name="nCtlColor">Colour of the n control.</param>
/// <returns></returns>
HBRUSH CMFCSample1Dlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = __super::OnCtlColor(pDC, pWnd, nCtlColor);

	if (nCtlColor == CTLCOLOR_STATIC && pWnd->GetDlgCtrlID() == IDC_STATICONLINE)
	{
		DWORD d = GetSysColor(COLOR_BTNFACE);
		COLORREF normal = RGB(GetRValue(d), GetGValue(d), GetBValue(d));
		COLORREF red = RGB(255, 0, 0);

		switch (mControllerStatus)
		{
		case CS_CONNECTING:
			pDC->SetBkColor(RGB(255, 255, 0));
			hbr = mYellowBrush;
			break;
		case CS_CONNECTED:

			if (mHealthy)
			{
				pDC->SetBkColor(RGB(0, 255, 0));
				hbr = mGreenBrush;
			}
			else
			{
				pDC->SetBkColor(RGB(255, 255, 0));
				hbr = mYellowBrush;

			}
			break;
		default:
			pDC->SetBkColor(normal);
			hbr = mNormalBrush; 
			break;
		}
	}
	return hbr;
}


/// <summary>
/// Called when close controller button is clicked
/// Closes the currently open controller
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonclose()
{
	CloseController();

	UpdateGUI();
}

/// <summary>
/// Called when user changes the channel mode
/// </summary>
void CMFCSample1Dlg::OnCbnSelchangeCombomode()
{
	int mode = cbChannelMode.GetCurSel();
	channelMode->SetCurrentValue(cbChannelMode.GetCurSel());

	// Adjust range depending on mode
	sliderBrightness.SetRangeMax(mode==0?100:1000, true);
	sliderBrightness.SetTicFreq(10);

	UpdateGUI();
}

/// <summary>
/// Called when user changes the active channel
/// </summary>
void CMFCSample1Dlg::OnCbnSelchangeCombochannel()
{
	ReleaseChannel();

	// Acquire register objects that we are interested in. Note we need to delete these when no longer needed
	activeChannel = activeController->GetChannelByIndex(cbChannelList.GetCurSel() );
	brightness = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("Brightness"), TRT_FLOAT);
	pulseLength = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("PulseWidth"), TRT_FLOAT);
	pulseDelay = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("PulseDelay"), TRT_FLOAT);
	channelMode = (CRegisterInteger*)activeChannel->GetRegisterByName(_T("ChannelMode"), TRT_INTEGER);
	focalPowerValue = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("FocalPowerValue"), TRT_FLOAT);



	// Setup the registers we want to monitor in the status area of the sample application
	channelLoadPower = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("ChannelLoadPower"), TRT_FLOAT);


	channelLoadPower->AddNotificationHandler(this);
	labelMonitor2.SetWindowTextW(channelLoadPower->GetCaption());
	labelMointor2Unit.SetWindowTextW(channelLoadPower->GetUserUnit()->GetCaption());
	UpdateChannelLoadPowerControls();


	// See if we support lens control
	if (activeChannel != NULL)
	{
		bool isLensChannel = false;
		
		CRegisterInteger *channelTypeReg = (CRegisterInteger*)activeChannel->GetRegisterByName(_T("ChannelType"), TRT_INTEGER);

		// If the register exists the check to see if a lens channel or lighting channel
		if (channelTypeReg != NULL)
		{
			// If the channel is a Lens channel then subscribe to focal power and enable lens control
			int channelType = 0;
			channelTypeReg->GetCurrentValue(&channelType);
			if (channelType == 1)
			{
				isLensChannel = true;
				// Add notification handlers so that we are called when the register values change
				if (focalPowerValue != NULL)
				{
					focalPowerValue->AddNotificationHandler(this);
					UpdateFocalPowerControls();
				}
			}
			delete channelTypeReg;
			channelTypeReg = NULL;
		}

		if (!isLensChannel)
		{
			if (focalPowerValue != NULL)
			{
				delete focalPowerValue;
				focalPowerValue = NULL;
			}
			LensControlUIToggle(false);
			if (brightness!=NULL)
			{
				brightness->AddNotificationHandler(this);
				UpdateBrightnessControls();
				pulseLength->AddNotificationHandler(this);
				UpdatePulseLengthControls();
				pulseDelay->AddNotificationHandler(this);
				UpdatePulseDelayControls();
				channelMode->AddNotificationHandler(this);
				UpdateChannelModeControl();
			}
		}
		// Is a lens channel. Now  configure GUI and subscribe to focal power property changed
		else
		{
			LensControlUIToggle(true);
		}		
	}
}


/// <summary>
/// Called when the application is closing
/// Delete any used resources
/// </summary>
void CMFCSample1Dlg::OnDestroy()
{
	__super::OnDestroy();
	CloseController();
	if (controllerManager != NULL)
	{
		controllerManager->RemoveNotificationHandler(this);
		delete controllerManager;
		controllerManager = NULL;
	}

	::DeleteObject(mNormalBrush);
	::DeleteObject(mYellowBrush);
	::DeleteObject(mGreenBrush);

}


#pragma endregion

#pragma region Triniti Notifacation Handlers

/// <summary>
/// Called when the controller manager reports a change to the discovery status
/// </summary>
/// <param name="controllerManager">Pointer to the ControllerManager that raised the event.</param>
/// <param name="deviceDiscoveryStatus">The device discovery status.</param>
void CMFCSample1Dlg::OnDeviceDiscoveryStatusChanged(CControllerManager *controllerManager, DeviceDiscoveryStatus deviceDiscoveryStatus)
{
	TRACE("CMFCSample1Dlg::OnDeviceDiscoveryStatusChanged");
}


/// <summary>
/// Called when a register value has changed
/// </summary>
/// <param name="reg">The reg.</param>
void CMFCSample1Dlg::OnPropertyChanged(CRegister* reg)
{
	if (wcscmp(reg->GetName(), _T("Brightness")) == 0)
	{
		UpdateBrightnessControls();
	}
	else if (wcscmp(reg->GetName(), _T("PulseDelay")) == 0)
	{
		UpdatePulseDelayControls();
	}
	else if (wcscmp(reg->GetName(), _T("PulseWidth")) == 0)
	{
		UpdatePulseLengthControls();
	}
	else if (wcscmp(reg->GetName(), _T("ChannelMode")) == 0)
	{
		UpdateChannelModeControl();
	}
	else if (wcscmp(reg->GetName(), _T("ControllerTemperature")) == 0)
	{
		UpdateControllerTemperatureControls();
	}
	else if (wcscmp(reg->GetName(), _T("ChannelLoadPower")) == 0)
	{
		UpdateChannelLoadPowerControls();
	}
	else if (wcscmp(reg->GetName(), _T("FocalPowerValue")) == 0)
	{
		UpdateFocalPowerControls();
	}

}


/// <summary>
/// Called when the active controllers status has changed.
/// </summary>
/// <param name="controller">The controller.</param>
/// <param name="controllerStatus">The controller status.</param>
void CMFCSample1Dlg::OnStatusChanged(CController* controller, ControllerStatus controllerStatus)
{
	mControllerStatus = controllerStatus;
	CString strMessage = controller->GetCaption();

	strMessage.Append(_T(" Controller "));

	switch (mControllerStatus)
	{
	case CS_CONNECTING:
		strMessage.Append(_T("Connecting"));
		break;
	case CS_CONNECTED:
		strMessage.Append(_T("Connected"));
		break;
	default:
		strMessage.Append(_T("Disconnected"));
		break;
	}

	// We could be called from a non UI thread so we need to do this
	HWND hWnd = GetSafeHwnd();
	CString *pString = new CString(strMessage);
	::SendMessage(hWnd, WM_APP, 0, reinterpret_cast<LPARAM>(pString));
}


/// <summary>
/// Called when the controller connection status changes
/// </summary>
/// <param name="controller">The controller.</param>
/// <param name="controllerConnectionStatus">The controller connection status.</param>
void CMFCSample1Dlg::OnConnectionStatusChanged(CController* controller, ControllerConnectionStatus controllerConnectionStatus)
{
	CString strMessage = controller->GetCaption();

	strMessage.Append(_T(" Controller Connection "));

	switch (controllerConnectionStatus)
	{
	case CCS_HEALTHY:
		strMessage.Append(_T("Healthy"));
		mHealthy = true;
		break;
	case CCS_FAULT:
		strMessage.Append(_T("Fault"));
		mHealthy = false;
		break;
	default:
		strMessage.Append(_T("Unknown"));
		break;
	}

	// We could be called from a non UI thread so we need to do this
	HWND hWnd = GetSafeHwnd();
	CString *pString = new CString(strMessage);
	::SendMessage(hWnd, WM_APP, 0, reinterpret_cast<LPARAM>(pString));
}

/// <summary>
/// Allows us to set the status bar text from non UI thread.
/// </summary>
/// <param name="">The .</param>
/// <param name="lParam">The l parameter.</param>
/// <returns></returns>
LRESULT CMFCSample1Dlg::OnUser(WPARAM, LPARAM lParam)
{
	CString* pString = reinterpret_cast<CString*>(lParam);
	ASSERT(pString != NULL);

	statusBar.SetPaneText(0, *pString);

	Invalidate();
	UpdateWindow();
	UpdateGUI();

	delete pString;
	return 0;
}

#pragma endregion

#pragma region Implementation

/// <summary>
/// Called to discover all available controllers and updates the
/// list on the user interface
/// </summary>
void CMFCSample1Dlg::DiscoverControllers()
{
	cbControllerList.ResetContent();

	if (controllerManager != NULL)
	{
		controllerManager->DiscoverControllers();
		int controllerCount = controllerManager->GetControllerCount();

		// Update the list of available controllers	
		for (int i = 0; i < controllerCount; i++)
		{
			CController* controller = controllerManager->GetController(i);
			int index = cbControllerList.AddString(controller->GetCaption());
			// Save the pointer to the controller in the combo box.
			// We can do this because the controller objects persist in the ControllerManager
			cbControllerList.SetItemDataPtr(index, (void *)controller);								
		}

		if (controllerCount > 0)
		{
			cbControllerList.SetCurSel(0);	// Select the first controller in the list
			OnCbnSelchangeCombocontroller();
		}
		
	}
}

/// <summary>
/// Closes the currently open controller and releases
/// any associated resources
/// </summary>
void CMFCSample1Dlg::CloseController()
{
	if (activeController != NULL)
	{
		ReleaseChannel();
		cbChannelList.ResetContent();

		if (controllerTemperature != NULL)
		{
			controllerTemperature->RemoveNotificationHandler(this);
			delete controllerTemperature;
			controllerTemperature = NULL;
		}
		
		labelMonitor1.SetWindowTextW(_T("----"));
		labelMonitor1Unit.SetWindowTextW(_T("-"));
		labelControllerTemperature.SetWindowTextW(_T(""));
		GetDlgItem(IDC_EDITRESULT)->SetWindowText(_T(""));

		activeController->Close();
		activeController->RemoveNotificationHandler(this);
		activeController = NULL;
	}
}

/// <summary>
/// Releases the currently active channel and update the user interface.
/// </summary>
void CMFCSample1Dlg::ReleaseChannel()
{
	if (activeChannel != NULL)
	{	// Release any active channel registers
		if (brightness != NULL)
		{
			brightness->RemoveNotificationHandler(this);
			delete brightness;
			brightness = NULL;
		}

		if (pulseDelay != NULL)
		{
			pulseDelay->RemoveNotificationHandler(this);
			delete pulseDelay;
			pulseDelay = NULL;
		}

		if (pulseLength != NULL)
		{
			pulseLength->RemoveNotificationHandler(this);
			delete pulseLength;
			pulseLength = NULL;
		}

		if (channelMode != NULL)
		{
			channelMode->RemoveNotificationHandler(this);
			delete channelMode;
			channelMode = NULL;
		}

		if (channelLoadPower != NULL)
		{
			channelLoadPower->RemoveNotificationHandler(this);
			delete channelLoadPower;
			channelLoadPower = NULL;
		}

		if (focalPowerValue != NULL)
		{
			focalPowerValue->RemoveNotificationHandler(this);
			delete focalPowerValue;
			focalPowerValue = NULL;
		}

		delete activeChannel;
		activeChannel = NULL;
	}

	labelMonitor2.SetWindowTextW(_T("----"));
	labelMointor2Unit.SetWindowTextW(_T("-"));
	labelChannelLoadPower.SetWindowTextW(_T(""));
	labelBrightness.SetWindowTextW(_T("0.00"));
	labelPulseDelay.SetWindowTextW(_T("0.00"));
	labelPulseLength.SetWindowTextW(_T("0.00"));
	sliderBrightness.SetPos(0);
	sliderPulseDelay.SetPos(0);
	sliderPulseLength.SetPos(0);
}


/// <summary>
/// Updates the GUI.
/// Enables or disables controls as required
/// </summary>
void CMFCSample1Dlg::UpdateGUI()
{
	// Update Controller buttons
	bool controllerOpen = activeController != NULL;
	bool isTriniti = activeController != NULL ? activeController->GetIsTriniti() : false;

	GetDlgItem(IDC_MFCBUTTONSEARCH)->EnableWindow(!controllerOpen);
	GetDlgItem(IDC_COMBOCONTROLLER)->EnableWindow(!controllerOpen);
	GetDlgItem(IDC_MFCBUTTONOPEN)->EnableWindow(!controllerOpen);
	GetDlgItem(IDC_MFCBUTTONCLOSE)->EnableWindow(controllerOpen );
	GetDlgItem(IDC_MFCBUTTONIMPORT)->EnableWindow(controllerOpen & mHealthy & isTriniti);
	GetDlgItem(IDC_MFCBUTTONEXPORT)->EnableWindow(controllerOpen & mHealthy & isTriniti);
	GetDlgItem(IDC_COMBOCHANNEL)->EnableWindow(controllerOpen & mHealthy & isTriniti);
	GetDlgItem(IDC_COMBOMODE)->EnableWindow(controllerOpen &mHealthy & isTriniti);
	GetDlgItem(IDC_MFCBUTTONSEND)->EnableWindow(controllerOpen & mHealthy);
	GetDlgItem(IDC_EDITCOMMAND)->EnableWindow(controllerOpen & mHealthy);


	bool channelOpen = activeChannel != NULL;

	GetDlgItem(IDC_SLIDERBRIGHTNESS)->EnableWindow(channelOpen & mHealthy);
	int v = -1;
	if (channelMode != NULL)
		channelMode->GetCurrentValue(&v);
	bool pulseControlsEnable = (activeChannel != NULL) & (v == CM_PULSE);
	GetDlgItem(IDC_SLIDERPULSEDELAY)->EnableWindow(pulseControlsEnable & mHealthy);
	GetDlgItem(IDC_SLIDERPULSELENGTH)->EnableWindow(pulseControlsEnable & mHealthy);
	
	// Only enable/disable open button based on controller count if we are not currently connected to a controller
	if (!controllerOpen)
	{
		GetDlgItem(IDC_MFCBUTTONOPEN)->EnableWindow((cbControllerList.GetCount() > 0) ? true : false);
	}
}

#pragma endregion


#pragma region UI control update from register

/// <summary>
/// Updates the channel mode control based on the current register value
/// </summary>
void CMFCSample1Dlg::UpdateChannelModeControl()
{
	int v;
	if (channelMode != NULL)
	{
		channelMode->GetCurrentValue(&v);
		cbChannelMode.SetCurSel(v);
		sliderBrightness.SetRangeMax(v == 0 ? 100 : 1000, true);
		sliderBrightness.SetTicFreq(10);
		UpdateGUI();
	}
}

/// <summary>
/// Updates the controller temperature controls based on the associated register.
/// </summary>
void CMFCSample1Dlg::UpdateControllerTemperatureControls()
{
	float value;
	CString work;

	if (controllerTemperature != NULL)
	{
		controllerTemperature->GetCurrentValue(&value);
		work.Format(_T("%3.2f"), value);
		labelControllerTemperature.SetWindowText(work);
	}
}

/// <summary>
/// Updates the channel load power controls.
/// </summary>
void CMFCSample1Dlg::UpdateChannelLoadPowerControls()
{
	float value;
	CString work;

	if (channelLoadPower != NULL)
	{
		channelLoadPower->GetCurrentValue(&value);
		work.Format(_T("%3.2f"), value);
		labelChannelLoadPower.SetWindowText(work);

	}
}


/// <summary>
/// Updates the brightness controls.
/// </summary>
void CMFCSample1Dlg::UpdateBrightnessControls()
{
	float value;
	CString work;

	if (brightness != NULL)
	{
		brightness->GetCurrentValue(&value);
		sliderBrightness.SetPos((int)value);
		work.Format(_T("%3.2f"), value);
		labelBrightness.SetWindowText(work);
	}
}

/// <summary>
/// Updates the brightness controls.
/// </summary>
void CMFCSample1Dlg::UpdateFocalPowerControls()
{
	float value;
	CString work;

	if (focalPowerValue != NULL)
	{
		focalPowerValue->GetCurrentValue(&value);
	
		if ( value < focalPowerMinValue || value > focalPowerMaxValue )
		{
			value = 0;
			focalPowerValue->SetCurrentValue(value);
		}
		
		sliderBrightness.SetPos((int)(value*100.0));
		work.Format(_T("%3.2f"), value);
		labelBrightness.SetWindowText(work);
	}
}


/// <summary>
/// Updates the pulse length controls.
/// </summary>
void CMFCSample1Dlg::UpdatePulseLengthControls()
{
	float value;
	CString work;

	if (pulseLength != NULL)
	{
		pulseLength->GetCurrentValue(&value);
		sliderPulseLength.SetPos((int)value);
		work.Format(_T("%3.2f"), value);
		labelPulseLength.SetWindowText(work);
	}
}

/// <summary>
/// Updates the pulse delay controls.
/// </summary>
void CMFCSample1Dlg::UpdatePulseDelayControls()
{
	float value;
	CString work;

	if (pulseDelay != NULL)
	{
		pulseDelay->GetCurrentValue(&value);
		sliderPulseDelay.SetPos((int)value);
		work.Format(_T("%3.2f"), value);
		labelPulseDelay.SetWindowText(work);
	}
}


/// <summary>
/// Called when send button clicked.
///	Send the command to the controller
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbuttonsend()
{
	if (activeController != NULL)
	{
		wchar_t command[1024] = { _T("") };
		wchar_t result[1024] = { _T("") };
		GetDlgItem(IDC_EDITCOMMAND)->GetWindowText(command, 1024);
		GetDlgItem(IDC_EDITRESULT)->SetWindowText(_T(""));

		if (activeController->SendCommand(command, result, 1024))
		{
			GetDlgItem(IDC_EDITRESULT)->SetWindowText(result);
		}
		else
		{
			LastError* lastError = activeController->GetLastError();
			AfxMessageBox(lastError->message, MB_ICONERROR | MB_OK);
		}
	}
}


/// <summary>
/// Override and process the translate message.
/// Inhibit escape or return closing the application
/// </summary>
/// <param name="pMsg">The p MSG.</param>
/// <returns></returns>
BOOL CMFCSample1Dlg::PreTranslateMessage(MSG* pMsg)
{
	if (pMsg->message == WM_KEYDOWN)
	{
		if (pMsg->wParam == VK_RETURN || pMsg->wParam == VK_ESCAPE)
		{
			return TRUE;                // Do not process further
		}
	}

	return CWnd::PreTranslateMessage(pMsg);
}

/// <summary>
/// Called when horizontal scroll bars change position
/// Handle sliders used to set registers on the controller
/// </summary>
/// <param name="nSBCode">The n sb code.</param>
/// <param name="nPos">The n position.</param>
/// <param name="pScrollBar">The p scroll bar.</param>
void CMFCSample1Dlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	switch ( pScrollBar->GetDlgCtrlID())
	{
		case IDC_SLIDERBRIGHTNESS:
			if (focalPowerValue != NULL)
			{
				focalPowerValue->SetCurrentValue(((float)sliderBrightness.GetPos())/100.0f);
			}
			else if (brightness != NULL)
			{
				brightness->SetCurrentValue((float)sliderBrightness.GetPos());
			}
			
			break;

		case IDC_SLIDERPULSEDELAY:

			if (pulseDelay != NULL)
			{
				pulseDelay->SetCurrentValue((float)sliderPulseDelay.GetPos());
			}
			break;

		case IDC_SLIDERPULSELENGTH:
			if (pulseLength != NULL)
			{
				pulseLength->SetCurrentValue((float)sliderPulseLength.GetPos());
			}
			break;
	}

	__super::OnHScroll(nSBCode, nPos, pScrollBar);
}

/// <summary>
/// Called when controller selection changes
/// Update the launch webpage button
/// </summary>
void CMFCSample1Dlg::OnCbnSelchangeCombocontroller()
{
	CController* controller = (CController*)cbControllerList.GetItemDataPtr(cbControllerList.GetCurSel());

	LPTSTR ipAddress = controller->GetIPAddress();

	if (controller != NULL && (_tcsstr(ipAddress, _T("COM"))==NULL))
	{
		// Set caption of open web page button and make visible
		CString work;
		work.Format(_T("Open web page for %s with IP %s"), controller->GetCaption(), ipAddress);

		GetDlgItem(IDC_MFCBTNWEBPAGE)->SetWindowText(work);
		GetDlgItem(IDC_MFCBTNWEBPAGE)->ShowWindow(SW_SHOW);
	}
	else
	{
		GetDlgItem(IDC_MFCBTNWEBPAGE)->SetWindowText(_T(""));
		GetDlgItem(IDC_MFCBTNWEBPAGE)->ShowWindow(SW_HIDE);
	}

}

/// <summary>
/// Called when Web page button clicked
/// </summary>
void CMFCSample1Dlg::OnBnClickedMfcbtnwebpage()
{
	CController* controller = (CController*)cbControllerList.GetItemDataPtr(cbControllerList.GetCurSel());
	if (controller != NULL)
	{
		CString work;
		work.Format(_T("http://%s"), controller->GetIPAddress());
		ShellExecute(NULL, _T("open"), work, NULL, NULL,
			SW_SHOWNORMAL);
	}
}


/// <summary>
/// Toggle the display for either lens or lighting control.
/// </summary>
/// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
void CMFCSample1Dlg::LensControlUIToggle(bool isEnabled)
{
	ToggleLightingControls(!isEnabled);

	// Get focal power min and max values
	if (activeChannel != NULL && isEnabled)
	{
		CRegisterFloat *focalPowerMin = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("FocalPowerMin"), TRT_FLOAT);
		float minValue = 0.0;
		if ( focalPowerMin)
			focalPowerMin->GetCurrentValue(&minValue);

		focalPowerMinValue = minValue;

		CRegisterFloat *focalPowerMax = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("FocalPowerMax"), TRT_FLOAT);
		float maxValue = 1500.0;
		if ( focalPowerMax)
			focalPowerMax->GetCurrentValue(&maxValue);
		
		focalPowerMaxValue = maxValue;

		float min = minValue * 100.0F;
		float max = maxValue * 100.0F;
		sliderBrightness.SetRange((int)min, (int)max, true);
		delete focalPowerMin;
		delete focalPowerMax;
	}
	else
	{
		sliderBrightness.SetRange(0, 1000, true);
	}
}

/// <summary>
/// Toggles the lighting controls for either lens or light control
/// </summary>
/// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
void CMFCSample1Dlg::ToggleLightingControls(bool isEnabled)
{
	int lightingControls[] = { 
		IDC_STATICMODE,
		IDC_COMBOMODE,
		IDC_STATICPULSEDELAYLABEL,
		IDC_STATICPULSEDELAY,
		IDC_SLIDERPULSEDELAY,
		IDC_STATICPULSELENGTHLABEL,
		IDC_STATICPULSELENGTH,
		IDC_SLIDERPULSELENGTH,
		IDC_STATICPULSEDELAYUNITS,
		IDC_STATICPULSELENGTHUNITS,
		IDC_STATICBRIGHTNESSUNITS,
		-1
	};

	int windowMode = SW_HIDE;
	CString title;

	if ( isEnabled )
	{
		sliderBrightness.SetTicFreq(100);
		windowMode = SW_SHOW;
		title = _T("Brightness");
	}
	else
	{
		sliderBrightness.SetTicFreq(100);

		CRegisterFloat *focalPowerMin = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("FocalPowerMin"), TRT_FLOAT);
		float minValue = 0.0;
		if (focalPowerMin)
		{
			focalPowerMin->GetCurrentValue(&minValue);
		}
		focalPowerMinValue = minValue;

		CRegisterFloat *focalPowerMax = (CRegisterFloat*)activeChannel->GetRegisterByName(_T("FocalPowerMax"), TRT_FLOAT);
		float maxValue = 1500.0;
		if (focalPowerMax)
		{
			focalPowerMax->GetCurrentValue(&maxValue);

		}
		focalPowerMaxValue = maxValue;

		title.Format(_T("Focal Power\n(Diopters)\n[%3.2f to %3.2f]"), minValue , maxValue);
		delete focalPowerMin;
		delete focalPowerMax;

	}

	GetDlgItem(IDC_STATICBRIGHTNESSLABEL)->SetWindowText(title);

	int i = 0;
	while (lightingControls[i]!=-1)
	{
		GetDlgItem(lightingControls[i])->ShowWindow(windowMode);
		i++;
	}

}

#pragma endregion




