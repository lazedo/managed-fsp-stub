using System.Reflection;
using System.Runtime.InteropServices;

namespace ManagedFSP
{
	public static class FSPConfig
	{
		public static string ProviderGuid;
        public static string ProviderPath;
        public static string ProviderName;
		public static string ProviderVersion;
		public static int DeviceCount;
		public static string DeviceNamePrefix;
		public static int DeviceIdPrefix;

		static FSPConfig()
		{
            DeviceCount = 1;
            DeviceIdPrefix = 1;
            ProviderVersion = "0x00010000";
            var assembly = Assembly.GetExecutingAssembly();
            ProviderPath = assembly.Location;
            ProviderName = ((AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title;
            ProviderGuid = "{" + ((GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value.ToUpper() + "}";
            DeviceNamePrefix = ((AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
		}
	}
}
