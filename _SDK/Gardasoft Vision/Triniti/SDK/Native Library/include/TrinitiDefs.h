//-----------------------------------------------------------------------------------------------
// <copyright file="TrinitiDefs.h" company="Gardasoft Products Ltd">
//    © Gardasoft Products Ltd. Copyright 2015 All rights reserved.
// </copyright>
// <author>Steve Cronk, Dharmesh Mistry</author>
//-----------------------------------------------------------------------------------------------
#pragma once

#include <atlstr.h>

namespace GardasoftControllerManagementAPINativeWrapper
{			
	/// <summary>
	/// Structure provides information about the API.
	/// </summary>
	public struct APIInfo
	{		
		/// <summary>
		/// Version of the API
		/// </summary>
		LPTSTR version;
	};
	
	/// <summary>
	/// Structure provides information about the API last error.
	/// </summary>
	public struct LastError
	{		
		/// <summary>
		/// The last error message.
		/// </summary>
		LPTSTR message;
	};

	/// <summary>
	/// Device discovery status.
	/// </summary>
	enum DeviceDiscoveryStatus
	{
		/// <summary>
		/// Device discovery in progress.
		/// </summary>
		DDS_IN_PROGRESS = 0,		
		/// <summary>
		/// Device discovery complete.
		/// </summary>
		DDS_COMPLETE,
	};


	/// <summary>
	/// Possible register types
	/// </summary>
	enum RegisterType
	{
		// Register type is unknown - No access.
		TRT_UNKNOWN,
		// Register type is integer.
		TRT_INTEGER,
		// Register type is float.
		TRT_FLOAT,
		// Register type is string.
		TRT_STRING
	};


	/// <summary>
	/// Controller register save mode.
	/// </summary>
	enum ControllerRegistersSaveMode
	{		
		/// <summary>
		/// Save everything.
		/// </summary>
		SM_EVERYTHING,		
		/// <summary>
		/// Save registers only.
		/// </summary>
		SM_REGISTERS_ONLY,		
		/// <summary>
		/// Save to register master.
		/// </summary>
		SM_REGISTER_MASTER,
	};
	
	/// <summary>
	/// Controller Status.
	/// </summary>
	enum ControllerStatus
	{		
		/// <summary>
		/// Controller Connecting.
		/// </summary>
		CS_CONNECTING = 0,
		/// <summary>
		/// Controller Connected.
		/// </summary>
		CS_CONNECTED,
		/// <summary>
		/// Controller Disconnected.
		/// </summary>
		CS_DISCONNECTED,
	};

	/// <summary>
	/// Identifies the current connection status for the controller.
	/// </summary>
	enum ControllerConnectionStatus
	{
		/// <summary>
		/// Controller connection is healthy.
		/// </summary>
		CCS_HEALTHY = 0,
		/// <summary>
		/// Controller connection is faulty.
		/// </summary>
		CCS_FAULT,
	};
	
	/// <summary>
	/// The different status types that a command can exist in.
	/// </summary>
	enum ControllerCommandStatus
	{
		/// <summary>
		/// Sending command to controller.
		/// </summary>
		CCS_SENDING = 0,
		/// <summary>
		/// Sent command to controller.
		/// </summary>
		CCS_SENT,
		/// <summary>
		/// Receiveing response from controller.
		/// </summary>
		CCS_RECEIVEING_RESPONSE,
		/// <summary>
		/// Response received from controller.
		/// </summary>
		CCS_RESPONSE_RECEIVED,
		/// <summary>
		/// No response received from controller.
		/// </summary>
		CCS_NO_RESPONSE_RECEIVED,
	};
	
	/// <summary>
	/// Register behaviour flags.
	/// </summary>
	enum Behaviour
	{		
		/// <summary>
		/// Register has no access.
		/// </summary>
		B_NO_ACCESS = 0x00,		
		/// <summary>
		/// Register is readable.
		/// </summary>
		B_READ = 0x01,
		/// <summary>
		/// Register is writeable.
		/// </summary>
		B_WRITE = 0x02,
		/// <summary>
		/// Register is dynamic. That is it could change internally.
		/// </summary>
		B_DYNAMIC = 0x04,
	};

	/// <summary>
	/// Channel mode.
	/// </summary>
	enum ChannelMode
	{		
		/// <summary>
		/// Continuous channel mode.
		/// </summary>
		CM_CONTINUOUS = 0,		
		/// <summary>
		/// Pulse channel mode.
		/// </summary>
		CM_PULSE,		
		/// <summary>
		/// Switched channel mode.
		/// </summary>
		CM_SWITCHED,		
		/// <summary>
		/// Selected channel mode.
		/// </summary>
		CM_SELECTED,
	};
	
	/// <summary>
	/// Channel status.
	/// </summary>
	enum ChannelStatus
	{		
		/// <summary>
		/// Reset.
		/// </summary>
		CST_RESET, //"Reset";
		/// <summary>
		/// Stopped.
		/// </summary>
		CST_STOPPED, //"Stopped";
		/// <summary>
		/// Not connected.
		/// </summary>
		CST_NO_CONNECT, //"Not connected";
		/// <summary>
		/// Detecting.
		/// </summary>
		CST_DETECTED, //"Detecting";
		/// <summary>
		/// Sensing.
		/// </summary>
		CST_SENSING, //"Sensing";
		/// <summary>
		/// Waiting for current rating.
		/// </summary>
		CST_PROMPT, //"Waiting for current rating";
		/// <summary>
		/// Connected.
		/// </summary>
		CST_CONNECTED, //"Connected";
		/// <summary>
		/// Tandem.
		/// </summary>
		CST_TANDEM, //"Tandem";
		/// <summary>
		/// Error.
		/// </summary>
		CST_ERROR, //“Error”
	};
}