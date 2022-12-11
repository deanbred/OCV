//-----------------------------------------------------------------------------------------------
// <copyright file="MFCSample1Dlg.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#include "TrinitiAPI.h"
#include "afxwin.h"
#include "afxbutton.h"


using namespace GardasoftControllerManagementAPINativeWrapper;


// CMFCSample1Dlg dialog
class CMFCSample1Dlg : public CDialogEx, CControllerManagerNotifications, CControllerNotifications, CRegisterNotifications
{
private:

	//Data Members
	CControllerManager *controllerManager;
	CController *activeController;
	CChannel *activeChannel;
	ControllerStatus mControllerStatus;
	
	HBRUSH mNormalBrush;
	HBRUSH mGreenBrush;
	HBRUSH mYellowBrush;

	bool mHealthy;

	// Some useful registers
	CRegisterFloat *brightness;
	CRegisterFloat *pulseLength;
	CRegisterFloat *pulseDelay;
	CRegisterInteger *channelMode;
	CRegisterFloat *controllerTemperature;
	CRegisterFloat *channelLoadPower;
	CRegisterFloat *focalPowerValue;

	float focalPowerMinValue;
	float focalPowerMaxValue;

private: //Helper functions
	void DiscoverControllers();
	void UpdateGUI();
	void UpdateBrightnessControls();
	void UpdateFocalPowerControls();
	void UpdatePulseDelayControls();
	void UpdatePulseLengthControls();
	void UpdateChannelModeControl();
	void UpdateControllerTemperatureControls();
	void UpdateChannelLoadPowerControls();
	void ReleaseChannel();
	void CloseController();
	void LensControlUIToggle(bool isEnabled);
	void ToggleLightingControls(bool isEnabled);

// Construction
public:
	CMFCSample1Dlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_MFCSAMPLE1_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support

	// Triniti Notification Handlers
	void OnDeviceDiscoveryStatusChanged(CControllerManager *cm, DeviceDiscoveryStatus deviceDiscoveryStatus);
	void OnStatusChanged(CController* controller, ControllerStatus controllerStatus);
	void OnConnectionStatusChanged(CController* controller, ControllerConnectionStatus controllerConnectionStatus);
	void OnPropertyChanged(CRegister* reg);
	LRESULT OnUser(WPARAM, LPARAM lParam);

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	CStatusBar statusBar;
	// ComboBox containing list of available controllers
	CComboBox cbControllerList;
	afx_msg void OnBnClickedMfcbuttonsearch();
	afx_msg void OnBnClickedMfcbuttonopen();
	// Combo box containing list of available modes for the channel
	CComboBox cbChannelMode;
	// ComboBox containing list of available channels for currently open controller
	CComboBox cbChannelList;
	afx_msg void OnBnClickedMfcbuttonexport();
	afx_msg void OnBnClickedMfcbuttonimport();
	// Static label used to indicate if controller is online
	CStatic labelOnline;
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnBnClickedMfcbuttonclose();
	afx_msg void OnCbnSelchangeCombomode();
	// Slicer for controlling LED brightness
	CSliderCtrl sliderBrightness;

	// Slicer for controlling pulse length
	CSliderCtrl sliderPulseLength;

	// Slicer for controlling pulse delay
	CSliderCtrl sliderPulseDelay;
	
	afx_msg void OnCbnSelchangeCombochannel();
	CStatic labelBrightness;
	CStatic labelPulseDelay;
	CStatic labelPulseLength;
	CStatic labelControllerTemperature;
	CStatic labelChannelLoadPower;
	afx_msg void OnDestroy();
	CStatic labelMonitor1;
	CStatic labelMonitor2;
	CStatic labelMonitor1Unit;
	CStatic labelMointor2Unit;	
	afx_msg void OnBnClickedMfcbuttonsend();
	BOOL PreTranslateMessage(MSG* pMsg);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnCbnSelchangeCombocontroller();
	afx_msg void OnBnClickedMfcbtnwebpage();
};
