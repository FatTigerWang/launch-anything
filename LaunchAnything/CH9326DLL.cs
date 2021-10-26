using System;
using System.Runtime.InteropServices;

namespace LaunchAnything
{
    class CH9326DLL
    {
        /*	BAUD rate setting	*/
        public const int B300 = 0x01;
        public const int B600 = 0x02;
        public const int B1200 = 0x03;
        public const int B2400 = 0x04;
        public const int B4800 = 0x05;
        public const int B9600 = 0x06;
        public const int B14400 = 0x07;
        public const int B19200 = 0x08;
        public const int B28800 = 0x09;
        public const int B38400 = 0x0A;
        public const int B57600 = 0x0B;
        public const int B76800 = 0x0C;
        public const int B115200 = 0x0D;
        /* Parity define	*/
        public const int P_ODD = 0x01;        //奇校验
        public const int P_EVEN = 0x02;		//偶校验
        public const int P_SPC = 0x03;		//空白位
        public const int P_NONE = 0x04;		//无校验
        /*	Data bits define		*/
        public const int BIT_5 = 0x01;
        public const int BIT_6 = 0x02;
        public const int BIT_7 = 0x03;
        public const int BIT_8 = 0x04;
        /* Stop bits define	*/
        public const int STOP_1 = 0x01;
        public const int STOP_2 = 0x02;
        //初始化DLL库。
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326DllInt")]
        public static extern bool CH9326DllInt();

        //CH9326GetHidGuid获取HID的GUID，用法参考CH9326DBG
        //等价于HidD_GetHidGuid,参考MSDN
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetHidGuid")]
        public static extern void CH9326GetHidGuid(ref Guid HidGuid);

        // 根据设备的VID/PID来打开设备
        // CH9326OpenDevice成功返回设备句柄，失败返回INVALID_HANDLE_VALUE
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326OpenDevice")]
        public static extern IntPtr CH9326OpenDevice(ushort USB_VID,                //设备的VID
                                ushort USB_PID                              //设备的PID
                                );

        // 根据设备链接名来打开设备
        // CH9326OpenDevice成功返回设备句柄，失败返回INVALID_HANDLE_VALUE
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326OpenDevicePath")]
        public static extern IntPtr CH9326OpenDevicePath(string DevicePath       //设备链接名
                                );

        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetDevicePath")]
        public static extern bool CH9326GetDevicePath(uint DevIndex,         //设备索引号，表示系统中第几个HID设备
                                            byte[] DevicePath,       //函数返回设备链接名
                                            uint DevicePathLen     //DevicePath的缓冲区长度
                                            );
        //等价于HidD_GetAttributes获取VID,PID，版本号,参考MSDN。成功返回TRUE,失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetAttributes")]
        public static extern uint CH9326GetAttributes(IntPtr hDeviceHandle,           //设备句柄
                                ref ushort pVID,                              //返回设备的VID
                                ref ushort pPID,                              //返回设备的PID
                                ref ushort pVer                               //返回设备的版本号
                                );

        //获取CH9326ReadData和CH9326WriteData一次读写的最大数据长度
        //成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetBufferLen")]
        public static extern uint CH9326GetBufferLen(IntPtr hDeviceHandle,          //设备句柄
                                ref ushort InputReportLen,                  //返回接受数据长度
                                ref ushort OutputReportLen                  //返回发送数据长度
                                );

        //设置CH9326ReadData和CH9326WriteData读写超时间隔，单位毫秒
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326SetTimeOut")]
        public static extern void CH9326SetTimeOut(IntPtr hDeviceHandle,                //设备句柄
                                uint ReadTimeOut,                           //设置读数据超时间隔
                                uint SendTimeOut                            //设置写数据超时间隔
                                );

