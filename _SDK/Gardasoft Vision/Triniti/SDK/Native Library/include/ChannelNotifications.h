//-----------------------------------------------------------------------------------------------
// <copyright file="ChannelNotifications.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------

#pragma once

#include "TrinitiAPI.h"

namespace GardasoftControllerManagementAPINativeWrapper
{
	class CChannel;
	
	/// <summary>
	/// Class provides Controller Channel event callbacks.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CChannelNotifications
	{
	public:
		CChannelNotifications(void);

		// Available Notifications
		virtual void OnRegisterAlarmStatusChanged(CChannel* channel, CRegister* reg, CAlarm* alarm);
	};

}