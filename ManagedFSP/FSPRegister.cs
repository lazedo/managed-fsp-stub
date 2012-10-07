using System;
using System.Linq;
using FAXCOMEXLib;

namespace ManagedFSP
{
    public static class FSPRegister
    {
        public static void Main(string[] args)
        {
            var faxServer = new FaxServerClass();
            faxServer.Connect("");
            var faxProvider = GetExistingFaxProvider(faxServer);

            if (faxProvider == null)
            {
                FSPConfig.DeviceCount = args != null && args.Length > 0 && int.TryParse(args[0], out FSPConfig.DeviceCount) ? FSPConfig.DeviceCount : FSPConfig.DeviceCount;
                RegisterFSP(faxServer);
            }
            else
            {
                UnregisterFSP(faxServer, faxProvider);
            }
        }

        private static FaxDeviceProvider GetExistingFaxProvider(FaxServer faxServer)
        {
            FaxDeviceProvider existing = null;

            faxServer.GetDeviceProviders().Cast<FaxDeviceProvider>().ToList().ForEach(provider =>
            {
                Console.WriteLine("Existing fax provider {0} {1} {2} {3}", provider.UniqueName, provider.FriendlyName, provider.ImageName,
                    string.Format("{0}.{1}.{2}.{3}", provider.MajorVersion, provider.MajorBuild, provider.MinorVersion, provider.MinorBuild));
                if (provider.UniqueName == FSPConfig.ProviderGuid) existing = provider;
            });

            Console.WriteLine("{0} matching fax provider {1} {2} {3} {4}", existing != null ? "Found" : "No", FSPConfig.ProviderGuid,
                existing != null ? existing.FriendlyName : FSPConfig.ProviderName,
                existing != null ? existing.ImageName : FSPConfig.ProviderPath,
                existing != null ? 
                    string.Format("{0}.{1}.{2}.{3}", existing.MajorVersion, existing.MajorBuild, existing.MinorVersion, existing.MinorBuild) :
                    FSPConfig.ProviderVersion);

            return existing;
        }

        private static void RegisterFSP(FaxServer faxServer)
        {
            var providerVersion = Convert.ToUInt32(FSPConfig.ProviderVersion, 16);
            faxServer.RegisterDeviceProvider(FSPConfig.ProviderGuid, FSPConfig.ProviderName, FSPConfig.ProviderPath, "", (int)providerVersion);
            Console.WriteLine("Registering fax provider {0} {1} {2} {3}", FSPConfig.ProviderGuid, FSPConfig.ProviderName, FSPConfig.ProviderPath, FSPConfig.ProviderVersion);
        }

        private static void UnregisterFSP(FaxServer faxServer, FaxDeviceProvider faxProvider)
        {
            faxServer.UnregisterDeviceProvider(FSPConfig.ProviderGuid);
            Console.WriteLine("Unregistering fax provider {0} {1} {2} {3}", FSPConfig.ProviderGuid, faxProvider.FriendlyName, faxProvider.ImageName,
                string.Format("{0}.{1}.{2}.{3}", faxProvider.MajorVersion, faxProvider.MajorBuild, faxProvider.MinorVersion, faxProvider.MinorBuild));
        }
    }
}
