//-----------------------------------------------------------------------------------------------
// <copyright file="Register.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#pragma region System Include Files

#include <msclr\marshal.h>
#include <atlstr.h>
#include <vcclr.h>

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

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

#pragma endregion


namespace GardasoftControllerManagementAPINativeWrapper
{

	class CRegisterNotifications;					// Forward declaration

	/// <summary>
	/// Class represents a base register on a controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CRegister
	{
		friend class CChannel;
		friend class CController;
		friend class CControllerManager;
		friend ref struct RegisterEventHandlers;

	private:
		vector<CRegisterNotifications*>* mNotificationHandlers;
		gcroot<RegisterEventHandlers^>* mEventHandler;
		void OnPropertyChanged(void);

	protected:
		LPTSTR mName;
		Behaviour mBehaviour;
		LPTSTR mCaption;
		LPTSTR mCategory;
		LPTSTR mDescription;
		CUnit* mDeviceUnit;
		CUnit* mUserUnit;
		CUnitType* mUnitType;
		RegisterType mCurrentValueType;
		bool mIsInAlarm;
		LPTSTR mTooltip;
		int mChannelIndex;
		CControllerManager* mControllerManager;
		CController* mController;

		struct LastError* mLastError;

		virtual void SubscribeToEvents(void);
		virtual void UnsubscribeFromEvents(void);

	public:
		CRegister();
		virtual ~CRegister();

		/*virtual void MonitorStop(void);*/

		virtual LPTSTR GetName(void) const;
		virtual Behaviour GetBehaviour(void) const;
		virtual LPTSTR GetCaption(void) const;
		virtual LPTSTR GetCategory(void) const;
		virtual LPTSTR GetDescription(void) const;
		virtual CUnit* GetDeviceUnit(void) const;
		virtual CUnit* GetUserUnit(void) const;
		virtual CUnitType* GetUnitType(void) const;
		virtual RegisterType GetCurrentValueType(void) const;
		virtual bool GetIsInAlarm(void) const;
		virtual LPTSTR GetTooltip(void) const;
		virtual int GetChannelIndex(void) const;
		virtual bool GetCurrentValue(int *value);
		virtual bool GetCurrentValue(float *value);
		virtual bool GetCurrentValue(LPTSTR value, int bufferSize);
		virtual void SetUserUnit(CUnit* unit);
		virtual bool SetCurrentValue(int value);
		virtual bool SetCurrentValue(float value);
		virtual bool SetCurrentValue(LPTSTR value);

		CController* GetParentController(void);
		struct LastError* GetLastError(void);

		void AddNotificationHandler(CRegisterNotifications *handler);
		bool RemoveNotificationHandler(CRegisterNotifications *handler);
	};
}
