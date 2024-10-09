using System;
using System.Runtime.InteropServices;

namespace WebAPI1toN.SecuSearchSDK3
{
    internal class Win32
    {
#if WINDOWS
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
#else
        // Linux alternatives can go here
        public static IntPtr LoadLibrary(string lpLibFileName)
        {
            // Implement logic for loading a library on Linux
            throw new NotImplementedException("LoadLibrary not implemented for Linux.");
        }

        public static bool FreeLibrary(IntPtr hModule)
        {
            // Implement logic for freeing a library on Linux
            throw new NotImplementedException("FreeLibrary not implemented for Linux.");
        }

        public static IntPtr GetProcAddress(IntPtr hModule, string lpProcName)
        {
            // Implement logic for getting a procedure address on Linux
            throw new NotImplementedException("GetProcAddress not implemented for Linux.");
        }
#endif
    }
}
