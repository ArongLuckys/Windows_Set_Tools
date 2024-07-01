using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Windows_Set_Tools
{
	internal static class Program
	{

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

		[DllImport("user32.dll")]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public static int windowHeight;
		public static IntPtr nxattr;
		public static string SetFile;
		public static bool wOK;
		public static List<AppInfo> appInfos; //要查找的全部应用程序变量


		/// <summary>
		/// 要变更的程序的结构体
		/// </summary>
		public struct AppInfo
		{
			public string name;
			public int wid;
			public int hig;
		}

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool createNew;
			// 在此方法返回时，如果创建了局部互斥体（即，如果 name 为 null 或空字符串）或指定的命名系统互斥体，则包含布尔值 true；  
			// 如果指定的命名系统互斥体已存在，则为false  
			using (Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
			{
				if (createNew)
				{
					//开机自启动 注册表
					RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
					registryKey.SetValue("Windows_Set_Tools", Application.ExecutablePath);

					//释放配置文件
					SetFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Windows_Set_Tools.Ar";
					if (!File.Exists(SetFile))
					{
						File.WriteAllText(SetFile, "");
					}

					//得到所有应用程序的结构体数据
					appInfos = new List<AppInfo>();
					List<string> temps = new List<string>();
					temps.AddRange(File.ReadAllLines(SetFile));

					for (int i = 0; i < temps.Count; i++)
					{
						if (temps[i].Length == 0)
						{
							temps.RemoveAt(i);
							i = 0;
						}
					}

					if (temps.Count == 0)
					{
						MessageBox.Show("没有设置要缩放的窗口，请检查后再启动程序", "Arong 承接NX二次开发(微信:13267623440)");
						Environment.Exit(1);
					}

					for (int i = 0; i < temps.Count; i++)
					{
						AppInfo appInfo = new AppInfo();

						string[] temp = temps[i].Split(',');

						if (temp.Length == 3)
						{
							try
							{
								appInfo.name = temp[0];
								appInfo.wid = int.Parse(temp[1]);
								appInfo.hig = int.Parse(temp[2]);
							}
							catch
							{
								MessageBox.Show("数据为空或者非整数，请检查后再启动程序", "Arong 承接NX二次开发(微信:13267623440)");
								Environment.Exit(1);
							}
							appInfos.Add(appInfo);
						}
						else
						{
							MessageBox.Show("第"+ i + "行\n" + temps[i] + "\n该行所需的数据缺少或者错误，请检查后再启动程序", "Arong 承接NX二次开发(微信:13267623440)");
							Environment.Exit(1);
						}

					}

					//自动调节 窗口大小
					System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Enabled = true, Interval = 1 };
					timer.Tick += Timer_Tick;
					timer.Start();

					MessageBox.Show("自动调节窗口大小 启动完成\n将持续为你自适应窗口大小", "Arong 承接NX二次开发(微信:13267623440)");
					Application.Run();
				}
				// 程序已经运行的情况，则弹出消息提示并终止此次运行  
				else
				{
					MessageBox.Show("服务程序已经在运行中，请不要重复启动", "Arong 承接NX二次开发(微信:13267623440)", MessageBoxButtons.OK, MessageBoxIcon.Information);
					// 终止此进程并为基础操作系统提供指定的退出代码。  
					Environment.Exit(1);
				}
			}
		}

		/// <summary>
		/// 监测窗口状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Timer_Tick(object sender, EventArgs e)
		{
			for (int i = 0; i < appInfos.Count; i++)
			{
				nxattr = FindWindow(null, appInfos[i].name);
				FindAttr(nxattr, appInfos[i].wid, appInfos[i].hig);
			}
		}

		/// <summary>
		/// 调整窗口大小
		/// </summary>
		/// <param name="nxattr"></param>
		static void FindAttr(IntPtr nxattr,int w,int h)
		{
			if (nxattr != IntPtr.Zero )
			{
				if (wOK == false)
				{
					RECT windowRect;
					GetWindowRect(nxattr, out windowRect);
					//windowHeight = windowRect.Bottom - windowRect.Top;
					MoveWindow(nxattr, windowRect.Left, windowRect.Top, w, h, true);
					wOK = true;
				}
			}
			else
			{
				wOK = false;
			}
			GC.Collect();
		}
	}
}
