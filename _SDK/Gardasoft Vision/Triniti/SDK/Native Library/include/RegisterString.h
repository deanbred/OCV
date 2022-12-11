//-----------------------------------------------------------------------------------------------
// <copyright file="RegisterString.h" company="Gardasoft Products Ltd">
//    � Gardasoft Products Ltd. Copyright 2015 All rights reserved.
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
	/// Class represents a string type register on a controller.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CRegisterString : public CRegister
	{
		friend class CChannel;

	private:
		LPTSTR mCurrentValue;

	public:
		CRegisterString(CControllerManager* controllerManager, CController* controller);
		virtual ~CRegisterString();

		bool GetCurrentValue(LPTSTR value, int bufferSize);
		bool SetCurrentValue(LPTSTR value);
	};
}
