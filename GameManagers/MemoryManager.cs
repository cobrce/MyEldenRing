using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MyEldenRing.GameManagers
{
    internal partial class MemoryManager
    {
        private IntPtr hProcess = IntPtr.Zero;
        private nuint GameMan;


        private const uint PROCEESS_ALL_ACCESS = 0x1F0FFF;
        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_READWRITE = 0x04;

        public bool IsOpen { get; private set; }
        internal void Close()
        {
            IsOpen = false;
            GameMan = 0;
            if (hProcess != IntPtr.Zero)
            {
                CloseHandle(hProcess);
                hProcess = IntPtr.Zero;
            }

        }

        internal bool Open(Process process, string pattern, int pointerOffset)
        {

            if (IsOpen)
                return false; ;

            hProcess = OpenProcess(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION | PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ | PROCESS_ACCESS_RIGHTS.PROCESS_VM_WRITE, false, process.Id);
            if (hProcess == IntPtr.Zero)
                return false;

            if (!FindAobAndExtractPointer(process.MainModule.BaseAddress, pattern, pointerOffset, out nuint gameManPtr))
            {
                Close();
                return false;
            }

            byte[] buffer = new byte[8];


            if (!ReadProcessMemory(hProcess, (nint)gameManPtr, buffer, 8, out int read))
            {
                Close();
                return false;
            }
            GameMan = (nuint)BitConverter.ToUInt64(buffer);
            if (GameMan==0)
            {
                Close();
                return false;    
            }
            IsOpen = true;
            return true;
        }

        internal bool Open(Process process)
        {
            const string gameManAOB = "48 8B 05 ?? ?? ?? ?? 80 B8 ?? ?? ?? ?? 0D 0F 94 C0 C3";
            const int pointerOffset = 3;
            return Open(process, gameManAOB, pointerOffset);
        }

        internal bool QuitOutGame()
        {
            if (!IsOpen || GameMan == 0)
                return false;

            byte[] data = { 1 };
            if (!WriteProcessMemory(hProcess, (nint)GameMan + 0x10, data, 1, out int _))
                return false;

            Thread.Sleep(100);

            data[0] = 0;
            if (!WriteProcessMemory(hProcess, (nint)GameMan + 0x10, data, 1, out _))
                return false;

            return true;
        }


        private bool FindAobAndExtractPointer(IntPtr baseAddress, string pattern, int pointerOffset, out nuint pointer)
        {
            pointer = default;
            if (!FindAOB(baseAddress, pattern, out nuint foundPointer))
                return false;

            byte[] offsetBytes = new byte[4];
            if (!ReadProcessMemory(this.hProcess, (nint)foundPointer + pointerOffset, offsetBytes, 4, out int _))
                return false;

            pointer = (nuint)((nint)foundPointer + pointerOffset + BitConverter.ToInt32(offsetBytes) + 4);
            return true;
        }

        struct AOB
        {
            internal List<byte> data;
            internal List<byte> mask;
            public AOB()
            {
                data = [];
                mask = [];
            }

        };

        private bool FindAOB(nint baseAddress, string pattern, out nuint result)
        {
            result = 0;

            AOB aob = AOBFromPattern(pattern);
            if (hProcess == nint.Zero)
                return false;

            // TODO : convert AOB to byte array

            nint currentAddress = baseAddress;
            while (VirtualQueryEx(hProcess, currentAddress, out MEMORY_BASIC_INFORMATION info,
                                    Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))))
            {
                currentAddress = (nint)(info.BaseAddress + info.RegionSize);
                if (info.State != MEM_COMMIT)
                    continue;

                byte[] buffer = new byte[(int)info.RegionSize];
                if (!ReadProcessMemory(hProcess, (IntPtr)info.BaseAddress, buffer, (IntPtr)buffer.Length, out int _))
                    continue;


                for (int i = 0; i < buffer.Length - aob.data.Count; i++)
                {
                    bool found = true;
                    for (int j = 0; j < aob.data.Count; j++)
                    {
                        if (aob.mask[j] == 0)
                            continue;
                        if (buffer[i + j] != aob.data[j])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        result = (nuint)(info.BaseAddress + (ulong)i);
                        return true;
                    }
                }

            }
            return false;
        }

        private static AOB AOBFromPattern(string pattern)
        {
            AOB aob = new();
            var split = pattern.Split(" ");

            foreach (var slice in split)
            {
                if (slice.Contains('?'))
                {
                    aob.mask.Add(0);
                    aob.data.Add(0);
                }
                else
                {
                    aob.mask.Add(1);
                    if (byte.TryParse(slice, NumberStyles.HexNumber, null, out byte result))
                        aob.data.Add(result);
                    else
                        aob.data.Add(0);
                }
            }
            return aob;
        }

        [LibraryImport("KERNEL32.dll", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In] byte[] lpBuffer,
            nint nSize,
            [Optional] out int lpNumberOfBytesWritten);

        [LibraryImport("KERNEL32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe partial bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            nint nSize,
            [Optional] out int lpNumberOfBytesRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public ulong RegionSize;
            public int State;
            public ulong Protect;
            public ulong Type;
        }

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        private static partial IntPtr OpenProcess(PROCESS_ACCESS_RIGHTS dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [LibraryImport("KERNEL32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CloseHandle(nint hObject);

        [Flags]
        public enum PROCESS_ACCESS_RIGHTS : uint
        {
            PROCESS_TERMINATE = 0x00000001,
            PROCESS_CREATE_THREAD = 0x00000002,
            PROCESS_SET_SESSIONID = 0x00000004,
            PROCESS_VM_OPERATION = 0x00000008,
            PROCESS_VM_READ = 0x00000010,
            PROCESS_VM_WRITE = 0x00000020,
            PROCESS_DUP_HANDLE = 0x00000040,
            PROCESS_CREATE_PROCESS = 0x00000080,
            PROCESS_SET_QUOTA = 0x00000100,
            PROCESS_SET_INFORMATION = 0x00000200,
            PROCESS_QUERY_INFORMATION = 0x00000400,
            PROCESS_SUSPEND_RESUME = 0x00000800,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000,
            PROCESS_SET_LIMITED_INFORMATION = 0x00002000,
            PROCESS_ALL_ACCESS = 0x001FFFFF,
            PROCESS_DELETE = 0x00010000,
            PROCESS_READ_CONTROL = 0x00020000,
            PROCESS_WRITE_DAC = 0x00040000,
            PROCESS_WRITE_OWNER = 0x00080000,
            PROCESS_SYNCHRONIZE = 0x00100000,
            PROCESS_STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        }
    }
}
