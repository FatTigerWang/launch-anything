using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaunchAnything
{
    public partial class Form1 : Form
    {

        IntPtr m_hHID = new IntPtr(-1);//设备句柄
        UInt16 m_wIODir = 0;//用来改变IO口输入输出状态的参数
        UInt16 m_wIOData = 0;//用来改变IO口拉高拉低的参数
        bool lower = false;
        public const uint GENERIC_READ = 0x80000000;//winapi函数CreateFile相关参数
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const int DBT_DEVICEARRIVAL = 0x8000;  // 检测到新设备
        public const int DBT_DEVICEREMOVEPENDING = 0x8003; // 设备将要移除
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  // 设备已经移除
        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005; // device interface class
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const uint DEVICE_ARRIVAL = 3;	// 设备插入事件,已经插入
        public const uint DEVICE_REMOVE_PEND = 1;	// 设备将要拔出
        public const uint DEVICE_REMOVE = 0;    // 设备拔出事件,已经拔出



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // 检测并得到设备句柄
            InitialDevice();

            // I/O 1  输出
            m_wIODir |= 0x01;

            //设置 I/O 方向
            if (CH9326DLL.CH9326SetIODir(m_hHID, m_wIODir) == 0)
            {
                MessageBox.Show("设置I/O方向失败");
                return;
            }

            // 设置 I/O 1 为低电平
            m_wIOData |= 0xFFFE;

            // 写 I/O
            if (CH9326DLL.CH9326WriteIOData(m_hHID, Convert.ToUInt16(m_wIOData & m_wIODir)) == 0)
            {
                MessageBox.Show("写I/O失败");
                return;
            }

            Task.Run(() =>
            {
                ushort data = 0;
                while (CH9326DLL.CH9326ReadIOData(m_hHID, ref data) is not 0)
                {
                    var readData = Convert.ToUInt16(data & 0x000F);
                    if ((readData & 0x02) > 0)
                    {
                        // 高电平
                        if (!lower)
                        {
                            lower = true;
                            this.button2.Invoke(() =>
                            {
                                this.button2.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                    else
                    {
                        if (lower)
                        {
                            lower = false;
                            this.button2.Invoke(() =>
                            {
                                this.button2.BackColor = System.Drawing.Color.Red;
                            });
                        }
                    }
                    Thread.Sleep(100);
                }
                MessageBox.Show("读I/O失败");
            });

        }

        public void InitialDevice()
        {
            uint deviceNo = 0;
            byte[] buf = new byte[1024];
            WinAPI.SetLastError(0);
            while (WinAPI.GetLastError() != ERROR_NO_MORE_ITEMS)
            {
                if (!CH9326DLL.CH9326GetDevicePath(deviceNo, buf, Convert.ToUInt32(buf.Length)))
                {
                    deviceNo++;
                    continue;
                }
                //打开设备,并获得设备句柄
                var hHID = WinAPI.CreateFile(buf, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                if (hHID == new IntPtr(-1))
                {
                    deviceNo++;
                    continue;
                }
                WinAPI.CloseHandle(hHID);
                var strpath = Encoding.Default.GetString(buf);//得到设备路径
                if (strpath.Contains("VID_1A86&PID_E010"))
                {
                    m_hHID = CH9326DLL.CH9326OpenDevicePath(strpath);
                    break;
                }
                deviceNo++;
            }
            if (m_hHID == new IntPtr(-1))
            {
                MessageBox.Show("未检测到设备", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
