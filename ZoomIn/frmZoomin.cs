using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vbAccelerator.Components.HotKey;

namespace vbAccelerator.Samples.ZoomIn
{

	#region UnmanagedMethods
	internal class UnmanagedMethods
	{
		internal const UInt32 DSTINVERT = 0x0550009;
		internal const UInt32 SRCCOPY = 0xCC0020;	
		internal const UInt32 COLORONCOLOR = 0x3;

		[DllImport("gdi32")]
		internal static extern UInt32 SetStretchBltMode (IntPtr hdc, UInt32 nStretchMode );
		[DllImport("gdi32")]
		internal static extern UInt32 StretchBlt (IntPtr hdc, int x, int y, int nWidth , int nHeight , 
			IntPtr hSrcDC, int xSrc , int ySrc , int nSrcWidth , int nSrcHeight , UInt32 dwRop );
		[DllImport("gdi32")]
		internal static extern bool PatBlt(IntPtr hdc, int left, int top, int width, int height, UInt32 op);
		[DllImport("user32")]
		internal static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport("user32")]
		internal static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);
	}
	#endregion

	#region frmZoomIn
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmZoomIn : HotKeyForm
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem mnuEdit;
		private System.Windows.Forms.MenuItem mnuCopy;
		private System.Windows.Forms.MenuItem mnuEditSep1;
		private System.Windows.Forms.MenuItem mnuRefresh;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuSaveAs;
		private System.Windows.Forms.MenuItem mnuClose;
		private System.Windows.Forms.MenuItem mnuOptions;
		private System.Windows.Forms.MenuItem mnuAutoRefresh;
		private System.Windows.Forms.MenuItem mnuOptionsSep1;
		private System.Windows.Forms.MenuItem mnuAlwaysOnTop;
		private System.Windows.Forms.MenuItem mnuShowGrid;
		private System.Windows.Forms.MenuItem mnuAbsolute;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.MenuItem mnuStatusBar;
		private System.Windows.Forms.StatusBar sbrMain;
		private System.Windows.Forms.VScrollBar vscZoom;
		private System.Windows.Forms.StatusBarPanel pnlLocation;
		private System.Windows.Forms.StatusBarPanel pnlColor;
		private System.Windows.Forms.StatusBarPanel pnlZoom;
		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.MenuItem mnuFileSep2;
		private System.Windows.Forms.MenuItem mnuPrint;
		private System.Windows.Forms.MenuItem mnuFileSep1;
		private System.Windows.Forms.Timer tmrRefresh;

		#region Member Variables
		private int m_cxZoomed = 0;
		private int m_cyZoomed = 0;
		private int m_cxScreenMax = 0;
		private int m_cyScreenMax = 0;
		private int m_nZoom = 2;
		private bool buttonDown = false;
		private int lastX = 0;
		private int lastY = 0;
		private Bitmap currentBitmap = null;
		#endregion

		#region Constructor and Destructor
		public frmZoomIn()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			m_cxZoomed = this.ClientRectangle.Width;
			m_cyZoomed = this.ClientRectangle.Height;
		
			m_cxScreenMax = 12800;
			m_cyScreenMax = 10240;

			this.Paint += new PaintEventHandler(frmZoomIn_Paint);
			//this.Resize += new EventHandler(frmZoomIn_Resize);
			this.MouseDown += new MouseEventHandler(frmZoomIn_MouseDown);
			this.MouseMove += new MouseEventHandler(frmZoomIn_MouseMove);
			this.MouseUp += new MouseEventHandler(frmZoomIn_MouseUp);

			this.SetStyle(
				ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint |
				ControlStyles.Opaque | ControlStyles.ResizeRedraw,  true);
			
			pnlZoom.Text = String.Format("x{0}",m_nZoom);
			this.TopMost = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
			this.mnuMain = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuSaveAs = new System.Windows.Forms.MenuItem();
			this.mnuFileSep2 = new System.Windows.Forms.MenuItem();
			this.mnuClose = new System.Windows.Forms.MenuItem();
			this.mnuEdit = new System.Windows.Forms.MenuItem();
			this.mnuCopy = new System.Windows.Forms.MenuItem();
			this.mnuEditSep1 = new System.Windows.Forms.MenuItem();
			this.mnuRefresh = new System.Windows.Forms.MenuItem();
			this.mnuOptions = new System.Windows.Forms.MenuItem();
			this.mnuAutoRefresh = new System.Windows.Forms.MenuItem();
			this.mnuOptionsSep1 = new System.Windows.Forms.MenuItem();
			this.mnuAlwaysOnTop = new System.Windows.Forms.MenuItem();
			this.mnuShowGrid = new System.Windows.Forms.MenuItem();
			this.mnuStatusBar = new System.Windows.Forms.MenuItem();
			this.mnuAbsolute = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuAbout = new System.Windows.Forms.MenuItem();
			this.sbrMain = new System.Windows.Forms.StatusBar();
			this.pnlLocation = new System.Windows.Forms.StatusBarPanel();
			this.pnlColor = new System.Windows.Forms.StatusBarPanel();
			this.pnlZoom = new System.Windows.Forms.StatusBarPanel();
			this.vscZoom = new System.Windows.Forms.VScrollBar();
			this.mnuPrint = new System.Windows.Forms.MenuItem();
			this.mnuFileSep1 = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.pnlLocation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlZoom)).BeginInit();
			this.SuspendLayout();
			// 
			// tmrRefresh
			// 
			this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
			// 
			// mnuMain
			// 
			this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuFile,
																					this.mnuEdit,
																					this.mnuOptions,
																					this.mnuHelp});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuSaveAs,
																					this.mnuFileSep1,
																					this.mnuPrint,
																					this.mnuFileSep2,
																					this.mnuClose});
			this.mnuFile.Text = "&File";
			// 
			// mnuSaveAs
			// 
			this.mnuSaveAs.Index = 0;
			this.mnuSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuSaveAs.Text = "Save &As...";
			this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
			// 
			// mnuFileSep2
			// 
			this.mnuFileSep2.Index = 3;
			this.mnuFileSep2.Text = "-";
			// 
			// mnuClose
			// 
			this.mnuClose.Index = 4;
			this.mnuClose.Text = "&Close";
			this.mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
			// 
			// mnuEdit
			// 
			this.mnuEdit.Index = 1;
			this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuCopy,
																					this.mnuEditSep1,
																					this.mnuRefresh});
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuCopy
			// 
			this.mnuCopy.Index = 0;
			this.mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.mnuCopy.Text = "&Copy";
			this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
			// 
			// mnuEditSep1
			// 
			this.mnuEditSep1.Index = 1;
			this.mnuEditSep1.Text = "-";
			// 
			// mnuRefresh
			// 
			this.mnuRefresh.Index = 2;
			this.mnuRefresh.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.mnuRefresh.Text = "&Refresh";
			this.mnuRefresh.Click += new System.EventHandler(this.mnuRefresh_Click);
			// 
			// mnuOptions
			// 
			this.mnuOptions.Index = 2;
			this.mnuOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mnuAutoRefresh,
																					   this.mnuOptionsSep1,
																					   this.mnuAlwaysOnTop,
																					   this.mnuShowGrid,
																					   this.mnuStatusBar,
																					   this.mnuAbsolute});
			this.mnuOptions.Text = "&Options";
			// 
			// mnuAutoRefresh
			// 
			this.mnuAutoRefresh.Index = 0;
			this.mnuAutoRefresh.Text = "&Auto Refresh";
			this.mnuAutoRefresh.Click += new System.EventHandler(this.mnuAutoRefresh_Click);
			// 
			// mnuOptionsSep1
			// 
			this.mnuOptionsSep1.Index = 1;
			this.mnuOptionsSep1.Text = "-";
			// 
			// mnuAlwaysOnTop
			// 
			this.mnuAlwaysOnTop.Checked = true;
			this.mnuAlwaysOnTop.Index = 2;
			this.mnuAlwaysOnTop.Text = "Always On &Top";
			this.mnuAlwaysOnTop.Click += new System.EventHandler(this.mnuAlwaysOnTop_Click);
			// 
			// mnuShowGrid
			// 
			this.mnuShowGrid.Checked = true;
			this.mnuShowGrid.Index = 3;
			this.mnuShowGrid.Text = "Show &Grid When Zoomed";
			this.mnuShowGrid.Click += new System.EventHandler(this.mnuShowGrid_Click);
			// 
			// mnuStatusBar
			// 
			this.mnuStatusBar.Index = 4;
			this.mnuStatusBar.Text = "&Status Bar";
			this.mnuStatusBar.Click += new System.EventHandler(this.mnuStatusBar_Click);
			// 
			// mnuAbsolute
			// 
			this.mnuAbsolute.Index = 5;
			this.mnuAbsolute.Text = "Absolute &Positions";
			this.mnuAbsolute.Click += new System.EventHandler(this.mnuAbsolute_Click);
			// 
			// mnuHelp
			// 
			this.mnuHelp.Index = 3;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuAbout});
			this.mnuHelp.Text = "&Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Index = 0;
			this.mnuAbout.Text = "&About...";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// sbrMain
			// 
			this.sbrMain.Location = new System.Drawing.Point(0, 125);
			this.sbrMain.Name = "sbrMain";
			this.sbrMain.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																					   this.pnlLocation,
																					   this.pnlColor,
																					   this.pnlZoom});
			this.sbrMain.ShowPanels = true;
			this.sbrMain.Size = new System.Drawing.Size(200, 20);
			this.sbrMain.TabIndex = 1;
			this.sbrMain.Text = "statusBar1";
			// 
			// pnlLocation
			// 
			this.pnlLocation.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.pnlLocation.Text = "(2204,1000)";
			this.pnlLocation.Width = 75;
			// 
			// pnlColor
			// 
			this.pnlColor.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.pnlColor.Text = "(255,255,255)";
			this.pnlColor.Width = 85;
			// 
			// pnlZoom
			// 
			this.pnlZoom.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.pnlZoom.Text = "x4";
			this.pnlZoom.Width = 24;
			// 
			// vscZoom
			// 
			this.vscZoom.CausesValidation = false;
			this.vscZoom.Dock = System.Windows.Forms.DockStyle.Right;
			this.vscZoom.LargeChange = 2;
			this.vscZoom.Location = new System.Drawing.Point(184, 0);
			this.vscZoom.Maximum = 32;
			this.vscZoom.Minimum = 1;
			this.vscZoom.Name = "vscZoom";
			this.vscZoom.Size = new System.Drawing.Size(16, 125);
			this.vscZoom.TabIndex = 2;
			this.vscZoom.Value = 4;
			this.vscZoom.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vscZoom_Scroll);
			// 
			// mnuPrint
			// 
			this.mnuPrint.Index = 2;
			this.mnuPrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.mnuPrint.Text = "&Print...";
			this.mnuPrint.Click += new System.EventHandler(this.mnuPrint_Click);
			// 
			// mnuFileSep1
			// 
			this.mnuFileSep1.Index = 1;
			this.mnuFileSep1.Text = "-";
			// 
			// frmZoomIn
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 145);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.vscZoom,
																		  this.sbrMain});
			this.Menu = this.mnuMain;
			this.Name = "frmZoomIn";
			this.Text = "ZoomIn - C#";
			this.Load += new System.EventHandler(this.frmZoomIn_Load);
			((System.ComponentModel.ISupportInitialize)(this.pnlLocation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlZoom)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmZoomIn());
		}

		#region Form Events
		private void frmZoomIn_Load(object sender, System.EventArgs e)
		{
			this.HotKeyPressed += new HotKeyPressedEventHandler(hotKey_Pressed);

			HotKey hotKey = new HotKey("Copy to clipboard", Keys.C, 
				HotKey.HotKeyModifiers.MOD_CONTROL | HotKey.HotKeyModifiers.MOD_SHIFT);
			this.HotKeys.Add(hotKey);
		}

		private void frmZoomIn_MouseDown(object sender, MouseEventArgs m)
		{
			if ((m.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				buttonDown = true;
				DrawZoomRect(m.X, m.Y);
			}
		}

		private void frmZoomIn_MouseMove(object sender, MouseEventArgs m)
		{
			if (buttonDown)
			{
				DrawZoomRect();
				DrawZoomRect(m.X, m.Y);
				Point pt = new Point(m.X, m.Y);
				Point ptScreen = this.PointToScreen(pt);
				pnlLocation.Text = String.Format("({0},{1})", ptScreen.X, ptScreen.Y);
			}
			else
			{
				if (mnuAbsolute.Checked)
				{
					Point pt = new Point(lastX, lastY);
					Point ptScreen = this.PointToScreen(pt);
					ptScreen.X = Bound(ptScreen.X, m_cxZoomed / 2, m_cxScreenMax - (m_cxZoomed / 2));
					ptScreen.Y = Bound(ptScreen.Y, m_cyZoomed / 2, m_cyScreenMax - (m_cyZoomed / 2));
					int x = ptScreen.X - m_cxZoomed /2 + m.X/m_nZoom ;
					int y = ptScreen.Y - m_cyZoomed /2 + m.Y/m_nZoom;
					pnlLocation.Text = String.Format("({0},{1})", x, y);
				}
				else
				{
					pnlLocation.Text = String.Format("({0},{1})", m.X, m.Y);
				}
				if (currentBitmap != null)
				{
					Color pixelColor = currentBitmap.GetPixel(m.X, m.Y);
					this.pnlColor.Text = String.Format("({0},{1},{2})", pixelColor.R, pixelColor.G, pixelColor.B);
				}
			}
		}

		private void frmZoomIn_MouseUp(object sender, MouseEventArgs m)
		{
			if ((m.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if (buttonDown)
				{
					DrawZoomRect(m.X, m.Y);
					buttonDown = false;
				}
			}
			
		}

		private void frmZoomIn_Paint(object sender, PaintEventArgs p)
		{
			CalcZoomedSize();

			if (currentBitmap == null)
			{
				currentBitmap = new Bitmap(m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed);
			}
			else
			{
				if ((currentBitmap.Width != m_nZoom * m_cxZoomed) || 
					(currentBitmap.Height != m_nZoom * m_cyZoomed))
				{
					currentBitmap = new Bitmap(m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed);
				}
			}
			Graphics gfx = Graphics.FromImage((Image)currentBitmap);


			Point pt = new Point(lastX, lastY);
			Point ptScreen = this.PointToScreen(pt);
			ptScreen.X = Bound(ptScreen.X, m_cxZoomed / 2, m_cxScreenMax - (m_cxZoomed / 2));
			ptScreen.Y = Bound(ptScreen.Y, m_cyZoomed / 2, m_cyScreenMax - (m_cyZoomed / 2));

			IntPtr hdcScreen = UnmanagedMethods.GetDC(IntPtr.Zero);

			// Draw on form:
			IntPtr hdc = p.Graphics.GetHdc();

			UnmanagedMethods.SetStretchBltMode(hdc, UnmanagedMethods.COLORONCOLOR);
			UnmanagedMethods.StretchBlt(hdc, 
				0, 0, m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed,
				hdcScreen, 
				ptScreen.X - m_cxZoomed / 2, ptScreen.Y - m_cyZoomed /2, 
				m_cxZoomed, m_cyZoomed, 
				UnmanagedMethods.SRCCOPY);

			p.Graphics.ReleaseHdc(hdc);
			
			// Draw on off-screen bitmap:
			hdc = gfx.GetHdc();

			UnmanagedMethods.SetStretchBltMode(hdc, UnmanagedMethods.COLORONCOLOR);
			UnmanagedMethods.StretchBlt(hdc, 
				0, 0, m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed,
				hdcScreen, 
				ptScreen.X - m_cxZoomed / 2, ptScreen.Y - m_cyZoomed /2, 
				m_cxZoomed, m_cyZoomed, 
				UnmanagedMethods.SRCCOPY);

			gfx.ReleaseHdc(hdc);

			UnmanagedMethods.ReleaseDC(IntPtr.Zero, hdcScreen);

			DrawGrid(p.Graphics);
		}	
		#endregion
		
		#region Miscellaneous Events
		private void hotKey_Pressed(object sender, HotKeyPressedEventArgs e)
		{
			Copy();
		}

		private void vscZoom_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			m_nZoom = vscZoom.Value;
			pnlZoom.Text = String.Format("x{0}",m_nZoom);			
			Invalidate();
		}

		private void tmrRefresh_Tick(object sender, System.EventArgs e)
		{
			Invalidate();
		}
		#endregion

		#region Menu Events
		
		private void mnuAbsolute_Click(object sender, System.EventArgs e)
		{
			mnuAbsolute.Checked = (mnuAbsolute.Checked ? false : true);
		}

		private void mnuStatusBar_Click(object sender, System.EventArgs e)
		{
			mnuStatusBar.Checked = (mnuStatusBar.Checked ? false : true);
			sbrMain.Visible = mnuStatusBar.Checked;
			Invalidate();
		}


		private void mnuShowGrid_Click(object sender, System.EventArgs e)
		{
			mnuShowGrid.Checked = (mnuShowGrid.Checked ? false : true);
			Invalidate();
		}

		private void mnuAlwaysOnTop_Click(object sender, System.EventArgs e)
		{
			mnuAlwaysOnTop.Checked = ((mnuAlwaysOnTop.Checked) ? false : true);
			this.TopMost = mnuAlwaysOnTop.Checked;
		}

		private void mnuRefresh_Click(object sender, System.EventArgs e)
		{
			Invalidate();
		}

		private void mnuAutoRefresh_Click(object sender, System.EventArgs e)
		{
			mnuAutoRefresh.Checked = ((mnuAutoRefresh.Checked) ? false : true);
			tmrRefresh.Enabled = mnuAutoRefresh.Checked;
		}

		private void mnuClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mnuSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog s = new SaveFileDialog();
			s.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.JPG|PNG (*.png)|*.PNG|GIF (*.gif)|*.GIF";
			s.DefaultExt = "BMP";
			s.FilterIndex = 1;
			s.OverwritePrompt = true;
			if (s.ShowDialog() == DialogResult.OK)
			{
				System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
				Bitmap bmp = ImageToBitmap();
				switch (s.FilterIndex)
				{
					case 2:
						imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
						break;
					case 3:
						imgFormat = System.Drawing.Imaging.ImageFormat.Png;
						break;
					case 4:
						imgFormat = System.Drawing.Imaging.ImageFormat.Gif;
						break;
				}
				bmp.Save(s.FileName, imgFormat);
			}

		}

		private void mnuCopy_Click(object sender, System.EventArgs e)
		{	
			Copy();
		}
		#endregion

		#region Implementation
		private void Copy()
		{
			Bitmap bmp = ImageToBitmap();
					
			System.Windows.Forms.Clipboard.SetDataObject(bmp, true);
		}

		private void DrawGrid(Graphics gfx)
		{
			if (mnuShowGrid.Checked)
			{
				if (m_nZoom < 2)
					return;

				Rectangle rect = this.ClientRectangle;
				rect.Width = rect.Width - vscZoom.Width;

				Pen pen = Pens.Black;

				for (int x = 0; x < m_cxZoomed; x++)
				{
					gfx.DrawLine(pen, x * m_nZoom, rect.Top, x * m_nZoom, rect.Bottom);
				}

				for (int y = 0; y < m_cyZoomed; y++)
				{
					gfx.DrawLine(pen, rect.Left, y * m_nZoom, rect.Right, y * m_nZoom);
				}
			}
		}

		private void CalcZoomedSize()
		{
			Rectangle rc = this.ClientRectangle;

			m_cxZoomed = ( (rc.Right - vscZoom.Width) / m_nZoom) + 1;
			m_cyZoomed = ( (rc.Bottom - sbrMain.Height) / m_nZoom) + 1;
		}

		private int Bound(int toBound, int minValue, int maxValue)
		{
			if (toBound < minValue)
				return minValue;
			else if (toBound > maxValue)
				return maxValue;
			else
				return toBound;
		}

		private void DrawZoomRect()
		{
			DrawZoomRect(lastX, lastY);
		}

		private void mnuPrint_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			vbAccelerator.Samples.ZoomIn.frmAbout a = new vbAccelerator.Samples.ZoomIn.frmAbout();
			a.ShowDialog(this);			
		}

		private Bitmap ImageToBitmap()
		{
			Bitmap bmp = new Bitmap(m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed);
			Graphics gfx = Graphics.FromImage((Image)bmp);

			CalcZoomedSize();

			Point pt = new Point(lastX, lastY);
			Point ptScreen = this.PointToScreen(pt);
			ptScreen.X = Bound(ptScreen.X, m_cxZoomed / 2, m_cxScreenMax - (m_cxZoomed / 2));
			ptScreen.Y = Bound(ptScreen.Y, m_cyZoomed / 2, m_cyScreenMax - (m_cyZoomed / 2));

			IntPtr hdcScreen = UnmanagedMethods.GetDC(IntPtr.Zero);
			IntPtr hdc = gfx.GetHdc();

			UnmanagedMethods.SetStretchBltMode(hdc, UnmanagedMethods.COLORONCOLOR);
			UnmanagedMethods.StretchBlt(hdc, 
				0, 0, m_nZoom * m_cxZoomed, m_nZoom * m_cyZoomed,
				hdcScreen, 
				ptScreen.X - m_cxZoomed / 2, ptScreen.Y - m_cyZoomed /2, 
				m_cxZoomed, m_cyZoomed, 
				UnmanagedMethods.SRCCOPY);

			gfx.ReleaseHdc(hdc);
			UnmanagedMethods.ReleaseDC(IntPtr.Zero, hdcScreen);

			return bmp;
		}

		private void DrawZoomRect(int mouseX, int mouseY )//IntPtr lParam)
		{
			if (buttonDown)
			{
				Point pt = new Point(mouseX, mouseY);
				Point screenPt = this.PointToScreen(pt);

				screenPt.X = Bound(screenPt.X, m_cxZoomed / 2, m_cxScreenMax - (m_cxZoomed / 2));
				screenPt.Y = Bound(screenPt.Y, m_cyZoomed / 2, m_cyScreenMax - (m_cyZoomed / 2));

				Rectangle rc = new Rectangle(
					screenPt.X - m_cxZoomed / 2,
					screenPt.Y - m_cyZoomed / 2,
					m_cxZoomed,
					m_cyZoomed);

				rc.Inflate(1,1);

				IntPtr hdc = UnmanagedMethods.GetDC(IntPtr.Zero);

				UnmanagedMethods.PatBlt(hdc, rc.Left,    rc.Top,     rc.Right-rc.Left, 1,  UnmanagedMethods.DSTINVERT);
				UnmanagedMethods.PatBlt(hdc, rc.Left,    rc.Bottom,  1, -(rc.Bottom-rc.Top),   UnmanagedMethods.DSTINVERT);
				UnmanagedMethods.PatBlt(hdc, rc.Right-1, rc.Top,     1,   rc.Bottom-rc.Top,   UnmanagedMethods.DSTINVERT);
				UnmanagedMethods.PatBlt(hdc, rc.Right,   rc.Bottom-1, -(rc.Right-rc.Left), 1, UnmanagedMethods.DSTINVERT);

				UnmanagedMethods.ReleaseDC(IntPtr.Zero, hdc);
				
				//lastLParam = lParam;	
				lastX = mouseX;
				lastY = mouseY;

				this.Invalidate();
			}
		}
		#endregion


	}
	#endregion
}
