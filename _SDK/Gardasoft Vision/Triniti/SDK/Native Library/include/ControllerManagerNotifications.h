//-----------------------------------------------------------------------------------------------
// <copyright file="ControllerNotifications.h" company="Gardasoft Products Ltd">
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
	/// Class provides Controller Manager event callbacks.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CControllerManagerNotifications
	{
	public:
		CControllerManagerNotifications();

		// Available notifications
		virtual void OnDeviceDiscoveryStatusChanged(CControllerManager *cm, DeviceDiscoveryStatus deviceDiscoveryStatus);
	};

}
