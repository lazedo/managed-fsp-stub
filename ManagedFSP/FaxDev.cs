using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.Interop;
using RGiesecke.DllExport;

namespace ManagedFSP
{
	public static class FaxDev
    {
        #region delegate

        public delegate void FaxLineCallback(
				IntPtr FaxHandle,
				UInt32 hDevice,
				UInt32 dwMessage,
				UIntPtr dwInstance,
				UIntPtr dwParam1,
				UIntPtr dwParam2,
				UIntPtr dwParam3
		);

		public delegate bool FaxServiceCallback(
				IntPtr FaxHandle,
				UInt32 DeviceId,
				UIntPtr Param1,
				UIntPtr Param2,
				UIntPtr Param3
		);

		public delegate bool FaxSendCallback(
				IntPtr FaxHandle,
				IntPtr CallHandle,
				UInt32 Reserved1,
				UInt32 Reserved2
		);

		#endregion

		#region enum & struct

        public enum FaxStatusId : uint
        {
                FS_INITIALIZING     = 0x20000000
            ,   FS_DIALING          = 0x20000001
            ,   FS_TRANSMITTING     = 0x20000002
            ,   FS_RECEIVING        = 0x20000004
            ,   FS_COMPLETED        = 0x20000008
            ,   FS_HANDLED          = 0x20000010
            ,   FS_LINE_UNAVAILABLE = 0x20000020
            ,   FS_BUSY             = 0x20000040
            ,   FS_NO_ANSWER        = 0x20000080
            ,   FS_BAD_ADDRESS      = 0x20000100
            ,   FS_NO_DIAL_TONE     = 0x20000200
            ,   FS_DISCONNECTED     = 0x20000400
            ,   FS_FATAL_ERROR      = 0x20000800
            ,   FS_NOT_FAX_CALL     = 0x20001000
            ,   FS_CALL_DELAYED     = 0x20002000
            ,   FS_CALL_BLACKLISTED = 0x20004000
            ,   FS_USER_ABORT       = 0x20200000
            ,   FS_ANSWERED         = 0x20800000
        }

        public static Dictionary<FaxStatusId, uint> StatusResultMap = new Dictionary<FaxStatusId, uint>() 
        { 
            { FaxStatusId.FS_INITIALIZING, ResultWin32.ERROR_SUCCESS },
            { FaxStatusId.FS_DIALING, ResultWin32.ERROR_SUCCESS },
            { FaxStatusId.FS_TRANSMITTING, ResultWin32.ERROR_SUCCESS },
            { FaxStatusId.FS_COMPLETED, ResultWin32.ERROR_SUCCESS },

            { FaxStatusId.FS_LINE_UNAVAILABLE, (uint)LineErr.LINEERR_INUSE},
            { FaxStatusId.FS_NO_ANSWER, ResultWin32.ERROR_CTX_MODEM_RESPONSE_NO_CARRIER },
            { FaxStatusId.FS_NO_DIAL_TONE, ResultWin32.ERROR_CTX_MODEM_RESPONSE_NO_DIALTONE },
            { FaxStatusId.FS_DISCONNECTED, (uint)LineErr.LINEERR_DISCONNECTED },
            { FaxStatusId.FS_BUSY, (uint)LineErr.LINEERR_CALLUNAVAIL },
            { FaxStatusId.FS_NOT_FAX_CALL, ResultWin32.ERROR_CTX_MODEM_RESPONSE_VOICE },
            { FaxStatusId.FS_FATAL_ERROR, ResultWin32.ERROR_GEN_FAILURE },

            { FaxStatusId.FS_BAD_ADDRESS, (uint)LineErr.LINEERR_INVALADDRESS },
            { FaxStatusId.FS_CALL_BLACKLISTED, (uint)LineErr.LINEERR_ADDRESSBLOCKED },
            { FaxStatusId.FS_USER_ABORT, (uint)LineErr.LINEERR_USERCANCELLED }
        };

