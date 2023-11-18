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

            if (!timer_RealTime.Enabled)
            {
                btStartMactive.Text = "Start";
                btStartMactive.BackColor = Color.Transparent;
            }
            else
            {
                btStartMactive.BackColor = Color.Indigo;
                btStartMactive.Text = "Stop";
            }
            ListBox.Items.Clear();

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

        private void btStartMactive_Click(object sender, EventArgs e)
        {
            timer_RealTime.Enabled = !timer_RealTime.Enabled;
            if (!timer_RealTime.Enabled)
            {
                btStartMactive.Text = "Start";
                btStartMactive.BackColor = Color.Transparent;
            }
            else
            {
                fInventory_EPC_List = "";
                ListBox.Items.Clear();
                btStartMactive.BackColor = Color.Indigo;
                fIsInventoryScan = false;
                btStartMactive.Text = "Stop";
                btStartMactive.ForeColor = Color.Transparent;


            }
        }

        private void timer_RealTime_Tick(object sender, EventArgs e)
        {
            if (fIsInventoryScan) return;
            fIsInventoryScan = true;
            GetRealtiemeData();
            fIsInventoryScan = false;
        }

        private void GetRealtiemeData()
        {
            byte[] ScanModeData = new byte[40960];
            int nLen, NumLen;
            string temp1 = "";
            string binarystr1 = "";
            string binarystr2 = "";

            string EPCStr = "";
            int ValidDatalength;
            string temp;
            ValidDatalength = 0;
            DataGridViewRow rows = new DataGridViewRow();
            int xtime = System.Environment.TickCount;
            fCmdRet = RWDev.ReadActiveModeData(ScanModeData, ref ValidDatalength, frmcomportindex);
            if (fCmdRet == 0)
            {
                try
                {
                    byte[] daw = new byte[ValidDatalength];
                    Array.Copy(ScanModeData, 0, daw, 0, ValidDatalength);
                    temp = ByteArrayToHexString(daw);
                    fInventory_EPC_List = fInventory_EPC_List + temp; //把字符串存进列表
                    nLen = fInventory_EPC_List.Length;
                    while (fInventory_EPC_List.Length > 18)
                    {
                        string FlagStr = Convert.ToString(fComAdr, 16).PadLeft(2, '0') + "EE00"; //查找头位置标志字符串
                        int nindex = fInventory_EPC_List.IndexOf(FlagStr);
                        if (nindex > 1)
                            fInventory_EPC_List = fInventory_EPC_List.Substring(nindex - 2);
                        else
                        {
                            fInventory_EPC_List = fInventory_EPC_List.Substring(2);
                            continue;
                        }

                        NumLen = Convert.ToInt32(fInventory_EPC_List.Substring(0, 2), 16) * 2 + 2; //取第一个帧的长度
                        if (fInventory_EPC_List.Length < NumLen)
                        {
                            break;
                        }

                        temp1 = fInventory_EPC_List.Substring(0, NumLen);
                        fInventory_EPC_List = fInventory_EPC_List.Substring(NumLen);
                        if (!CheckCRC(temp1)) continue;
                        EPCStr = temp1.Substring(12, temp1.Length - 18);

                        ListBox.Items.Add(EPCStr);
                    }
                }
                catch (System.Exception ex)
                {
                    ex.ToString();
                }
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
    }
}



