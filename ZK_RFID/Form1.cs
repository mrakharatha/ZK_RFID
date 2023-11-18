using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using UHF;
using ZK_RFID406Demo;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ZK_RFID
{
	public partial class Form1 : Form
	{


		public Form1()
		{
			InitializeComponent();
		}

		private byte fComAdr = 0xff;
		private int fCmdRet = 30;
		private int frmcomportindex;
		private string fInventory_EPC_List;
		private volatile bool fIsInventoryScan = false;
		byte[] ReadAdr = new byte[2];
		byte[] Psd = new byte[4];
		byte ReadLen = 0;
		byte ReadMem = 0;
		private int AA_times = 0;
		private byte Scantime = 0;
		private byte Qvalue = 0;
		private byte Session = 0;
		private byte TIDFlag = 0;
		private int total_turns = 0;
		private int total_tagnum = 0;
		private int total_time = 0;
		private int targettimes = 0;
		byte[] antlist = new byte[4];
		private byte InAnt = 0;
		private byte Target = 0;
		private volatile bool toStopThread = false;
		private Thread mythread = null;
		private byte FastFlag = 0;

		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

		[DllImport("User32.dll", EntryPoint = "FindWindow")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		public const int USER = 0x0400;
		public const int WM_SENDTAG = USER + 101;
		public const int WM_SENDTAGSTAT = USER + 102;
		public const int WM_SENDSTATU = USER + 103;
		public const int WM_SENDBUFF = USER + 104;
		public const int WM_MIXTAG = USER + 105;

		private int CommunicationTime = 0;
		private void btConnectTcp_Click(object sender, EventArgs e)
		{
			try
			{
				string ipAddress = ipServerIP.Text;
				int nPort = Convert.ToInt32(tb_Port.Text);
				fComAdr = 255;
				int frmPortIndex = 0;
				fCmdRet = RWDev.OpenNetPort(nPort, ipAddress, ref fComAdr, ref frmPortIndex);
				if (fCmdRet != 0)
				{
					string strLog = "Connect reader failed: " + GetReturnCodeDesc(fCmdRet);
					MessageBox.Show(strLog, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					return;
				}
				else
				{
					frmcomportindex = frmPortIndex;
					//string strLog = "Connect: " + ipAddress + "@" + nPort.ToString();
					//MessageBox.Show(strLog, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				btConnectTcp.Enabled = false;
				btDisConnectTcp.Enabled = true;
				btStartMactive.Enabled = true;
				btConnectTcp.ForeColor = Color.Black;
				btDisConnectTcp.ForeColor = Color.Indigo;
				btStartMactive.ForeColor = Color.Indigo;

			}
			catch (System.Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}


		private void btDisConnectTcp_Click(object sender, EventArgs e)
		{
			if (frmcomportindex > 1023)
				fCmdRet = RWDev.CloseNetPort(frmcomportindex);
			if (fCmdRet == 0) frmcomportindex = -1;
			btConnectTcp.Enabled = true;
			btDisConnectTcp.Enabled = false;
			btStartMactive.Enabled = false;
			btConnectTcp.ForeColor = Color.Indigo;
			btDisConnectTcp.ForeColor = Color.Black;
			btStartMactive.ForeColor = Color.Black;

			//if (!timer_RealTime.Enabled)
			//{
			//	btStartMactive.Text = "Start";
			//	btStartMactive.BackColor = Color.Transparent;
			//}
			//else
			//{
			//	btStartMactive.BackColor = Color.Indigo;
			//	btStartMactive.Text = "Stop";
			//}
			//ListBox.Items.Clear();

		}



		private string GetReturnCodeDesc(int cmdRet)
		{
			switch (cmdRet)
			{
				case 0x00:
				case 0x26:
					return "success";
				case 0x01:
					return "Return before Inventory finished";
				case 0x02:
					return "the Inventory-scan-time overflow";
				case 0x03:
					return "More Data";
				case 0x04:
					return "Reader module MCU is Full";
				case 0x05:
					return "Access Password Error";
				case 0x09:
					return "Destroy Password Error";
				case 0x0a:
					return "Destroy Password Error Cannot be Zero";
				case 0x0b:
					return "Tag Not Support the command";
				case 0x0c:
					return "Use the commmand,Access Password Cannot be Zero";
				case 0x0d:
					return "Tag is protected,cannot set it again";
				case 0x0e:
					return "Tag is unprotected,no need to reset it";
				case 0x10:
					return "There is some locked bytes,write fail";
				case 0x11:
					return "can not lock it";
				case 0x12:
					return "is locked,cannot lock it again";
				case 0x13:
					return "Parameter Save Fail,Can Use Before Power";
				case 0x14:
					return "Cannot adjust";
				case 0x15:
					return "Return before Inventory finished";
				case 0x16:
					return "Inventory-Scan-Time overflow";
				case 0x17:
					return "More Data";
				case 0x18:
					return "Reader module MCU is full";
				case 0x19:
					return "'Not Support Command Or AccessPassword Cannot be Zero";
				case 0x1A:
					return "Tag custom function error";
				case 0xF8:
					return "Check antenna error";
				case 0xF9:
					return "Command execute error";
				case 0xFA:
					return "Get Tag,Poor Communication,Inoperable";
				case 0xFB:
					return "No Tag Operable";
				case 0xFC:
					return "Tag Return ErrorCode";
				case 0xFD:
					return "Command length wrong";
				case 0xFE:
					return "Illegal command";
				case 0xFF:
					return "Parameter Error";
				case 0x30:
					return "Communication error";
				case 0x31:
					return "CRC checksummat error";
				case 0x32:
					return "Return data length error";
				case 0x33:
					return "Communication busy";
				case 0x34:
					return "Busy,command is being executed";
				case 0x35:
					return "ComPort Opened";
				case 0x36:
					return "ComPort Closed";
				case 0x37:
					return "Invalid Handle";
				case 0x38:
					return "Invalid Port";
				case 0xEE:
					return "Return Command Error";
				default:
					return "";
			}
		}
		public static string ByteArrayToHexString(byte[] data)
		{
			StringBuilder sb = new StringBuilder(data.Length * 3);
			foreach (byte b in data)
				sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
			return sb.ToString().ToUpper();

		}
		private bool CheckCRC(string s)
		{
			int i, j;
			int current_crc_value;
			byte crcL, crcH;
			byte[] data = HexStringToByteArray(s);
			current_crc_value = 0xFFFF;
			for (i = 0; i <= (data.Length - 1); i++)
			{
				current_crc_value = current_crc_value ^ (data[i]);
				for (j = 0; j < 8; j++)
				{
					if ((current_crc_value & 0x01) != 0)
						current_crc_value = (current_crc_value >> 1) ^ 0x8408;
					else
						current_crc_value = (current_crc_value >> 1);
				}
			}
			crcL = Convert.ToByte(current_crc_value & 0xFF);
			crcH = Convert.ToByte((current_crc_value >> 8) & 0xFF);
			if (crcH == 0 && crcL == 0)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		public static byte[] HexStringToByteArray(string s)
		{
			s = s.Replace(" ", "");
			byte[] buffer = new byte[s.Length / 2];
			for (int i = 0; i < s.Length; i += 2)
				buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
			return buffer;
		}



		private void btStartMactive_Click(object sender, EventArgs e)
		{
			if (btStartMactive.Text == "Start")
			{

				ReadMem = (byte)2;
				ReadAdr = HexStringToByteArray("0000");
				ReadLen = Convert.ToByte("06", 16);
				Psd = HexStringToByteArray("00000000");


				ListBox.DataSource = null;
				AA_times = 0;
				Scantime = Convert.ToByte(0 + 3);
				Qvalue = Convert.ToByte(4 | 0x80);

				Session = Convert.ToByte(0);
				TIDFlag = 1;
				total_turns = 0;
				total_tagnum = 0;
				targettimes = Convert.ToInt32("2");
				total_time = System.Environment.TickCount;
				fIsInventoryScan = false;
				btStartMactive.BackColor = Color.Indigo;
				btStartMactive.Text = "Stop";
				Array.Clear(antlist, 0, 4);
				//if (check_ant1.Checked)
				//{
				antlist[0] = 1;
				InAnt = 0x80;
				//}
				//if (check_ant2.Checked)
				//{
				antlist[1] = 1;
				InAnt = 0x81;
				//}

				Target = (byte)0;
				toStopThread = false;
				if (fIsInventoryScan == false)
				{
					mythread = new Thread(new ThreadStart(inventory));
					mythread.Start();
				}

			}
			else
			{
				btStartMactive.BackColor = Color.Transparent;
				btStartMactive.Text = "Start";
				if (fIsInventoryScan)
				{
					toStopThread = true;//标志，接收数据线程判断stop为true，正常情况下会自动退出线程                            
					if (mythread.Join(3000))
					{
						try
						{
							mythread.Abort();//若线程无法退出，强制结束
						}
						catch (Exception exp)
						{
							MessageBox.Show(exp.Message, "Thread error");
						}
					}
					fIsInventoryScan = false;
				}
			
			}
		}



		private void inventory()
		{
			fIsInventoryScan = true;
			while (!toStopThread)
			{
				for (int m = 0; m < 4; m++)
				{
					switch (m)
					{
						case 0:
							InAnt = 0x80;
							break;
						case 1:
							InAnt = 0x81;
							break;
						case 2:
							InAnt = 0x82;
							break;
						case 3:
							InAnt = 0x83;
							break;
					}
					FastFlag = 1;
					if (antlist[m] == 1)
						flashmix_G2();
				}
				Thread.Sleep(5);
			}
			fIsInventoryScan = false;
		}




		private void flashmix_G2()
		{

			byte Ant = 0;
			int CardNum = 0;
			int Totallen = 0;
			int EPClen, m;
			byte[] EPC = new byte[50000];
			int CardIndex;
			string temps, temp;
			temp = "";
			string sEPC;
			byte MaskMem = 0;
			byte[] MaskAdr = new byte[2];
			byte MaskLen = 0;
			byte[] MaskData = new byte[100];
			byte MaskFlag = 0;
			MaskFlag = 0;
			int cbtime = System.Environment.TickCount;
			CardNum = 0;
			fCmdRet = RWDev.InventoryMix_G2(ref fComAdr, Qvalue, Session, MaskMem, MaskAdr, MaskLen, MaskData, MaskFlag, ReadMem, ReadAdr, ReadLen, Psd, Target, InAnt, Scantime, FastFlag, EPC, ref Ant, ref Totallen, ref CardNum, frmcomportindex);
			int cmdTime = System.Environment.TickCount - cbtime;//命令时间

			if (CardNum == 0)
			{
				if (Session > 1)
					AA_times = AA_times + 1;//没有得到标签只更新状态栏
				IntPtr ptrWnd = IntPtr.Zero;
				ptrWnd = FindWindow(null, "Form1");
				if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
				{
					string para = fCmdRet.ToString();
					SendMessage(ptrWnd, WM_SENDSTATU, IntPtr.Zero, para);
				}
				return;
			}
			AA_times = 0;

			if ((fCmdRet == 1) || (fCmdRet == 2) || (fCmdRet == 0x26))//代表已查找结束，
			{
				byte[] daw = new byte[Totallen];
				Array.Copy(EPC, daw, Totallen);
				temps = ByteArrayToHexString(daw);
				if (fCmdRet == 0x26)
				{
					string SDCMD = temps.Substring(0, 12);
					temps = temps.Substring(12);
					daw = HexStringToByteArray(temps);
					byte[] datas = new byte[6];
					datas = HexStringToByteArray(SDCMD);
					int tagrate = datas[0] * 256 + datas[1];
					int tagnum = datas[2] * 256 * 256 * 256 + datas[3] * 256 * 256 + datas[4] * 256 + datas[5];
					total_tagnum = total_tagnum + tagnum;
					IntPtr ptrWnd = IntPtr.Zero;
					ptrWnd = FindWindow(null, "Form1");
					if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
					{
						string para = tagrate.ToString() + "," + total_tagnum.ToString() + "," + cmdTime.ToString();
						SendMessage(ptrWnd, WM_SENDTAGSTAT, IntPtr.Zero, para);
					}
				}
				m = 0;
				int lastnum = 0;
				string epcandtid = "";//标记整合数据
				for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
				{
					EPClen = daw[m + 1];
					string tempA = temps.Substring(m * 2, EPClen * 2 + 6);
					sEPC = tempA.Substring(4, tempA.Length - 6);
					string RSSI = Convert.ToInt32(tempA.Substring(tempA.Length - 2, 2), 16).ToString();
					int gnum = Convert.ToInt32(tempA.Substring(0, 2), 16);

					bool nflag = false;
					if (gnum < 0x80)//EPC号
					{
						lastnum = gnum;
						epcandtid = sEPC;
					}
					else//附带数据
					{
						if (((lastnum & 0x3F) == ((gnum & 0x3F) - 1)) || ((lastnum & 0x3F) == 127 && ((gnum & 0x3F) == 0)))//相邻的滚码
						{
							epcandtid = epcandtid + "-" + sEPC;
							IntPtr ptrWnd = IntPtr.Zero;
							ptrWnd = FindWindow(null, "Form1");
							if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
							{
								string para = epcandtid + "," + RSSI.ToString() + " ";
								SendMessage(ptrWnd, WM_MIXTAG, IntPtr.Zero, para);
							}
						}
						else
						{
							epcandtid = "";
						}
					}

					m = m + EPClen + 3;

				}
			}
			if ((fCmdRet == 1) || (fCmdRet == 2) || (fCmdRet == 0xFB))
			{
				if (cmdTime > CommunicationTime)
					cmdTime = cmdTime - CommunicationTime;//减去通讯时间等于标签的实际时间
				int tagrate = (CardNum * 1000) / cmdTime;//速度等于张数/时间
				total_tagnum = total_tagnum + CardNum;
				IntPtr ptrWnd = IntPtr.Zero;
				ptrWnd = FindWindow(null, "Form1");
				if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
				{
					string para = tagrate.ToString() + "," + total_tagnum.ToString() + "," + cmdTime.ToString();
					SendMessage(ptrWnd, WM_SENDTAGSTAT, IntPtr.Zero, para);
				}
			}
			IntPtr ptrWnd1 = IntPtr.Zero;
			ptrWnd1 = FindWindow(null, "Form1");
			if (ptrWnd1 != IntPtr.Zero)         // 检查当前统计窗口是否打开
			{
				string para = fCmdRet.ToString();
				SendMessage(ptrWnd1, WM_SENDSTATU, IntPtr.Zero, para);
			}
		}



		protected override void DefWndProc(ref Message m)
		{
			if (m.Msg == WM_MIXTAG)
			{
				var tagInfo = Marshal.PtrToStringAnsi(m.LParam);
				if (tagInfo == null) return;
				var index = tagInfo.IndexOf(',');
				var sEpc = tagInfo.Substring(0, index);
				var n = sEpc.IndexOf("-", StringComparison.Ordinal);
				var sData = sEpc.Substring(n + 1);
				ListBox.Items.Add(sData);
			}
			else
				base.DefWndProc(ref m);
		}

		
	}
}



