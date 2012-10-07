using System;
using System.Linq;
using System.Collections.Generic;

namespace ManagedFSP
{
	public class JobHandle
	{
        public IntPtr LineHandle;
        public UInt32 DeviceId;
        public IntPtr FaxStatusCompletionPort;
        public UIntPtr FaxStatusCompletionKey;
        public bool Status;
        public FaxDev.FaxStatusId StatusId;
        public FaxDev.FAX_SEND SendInfo;
        public FaxDev.FaxSendCallback FaxSendCallback;
        public FaxDev.FAX_RECEIVE ReceiveInfo;
		
		public static Dictionary<int, JobHandle> JobHandles = new Dictionary<int, JobHandle>();
		private static object lockHandle = new object();

		public static IntPtr Add(JobHandle jobHandle)
		{
			lock (lockHandle)
			{
                int key = 0;

                if (JobHandles.Count > 0)
                {
                    key = JobHandles.Max(j => j.Key);
                }
                
				JobHandles.Add(++key, jobHandle);
                return new IntPtr(key);
		    }
		}

		public static void Remove(int key)
		{
			lock (lockHandle)
			{
				JobHandles.Remove(key);
			}
		}
	}
}
