//-----------------------------------------------------------------------------------------------
// <copyright file="RegisterNotifications.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------

#pragma once

#pragma region User Include Files
#pragma endregion

namespace GardasoftControllerManagementAPINativeWrapper
{
	class CRegister;

	/// <summary>
	/// Class provides Controller Register event callbacks.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CRegisterNotifications
	{
	public:
		CRegisterNotifications();

		// Available Notifications
		virtual void OnPropertyChanged(CRegister* r);
	};

}