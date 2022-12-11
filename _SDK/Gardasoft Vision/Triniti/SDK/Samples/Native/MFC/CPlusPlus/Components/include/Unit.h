//-----------------------------------------------------------------------------------------------
// <copyright file="Unit.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#pragma region System Include Files

#include <msclr\marshal.h>
#include <tchar.h>

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



#pragma endregion


namespace GardasoftControllerManagementAPINativeWrapper
{

	/// <summary>
	/// Class represents a Register unit.
	/// </summary>
	class CONTROLLERMANAGERDLL_API CUnit
	{		
		friend class CChannel;
		friend class CController;

	private:
		LPTSTR mName;
		LPTSTR mCaption;
		float mScale;

	public:
		CUnit();
		CUnit(LPTSTR name, float scale, LPTSTR caption = _T(""));
		virtual ~CUnit();

		LPTSTR GetName(void);
		LPTSTR GetCaption(void);
		float GetScale(void);
	};
}

