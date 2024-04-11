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

		static IntPtr nxattr;
		static int windowHeight;
		static string[] name; //要查找的窗口名称
		static string SetFile;
		static DateTime sj;
		static bool OK,wOK;
		static int w, h;
		static int use_number; //调节计数

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
					MessageBox.Show("自动调节 窗口大小 启动");
					Console.WriteLine("出错" + wOK);
					//开机自启动 注册表
					RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
					registryKey.SetValue("Windows_Set_Tools", Application.ExecutablePath);

					sj = Properties.Settings.Default.UPDATE;
					Console.WriteLine(sj.ToString());

					//释放配置文件
					SetFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Windows_Set_Tools.Ar";
					if (!File.Exists(SetFile))
					{
						File.WriteAllText(SetFile, "");
					}

					string[] temps = File.ReadAllLines(SetFile);
					name = temps[0].Split(',');
					w = int.Parse(temps[1]);
					h = int.Parse(temps[2]);

					//自动调节 窗口大小
					System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Enabled = true, Interval = 1 };
					timer.Start();
					timer.Tick += Timer_Tick;

					//报告点击量
					//System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer { Enabled = true, Interval = 1 };
					//timer2.Start();
					//timer2.Tick += Timer2_Tick;
					Application.Run();
				}
				// 程序已经运行的情况，则弹出消息提示并终止此次运行  
				else
				{
					MessageBox.Show("服务程序已经在运行中，请不要重复启动", "Arong", MessageBoxButtons.OK, MessageBoxIcon.Information);
					// 终止此进程并为基础操作系统提供指定的退出代码。  
					Environment.Exit(1);
				}
			}
		}

		//private static void Timer2_Tick(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		string[] temp = File.ReadAllLines(SetFile);
		//		if (temp.Length != 0)
		//		{
		//			if (File.Exists(temp[3]) && File.GetLastWriteTime(temp[3]) > sj && OK == false)
		//			{
		//				string[] temp2 = File.ReadAllLines(temp[3],Encoding.GetEncoding("gb2312"));
		//				if (temp2.Length != 0)
		//				{
		//					string[] temp3 = temp2[temp2.Length - 1].Split(',');
		//					info info = new info(temp3[0] + "   使用完成");
		//					info.Show();
		//					sj = File.GetLastWriteTime(temp[3]);
		//					Properties.Settings.Default.UPDATE = File.GetLastWriteTime(temp[3]);
		//					Properties.Settings.Default.Save();
		//					OK = true;
		//					Console.WriteLine(sj.ToString());
		//				}
		//			}
		//		}
		//		OK = false;
		//	}
		//	catch 
		//	{
		//		Console.WriteLine("出错");
		//	}
		//}

		private static void Timer_Tick(object sender, EventArgs e)
		{
			string[] temps = File.ReadAllLines(SetFile);
			name = temps[0].Split(',');
			w = int.Parse(temps[1]);
			h = int.Parse(temps[2]);

			for (int i = 0; i < name.Length; i++)
			{
				nxattr = FindWindow(null, name[i]);
				FindAttr(nxattr);
			}
		}

		static void FindAttr(IntPtr nxattr)
		{
			if (nxattr != IntPtr.Zero )
			{
				if (wOK == false)
				{
					RECT windowRect;
					GetWindowRect(nxattr, out windowRect);
					windowHeight = windowRect.Bottom - windowRect.Top;
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