        //读一包数据，最大长度为CH9326GetBufferLen返回的InputReportLen-2，由芯片固件决定.
        //CH9326读一包数据最大长度为31.成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ReadData")]
        public static extern uint CH9326ReadData(IntPtr hDeviceHandle,              //设备句柄
                                byte[] ReadBuffer,                      //指向接受数据缓冲区
                                ref uint pReadLen,                          //返回实际读到的数据长度
                                IntPtr hEventObject                     //等待事件句柄
                                );

        //写一包数据，最大长度为CH9326GetBufferLen返回的OutputReportLen-2，由芯片固件决定.
        //CH9326写一包数据最大长度为31.成功返回TRUE，失败返回false	
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326WriteData")]
        public static extern uint CH9326WriteData(IntPtr hDeviceHandle,           //设备句柄
                                byte[] SendBuffer,                          //指向发送数据缓冲区
                                uint SentLen,                           //要发送数据的长度
                                IntPtr hEventObject                     //等待事件句柄
                                );
        /*
        ucRate	: 1=300(ucRate为1时对应波特率300),2=600,3=1200,4=2400,5=4800,6=9600(默认值),7=14400,8=19200,9=28800,10=38400,11=57600,12=76800,13=115200
        ucCheck	: 1=奇校验,2=偶校验,3=空白位,4=无校验(默认值)
        ucStop	: 1=1位(默认值)，2=2位
        ucData	：1=5位，2=6位，3=7位，4=8位(默认值)
        */
        //设置波特率，ucInterval为接受数据的超时间隔，如果接受到的数据少于31个，
        //并且超过ucInterval时间间隔还没接受到数据，CH9326将打包数据上传给PC
        //成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326SetRate")]
        public static extern uint CH9326SetRate(IntPtr hDeviceHandle,               //设备句柄
                                byte ucRate,                                //波特率
                                byte ucCheck,                           //校验模式
                                byte ucStop,                                //停止位
                                byte ucData,                             //数据位
                                byte ucInterval                          //HID接收时的时间间隔，0x10=3MS((默认值)),0x20约等于6MS，,0x30约等于9MS
                                );

        /*
        CH9326InitThreadData,CH9326GetThreadDataLen,CH9326ClearThreadData,
        CH9326ReadThreadData,CH9326StopThread
        上面这组函数主要是为了方便读数据操作，建议客户使用这种方式读数据。
        CH9326InitThreadData:创建一个线程和私有堆，在线程内部的调用CH9326ReadData读数据，读到的数据缓冲在内部的私有堆中。
        CH9326GetThreadDataLen:获取内部缓冲区中数据的长度。
        CH9326ReadThreadData:读内部缓冲区中数据。
        CH9326ClearThreadData:清空缓冲区所有数据。
        CH9326StopThread:停止内部线程读取，并清空内部缓冲区。
        */

        //初始化线程和私有堆，成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326InitThreadData")]
        public static extern uint CH9326InitThreadData(IntPtr hDeviceHandle   //设备句柄
                                );

        //查询数据，返回线程缓冲区中有多少数据
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetThreadDataLen")]
        public static extern uint CH9326GetThreadDataLen(IntPtr hDeviceHandle  //设备句柄
                                );
        //读线程中数据，成功返回TRUE，失败返回false
        //注意：ReadLen参数在调用之前为要读取的数据长度，长度不能超过ReadBuffer缓冲的大小，否则会发送内存访问溢出
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ReadThreadData")]
        public static extern uint CH9326ReadThreadData(IntPtr hDeviceHandle,  //HID设备句柄
                                byte[] ReadBuffer,                          //缓冲区首地址
                                ref uint ReadLen                      //调用之前为要读取的数据长度，返回实际读到的数据长度
                                );
        //清空缓冲区数据
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ClearThreadData")]
        public static extern void CH9326ClearThreadData(IntPtr hDeviceHandle  //设备句柄
                                );

        //停止内部线程读取，并清空内部缓冲区
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326StopThread")]
        public static extern void CH9326StopThread(IntPtr hDeviceHandle       //设备句柄
                                );

