//-----------------------------------------------------------------------------------------------
// <copyright file="RegisterInteger.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

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


#pragma endregion


namespace GardasoftControllerManagementAPINativeWrapper
{
	/// <summary>
	/// Class represents an integer type register on a controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CRegisterInteger : public CRegister
	{
		friend class CChannel;
		friend class CController;

	private:
		int mCurrentValue;

	public:
		CRegisterInteger();
		CRegisterInteger(CControllerManager* controllerManager, CController* controller);
		virtual ~CRegisterInteger();

		bool MonitorStart(int loLimit, int hiLimit);
		void MonitorStop(void);

		bool GetCurrentValue(float *value);
		bool GetCurrentValue(int *value);
		bool GetCurrentValue(LPTSTR value, int bufferSize);

		bool SetCurrentValue(float value);
		bool SetCurrentValue(int value);
		bool SetCurrentValue(LPTSTR value);
	};
}
