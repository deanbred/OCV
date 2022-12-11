//-----------------------------------------------------------------------------------------------
// <copyright file="RegisterFloat.h" company="Gardasoft Products Ltd">
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
	/// Class represents a float type register on a controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CRegisterFloat : public CRegister
	{
		friend class CChannel;
		friend class CController;

	private:
		float mCurrentValue;

	public:
		CRegisterFloat();
		CRegisterFloat(CControllerManager* controllerManager, CController* controller);
		virtual ~CRegisterFloat();

		bool MonitorStart(float loLimit, float hiLimit);
		void MonitorStop(void);

		bool GetCurrentValue(float *value);
		bool GetCurrentValue(int *value);
		bool GetCurrentValue(LPTSTR value, int bufferSize);

		bool SetCurrentValue(float value);
		bool SetCurrentValue(int value);
		bool SetCurrentValue(LPTSTR value);

	};
}
