//-----------------------------------------------------------------------------------------------
// <copyright file="Channel.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------

#pragma once

#pragma region System Includes
#include <msclr\marshal.h>
#pragma endregion

#include "TrinitiAPI.h"

#ifdef CONTROLLERMANAGERAPIDLL_EXPORTS
#define CONTROLLERMANAGERDLL_API __declspec(dllexport) 
#else
#define CONTROLLERMANAGERDLL_API __declspec(dllimport) 
#endif


namespace GardasoftControllerManagementAPINativeWrapper
{
	class CChannelNotifications;		// Forward declaration of notification class

	/// <summary>
	/// Class represents a channel on a triniti controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CChannel
	{

		friend ref struct ChannelEventHandlers;

	private:
		int mNumber;
		CControllerManager* mControllerManager;
		CController* mController;

		void SubscribeToEvents(void);
		void UnsubscribeFromEvents(void);
		struct LastError* mLastError;

		vector<CChannelNotifications*>* mNotificationHandlers;
		gcroot<ChannelEventHandlers^>* mEventHandler;

		void OnRegisterAlarmStatusChanged(LPTSTR regName, CAlarm* alarm);

	protected:


	public:
		CChannel();
		CChannel(CControllerManager* cm, CController* c);
		virtual ~CChannel();

		//void ExportConfiguration(LPTSTR filename, SaveMode saveMode);
		//void ImportConfiguration(LPTSTR filename);

		int GetNumber(void);
		void SetNumber(int number);
		CController* GetParentController(void) const;

		int GetRegisterCount(void) const;
		RegisterType GetRegisterTypeByName(LPTSTR name);
		CRegister* GetRegisterByName(LPTSTR name, RegisterType regType);
		RegisterType GetRegisterTypeByIndex(int index);
		CRegister* GetRegisterByIndex(int index, RegisterType regType);

		void AddNotificationHandler(CChannelNotifications* handler);
		bool RemoveNotificationHandler(CChannelNotifications* handler);
	};
}