using System;
using System.Runtime.InteropServices;
using System.Text;


namespace AOT_Test
{

    public class SharedMemory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenFileMapping(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

        const uint FILE_MAP_ALL_ACCESS = 0xF001F;
        const uint PAGE_READWRITE = 0x04;
        const uint FILE_MAP_READ = 0x0004;

        public static bool WriteToSharedMemory(string sharedMemoryName, string data)
        {
            var len = (uint)(data.Length * sizeof(char));
            IntPtr hMapFile = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, PAGE_READWRITE, 0, len, sharedMemoryName);
            if (hMapFile == IntPtr.Zero)
            {
                return false;
            }

            IntPtr pBuf = MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, len);
            if (pBuf == IntPtr.Zero)
            {
                Console.WriteLine($"#2 {Marshal.GetLastSystemError()}");
                CloseHandle(hMapFile);
                return false;
            }

            Marshal.Copy(data.ToCharArray(), 0, pBuf, data.Length);

            UnmapViewOfFile(pBuf);
            //CloseHandle(hMapFile);

            return true;
        }

        public static string ReadFromSharedMemory(string sharedMemoryName)
        {
            IntPtr hMapFile = OpenFileMapping(FILE_MAP_READ, false, sharedMemoryName);
            if (hMapFile == IntPtr.Zero)
            {
                return string.Empty;
            }

            IntPtr pBuf = MapViewOfFile(hMapFile, FILE_MAP_READ, 0, 0, 0);
            if (pBuf == IntPtr.Zero)
            {
                CloseHandle(hMapFile);
                return string.Empty;
            }

            string data = Marshal.PtrToStringUni(pBuf); // 假设数据是 Unicode 字符串

            UnmapViewOfFile(pBuf);
            CloseHandle(hMapFile);

            return data;
        }
    }

}
