using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;

namespace Lemony.SystemInfo
{
    ///  
    /// 系统信息类 - 获取CPU、内存、磁盘、进程信息 
    ///  
    public class SystemInfo
    {
        private int m_ProcessorCount = 0;   //CPU个数 
        private PerformanceCounter pcCpuLoad;   //CPU计数器 
        private long m_PhysicalMemory = 0;   //物理内存 

        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 268435456;
        private const int WS_BORDER = 8388608;

        #region AIP声明 
        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);
        #endregion

        #region 构造函数 
        ///  
        /// 构造函数，初始化计数器等 
        ///  
        public SystemInfo()
        {
            //初始化CPU计数器 
            pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.MachineName = ".";
            pcCpuLoad.NextValue();

            //CPU个数 
            m_ProcessorCount = Environment.ProcessorCount;

            //获得物理内存 
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    m_PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }
        }
        #endregion

        #region CPU个数 
        /// <summary>
        /// 获取CPU个数 
        /// </summary>
        public int ProcessorCount
        {
            get
            {
                return m_ProcessorCount;
            }
        }
        #endregion

        #region CPU占用率 
        /// <summary>
        ///获取CPU占用率 
        /// </summary>
        public float CpuLoad
        {
            get
            {
                return pcCpuLoad.NextValue();
            }
        }
        #endregion

        #region 可用内存 
        /// <summary>
        /// 获取可用内存 
        /// </summary>
        public long MemoryAvailable
        {
            get
            {
                long availablebytes = 0;
                //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PerfRawData_PerfOS_Memory"); 
                //foreach (ManagementObject mo in mos.Get()) 
                //{ 
                //    availablebytes = long.Parse(mo["Availablebytes"].ToString()); 
                //} 
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                    {
                        availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                    }
                }
                return availablebytes;
            }
        }
        #endregion

        #region 物理内存 
        /// <summary>
        /// 获取物理内存 
        /// </summary>
        public long PhysicalMemory
        {
            get
            {
                return m_PhysicalMemory;
            }
        }
        #endregion

        #region 获得进程列表 
        /// <summary>
        /// 获得进程列表 
        /// </summary>
        /// <returns></returns>
        public List<ProcessInfo> GetProcessInfo()
        {
            List<ProcessInfo> pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcesses();
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo()
                    {
                        Id = instance.Id,
                        ProcessName = instance.ProcessName,
                        TotalMilliseconds = instance.TotalProcessorTime.TotalMilliseconds,
                        WorkingSet64 = instance.WorkingSet64,
                        FileName = instance.MainModule.FileName
                    });
                }
                catch { }
            }
            return pInfo;
        }
        /// <summary>
        /// 获得特定进程信息 
        /// </summary>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public List<ProcessInfo> GetProcessInfo(string ProcessName)
        {
            List<ProcessInfo> pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo(){
                        Id =instance.Id,
                        ProcessName=instance.ProcessName,
                        TotalMilliseconds=instance.TotalProcessorTime.TotalMilliseconds,
                        WorkingSet64=instance.WorkingSet64,
                        FileName=instance.MainModule.FileName});
                }
                catch { }
            }
            return pInfo;
        }
        #endregion

        #region 结束指定进程 
        /// <summary>
        /// 结束指定进程 
        /// </summary>
        /// <param name="pid"></param>
        public static void EndProcess(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch { }
        }
        #endregion

        #region 查找所有应用程序标题 
        /// <summary>
        /// 查找所有应用程序标题 
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public static List<string> FindAllApps(int Handle)
        {
            List<string> Apps = new List<string>();
            int hwCurr;
            hwCurr = GetWindow(Handle, GW_HWNDFIRST);
            while (hwCurr > 0)
            {
                int IsTask = (WS_VISIBLE | WS_BORDER);
                int lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
                bool TaskWindow = ((lngStyle & IsTask) == IsTask);
                if (TaskWindow)
                {
                    int length = GetWindowTextLength(new IntPtr(hwCurr));
                    StringBuilder sb = new StringBuilder(2 * length + 1);
                    GetWindowText(hwCurr, sb, sb.Capacity);
                    string strTitle = sb.ToString();
                    if (!string.IsNullOrEmpty(strTitle))
                    {
                        Apps.Add(strTitle);
                    }
                }
                hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
            }
            return Apps;
        }
        #endregion
        #region 获取磁盘信息
        public static List<DriveInfo> GetDrives()
        {
            System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("共有【{0}】个驱动器\n", allDrives.Length);
            List<DriveInfo> DrivesList = new List<DriveInfo>();
            foreach (System.IO.DriveInfo f in allDrives)
            {
                //sb.AppendFormat("驱动器名：{0}\n", f.Name);
                //sb.AppendFormat("是否就绪：{0}\n", f.IsReady ? "是" : "否");
                //sb.AppendFormat("z卷标：{0}\n", f.VolumeLabel);
                //sb.AppendFormat("驱动类型：{0}\n", f.DriveType);
                //sb.AppendFormat("分区格式：{0}\n", f.DriveFormat);
                //sb.AppendFormat("根目录：{0}\n", f.RootDirectory);
                //sb.AppendFormat("总大小：{0}\n", CalculateSpace(f.TotalSize));
                //sb.AppendFormat("总剩余空间：{0}\n", CalculateSpace(f.TotalFreeSpace));
                //sb.AppendFormat("可用剩余空间：{0}\n", CalculateSpace(f.AvailableFreeSpace));
                if (f.IsReady)
                {
                    DrivesList.Add(new DriveInfo()
                    {
                        Name = f.Name,
                        IsReady = f.IsReady ? "是" : "否",
                        VolumeLabel = f.VolumeLabel,
                        DriveFormat = f.DriveFormat,
                        TotalSize = CalculateSpace(f.TotalSize),
                        TotalFreeSpace = CalculateSpace(f.TotalFreeSpace),
                    });
                }
            }
            return DrivesList;
        }

        /// <summary>
        /// 转换单位
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static string CalculateSpace(long b)
        {
            double r = 0;
            Int32 step = 1024;
            if (b < step) return string.Format("{0}B", b);

            r = b / step;
            if (r < step) return String.Format("{0}KB", r);

            r = b / Math.Pow(step, 2);
            if (r < step) return string.Format("{0:N2}MB", r);

            r = b / Math.Pow(step, 3);
            if (r < step) return string.Format("{0:N2}GB", r);

            r = b / Math.Pow(step, 4);
            if (r < step) return string.Format("{0:N2}TB", r);

            return b.ToString();
        }
        #endregion
    }

    public class  ProcessInfo{
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public double  TotalMilliseconds { get; set; }
        public long WorkingSet64 { get; set; }
        public string FileName { get; set; }

    }
    public class DriveInfo
    {
        /// <summary>
        /// 驱动器名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否就绪
        /// </summary>
        public string IsReady { get; set; }
        /// <summary>
        /// 卷标
        /// </summary>
        public string VolumeLabel { get; set; }
        /// <summary>
        /// 分区格式
        /// </summary>
        public string DriveFormat { get; set; }
        /// <summary>
        /// 总大小
        /// </summary>
        public string TotalSize { get; set; }
        /// <summary>
        /// 总剩余空间
        /// </summary>
        public string TotalFreeSpace { get; set; }
    }
}