//-----------------------------------------------------------------------------------------------
// <copyright file="Controller.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#pragma region System Include Files

#include <msclr\marshal.h>
#include <atlstr.h>

#pragma endregion


#pragma region User Include Files

#include "TrinitiAPI.h"

#pragma endregion


#pragma region Defines

#ifdef CONTROLLERMANAGERAPIDLL_EXPORTS
#define CONTROLLERMANAGERDLL_API __declspec(dllexport) 
#else
#define CONTROLLERMANAGERDLL_API __declspec(dllimport) 
#endif

#pragma endregion


#pragma region Using namespaces


#pragma endregion


namespace GardasoftControllerManagementAPINativeWrapper
{
	

	class CControllerNotifications;		// Forward declaration of notificaion class

	/// <summary>
	/// Class represents a Triniti controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CController
	{
		friend class CControllerManager;
		friend class CChannel;
		friend ref struct ControllerEventHandlers;

	private:
		//void alloc(void);
		//void free(void);
		void SubscribeToEvents(void);
		void UnsubscribeFromEvents(void);
		struct LastError* mLastError;

		//CController* mController;
		CControllerManager* mControllerManager;
		LPTSTR mCaption;
		LPTSTR mIPAddress;
		bool mIsOpen;
		bool mIsTriniti;
		ControllerStatus mStatus;
		ControllerConnectionStatus mConnectionStatus;
		ControllerCommandStatus mCommandStatus;
		LPTSTR mManufacturer;
		LPTSTR mModel;
		int mSerialNumber;
		LPTSTR mMACAddress;
		LPTSTR mLastCommand;
		LPTSTR mLastResponse;
		int mCommunicationsMonitorInterval;

		vector<CControllerNotifications*>* mNotificationHandlers;
		gcroot<ControllerEventHandlers^>* mEventHandler;

		void OnStatusChanged(ControllerStatus controllerStatus);
		void OnConnectionStatusChanged(ControllerConnectionStatus controllerConnectionStatus);
		void OnCommandStatusChanged(ControllerCommandStatus controllerCommandStatus, LPTSTR message);
		void OnRegisterAlarmStatusChanged(LPTSTR regName, CAlarm* alarm);

	protected:


	public:
		CController(void);
		CController(CControllerManager* cm);
		virtual ~CController(void);

		bool Open(void);
		bool Close(void);
		bool ClearError(void);
		bool SendCommand(LPTSTR command, LPTSTR result, int bufferSize);

		int GetRegisterCount(void) const;
		int GetChannelCount(void) const;
		RegisterType GetRegisterTypeByName(LPTSTR name);
		CRegister* GetRegisterByName(LPTSTR name, RegisterType regType);
		RegisterType GetRegisterTypeByIndex(int index);
		CRegister* GetRegisterByIndex(int index, RegisterType regType);
		CChannel* GetChannelByIndex(int index);
	
		LPTSTR GetCaption(void);
		LPTSTR GetIPAddress(void);
		bool GetIsOpen(void);
		bool GetIsTriniti(void);
		ControllerConnectionStatus GetConnectionStatus(void);
		ControllerStatus GetStatus(void);
		ControllerCommandStatus GetCommandStatus(void);
		LPTSTR GetManufacturer(void);
		LPTSTR GetModel(void);
		int GetSerialNumber(void);
		LPTSTR GetMACAddress(void);
		LPTSTR GetLastCommand(void);
		LPTSTR GetLastResponse(void);
		int GetCommunicationsMonitorInterval(void);
		void SetCommunicationsMonitorInterval(int interval);
		bool StoreConfiguration(void);
		bool ExportConfiguration(LPTSTR filename, ControllerRegistersSaveMode saveMode = SM_EVERYTHING);
		bool ImportConfiguration(LPTSTR filename);

		struct LastError* GetLastError(void);

		void AddNotificationHandler(CControllerNotifications *handler);
		bool RemoveNotificationHandler(CControllerNotifications *handler);
	};
}