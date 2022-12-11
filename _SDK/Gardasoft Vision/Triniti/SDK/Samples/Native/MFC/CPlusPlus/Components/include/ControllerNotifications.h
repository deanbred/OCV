//-----------------------------------------------------------------------------------------------
// <copyright file="GChannelNotifications.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------

#pragma once

#pragma region User Include Files

#pragma endregion

namespace GardasoftControllerManagementAPINativeWrapper
{

	/// <summary>
	/// Class provides Controller event callbacks.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CControllerNotifications
	{
	public:
		CControllerNotifications();

		// Available Notifications
		virtual void OnStatusChanged(CController* controller, ControllerStatus controllerStatus);
		virtual void OnConnectionStatusChanged(CController* controller, ControllerConnectionStatus controllerConnectionStatus);
		virtual void OnCommandStatusChanged(CController* controller, ControllerCommandStatus controllerCommandStatus, LPTSTR message);
		virtual void OnRegisterAlarmStatusChanged(CController* controller, LPTSTR registerName, CAlarm* alarm);
	};
}

