using System;
using System.Runtime.InteropServices;
namespace LaunchAnything
{
    //该结构体为例程中用来获取设备信息
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEV_BROADCAST_DEVICEINTERFACE
    {
        public uint dbcc_size;
        public uint dbcc_devicetype;
        public uint dbcc_reserved;
        public Guid dbcc_classguid;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] dbcc_name;
    }
    class WinAPI
    {

        ///以下部分函数为例程中可能用到的API函数，非CH9326DLL中的函数
        [DllImport("user32.dll", EntryPoint = "RegisterDeviceNotification")]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, ref DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, uint Flags);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32.dll", EntryPoint = "CreateFile")]
        public static extern IntPtr CreateFile(
              byte[] lpFileName,
              uint dwDesiredAccess,
              uint dwShareMode,
              IntPtr lpSecurityAttributes,
              uint dwCreationDisposition,
              uint dwFlagsAndAttributes,
              IntPtr hTemplateFile
            );

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern uint SetLastError(uint errorcode);

        [DllImport("kernel32.dll", EntryPoint = "CreateEvent")]
        public static extern IntPtr CreateEvent(
          IntPtr lpEventAttributes,
          bool bManualReset,
          bool bInitialState,
          string lpName
        );
        ////////////////到此结束/////////////


    }
}
