using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attr_Tools
{
	public partial class info : Form
	{
		public info(string str)
		{
			InitializeComponent();
			this.Location = new Point(Screen.PrimaryScreen.Bounds.Width/2 - 150, 2);
			label3.Text = str;
		}

		/// <summary>
		/// 2秒关闭
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void info_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Pen p = new Pen(Color.Black, 2);
			g.DrawRectangle(p, 0, 0, 300, 30);
		}
	}
}
