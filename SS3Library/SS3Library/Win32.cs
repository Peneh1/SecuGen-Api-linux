using System;
using System.Runtime.InteropServices;

namespace WebAPI1toN.SecuSearchSDK3
{
    internal class Win32
    {

        // Linux alternatives
        [DllImport("libdl.so.2", EntryPoint = "dlopen", CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libdl.so.2", EntryPoint = "dlsym", CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        private const int RTLD_NOW = 2; // Flags for dlopen

        public static IntPtr LoadLibrary(string lpLibFileName)
        {
            var handle = dlopen(lpLibFileName, RTLD_NOW);
            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Unable to load library: {lpLibFileName}");
            }
            return handle;
        }

        public static bool FreeLibrary(IntPtr hModule)
        {
            return dlclose(hModule) == 0;
        }

        public static IntPtr GetProcAddress(IntPtr hModule, string lpProcName)
        {
            var procAddress = dlsym(hModule, lpProcName);
            if (procAddress == IntPtr.Zero)
            {
                throw new EntryPointNotFoundException($"Procedure not found: {lpProcName}");
            }
            return procAddress;
        }

    }
}
