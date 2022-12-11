using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace CsLibIf

{
	public class LibIf
	{ 
		const string libPath_32 = "SiVCamDll.dll";
		const string libPath_64 = "SiVCamDll_64.dll";

#if WIN64
		const string libPath = libPath_64;
#else
		const string libPath = libPath_32;
#endif

		IntPtr pnt = IntPtr.Zero;
		short[] imArr;

		[DllImport(libPath, EntryPoint = "ScanInterfaces", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibScanInterfaces(StringBuilder xmlList, ref UInt32 xmlSize, bool forceScan);
		[DllImport(libPath, EntryPoint = "GetErrorString", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetErrorString(UInt32 number, StringBuilder strRet, ref UInt32 strRetSize);
		[DllImport(libPath, EntryPoint = "OpenCamera", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibOpenCamera(StringBuilder name, StringBuilder plugin);
		[DllImport(libPath, EntryPoint = "CloseCamera", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibCloseCamera(int camHandle);
		[DllImport(libPath, EntryPoint = "CameraHwReset", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibCameraHwReset(StringBuilder name, StringBuilder plugin);
		[DllImport(libPath, EntryPoint = "GetStatusNames", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatusNames(int camHandle, StringBuilder retStr, UInt32 retSize);
		[DllImport(libPath, EntryPoint = "GetParameterNames", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameterNames(int camHandle, StringBuilder retStr, UInt32 retSize);
		[DllImport(libPath, EntryPoint = "GetStatus", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatus(int camHandle);
		[DllImport(libPath, EntryPoint = "GetStatusValue", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatusValue(int camHandle, StringBuilder displayName, StringBuilder valStr, UInt32 valStrSize);
		[DllImport(libPath, EntryPoint = "GetStatusItem", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatusItem(int camHandle, StringBuilder displayName, StringBuilder valStr, UInt32 valStrSize, ref UInt16 unitType,
			StringBuilder unitStr, UInt32 unitLen, StringBuilder stepStr, UInt32 stepStrLen);
		[DllImport(libPath, EntryPoint = "GetStatusPulldownItem", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatusPulldownItem(int camHandle, StringBuilder displayName, UInt32 PulldownIndex,
			StringBuilder PulldownValStr, UInt32 ValStrLen,
			StringBuilder PulldownName, UInt32 NameLen);
		[DllImport(libPath, EntryPoint = "GetStatusBitField", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetStatusBitField(int camHandle, StringBuilder displayName, StringBuilder maskStr, UInt32 maskLen,
			StringBuilder displStr, UInt32 displLen);
		[DllImport(libPath, EntryPoint = "GetParameters", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameters(int camHandle);
		[DllImport(libPath, EntryPoint = "UpdateParameters", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibUpdateParameters(int camHandle);
		[DllImport(libPath, EntryPoint = "GetParameterValue", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameterValue(int camHandle, StringBuilder displayName, StringBuilder valStr, UInt32 valStrSize);
		[DllImport(libPath, EntryPoint = "GetParameterItem", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameterItem(int camHandle, StringBuilder displayName, StringBuilder valStr, UInt32 valStrSize,
			StringBuilder minStr, UInt32 minLen, StringBuilder maxStr, UInt32 maxLen, ref UInt16 unitType,
			StringBuilder unitStr, UInt32 unitLen, StringBuilder stepStr, UInt32 stepStrLen);
		[DllImport(libPath, EntryPoint = "GetParameterBitField", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameterBitField(int camHandle, StringBuilder displayName, StringBuilder maskStr, UInt32 maskLen,
			StringBuilder displStr, UInt32 displLen);
		[DllImport(libPath, EntryPoint = "GetParameterPulldownItem", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetParameterPulldownItem(int camHandle, StringBuilder displayName, UInt32 PulldownIndex,
			StringBuilder PulldownValStr, UInt32 ValStrLen,
			StringBuilder PulldownName, UInt32 NameLen);
		[DllImport(libPath, EntryPoint = "SetParameterValue", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibSetParameterValue(int camHandle, StringBuilder displayName, StringBuilder valStr);
		[DllImport(libPath, EntryPoint = "SendParameters", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibSendParameters(int camHandle);
		[DllImport(libPath, EntryPoint = "GetImageSize", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetImageSize(int camHandle, ref UInt32 imgSerLen, ref UInt32 imgParLen, ref UInt32 is16,
													ref UInt32 nSerCCD, ref UInt32 nParCCD, ref UInt32 nSerSect, ref UInt32 nParSect);
		[DllImport(libPath, EntryPoint = "GetSoftwareSummary", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetSoftwareSummary(int camHandle, StringBuilder retStr, UInt32 retSize);
		[DllImport(libPath, EntryPoint = "IssueCommand", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibIssueCommand(int camHandle, StringBuilder postName, StringBuilder argStr, StringBuilder retStr, UInt32 retSize);


		[DllImport(libPath, EntryPoint = "PrepareAcqFilm", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibPrepareAcqFilm(int camHandle, UInt16 SerLen, UInt16 ParLen, ushort[] imArr, UInt16 nBuffers, uint is16);

		[DllImport(libPath, EntryPoint = "PrepareAcqFilm", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibPrepareAcqFilm(int camHandle, UInt16 SerLen, UInt16 ParLen, IntPtr imArr, UInt16 nBuffers, uint is16);


		[DllImport(libPath, EntryPoint = "AcqStatus", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibAcqStatus(int camHandle, ref UInt16 pctRead, ref UInt32 CurrentFrame, ref UInt32 Flags);
		[DllImport(libPath, EntryPoint = "EndAcq", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibEndAcq(int camHandle, bool forceAbort);
		[DllImport(libPath, EntryPoint = "ReadoutStatus", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibReadoutStatus(int camHandle, ref double pctExposed, ref UInt16 pctRead);
		[DllImport(libPath, EntryPoint = "ExposureRemainingTime", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibExposureRemainingTime(int camHandle, ref UInt32 remainingTime);
		[DllImport(libPath, EntryPoint = "GetXmlFile", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
		public static extern int LibGetXmlFile(int camHandle, StringBuilder name, StringBuilder xmlStr, ref UInt32 xmlSize);

		//public uint[] imArr = new uint[1024];

		/// <summary>
		///	This enum contains the values that describe the possible subtypes for the G4 parameter descriptions.<br>
		///	NOT_USED signifies an unused entry in a table.<br>
		///	S800_PRESSURE signifies a pressure entry in the S800 look-up-table format.<br>
		///	MILLITORR signifies a pressure entry in milliTorr.<br>
		///	TEMPERATURE signifies a temperature in degrees K * 10.<br>
		///	MILLIVOLTS signifies a voltage in millivolts.<br>
		///	VOLTS signifies a voltage in volts.<br>
		///	MILLIAMPS signifies a current in milliamps.<br>
		///	MILLISECONDS signifies a time in milliseconds.<br>
		///	INDEX signifies a list index entry with a string describing each possible value.<br>
		///	SPARSE_INDEX signifies a list index entry with a string describing each possible value and the value.<br>
		///	BITFIELD signifies a bitfield where each bit has a name.<br>
		///	NUMBER signifies a number with no units but with a minimum and a maximum.<br>
		///	NUMBER_WITH_UNITS signifies a number with a units name, a minimum, and a maximum.<br>
		///	IP_ADDR_ENTRY signifies an IP address stored as a 32-bit value.<br>
		///	REL_HUMID_ENTRY signifies a relative humidity in % * 100.<br>
		///	NANOMETERS signifies a distance in nanometers.<br></br>
		///	NANOSECONDS signifies a time in nanoseconds.<br></br>
		///	STRING signifies text consisting of bytes representing printable ASCII characters.<br></br>
		/// </summary>
		public enum UnitType
        {
            NOT_USED = 0, S800_PRESSURE, MILLITORR, TEMPERATURE_KELVIN_10, MILLIVOLTS, VOLTS, MILLIAMPS, MILLISECONDS,
            INDEX, SPARSE_INDEX, BITFIELD, NUMBER, NUMBER_WITH_UNITS, IP_ADDR_ENTRY, REL_HUMID_ENTRY,
            NANOMETERS, NANOSECONDS, STRING
        };

		/// <summary>
		/// ScanInterfaces causes all plugins to return their interface scan.
		/// The result is compiled in an XML string.
		/// </summary>
		/// <param name="forceRescan">forces rescanning of the interfaces, otherwise returns results from last scan</param>
		/// <returns></returns>
		public string ScanInterfaces(bool forceRescan)
        {
            UInt32 xmlSize = 10000;
            StringBuilder Data = new StringBuilder((Int32)xmlSize);
            int err = LibScanInterfaces(Data, ref xmlSize, true);
            string myStr = Data.ToString();
            return myStr;
        }
        /// <summary>
        /// GetErrorString returns a string containing the error description for a given ereror number.
        /// </summary>
        /// <param name="errID">error number</param>
        /// <returns>description string (empty when error)</returns>
        public string GetErrorString(UInt32 errID)
        {
            UInt32 strSize = 10000;
            StringBuilder Data = new StringBuilder((Int32)strSize);
            Data.Append("test");
            int err = LibGetErrorString(errID, Data, ref strSize);
            string myStr = Data.ToString();
            return myStr;
        }
        public int OpenCamera(string name, string plugin)
        {
            StringBuilder myName = new StringBuilder(100);
            StringBuilder myPlugin = new StringBuilder(100);
            myName.Append(name);
            myPlugin.Append(plugin);
            int camHandle = LibOpenCamera(myName, myPlugin);
            return camHandle;
        }
        public int CloseCamera(int camHandle)
        {
            int error = LibCloseCamera(camHandle);
            return error;
        }
        public int CameraHwReset(string name, string plugin)
        {
            StringBuilder myName = new StringBuilder(100);
            StringBuilder myPlugin = new StringBuilder(100);
            myName.Append(name);
            myPlugin.Append(plugin);
            int camHandle = LibCameraHwReset(myName, myPlugin);
            return camHandle;
        }
        public string GetXmlFile(int camHandle, string fileName)
        {
            UInt32 xmlSize = 100000;
            StringBuilder xmlData = new StringBuilder((Int32)xmlSize);
            StringBuilder myName = new StringBuilder(100);
            myName.Append(fileName);
            int error = LibGetXmlFile(camHandle, myName, xmlData, ref xmlSize);
            string myStr = xmlData.ToString();
            return myStr;
        }
        public string GetStatusNames(int camHandle)
        {
            UInt32 retSize = 100000;
            StringBuilder retData = new StringBuilder((Int32)retSize);
            int error = LibGetStatusNames(camHandle, retData, retSize);
            string myStr = retData.ToString();
            return myStr;
        }
        public int GetStatus(int camHandle)
        {
            return LibGetStatus(camHandle);
        }
        public int GetStatusItem(int camHandle, string displayName, ref string valStr, ref UnitType uType, ref string unitStr, ref string stepStr)
        {
            StringBuilder dName = new StringBuilder(100);
            dName.Append(displayName);
            UInt32 vSize = 100;
            StringBuilder vStr = new StringBuilder((Int32)vSize);
            UInt32 uSize = 100;
            StringBuilder uStr = new StringBuilder((Int32)uSize);
            UInt32 sSize = 100;
            StringBuilder sStr = new StringBuilder((Int32)sSize);
            UInt16 uuType = 0;
            int error = LibGetStatusItem(camHandle, dName, vStr, vSize, ref uuType, uStr, uSize, sStr, sSize);
            valStr = vStr.ToString();
            uType = (UnitType)uuType;
            unitStr = uStr.ToString();
            stepStr = sStr.ToString();
            return error;
        }


		public int GetStatusPulldownItem(int camHandle, string displayName, UInt32 PulldownIndex, ref string PulldownValStr, ref string PulldownName)
		{
			int error = 0;
			StringBuilder dName = new StringBuilder(100);
			dName.Append(displayName);
			UInt32 vSize = 100;
			StringBuilder vStr = new StringBuilder((Int32)vSize);
			UInt32 nSize = 100;
			StringBuilder nStr = new StringBuilder((Int32)nSize);
			error = LibGetStatusPulldownItem(camHandle, dName, PulldownIndex, vStr, vSize, nStr, nSize);
			PulldownValStr = vStr.ToString();
			PulldownName = nStr.ToString();
			return error;
		}

		public int GetStatusBitField(int camHandle, string displayName, ref string maskStr, ref string bitDisplay)
		{
			int error = 0;
			StringBuilder dName = new StringBuilder(100);
			dName.Append(displayName);
			UInt32 vmaskStrLen = 100;
			StringBuilder vmaskStr = new StringBuilder((Int32)vmaskStrLen);
			UInt32 vbitDisplayLen = 100;
			StringBuilder vbitDisplay = new StringBuilder((Int32)vbitDisplayLen);
			error = LibGetStatusBitField(camHandle, dName, vmaskStr, vmaskStrLen, vbitDisplay, vbitDisplayLen);
			maskStr = vmaskStr.ToString();
			bitDisplay = vbitDisplay.ToString();
			return error;
		}


		public string GetStatusValue(int camHandle, string displayName)
        {
            StringBuilder dName = new StringBuilder(100);
            dName.Append(displayName);
            UInt32 vSize = 100;
            StringBuilder vStr = new StringBuilder((Int32)vSize);
            int error = LibGetStatusValue(camHandle, dName, vStr, vSize);
            if (error != 0) return "ERROR : " + GetErrorString((uint)error);
			else return vStr.ToString();
        }
		public int GetParameters(int camHandle)
		{
			return LibGetParameters(camHandle);
		}
		public int UpdateParameters(int camHandle)
		{
			return LibUpdateParameters(camHandle);
		}
		public int GetParameterItem(int camHandle, string displayName, ref string valStr, ref string minStr, ref string maxStr, ref UnitType uType, ref string unitStr, ref string stepStr)
        {
            StringBuilder dName = new StringBuilder(100);
            dName.Append(displayName);
            UInt32 vSize = 100;
            StringBuilder vStr = new StringBuilder((Int32)vSize);
            UInt32 mminSize = 100;
            StringBuilder mminStr = new StringBuilder((Int32)mminSize);
            UInt32 mmaxSize = 100;
            StringBuilder mmaxStr = new StringBuilder((Int32)mmaxSize);
            UInt32 uSize = 100;
            StringBuilder uStr = new StringBuilder((Int32)uSize);
            UInt32 sSize = 100;
            StringBuilder sStr = new StringBuilder((Int32)sSize);
            UInt16 uuType = 0;
            int error = LibGetParameterItem(camHandle, dName, vStr, vSize, mminStr, mminSize, mmaxStr, mmaxSize, ref uuType, uStr, uSize, sStr, sSize);
            valStr = vStr.ToString();
            minStr = mminStr.ToString();
            maxStr = mmaxStr.ToString();
            uType = (UnitType)uuType;
            unitStr = uStr.ToString();
            stepStr = sStr.ToString();
            return error;
        }
		public int GetParameterBitField(int camHandle, string displayName, ref string maskStr, ref string bitDisplay)
		{
			int error = 0;
			StringBuilder dName = new StringBuilder(100);
			dName.Append(displayName);
			UInt32 vmaskStrLen = 100;
			StringBuilder vmaskStr = new StringBuilder((Int32)vmaskStrLen);
			UInt32 vbitDisplayLen = 100;
			StringBuilder vbitDisplay = new StringBuilder((Int32)vbitDisplayLen);
			error = LibGetParameterBitField(camHandle, dName, vmaskStr, vmaskStrLen, vbitDisplay, vbitDisplayLen);
			maskStr = vmaskStr.ToString();
			bitDisplay = vbitDisplay.ToString();
			return error;
		}
		public int GetParameterPulldownItem(int camHandle, string displayName, UInt32 PulldownIndex, ref string PulldownValStr, ref string PulldownName)
		{
			int error = 0;
			StringBuilder dName = new StringBuilder(100);
			dName.Append(displayName);
			UInt32 vSize = 100;
			StringBuilder vStr = new StringBuilder((Int32)vSize);
			UInt32 nSize = 100;
			StringBuilder nStr = new StringBuilder((Int32)nSize);
			error = LibGetParameterPulldownItem(camHandle, dName, PulldownIndex, vStr, vSize, nStr, nSize);
			PulldownValStr = vStr.ToString();
			PulldownName = nStr.ToString();
			return error;
		}
		public string GetParameterNames(int camHandle)
        {
            UInt32 retSize = 100000;
            StringBuilder retData = new StringBuilder((Int32)retSize);
            int error = LibGetParameterNames(camHandle, retData, retSize);
            string myStr = retData.ToString();
            return myStr;
        }
        public string GetParameterValue(int camHandle, string displayName)
        {
            StringBuilder dName = new StringBuilder(100);
            dName.Append(displayName);
            UInt32 vSize = 100;
            StringBuilder vStr = new StringBuilder((Int32)vSize);
            int error = LibGetParameterValue(camHandle, dName, vStr, vSize);
			//string errorStr;
			//if (error != 0) errorStr = GetErrorString((uint)error);
            if (error != 0) return "ERROR : " + GetErrorString((uint)error);
			else return vStr.ToString();
        }
        public int SetParameterValue(int camHandle, string displayName, string valueStr)
        {
            StringBuilder dName = new StringBuilder(100);
            dName.Append(displayName);
            StringBuilder vStr = new StringBuilder(100);
            vStr.Append(valueStr);
            int error = LibSetParameterValue(camHandle, dName, vStr);
            return error;
        }
        public int SendParameters(int camHandle)
        {
            return LibSendParameters(camHandle);
        }
        public int GetImageSize(int camHandle, ref UInt32 imgSerLen, ref UInt32 imgParLen, ref UInt32 is16, ref UInt32 nSerCCD, ref UInt32 nParCCD, ref UInt32 nSerSect, ref UInt32 nParSect)
        {
            return LibGetImageSize(camHandle, ref imgSerLen, ref imgParLen, ref is16, ref nSerCCD, ref nParCCD, ref nSerSect, ref nParSect);
        }
        public string IssueCommand(int camHandle, string postName, string argStr)
        {
            StringBuilder pName = new StringBuilder(100);
            pName.Append(postName);
            StringBuilder aStr = new StringBuilder(1000);
            aStr.Append(argStr);
            UInt32 retSize = 100000;
            StringBuilder retStr = new StringBuilder((Int32)retSize);
            int error = LibIssueCommand(camHandle, pName, aStr, retStr, retSize);
            if (error != 0) return "ERROR : " + GetErrorString((uint)error);
			else return retStr.ToString();
        }

		public string GetSoftwareSummary(int camHandle)
		{
			UInt32 retSize = 100000;
			StringBuilder retData = new StringBuilder((Int32)retSize);
			int error = LibGetSoftwareSummary(camHandle, retData, retSize);
			string myStr = retData.ToString();
			return myStr;
		}
		public int PrepareAcqFilm(int camHandle, UInt16 SerLen, UInt16 ParLen, ushort[] imArr, UInt16 nBuffers, uint is16)
		{
			int error = LibPrepareAcqFilm(camHandle, SerLen, ParLen, imArr, nBuffers, is16);
			return 0;
		}
		public int PrepareAcqFilm(int camHandle, UInt16 SerLen, UInt16 ParLen, UInt16 nBuffers, uint is16)
		{
			imArr = new short[SerLen * ParLen * nBuffers];
			imArr[0] = 42;
			imArr[1] = 256;
			int size = Marshal.SizeOf(imArr[0]) * imArr.Length;
			if (pnt != IntPtr.Zero)		// free buffer if there was one
			{
				Marshal.FreeHGlobal(pnt);
				pnt = IntPtr.Zero;
			}
			pnt = Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(imArr, 0, pnt, imArr.Length);		// Copy the array to unmanaged memory.
				int myerror = LibPrepareAcqFilm(camHandle, SerLen, ParLen, pnt, nBuffers, is16);
			}
			finally
			{
			}
			return 0;
		}
		public int EndAcq(int camHandle, bool forceAbort)
        {
            return LibEndAcq(camHandle,forceAbort);
        }
        public int AcqStatus(int camHandle, ref UInt16 pctRead, ref UInt32 CurrentFrame, ref UInt32 ErrorFlags)
        {
            return LibAcqStatus(camHandle, ref pctRead, ref CurrentFrame, ref ErrorFlags);
        }
        public int ExposureRemainingTime(int camHandle, ref UInt32 remainingTime)
        {
            return LibExposureRemainingTime(camHandle, ref remainingTime);
        }
		public ushort[] GetUshortImage()
		{
			if (pnt != IntPtr.Zero)
			{
				Marshal.Copy(pnt, imArr, 0, imArr.Length);
				Marshal.FreeHGlobal(pnt);
				pnt = IntPtr.Zero;
			}
			ushort[] imArrU = new ushort[imArr.Length];
			imArrU = Array.ConvertAll<short, ushort>(imArr, delegate (short item) { return (ushort)item; });
			return imArrU;
		}
	}
}