        //设置产品描述符信息，请谨慎使用，特别要注意设置新VID/PID时，
        //打开设备时，CH9326OpenDevice对应的VID/PID要做相应的修改，否则打开失败。
        //成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326SetDeviceCfg")]
        public static extern uint CH9326SetDeviceCfg(IntPtr hDeviceHandle,    //设备句柄
                                ushort wVID,                        //设置新的VID
                                ushort wPID,                        //设置新的PID
                                ushort wPower,                      //设置新的设备电流,50~150 
                                byte[] strManufacturer,                      //设置新的厂商描述符
                                uint uMSlen,                      //设置新的厂商描述符长度，最大29
                                byte[] strProduct,                           //设置新的产品描述符
                                uint uPSlen,                      //设置新的产品描述符长度，最大29
                                byte[] strSerialNo,                          //设置新的产品序列号
                                uint uSSlen                       //设置新的产品序列号长度，最大29
                                );

        //设置默认波特率，上电后的默认波特率，不设置时CH9326默认波特率是9600，参数含义参考CH9326SetRate
        //成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ResetToDefault")]
        public static extern uint CH9326ResetToDefault(IntPtr hDeviceHandle,        //设备句柄
                                byte ucRate,                                //波特率
                                byte ucCheck,                           //校验模式
                                byte ucStop,                                //停止位
                                byte ucData,                             //数据位
                                byte ucInterval                          //HID接受时的时间间隔，0x10=3MS((默认值)),0x20约等于6MS，,0x30约等于9MS
                                );

        //恢复出厂默认设置，使CH9326SetDeviceCfg和CH9326SetDefaultRate的设置无效。
        //成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ResetToDefault")]
        public static extern uint CH9326ResetToDefault(IntPtr hDeviceHandle 	        //设备句柄
                                );

        //设置16位IO口方向，成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326SetIODir")]
        public static extern uint CH9326SetIODir(IntPtr hDeviceHandle,                //设备句柄
                                ushort usDir                                //16位IO口的输入输出设置，对应位：1是输出，0是输入                           
                                );

        //设置16位IO口的电平，成功返回TRUE，失败返回false
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326WriteIOData")]
        public static extern uint CH9326WriteIOData(IntPtr hDeviceHandle,             //设备句柄
                                ushort usData                               //16位IO口输出的高低电平设置，对应位：1是输出高电平，0是输出地电平
                                );

        //获取16位IO口的引脚电平，成功返回TRUE，失败返回false  

        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326ReadIOData")]
        public static extern uint CH9326ReadIOData(IntPtr hDeviceHandle,              //设备句柄
                                ref ushort pData                               //获取到的16位IO口的引脚电平
                                );

        //获取厂商字符串描述符，成功返回TRUE，失败返回false 
        //等价于HidD_GetManufacturerString，参考MSDN 
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetManufacturerString")]
        public static extern uint CH9326GetManufacturerString(IntPtr hDeviceHandle,  //设备句柄
                                byte[] Buffer,                                      //缓冲区首地址
                                uint BufferLength                          //缓冲区大小
                                );

        //获取产品字符串描述符，成功返回TRUE，失败返回false 
        //等价于HidD_GetProductString，参考MSDN 
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetProductString")]
        public static extern uint CH9326GetProductString(IntPtr hDeviceHandle,        //设备句柄
                                byte[] Buffer,                                      //缓冲区首地址
                                uint BufferLength                          //缓冲区大小
                                );

        //获取序列号字符串描述符，成功返回TRUE，失败返回false
        //等价于HidD_GetSerialNumberString，参考MSDN
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326GetSerialNumberString")]
        public static extern uint CH9326GetSerialNumberString(IntPtr hDeviceHandle,   //设备句柄
                                byte[] Buffer,                                      //缓冲区首地址
                                uint BufferLength                          //缓冲区大小
                                );

        //关闭设备
        [DllImport("libs/CH9326DLL.dll", EntryPoint = "CH9326CloseDevice")]
        public static extern void CH9326CloseDevice(IntPtr hDeviceHandle              //设备句柄
                                );
    }
}