        [StructLayout(LayoutKind.Sequential)]
		public struct FAX_SEND
		{
			public UInt32 SizeOfStruct;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string FileName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string CallerName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string CallerNumber;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string ReceiverName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string ReceiverNumber;
            [MarshalAs(UnmanagedType.Bool)]
            public bool Branding;
			public IntPtr CallHandle;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public UInt32[] Reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FAX_RECEIVE
		{
			public UInt32 SizeOfStruct;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string FileName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string ReceiverName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string ReceiverNumber;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public UInt32[] Reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FAX_DEV_STATUS
		{
			public UInt32 SizeOfStruct;
			public UInt32 StatusId;
			public UInt32 StringId;
			public UInt32 PageCount;
			public IntPtr CSI;
			public IntPtr CallerId;
			public IntPtr RoutingInfo;
			public UInt32 ErrorCode;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public UInt32[] Reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct LINEMESSAGE
		{
			public UInt32 hDevice;
			public UInt32 dwMessageID;
			public UIntPtr dwCallbackInstance;
			public UIntPtr dwParam1;
			public UIntPtr dwParam2;
			public UIntPtr dwParam3;
		}

		#endregion

		#region DllImport

        [DllImport("kernel32.dll")]
        static extern void SetLastError(uint dwErrCode);

		[DllImport("kernel32.dll", SetLastError = false)]
		public static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);

		[DllImport("kernel32.dll")]
        public static extern bool PostQueuedCompletionStatus(IntPtr CompletionPort,
           uint dwNumberOfBytesTransferred, UIntPtr dwCompletionKey,
           IntPtr lpOverlapped);

        public static bool PostStatusHelper(JobHandle jobHandle)
        {
            IntPtr FaxStatus = HeapAlloc(GlobalHandle.HeapHandle, 0, (UIntPtr)Marshal.SizeOf(typeof(FAX_DEV_STATUS)));
            
            if (FaxStatus == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            var sFaxStatus = (FAX_DEV_STATUS)Marshal.PtrToStructure(FaxStatus, typeof(FAX_DEV_STATUS));
            sFaxStatus.SizeOfStruct = (uint)Marshal.SizeOf(typeof(FAX_DEV_STATUS));
            sFaxStatus.StatusId = (uint)jobHandle.StatusId;
            sFaxStatus.ErrorCode = StatusResultMap[jobHandle.StatusId];

            return PostQueuedCompletionStatus(jobHandle.FaxStatusCompletionPort, sFaxStatus.SizeOfStruct, jobHandle.FaxStatusCompletionKey, FaxStatus);
        }

		#endregion

		#region DllExport

        [DllExport("FaxDevVirtualDeviceCreation", CallingConvention = CallingConvention.Winapi)]
        public static bool VirtualDeviceCreation(
				ref UInt32 DeviceCount,
                [MarshalAs(UnmanagedType.LPWStr)]
				StringBuilder DeviceNamePrefix,
				ref UInt32 DeviceIdPrefix,
				IntPtr CompletionPort,
				UIntPtr CompletionKey
		)
		{
			DeviceCount = (uint)FSPConfig.DeviceCount;
            DeviceNamePrefix.Append(FSPConfig.DeviceNamePrefix);
            DeviceIdPrefix = (uint)FSPConfig.DeviceIdPrefix;
			GlobalHandle.LineStatusCompletionPort = CompletionPort;
			GlobalHandle.LineStatusCompletionKey = CompletionKey;
			return true;
		}

        [DllExport("FaxDevInitialize", CallingConvention = CallingConvention.Winapi)]
        public static bool Initialize(
				IntPtr LineAppHandle,
				IntPtr HeapHandle,
				ref IntPtr LineCallbackFunction,
				IntPtr ServiceCallbackFunction
		)
		{
			GlobalHandle.LineAppHandle = LineAppHandle;
			GlobalHandle.HeapHandle = HeapHandle;
            LineCallbackFunction = Marshal.GetFunctionPointerForDelegate(new FaxLineCallback(FaxLineCallbackHandler));
			return true;
		}

        [DllExport("FaxDevStartJob", CallingConvention = CallingConvention.Winapi)]
        public static bool StartJob(
				IntPtr LineHandle,
				UInt32 DeviceId,
				ref IntPtr FaxHandle,
				IntPtr CompletionPort,
				UIntPtr CompletionKey
		)
		{
            var jobHandle = new JobHandle()
            {
                LineHandle = LineHandle,
                DeviceId = DeviceId,
                FaxStatusCompletionPort = CompletionPort,
                FaxStatusCompletionKey = CompletionKey,
                Status = true,
                StatusId = FaxStatusId.FS_INITIALIZING
            };
			FaxHandle = JobHandle.Add(jobHandle);
            PostStatusHelper(jobHandle);
			return true;
		}

        [DllExport("FaxDevSend", CallingConvention.Winapi)]
        public static bool Send(
				IntPtr FaxHandle,
				IntPtr FaxSend,
				IntPtr FaxSendCallback
		)
		{
			var jobHandle = JobHandle.JobHandles[(int)FaxHandle];
            jobHandle.SendInfo = (FAX_SEND)Marshal.PtrToStructure(FaxSend, typeof(FAX_SEND));

            jobHandle.StatusId = FaxStatusId.FS_DIALING;
            PostStatusHelper(jobHandle);

            if (FaxSendCallback != IntPtr.Zero)
            {
                jobHandle.FaxSendCallback = (FaxSendCallback)Marshal.GetDelegateForFunctionPointer(FaxSendCallback, typeof(FaxSendCallback));
            }

            jobHandle.StatusId = FaxStatusId.FS_TRANSMITTING;
            PostStatusHelper(jobHandle);

			jobHandle.Status = new Random().Next(2) == 1;
            jobHandle.StatusId = jobHandle.Status ? FaxStatusId.FS_COMPLETED : (FaxStatusId)StatusResultMap.Keys.ToList()[new Random().Next(4, StatusResultMap.Count)];
            PostStatusHelper(jobHandle);

            if (jobHandle.StatusId != FaxStatusId.FS_COMPLETED)
            {
                SetLastError((uint)StatusResultMap[jobHandle.StatusId]);
            }

            return jobHandle.Status;
		}

        [DllExport("FaxDevReceive", CallingConvention.Winapi)]
        public static bool Receive(
				IntPtr FaxHandle,
				IntPtr CallHandle,
				IntPtr FaxReceive
		)
		{
			throw new NotImplementedException();
		}

        [DllExport("FaxDevReportStatus", CallingConvention.Winapi)]
        public static bool ReportStatus(
				IntPtr FaxHandle,
				IntPtr FaxStatus,
				UInt32 FaxStatusSize,
				ref UInt32 FaxStatusSizeRequired
		)
		{
			var jobHandle = JobHandle.JobHandles[(int)FaxHandle];

			if (FaxStatusSize == 0 && FaxStatus == IntPtr.Zero)
			{
				FaxStatusSizeRequired = (uint)Marshal.SizeOf(typeof(FAX_DEV_STATUS));
				return true;
			}

            var sFaxStatus = (FAX_DEV_STATUS)Marshal.PtrToStructure(FaxStatus, typeof(FAX_DEV_STATUS));
            sFaxStatus.SizeOfStruct = (uint)Marshal.SizeOf(typeof(FAX_DEV_STATUS));
            sFaxStatus.StatusId = (uint)jobHandle.StatusId;
            sFaxStatus.ErrorCode = StatusResultMap[jobHandle.StatusId];
            
            string csi = jobHandle.SendInfo.ReceiverNumber + " " + jobHandle.SendInfo.ReceiverName + '\0';
            sFaxStatus.CSI = new IntPtr(FaxStatus.ToInt64() + sFaxStatus.SizeOfStruct);
            Marshal.Copy(csi.ToCharArray(), 0, sFaxStatus.CSI, csi.Length);
            string callerId = jobHandle.SendInfo.CallerNumber + " " + jobHandle.SendInfo.CallerName + '\0';
            sFaxStatus.CallerId = new IntPtr(sFaxStatus.CSI.ToInt64() + (Marshal.SizeOf(typeof(char)) * csi.Length));
            Marshal.Copy(callerId.ToCharArray(), 0, sFaxStatus.CallerId, callerId.Length);
            return true;
		}

        [DllExport("FaxDevAbortOperation", CallingConvention.Winapi)]
        public static bool AbortOperation(
				IntPtr FaxHandle
		)
		{
            var jobHandle = JobHandle.JobHandles[(int)FaxHandle];
            jobHandle.StatusId = FaxStatusId.FS_USER_ABORT;
            return PostStatusHelper(jobHandle);
        }

        [DllExport("FaxDevEndJob", CallingConvention.Winapi)]
        public static bool EndJob(
				IntPtr FaxHandle
		)
		{
			JobHandle.Remove((int)FaxHandle);
			return true;
		}

        [DllExport("FaxDevShutdown", CallingConvention.Winapi)]
        public static int Shutdown()
		{
			return 0;
		}
        
        #endregion

        #region callback handler

        public static void FaxLineCallbackHandler(
                IntPtr FaxHandle,
                UInt32 hDevice,
                UInt32 dwMessage,
                UIntPtr dwInstance,
                UIntPtr dwParam1,
                UIntPtr dwParam2,
                UIntPtr dwParam3
        )
        {
        }

        public static bool FaxServiceCallbackHandler(
                IntPtr FaxHandle,
                UInt32 DeviceId,
                UIntPtr Param1,
                UIntPtr Param2,
                UIntPtr Param3
        )
        {
            return true;
        }

        public static bool FaxSendCallbackHandler(
                IntPtr FaxHandle,
                IntPtr CallHandle,
                UInt32 Reserved1,
                UInt32 Reserved2
        )
        {
            return true;
        }

        #endregion
    }
}
