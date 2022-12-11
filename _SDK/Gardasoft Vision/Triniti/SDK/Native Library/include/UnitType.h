//-----------------------------------------------------------------------------------------------
// <copyright file="UnitType.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#pragma region System Include Files

#include <msclr\marshal.h>
#include <tchar.h>
#include <vector>

#pragma endregion


#pragma region User Include Files

#include "Unit.h"

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

	/// <summary>
	/// Class represents a Register unit type.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CUnitType
	{
		friend class CChannel;
		friend class CController;

	private:
		LPTSTR mName;
		vector<CUnit*>* mUnits;

	public:
		CUnitType();
		virtual ~CUnitType();

		LPTSTR GetName(void);
		size_t GetUnitCount(void);
		CUnit* GetUnitByIndex(int index);
	};
}