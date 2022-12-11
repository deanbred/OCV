//-----------------------------------------------------------------------------------------------
// <copyright file="ControllerManager.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once


#pragma region System Include Files

#include <stdio.h>
#include <Windows.h>
#include <vector>
#include <tchar.h>
#include <atlstr.h>

#pragma endregion


#pragma region User Include Files
#pragma endregion


#pragma region Defines

#ifdef CONTROLLERMANAGERAPIDLL_EXPORTS
#define CONTROLLERMANAGERDLL_API __declspec(dllexport) 
#else
#define CONTROLLERMANAGERDLL_API __declspec(dllimport) 
#endif

#pragma endregion


#pragma region Using namespaces

using namespace std;

#pragma endregion


namespace GardasoftControllerManagementAPINativeWrapper
{
	class CControllerManagerNotifications;		// Forward declaration of notificaion class
	
	/// <summary>
	/// Class responsible for managing all controllers in the system.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CControllerManager
	{
		friend ref struct ControllerManagerEventHandlers;

	private:
		void alloc(void);
		void free(void);
		void SubscribeToEvents(void);
		void UnsubscribeFromEvents(void);
		void PopulateControllersList(void);

		CControllerManager* mInstance;
		APIInfo* mAPIInfo;

	protected:
		vector<CController*> *mControllers;
		vector<CControllerManagerNotifications*>* mNotificationHandlers;

		void OnDeviceDiscoveryStatusChanged(DeviceDiscoveryStatus deviceDiscoveryStatus);

	public:
		CControllerManager();
		virtual ~CControllerManager();

		void Initialise(void);
		APIInfo* GetAPIInfo(void);
		void DiscoverControllers(void);
		int GetControllerCount(void) const;
		CController* GetController(int index);
		CController* GetControllerBySerialNumber(int serialNumber);
		//vector<CController*>* GetControllers(void);
		void AddNotificationHandler(CControllerManagerNotifications *handler);
		bool RemoveNotificationHandler(CControllerManagerNotifications *handler);

	};
}

