//-----------------------------------------------------------------------------------------------
// <copyright file="Alarm.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once


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
	/// <summary>
	/// Class represents a Register alarm.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CAlarm
	{
	private:
		float mLoLimit;
		float mHiLimit;
		bool mIsTriggered;

	public:
		CAlarm(float loLimit, float hiLimit, bool isTriggered);
		virtual ~CAlarm();

		float GetLoLimit(void) const;
		float GetHiLimit(void) const;
		bool GetIsTriggered(void) const;
	};
}
