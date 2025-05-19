using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;

//using System.Windows.Input;



namespace AnvizDemo
{
    

    public partial class FrmMain : Form
    {
        //IntPtr buff_fin;
        //byte[] buff_fin;
        byte[] buff_fin = new byte[15360];
        int len_fin = 0;

        byte[] buff_fP_picture = new byte[1024000];
        int len_fP_picture = 0;


        int DevCount;
        Dictionary<int, int> DevTypeFlag = new Dictionary<int, int>();// 0x10:DR, 0x400:msg content 200 and unicode
        IntPtr anviz_handle;

        public static byte[] StructToBytes(object structObj, int size = 0)
        {
            if (size == 0)
            {
                size = Marshal.SizeOf(structObj); //得到结构体大小
            }
            IntPtr buffer = Marshal.AllocHGlobal(size);  //开辟内存空间
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);   //填充内存空间
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);   //填充数组
                return bytes;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);   //释放内存
            }
        }
        public static object BytesToStruct(byte[] bytes, Type strcutType, int nSize)
        {
            if (bytes == null)
            {
            }
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(nSize);
            //Debug.LogError("Type: " + strcutType.ToString() + "---TypeSize:" + size + "----packetSize:" + nSize);
            try
            {
                Marshal.Copy(bytes, 0, buffer, nSize);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public void dbg_info(string dbg_string)     //调试程序
        {
            Debug.WriteLine("time:" + DateTime.Now.ToString("HH-mm-ss") + " ms:" + DateTime.Now.Millisecond.ToString() + " ----[  " + dbg_string + "  ]", " SDK_DBG_INFO");
        }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.config3.Hide();
            this.Text += ("Ver:" + AnvizNew.CChex_Version().ToString());
            AnvizNew.CChex_Init();

            //AnvizNew.CChex_Set_Service_Port(5810);
            //AnvizNew.CChex_Set_Service_Disenable();

            

            anviz_handle = AnvizNew.CChex_Start();

            
            log_add_string(AnvizNew.CChex_Get_Service_Port(anviz_handle).ToString());


            DevCount = 0;
            if (anviz_handle != null)
            {
                this.timer1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Startup errors,Please restart the program.");
            }


        }

        public static int GetLength(string str)
        {
            if (str.Length == 0)
                return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }


        private string byte_array_to_srring(byte[] EmployeeId)
        {
            string tmp = null;
            for (int i = 0; i < EmployeeId.Length; i++)
            {
                tmp = tmp + EmployeeId[i].ToString("X2");
            }
            return tmp;
        }
        private string Employee_array_to_srring(byte[] EmployeeId)
        {
            byte[] temp = new byte[8];
            int i;
            for (i = 0; i < 5; i++)
            {
                temp[8 - 4 - i] = EmployeeId[i];
            }
            return BitConverter.ToInt64(temp, 0).ToString();
        }
        private string CardId_array_to_string(byte[] CardId)
        {
            byte[] temp = new byte[8];
            int i;
            for (i = 0; i < 7; i++)
            {
                temp[8 - 2 - i] = CardId[i];
            }
            Int64 temp_int = BitConverter.ToInt64(temp, 0);
            if (temp_int == 0xffffffffffffff)
            {
                temp_int = 0;
            }
            return temp_int.ToString();
        }

        private string byte_to_string(byte[] StringData)
        {
            return Encoding.Default.GetString(StringData).Replace("\0", "");
            //return Encoding.UTF8.GetString(StringData).Replace("\0", "");
        }

        private string byte_to_unicode_string(byte[] StrData)
        {
            //log_add_string(Encoding.BigEndianUnicode.GetString(StringData));
            int i;
            byte[] StringData = new byte[StrData.Length];

            for (i = 0; i+1 < StringData.Length; i+=2)
            {
                StringData[i] = StrData[i + 1];
                StringData[i + 1] = StrData[i];
            }
            return Encoding.Unicode.GetString(StringData).Replace("\0", "");
            //return Encoding.UTF8.GetString(StringData).Replace("\0", "");
        }
        private byte[] string_to_my_unicodebyte(int bytemax, string str)
        {
            if (bytemax > 0)
            {
                byte[] byte_out = new byte[bytemax];// unicode
                int i;                           // 
                byte[] bytestr = Encoding.Unicode.GetBytes(str);
                for (i = 0; i < bytemax; i += 2)
                {
                    if (i < bytestr.Length)
                    {
                        byte_out[i] = bytestr[i + 1];
                        byte_out[i + 1] = bytestr[i];
                        continue;
                    }
                    byte_out[i] = 0;
                }
                return byte_out;
            }
            return null;
        }

        private void string_to_byte(string str, byte[] value, byte len)
        {
            int i;
            ulong ul_value = Convert.ToUInt64(str);
            for (i = 0; i < len; i++)
            {
                value[i] = (byte)((ul_value & ((ulong)0xFF << (8 * (len - i - 1)))) >> (8 * (len - i - 1)));
            }
        }
        private void string_to_byte_begin(string str, byte[] value, byte len,int Begin)
        {
            int i;
            ulong ul_value = Convert.ToUInt64(str);
            for (i = 0; i < len; i++)
            {
                value[Begin+i] = (byte)((ul_value & ((ulong)0xFF << (8 * (len - i - 1)))) >> (8 * (len - i - 1)));
            }
        }

        private void string_to_byte_add_num(string str, byte[] value, byte len,int AddNum)
        {
            int i;
            ulong ul_value = Convert.ToUInt64(str) +(UInt64)AddNum;
            for (i = 0; i < len; i++)
            {
                value[i] = (byte)((ul_value & ((ulong)0xFF << (8 * (len - i - 1)))) >> (8 * (len - i - 1)));
            }
        }

        private void uint64_to_byte(UInt64 ul_value, byte[] value, byte len)
        {
            int i;
            for (i = 0; i < len; i++)
            {
                value[i] = (byte)((ul_value & ((ulong)0xFF << (8 * (len - i - 1)))) >> (8 * (len - i - 1)));
            }
        }

        private UInt64 byte_to_uint64(byte[] value)
        {
            int i;
            UInt64 temp = 0;
            for (i = 0; i < 8 ; i++)
            {
                temp += ((UInt64)value[i]) << (8 * (7-i));
            }
            return temp;
        }

        private byte[] get_card_id(byte[] EmployeeId, byte[] WorkType)
        {
            byte[] card_id = new byte[8];

            card_id[7] = 0;
            card_id[6] = 0;
            card_id[5] = WorkType[1];
            card_id[4] = WorkType[2];
            card_id[3] = EmployeeId[1];
            card_id[2] = EmployeeId[2];
            card_id[1] = EmployeeId[3];
            card_id[0] = EmployeeId[4];
            return card_id;
        }

        private uint swapInt32(uint value)
        {
            return ((value & 0x000000FF) << 24) |
           ((value & 0x0000FF00) << 8) |
           ((value & 0x00FF0000) >> 8) |
           ((value & 0xFF000000) >> 24);
        }

        private void memset(byte[] buf, byte val, int size)
        {
            int i;
            for (i = 0; i < size; i++)
                buf[i] = val;  
        }
        private string bytetoipstr(byte[] ipbyte)
        {
            return ipbyte[0] + "."+ ipbyte[1] + "." + ipbyte[2] + "." + ipbyte[3];
        }

        private bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^\d*$");
        }

        private void log_add_string(string info_buff)   //实现自动滚动
        {
            bool scroll = false;
            //大于1000条清空
            if (this.listBoxLog.Items.Count >= 1000)
            {
                this.listBoxLog.Items.Clear();
            }

            if (this.listBoxLog.TopIndex == this.listBoxLog.Items.Count - (int)(this.listBoxLog.Height / this.listBoxLog.ItemHeight))
            {
                scroll = true;
            }
            this.listBoxLog.Items.Add(info_buff);
            if (scroll)
            {
                this.listBoxLog.TopIndex = this.listBoxLog.Items.Count - (int)(this.listBoxLog.Height / this.listBoxLog.ItemHeight);
            }

        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnvizNew.CChex_Stop(anviz_handle);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }
        

        private void getmsg_Click(object sender, EventArgs e)
        {
            listViewMsg.Items.Clear();
            if (textBox1.Text.Trim() != string.Empty)
            {
                if (listViewDevice.SelectedItems.Count != 0)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    byte msg_idx = Convert.ToByte(textBox1.Text.ToString());
                    //Console.WriteLine("dev_idx:"+listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    AnvizNew.CChex_MsgGetByIdx(anviz_handle, dev_idx, msg_idx);
                }
                else
                {
                    MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Please enter message idx", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addmsg_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() != string.Empty && textBox3.Text.Trim() != string.Empty)
            {
                if (listViewDevice.SelectedItems.Count != 0)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    int head_len = 0;
                    int send_len = 0;
                    byte[] send_buff = null;
                    int text_len;
                    IntPtr tmp_ptr = IntPtr.Zero;
                    byte temb;
                    byte[] text_content;// = System.Text.Encoding.Unicode.GetBytes(textBox3.Text.ToString())

                    if ((DevTypeFlag[dev_idx] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                    {
                        if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.MessageType.DEV_TYPE_FLAG_MSG_ASCII_48)
                        {
                            text_content = System.Text.Encoding.ASCII.GetBytes(textBox3.Text.ToString());
                            text_len = text_content.Length;
                        }
                        else
                        {                                                                   //UNICODE
                            text_content = System.Text.Encoding.Unicode.GetBytes(textBox3.Text.ToString());
                            text_len = text_content.Length;
                            for (int i = 0; i + 1 < text_len; i += 2)
                            {
                                temb = text_content[i];
                                text_content[i] = text_content[i + 1];
                                text_content[i + 1] = temb;
                            }
                        }


                        if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_MESSAGE_FOR_SEATS_200) // msg content 100*2
                        {
                            //限制长度(多出来的长度切除)
                            if (text_len >= 200)
                            {
                                text_len = 200;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID msg_head = new AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID();
                            msg_head.EmployeeId = string_to_my_unicodebyte(28,msg_p_id.Text);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);
                            msg_head.StartHour = Convert.ToByte(dateTimePicker1.Value.Hour);
                            msg_head.StartMin = Convert.ToByte(dateTimePicker1.Value.Minute);
                            msg_head.StartSec = Convert.ToByte(dateTimePicker1.Value.Second);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);
                            msg_head.EndHour = Convert.ToByte(dateTimePicker2.Value.Hour);
                            msg_head.EndMin = Convert.ToByte(dateTimePicker2.Value.Minute);
                            msg_head.EndSec = Convert.ToByte(dateTimePicker2.Value.Second);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }
                        else if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.MessageType.DEV_TYPE_FLAG_MSG_UNICODE_96)// msg contect 48*2
                        {
                            //限制长度(多出来的长度切除)

                            if (text_len >= 48 * 2)
                            {
                                text_len = 48 * 2;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_head = new AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID();
                            msg_head.EmployeeId = string_to_my_unicodebyte(28,msg_p_id.Text);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }
                        else
                        {
                            if (text_len >= 48)
                            {
                                text_len = 48;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_head = new AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID();
                            msg_head.EmployeeId = new byte[5];
                            string_to_byte(msg_p_id.Text, msg_head.EmployeeId, 5);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }



                        AnvizNew.CChex_MsgAddNew(anviz_handle, dev_idx, send_buff, send_len);
                        Marshal.FreeHGlobal(tmp_ptr); // free the memory  
                    }
                    else
                    {
                        if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.MessageType.DEV_TYPE_FLAG_MSG_ASCII_48)
                        {
                            text_content = System.Text.Encoding.ASCII.GetBytes(textBox3.Text.ToString());
                            text_len = text_content.Length;
                        }
                        else
                        {                                                                   //UNICODE
                            text_content = System.Text.Encoding.Unicode.GetBytes(textBox3.Text.ToString());
                            text_len = text_content.Length;
                            for (int i = 0; i + 1 < text_len; i += 2)
                            {
                                temb = text_content[i];
                                text_content[i] = text_content[i + 1];
                                text_content[i + 1] = temb;
                            }
                        }


                        if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_MESSAGE_FOR_SEATS_200) // msg content 100*2
                        {
                            //限制长度(多出来的长度切除)
                            if (text_len >= 200)
                            {
                                text_len = 200;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU msg_head = new AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU();
                            msg_head.EmployeeId = new byte[5];
                            string_to_byte(msg_p_id.Text, msg_head.EmployeeId, 5);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);
                            msg_head.StartHour = Convert.ToByte(dateTimePicker1.Value.Hour);
                            msg_head.StartMin = Convert.ToByte(dateTimePicker1.Value.Minute);
                            msg_head.StartSec = Convert.ToByte(dateTimePicker1.Value.Second);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);
                            msg_head.EndHour = Convert.ToByte(dateTimePicker2.Value.Hour);
                            msg_head.EndMin = Convert.ToByte(dateTimePicker2.Value.Minute);
                            msg_head.EndSec = Convert.ToByte(dateTimePicker2.Value.Second);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }
                        else if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xF00) == (int)AnvizNew.MessageType.DEV_TYPE_FLAG_MSG_UNICODE_96)// msg contect 48*2
                        {
                            //限制长度(多出来的长度切除)

                            if (text_len >= 48 * 2)
                            {
                                text_len = 48 * 2;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_head = new AnvizNew.CCHEX_MSGHEAD_INFO_STRU();
                            msg_head.EmployeeId = new byte[5];
                            string_to_byte(msg_p_id.Text, msg_head.EmployeeId, 5);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }
                        else
                        {
                            if (text_len >= 48)
                            {
                                text_len = 48;
                            }
                            head_len = Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));//消息头长度
                            send_len = head_len + text_len;     //总得长度
                            send_buff = new byte[send_len];
                            tmp_ptr = Marshal.AllocHGlobal(send_len);
                            AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_head = new AnvizNew.CCHEX_MSGHEAD_INFO_STRU();
                            msg_head.EmployeeId = new byte[5];
                            string_to_byte(msg_p_id.Text, msg_head.EmployeeId, 5);
                            msg_head.StartYear = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                            msg_head.StartMonth = Convert.ToByte(dateTimePicker1.Value.Month);
                            msg_head.StartDay = Convert.ToByte(dateTimePicker1.Value.Day);

                            msg_head.EndYear = Convert.ToByte(dateTimePicker2.Value.Year - 2000);
                            msg_head.EndMonth = Convert.ToByte(dateTimePicker2.Value.Month);
                            msg_head.EndDay = Convert.ToByte(dateTimePicker2.Value.Day);

                            Marshal.StructureToPtr(msg_head, tmp_ptr, false); //Copy到分配的非托管内存中
                            Marshal.Copy(tmp_ptr, send_buff, 0, head_len);

                            //把消息内容复制到数组
                            for (int i = 0; i < text_len; i++)
                            {
                                send_buff[head_len + i] = text_content[i];
                            }
                        }



                        AnvizNew.CChex_MsgAddNew(anviz_handle, dev_idx, send_buff, send_len);
                        Marshal.FreeHGlobal(tmp_ptr); // free the memory  
                    }

                    

                }
                else
                {
                    MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please complete", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delmsg_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != string.Empty)
            {
                if (listViewDevice.SelectedItems.Count != 0)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    byte msg_idx = Convert.ToByte(textBox1.Text.ToString());
                    AnvizNew.CChex_MsgDelByIdx(anviz_handle, dev_idx, msg_idx);
                }
                else
                {
                    MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Please enter message idx", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getallmsghead_Click(object sender, EventArgs e)
        {
            listViewMsg.Items.Clear();
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_MsgGetAllHead(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void reboot_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CChex_RebootDevice(anviz_handle, dev_idx);

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settime_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CChex_SetTime(anviz_handle, dev_idx, Convert.ToInt32(Box6_year.Text.ToString()), Convert.ToInt32(Box6_month.Text.ToString()),
                    Convert.ToInt32(Box6_day.Text.ToString()), Convert.ToInt32(Box6_hour.Text.ToString()), Convert.ToInt32(Box6_min.Text.ToString()), Convert.ToInt32(Box6_sec.Text.ToString()));

                MessageBox.Show(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void buttonGetNet_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetNetConfig(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBoxLog_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 3) //Crtl+C
            {
                string s = string.Empty;
                foreach (var item in listBoxLog.SelectedItems)
                {
                    s += item.ToString();
                    s += Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(s))
                {
                    Clipboard.SetText(s);
                }
            }
        }

        private void buttonSetNet_Click(object sender, EventArgs e)
        {
            AnvizNew.CCHEX_NETCFG_INFO_STRU dev_info = new AnvizNew.CCHEX_NETCFG_INFO_STRU();
            dev_info.IpAddr = new byte[4];
            dev_info.IpMask = new byte[4];
            dev_info.MacAddr = new byte[6];
            dev_info.GwAddr = new byte[4];
            dev_info.ServAddr = new byte[4];
            dev_info.Port = new byte[2];
            int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

            if (listViewDevice.SelectedItems.Count != 0)
            {
                if (textBoxIp.Text.Trim() != string.Empty && textBoxMask.Text.Trim() != string.Empty && textBoxGw.Text.Trim() != string.Empty && textBoxMac.Text.Trim() != string.Empty && textBoxPort.Text.Trim() != string.Empty && textBoxServIp.Text.Trim() != string.Empty && textBoxMode.Text.Trim() != string.Empty)
                {
                    string[] ip_array = textBoxIp.Text.Split('.');
                    dev_info.IpAddr[0] = Convert.ToByte(ip_array[0]);
                    dev_info.IpAddr[1] = Convert.ToByte(ip_array[1]);
                    dev_info.IpAddr[2] = Convert.ToByte(ip_array[2]);
                    dev_info.IpAddr[3] = Convert.ToByte(ip_array[3]);

                    string[] mask_array = textBoxMask.Text.Split('.');
                    dev_info.IpMask[0] = Convert.ToByte(mask_array[0]);
                    dev_info.IpMask[1] = Convert.ToByte(mask_array[1]);
                    dev_info.IpMask[2] = Convert.ToByte(mask_array[2]);
                    dev_info.IpMask[3] = Convert.ToByte(mask_array[3]);

                    string[] mac_array = textBoxMac.Text.Split('.');
                    dev_info.MacAddr[0] = Convert.ToByte(Convert.ToInt64(mac_array[0], 16));
                    dev_info.MacAddr[1] = Convert.ToByte(Convert.ToInt64(mac_array[1], 16));
                    dev_info.MacAddr[2] = Convert.ToByte(Convert.ToInt64(mac_array[2], 16));
                    dev_info.MacAddr[3] = Convert.ToByte(Convert.ToInt64(mac_array[3], 16));
                    dev_info.MacAddr[4] = Convert.ToByte(Convert.ToInt64(mac_array[4], 16));
                    dev_info.MacAddr[5] = Convert.ToByte(Convert.ToInt64(mac_array[5], 16));

                    string[] gw_array = textBoxGw.Text.Split('.');
                    dev_info.GwAddr[0] = Convert.ToByte(gw_array[0]);
                    dev_info.GwAddr[1] = Convert.ToByte(gw_array[1]);
                    dev_info.GwAddr[2] = Convert.ToByte(gw_array[2]);
                    dev_info.GwAddr[3] = Convert.ToByte(gw_array[3]);

                    string[] serv_array = textBoxServIp.Text.Split('.');
                    dev_info.ServAddr[0] = Convert.ToByte(serv_array[0]);
                    dev_info.ServAddr[1] = Convert.ToByte(serv_array[1]);
                    dev_info.ServAddr[2] = Convert.ToByte(serv_array[2]);
                    dev_info.ServAddr[3] = Convert.ToByte(serv_array[3]);


                    dev_info.Port[0] = Convert.ToByte(Convert.ToInt32(textBoxPort.Text) >> 8);
                    dev_info.Port[1] = Convert.ToByte(Convert.ToInt32(textBoxPort.Text) & 0xff);

                    dev_info.Mode = Convert.ToByte(textBoxMode.Text);
                    dev_info.RemoteEnable = Convert.ToByte(textBoxRemote.Text);

                    if (checkBoxDHCP.Checked == false)
                    {
                        dev_info.DhcpEnable = 0;
                    }
                    else
                    {
                        dev_info.DhcpEnable = 1;
                    }

                    AnvizNew.CChex_SetNetConfig(anviz_handle, dev_idx, ref dev_info);

                }
                else
                {
                    MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please fill out the necessary configuration information", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            int ret = 0;
            int[] Type = new int[1];
            int[] dev_idx = new int[1];
            IntPtr pBuff;
            int len = 32000;




            pBuff = Marshal.AllocHGlobal(len);
            while (true)
            {
                if (anviz_handle == IntPtr.Zero)
                {
                    break;
                }

                

                ret = AnvizNew.CChex_Update(anviz_handle, dev_idx, Type, pBuff, len);
                //dbg_info("Update~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                if (ret > 0)
                {
                    dbg_info("Msg Type : " +Type[0]);
                    switch (Type[0])
                    {
                        case (int)AnvizNew.MsgType.CCHEX_RET_RECORD_INFO_CARD_BYTE7_TYPE:
                            {
                                AnvizNew.CCHEX_RET_RECORD_INFO_STRU_CARD_ID_LEN_7 record_info;
                                record_info = (AnvizNew.CCHEX_RET_RECORD_INFO_STRU_CARD_ID_LEN_7)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_RECORD_INFO_STRU_CARD_ID_LEN_7));


                                DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                    + " Date:" + dateStr
                                    + "PersonID: " + CardId_array_to_string(record_info.CardId)
                                    + " RecType:" + record_info.RecordType.ToString()
                                    + " ]" + "(" + record_info.CurIdx.ToString() + "/" + record_info.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_TM_UPLOAD_RECORD_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU result;
                                result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Modify person OK Employeeid =" + Employee_array_to_srring(result.EmployeeId) + "  machineid :" + result.MachineId.ToString());
                                    // TODO: modify listView_person.Items
                                }
                                else
                                {
                                    MessageBox.Show("Modify person Fail! Employeeid =" + Employee_array_to_srring(result.EmployeeId));
                                }

                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_TM_NEW_RECORD_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_TM_RECORD_BY_EMPLOYEE_TIME_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_TM_ALL_RECORD_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_TM_RECORD_INFO_STRU record_info;
                                record_info = (AnvizNew.CCHEX_RET_TM_RECORD_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_TM_RECORD_INFO_STRU));
                                DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                    + " Date:" + dateStr
                                    + "PersonID: " + Employee_array_to_srring(record_info.EmployeeId)
                                    + " RecType:" + record_info.RecordType.ToString() 
                                    + " TM:" + ((record_info.Temperature[0]<<8 )+ record_info.Temperature[1]).ToString()
                                    + " ]" + "(" + record_info.CurIdx.ToString() + "/" + record_info.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_RECORD_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_NEW_RECORD_INFO_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID record_info;
                                    record_info = (AnvizNew.CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_RECORD_INFO_STRU_VER_4_NEWID));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + byte_to_unicode_string(record_info.EmployeeId)
                                        + " RecType:" + record_info.RecordType.ToString()
                                        + " ]" + "(" + record_info.CurIdx.ToString() + "/" + record_info.TotalCnt.ToString() + ")";
                                    log_add_string(info_buff);
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_RECORD_INFO_STRU record_info;
                                    record_info = (AnvizNew.CCHEX_RET_RECORD_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_RECORD_INFO_STRU));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + Employee_array_to_srring(record_info.EmployeeId)
                                        + " RecType:" + record_info.RecordType.ToString()
                                        + " ]" + "(" + record_info.CurIdx.ToString() + "/" + record_info.TotalCnt.ToString() + ")";
                                    log_add_string(info_buff);
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_LIVE_SEND_ATTENDANCE_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU_VER_4_NEWID record_info;
                                    record_info = (AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU_VER_4_NEWID));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "AUTO Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + byte_to_unicode_string(record_info.EmployeeId)
                                        + " RecType:" + record_info.RecordType.ToString()
                                        + " ]";
                                    log_add_string(info_buff);
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU record_info;
                                    record_info = (AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_LIVE_SEND_ATTENDANCE_STRU));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "AUTO Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + Employee_array_to_srring(record_info.EmployeeId)
                                        + " RecType:" + record_info.RecordType.ToString()
                                        + " ]";
                                    log_add_string(info_buff);
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_RECORD_NUMBER_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_RECORD_NUMBER_STRU record_info;
                                record_info = (AnvizNew.CCHEX_RET_GET_RECORD_NUMBER_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_RECORD_NUMBER_STRU));
                                p_record_004.Text = record_info.record_num.ToString();

                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_RECORD_BY_EMPLOYEE_TIME_TYPE:
                            {

                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID record_info;
                                    record_info = (AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU_VER_4_NEWID));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                                    string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + byte_to_unicode_string(record_info.EmployeeId)
                                        + " RecType:" + record_info.record_type.ToString() + " " + record_info.CurIdx.ToString()
                                        + "/" + record_info.TotalCnt.ToString();
                                    log_add_string(info_buff);
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU record_info;
                                    record_info = (AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_EMPLOYEE_RECORD_BY_TIME_STRU));


                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(record_info.date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "Record Info ----[ Mid:" + record_info.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + Employee_array_to_srring(record_info.EmployeeId)
                                        + " RecType:" + record_info.record_type.ToString() + " " + record_info.CurIdx.ToString()
                                        + "/" + record_info.TotalCnt.ToString();
                                    log_add_string(info_buff);
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEV_LOGIN_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEV_LOGIN_STRU dev_info;
                                dev_info = (AnvizNew.CCHEX_RET_DEV_LOGIN_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEV_LOGIN_STRU));
                                string info_buff = "Dev Login --- [MachineId:" + dev_info.MachineId
                                    + " Version:" + byte_to_string(dev_info.Version)
                                    + " DevType:" + byte_to_string(dev_info.DevType)
                                    + " Addr:" + byte_to_string(dev_info.Addr)
                                    + "]";
                                //log_add_string(info_buff + "    " + dev_info.DevTypeFlag.ToString());
                                log_add_string(info_buff + "    " + Convert.ToString(dev_info.DevTypeFlag, 16));

                                //添加到设备列表
                                int dev_list_len = this.listViewDevice.Items.Count;
                                ListViewItem lvi = new ListViewItem();
                                lvi.ImageIndex = dev_list_len;
                                lvi.Text = dev_info.MachineId.ToString();
                                lvi.SubItems.Add(dev_info.DevIdx.ToString());
                                lvi.SubItems.Add(byte_to_string(dev_info.Addr));
                                lvi.SubItems.Add(byte_to_string(dev_info.Version));
                                this.listViewDevice.Items.Add(lvi);

                                DevCount++;
                                this.groupBox2.Text = "Device:(" + DevCount.ToString() + ")";

                                DevTypeFlag.Add(dev_info.DevIdx, (int)dev_info.DevTypeFlag);


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEV_LOGIN_CHANGE_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEV_LOGIN_STRU dev_info;
                                dev_info = (AnvizNew.CCHEX_RET_DEV_LOGIN_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEV_LOGIN_STRU));
                                string info_buff = "Login Change --- [MachineId:" + dev_info.MachineId
                                    + " Version:" + byte_to_string(dev_info.Version)
                                    + " DevType:" + byte_to_string(dev_info.DevType)
                                    + " Addr:" + byte_to_string(dev_info.Addr)
                                    + "]";
                                log_add_string(info_buff + "    " + Convert.ToString(dev_info.DevTypeFlag, 16));
                                //log_add_string(info_buff + "    " + dev_info.DevTypeFlag.ToString());
                                DevTypeFlag[dev_info.DevIdx] = (int)dev_info.DevTypeFlag;
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEV_LOGOUT_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEV_LOGOUT_STRU dev_info;
                                dev_info = (AnvizNew.CCHEX_RET_DEV_LOGOUT_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEV_LOGOUT_STRU));
                                string info_buff = "Dev Logout --- [MachineId:" + dev_info.MachineId
                                    + " Version:" + byte_to_string(dev_info.Version)
                                    + " DevType:" + byte_to_string(dev_info.DevType)
                                    + " Live:" + dev_info.Live
                                    + " Addr:" + byte_to_string(dev_info.Addr)
                                    + "]";


                                DevCount--;
                                this.groupBox2.Text = "Device:(" + DevCount.ToString() + ")";
                                log_add_string(info_buff);

                                ListViewItem foundItem = null;
                                if (foundItem == null)//just  test  has   machineid  
                                {
                                    int i = 0;
                                    for (i = 0; i < listViewDevice.Items.Count; i++)
                                    {
                                        foundItem = this.listViewDevice.Items[i];
                                        if (foundItem.Text.ToString() == dev_info.MachineId.ToString() && Convert.ToInt32(foundItem.SubItems[1].Text.ToString()) == dev_info.DevIdx)
                                        {
                                            break;
                                        }
                                        foundItem = null;
                                    }
                                }
                                //ListViewItem foundItem = this.listViewDevice.FindItemWithText(dev_info.MachineId.ToString(), false, 0);



                                if (foundItem != null)
                                {
                                    DevTypeFlag.Remove(dev_idx[0]);
                                    this.listViewDevice.Items.Remove(foundItem);
                                }

                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGADDNEW_UNICODE_S_DATE_INFO_TYPE:
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针
                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID));

                                        info_buff = "ADD Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay
                                                    + " "
                                                    + msg_data_rsp.StartHour + ":"
                                                    + msg_data_rsp.StartMin + ":"
                                                    + msg_data_rsp.StartSec
                                                    + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + " "
                                                    + msg_data_rsp.EndHour + ":"
                                                    + msg_data_rsp.EndMin + ":"
                                                    + msg_data_rsp.EndSec
                                                    + "]";
                                        byte[] ys = new byte[200];   //注意:后面消息内容数据长度限制200
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 200);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = byte_to_unicode_string(ys);


                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU));

                                        info_buff = "ADD Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay
                                                    + " "
                                                    + msg_data_rsp.StartHour + ":"
                                                    + msg_data_rsp.StartMin + ":"
                                                    + msg_data_rsp.StartSec
                                                    + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + " "
                                                    + msg_data_rsp.EndHour + ":"
                                                    + msg_data_rsp.EndMin + ":"
                                                    + msg_data_rsp.EndSec
                                                    + "]";
                                        byte[] ys = new byte[200];   //注意:后面消息内容数据长度限制200
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 200);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = byte_to_unicode_string(ys);

                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }



                                }
                                else
                                {
                                    info_buff = "add msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETBYIDX_UNICODE_S_DATE_INFO_TYPE:     //获取单条消息
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针
                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID));

                                        info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay
                                                    + " "
                                                    + msg_data_rsp.StartHour + ":"
                                                    + msg_data_rsp.StartMin + ":"
                                                    + msg_data_rsp.StartSec
                                                    + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + " "
                                                    + msg_data_rsp.EndHour + ":"
                                                    + msg_data_rsp.EndMin + ":"
                                                    + msg_data_rsp.EndSec
                                                    + "]";
                                        byte[] ys = new byte[200];   //注意:后面消息内容数据长度限制200
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 200);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Unicode.GetString(ys);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(byte_to_unicode_string(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU));

                                        info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay
                                                    + " "
                                                    + msg_data_rsp.StartHour + ":"
                                                    + msg_data_rsp.StartMin + ":"
                                                    + msg_data_rsp.StartSec
                                                    + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + " "
                                                    + msg_data_rsp.EndHour + ":"
                                                    + msg_data_rsp.EndMin + ":"
                                                    + msg_data_rsp.EndSec
                                                    + "]";
                                        byte[] ys = new byte[200];   //注意:后面消息内容数据长度限制200
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 200);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Unicode.GetString(ys);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(Employee_array_to_srring(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }



                                }
                                else
                                {
                                    info_buff = "get msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGADDNEW_INFO_TYPE:
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针

                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));

                                        info_buff = "ADD Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        byte[] ys = new byte[48];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 48);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Default.GetString(ys);


                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));

                                        info_buff = "ADD Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        byte[] ys = new byte[48];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 48);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Default.GetString(ys);



                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }

                                }
                                else
                                {
                                    info_buff = "add msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETBYIDX_INFO_TYPE:     //获取单条消息
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针

                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));

                                        info_buff = "Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        byte[] ys = new byte[48];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 48);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Default.GetString(ys);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(byte_to_unicode_string(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                            + msg_data_rsp.StartMonth + " - "
                                                            + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));

                                        info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        byte[] ys = new byte[48];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, 48);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Default.GetString(ys);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(Employee_array_to_srring(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                            + msg_data_rsp.StartMonth + " - "
                                                            + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }

                                }
                                else
                                {
                                    info_buff = "get msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGADDNEW_UNICODE_INFO_TYPE:
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针
                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));
                                        info_buff = "ADD Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        int ys_len = msg_head.Len - Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));
                                        byte[] ys = new byte[ys_len];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, ys_len);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = byte_to_unicode_string(ys);
                                        log_add_string(tmp_string);
                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));

                                        info_buff = "ADD Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        int ys_len = msg_head.Len - Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));
                                        byte[] ys = new byte[ys_len];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, ys_len);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = byte_to_unicode_string(ys);
                                        log_add_string(tmp_string);

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }

                                }
                                else
                                {
                                    info_buff = "add msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETBYIDX_UNICODE_INFO_TYPE:     //获取单条消息
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU msg_head;
                                msg_head = (AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU));
                                if (msg_head.Result == 0)
                                {
                                    IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)));//获取到后面数据段的起始指针
                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));

                                        info_buff = "Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        int ys_len = msg_head.Len - Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));
                                        byte[] ys = new byte[ys_len];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, ys_len);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Unicode.GetString(ys);
                                        log_add_string(tmp_string);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(byte_to_unicode_string(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                            + msg_data_rsp.StartMonth + " - "
                                                            + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }
                                    else
                                    {
                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU)Marshal.PtrToStructure(msg_data_p, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));

                                        info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                    + " Start Time["
                                                    + msg_data_rsp.StartYear + " - "
                                                    + msg_data_rsp.StartMonth + " - "
                                                    + msg_data_rsp.StartDay + "]------"
                                                    + " End Time["
                                                    + msg_data_rsp.EndYear + " - "
                                                    + msg_data_rsp.EndMonth + " - "
                                                    + msg_data_rsp.EndDay
                                                    + "]";
                                        int ys_len = msg_head.Len - Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));
                                        byte[] ys = new byte[ys_len];
                                        IntPtr msg_content_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETBYIDX_UNICODE_STRU)) + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)));//获取到后面数据段的起始指针
                                        Marshal.Copy(msg_content_p, ys, 0, ys_len);
                                        //string msg_info = Marshal.PtrToStringUni(msg_content_p);
                                        String tmp_string = System.Text.Encoding.Unicode.GetString(ys);
                                        log_add_string(tmp_string);

                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(Employee_array_to_srring(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                            + msg_data_rsp.StartMonth + " - "
                                                            + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            lvi.SubItems.Add(tmp_string);
                                            this.listViewMsg.Items.Add(lvi);
                                        }

                                        //Console.WriteLine(tmp_string);
                                        info_buff = info_buff + tmp_string;
                                        //Encoding.UTF32

                                    }

                                }
                                else
                                {
                                    info_buff = "get msg err...";

                                }
                                //Console.WriteLine(info_buff);
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETALLHEAD_INFO_TYPE:        //获取全部消息      ASCII版本
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETALLHEADUNICODE_INFO_TYPE:        //获取全部消息         UNICODE版本
                            {
                                AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU msg_head_rsp;
                                msg_head_rsp = (AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU));

                                IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU)));//获取到后面数据段的起始指针
                                if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    int number = (ret - Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU))) / Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));
                                    log_add_string(number.ToString());
                                    for (int i = 0; i < number; i++)
                                    {
                                        IntPtr ptr = new IntPtr(msg_data_p.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)) * i);

                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(ptr, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU_VER_4_NEWID));
                                        string info_buff = "Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                        + " Start Time["
                                                        + msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + "]------"
                                                        + " End Time["
                                                        + msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + "]";
                                        //Console.WriteLine(info_buff);
                                        //添加到列表
                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(byte_to_unicode_string(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            //lvi.SubItems.Add();
                                            this.listViewMsg.Items.Add(lvi);
                                        }
                                        log_add_string(info_buff);
                                    }
                                }
                                else
                                {
                                    int number = (ret - Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU))) / Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));
                                    log_add_string(number.ToString() + "ret = " + ret.ToString());
                                    for (int i = 0; i < number; i++)
                                    {
                                        IntPtr ptr = new IntPtr(msg_data_p.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU)) * i);

                                        AnvizNew.CCHEX_MSGHEAD_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEAD_INFO_STRU)Marshal.PtrToStructure(ptr, typeof(AnvizNew.CCHEX_MSGHEAD_INFO_STRU));
                                        string info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                        + " Start Time["
                                                        + msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + "]------"
                                                        + " End Time["
                                                        + msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + "]";
                                        //Console.WriteLine(info_buff);
                                        //添加到列表
                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(Employee_array_to_srring(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay);
                                            //lvi.SubItems.Add();
                                            this.listViewMsg.Items.Add(lvi);
                                        }
                                    }

                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_MSGGETALLHEADUNICODE_S_DATE_INFO_TYPE:        //获取全部消息  Seats公司定制
                            {
                                AnvizNew.CCHEX_RET_MSGADDNEW_UNICODE_STRU msg_head_rsp;
                                msg_head_rsp = (AnvizNew.CCHEX_RET_MSGADDNEW_UNICODE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MSGADDNEW_UNICODE_STRU));

                                IntPtr msg_data_p = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU)));//获取到后面数据段的起始指针

                                if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    int number = (ret - Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU))) / Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID));
                                    log_add_string(number.ToString());
                                    for (int i = 0; i < number; i++)
                                    {
                                        IntPtr ptr = new IntPtr(msg_data_p.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID)) * i);

                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(ptr, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU_VER_4_NEWID));
                                        string info_buff = "Msg Info --- [EmployeeId:" + byte_to_unicode_string(msg_data_rsp.EmployeeId)
                                                        + " Start Time["
                                                        + msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec
                                                        + "]------"
                                                        + " End Time["
                                                        + msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec
                                                        + "]";
                                        //Console.WriteLine(info_buff);
                                        //添加到列表
                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(byte_to_unicode_string(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec);
                                            //lvi.SubItems.Add();
                                            this.listViewMsg.Items.Add(lvi);
                                        }
                                    }
                                }
                                else
                                {
                                    int number = (ret - Marshal.SizeOf(typeof(AnvizNew.CCHEX_RET_MSGGETALLHEAD_UNICODE_STRU))) / Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU));
                                    log_add_string(number.ToString());
                                    for (int i = 0; i < number; i++)
                                    {
                                        IntPtr ptr = new IntPtr(msg_data_p.ToInt64() + Marshal.SizeOf(typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)) * i);

                                        AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU msg_data_rsp;
                                        msg_data_rsp = (AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU)Marshal.PtrToStructure(ptr, typeof(AnvizNew.CCHEX_MSGHEADUNICODE_INFO_STRU));
                                        string info_buff = "Msg Info --- [EmployeeId:" + Employee_array_to_srring(msg_data_rsp.EmployeeId)
                                                        + " Start Time["
                                                        + msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec
                                                        + "]------"
                                                        + " End Time["
                                                        + msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec
                                                        + "]";
                                        //Console.WriteLine(info_buff);
                                        //添加到列表
                                        {
                                            int msg_list_len = this.listViewMsg.Items.Count;
                                            ListViewItem lvi = new ListViewItem();
                                            lvi.ImageIndex = msg_list_len;
                                            lvi.Text = msg_list_len.ToString();
                                            lvi.SubItems.Add(Employee_array_to_srring(msg_data_rsp.EmployeeId));
                                            lvi.SubItems.Add(msg_data_rsp.StartYear + " - "
                                                        + msg_data_rsp.StartMonth + " - "
                                                        + msg_data_rsp.StartDay
                                                        + " "
                                                        + msg_data_rsp.StartHour + ":"
                                                        + msg_data_rsp.StartMin + ":"
                                                        + msg_data_rsp.StartSec);
                                            lvi.SubItems.Add(msg_data_rsp.EndYear + " - "
                                                        + msg_data_rsp.EndMonth + " - "
                                                        + msg_data_rsp.EndDay
                                                        + " "
                                                        + msg_data_rsp.EndHour + ":"
                                                        + msg_data_rsp.EndMin + ":"
                                                        + msg_data_rsp.EndSec);
                                            //lvi.SubItems.Add();
                                            this.listViewMsg.Items.Add(lvi);
                                        }
                                    }
                                }



                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_REBOOT_TYPE:
                            {
                                AnvizNew.CCHEX_RET_REBOOT_STRU reboot_rsp;

                                reboot_rsp = (AnvizNew.CCHEX_RET_REBOOT_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_REBOOT_STRU));
                                string info_buff = "Dev Reboot --- [MachineId:" + reboot_rsp.MachineId.ToString() + "]";
                                log_add_string(info_buff);
                                //this.listBoxLog.Items.Add(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_BELL_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_BELL_INFO_STRU bellinfo;

                                bellinfo = (AnvizNew.CCHEX_RET_GET_BELL_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_BELL_INFO_STRU));
                                
                                for(int i = 0;i<30;i++)
                                {
                                    string info_buff =  "NumberId: " +i.ToString() + " hour: "+ bellinfo.time_point[i].hour.ToString() +" min: " + bellinfo.time_point[i].minute.ToString() + " flag_week: " + bellinfo.time_point[i].flag_week.ToString();
                                    log_add_string(info_buff);
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_RECORD_INFO_STATUS_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEV_STATUS_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEV_STATUS_STRU status;

                                status = (AnvizNew.CCHEX_RET_DEV_STATUS_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEV_STATUS_STRU));
                                string info_buff = "Dev Status --- [MachineId:" + status.MachineId.ToString()
                                    + ", EmployeeNum:" + status.EmployeeNum.ToString()
                                    + ", FingerPrtNum:" + status.FingerPrtNum.ToString()
                                    + ", PasswdNum:" + status.PasswdNum.ToString()
                                    + ", CardNum:" + status.CardNum.ToString()
                                    + ", TotalRecNum:" + status.TotalRecNum.ToString()
                                    + ", NewRecNum:" + status.NewRecNum.ToString()
                                    + "]";
                                log_add_string(info_buff + Type[0].ToString());
                                //this.listBoxLog.Items.Add(info_buff);
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_SETTIME_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU msg_rsp;
                                msg_rsp = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                string info_buff = "Dev SetTime --- [MachineId: " + msg_rsp.MachineId.ToString() + ", Result: " + msg_rsp.Result.ToString() + "]";
                                log_add_string(info_buff);
                                //this.listBoxLog.Items.Add(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GETNETCFG_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GETNETCFG_STRU dev_net_info;
                                dev_net_info = (AnvizNew.CCHEX_RET_GETNETCFG_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GETNETCFG_STRU));
                                this.textBoxIp.Text = dev_net_info.Cfg.IpAddr[0] + "." + dev_net_info.Cfg.IpAddr[1] + "." + dev_net_info.Cfg.IpAddr[2] + "." + dev_net_info.Cfg.IpAddr[3];
                                this.textBoxMask.Text = dev_net_info.Cfg.IpMask[0] + "." + dev_net_info.Cfg.IpMask[1] + "." + dev_net_info.Cfg.IpMask[2] + "." + dev_net_info.Cfg.IpMask[3];
                                this.textBoxGw.Text = dev_net_info.Cfg.GwAddr[0] + "." + dev_net_info.Cfg.GwAddr[1] + "." + dev_net_info.Cfg.GwAddr[2] + "." + dev_net_info.Cfg.GwAddr[3];
                                this.textBoxMac.Text = dev_net_info.Cfg.MacAddr[0].ToString("X2") + "." + dev_net_info.Cfg.MacAddr[1].ToString("X2") + "." + dev_net_info.Cfg.MacAddr[2].ToString("X2") + "." + dev_net_info.Cfg.MacAddr[3].ToString("X2") + "." + dev_net_info.Cfg.MacAddr[4].ToString("X2") + "." + dev_net_info.Cfg.MacAddr[5].ToString("X2");
                                this.textBoxServIp.Text = dev_net_info.Cfg.ServAddr[0] + "." + dev_net_info.Cfg.ServAddr[1] + "." + dev_net_info.Cfg.ServAddr[2] + "." + dev_net_info.Cfg.ServAddr[3];
                                this.textBoxPort.Text = (dev_net_info.Cfg.Port[0] << 8 | dev_net_info.Cfg.Port[1]).ToString();
                                this.textBoxMode.Text = dev_net_info.Cfg.Mode.ToString();

                                this.textBoxRemote.Text = dev_net_info.Cfg.RemoteEnable.ToString();

                                if (dev_net_info.Cfg.DhcpEnable != 0)
                                {
                                    checkBoxDHCP.Checked = true;
                                }
                                else
                                {
                                    checkBoxDHCP.Checked = false;
                                }
                                string info_buff = "Get Net Config :Result:" + dev_net_info.Result;
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SETNETCFG_TYPE:
                            {
                                string info_buff;
                                AnvizNew.CCHEX_RET_SETNETCFG_STRU dev_net_rsp;
                                dev_net_rsp = (AnvizNew.CCHEX_RET_SETNETCFG_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_SETNETCFG_STRU));

                                info_buff = "Set Net Config :Result:" + dev_net_rsp.Result;
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_SN_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_SN_STRU sn_info;

                                sn_info = (AnvizNew.CCHEX_RET_GET_SN_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_SN_STRU));
                                this.textBox_sn.Text = "";
                                for (int i = 0; i < AnvizNew.SN_LEN && (char)sn_info.sn[i] != '\0'; i++)
                                {
                                    this.textBox_sn.Text += ((char)sn_info.sn[i]).ToString();
                                }
                                //this.textBox_sn.Text += ;//string.Join("", sn_info.sn); //sn_info.sn.ToString();
                                string info_buff = "SN Status --- [MachineId:" + sn_info.MachineId.ToString()
                                    + ", SN:" + this.textBox_sn.Text
                                    + "]";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_BASIC_CFG_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_BASIC_CFG_STRU basic_cfg;

                                basic_cfg = (AnvizNew.CCHEX_RET_GET_BASIC_CFG_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_BASIC_CFG_STRU));
                                this.comboBox1.SelectedIndex = basic_cfg.Cfg.volume;
                                /*this.comboBox2.SelectedIndex = basic_cfg.Cfg.date_time_format >> 4;  // high 4 bits
                                this.comboBox3.SelectedIndex = basic_cfg.Cfg.date_time_format & 0x0F;  // low 4 bits*/
                                this.comboBox2.SelectedIndex = basic_cfg.Cfg.date_format;
                                this.comboBox3.SelectedIndex = basic_cfg.Cfg.time_format;
                                /*int pwd_len = basic_cfg.Cfg.password[0] >> 4;  // high 4 bits of first byte
                                int pwd = (basic_cfg.Cfg.password[0] & 0x0F) << 16;
                                pwd += basic_cfg.Cfg.password[1] << 8;
                                pwd += basic_cfg.Cfg.password[2];*/
                                uint pwd = basic_cfg.Cfg.password;
                                this.textBox_pwd.Text = pwd.ToString();
                                this.textBox4_sleep_time.Text = basic_cfg.Cfg.delay_for_sleep.ToString();
                                string info_buff = "basic config --- [MachineId:" + basic_cfg.MachineId.ToString()
                                    + ", volume:" + basic_cfg.Cfg.volume.ToString()
                                    + ", psw(len=):" + pwd.ToString()
                                    + ", date_format:" + basic_cfg.Cfg.date_format.ToString()
                                    + "]";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_ONE_EMPLOYEE_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_LIST_PERSON_INFO_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_FLAG_CARDNO_BYTE_7)
                                {
                                    AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7 person_list;
                                    person_list = (AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_CARD_LEN_7));


                                    log_add_string(person_list.MachineId.ToString());
                                    log_add_string(person_list.CurIdx.ToString() + "/" + person_list.TotalCnt.ToString());
                                    // add person to listView_person
                                    int person_list_len = this.listView_person.Items.Count;
                                    ListViewItem lvi = new ListViewItem();
                                    lvi.ImageIndex = person_list_len;
                                    lvi.Text = (person_list_len + 1).ToString();
                                    //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                    byte[] temp = new byte[8];
                                    int i;
                                    for (i = 0; i < 5; i++)
                                    {
                                        //temp[8-5+i] = person_list.EmployeeId[i];
                                        temp[8 - 4 - i] = person_list.EmployeeId[i];
                                    }
                                    lvi.SubItems.Add(BitConverter.ToInt64(temp, 0).ToString());
                                    lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeName));
                                    if (person_list.password == 0xFFFFF)
                                    {
                                        lvi.SubItems.Add("");
                                    }
                                    else
                                    {
                                        //lvi.SubItems.Add(person_list.password.ToString());
                                        String tempstr = person_list.password.ToString();
                                        for (int i1 = 0; i1 < person_list.password_len - tempstr.Length; i1++)
                                        {
                                            tempstr = "0" + tempstr;
                                        }
                                        lvi.SubItems.Add(tempstr);
                                    }
                                    lvi.SubItems.Add(CardId_array_to_string(person_list.card_id));
                                    lvi.SubItems.Add(person_list.DepartmentId.ToString());
                                    lvi.SubItems.Add(person_list.GroupId.ToString());
                                    lvi.SubItems.Add(person_list.Mode.ToString());
                                    lvi.SubItems.Add(person_list.Fp_Status.ToString());
                                    lvi.SubItems.Add(person_list.Rserved2.ToString());
                                    lvi.SubItems.Add(person_list.Special.ToString());

                                    
                                    this.listView_person.Items.Add(lvi);
                                }
                                else
                                {
                                    if ((int)AnvizNew.MsgType.CCHEX_RET_GET_ONE_EMPLOYEE_INFO_TYPE == Type[0])
                                    {
                                        AnvizNew.CCHEX_RET_PERSON_INFO_STRU one_person;
                                        one_person = (AnvizNew.CCHEX_RET_PERSON_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_PERSON_INFO_STRU));
                                        if (one_person.TotalCnt == 0)
                                        {
                                            MessageBox.Show("Get One  Fail!");
                                            break;
                                        }

                                    }
                                    if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                    {
                                        AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4 person_list;
                                        person_list = (AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4));

                                        log_add_string(person_list.MachineId.ToString());
                                        log_add_string(person_list.CurIdx.ToString() + "/" + person_list.TotalCnt.ToString());
                                        int person_list_len = this.listView_person.Items.Count;
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = person_list_len;
                                        lvi.Text = (person_list_len + 1).ToString();
                                        //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                        lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeId));
                                        lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeName));
                                        if (person_list.password == 0xFFFFF)
                                        {
                                            lvi.SubItems.Add("");
                                        }
                                        else
                                        {
                                            String tempstr = person_list.password.ToString();
                                            for (int i1 = 0; i1 < person_list.password_len - tempstr.Length; i1++)
                                            {
                                                tempstr = "0" + tempstr;
                                            }
                                            lvi.SubItems.Add(tempstr);
                                        }
                                        if (person_list.max_card_id == 6 && person_list.card_id == 0xFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else if (person_list.max_card_id == 10 && person_list.card_id == 0xFFFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else
                                        {
                                            lvi.SubItems.Add(person_list.card_id.ToString());
                                        }
                                        lvi.SubItems.Add(person_list.DepartmentId.ToString());
                                        lvi.SubItems.Add(person_list.GroupId.ToString());
                                        lvi.SubItems.Add(person_list.Mode.ToString());
                                        lvi.SubItems.Add(person_list.Fp_Status.ToString());
                                        lvi.SubItems.Add(person_list.Rserved2.ToString());
                                        lvi.SubItems.Add(person_list.Special.ToString());

                                        DateTime date1 = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(person_list.start_date, 0)));

                                        DateTime date2 = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(person_list.end_date, 0)));
                                        //text_time2.Text = date2.ToString("yyyy-MM-dd HH:mm:ss");
                                        lvi.SubItems.Add(date1.ToString("yyyy-MM-dd HH:mm:ss"));
                                        lvi.SubItems.Add(date2.ToString("yyyy-MM-dd HH:mm:ss"));
                                        this.listView_person.Items.Add(lvi);
                                    }
                                    else if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_EMPLOYEE_FOR_W2_ADD_TIME)//this for ANVIZ_CUSTOM_FOR_ComsecITech_W2  SDK
                                    {
                                        AnvizNew.CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 person_list;
                                        person_list = (AnvizNew.CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2));


                                        int person_list_len = this.listView_person.Items.Count;
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = person_list_len;
                                        lvi.Text = (person_list_len + 1).ToString();
                                        //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                        byte[] temp = new byte[8];
                                        int i;
                                        for (i = 0; i < 5; i++)
                                        {
                                            //temp[8-5+i] = person_list.EmployeeId[i];
                                            temp[8 - 4 - i] = person_list.EmployeeId[i];
                                        }
                                        lvi.SubItems.Add(BitConverter.ToInt64(temp, 0).ToString());
                                        lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeName));
                                        if (person_list.password == 0xFFFFF)
                                        {
                                            lvi.SubItems.Add("");
                                        }
                                        else
                                        {
                                            String tempstr = person_list.password.ToString();
                                            for (int i1 = 0; i1 < person_list.password_len - tempstr.Length; i1++)
                                            {
                                                tempstr = "0" + tempstr;
                                            }
                                            lvi.SubItems.Add(tempstr);
                                        }
                                        if (person_list.max_card_id == 6 && person_list.card_id == 0xFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else if (person_list.max_card_id == 10 && person_list.card_id == 0xFFFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else
                                        {
                                            lvi.SubItems.Add(person_list.card_id.ToString());
                                        }
                                        lvi.SubItems.Add(person_list.DepartmentId.ToString());
                                        lvi.SubItems.Add(person_list.GroupId.ToString());
                                        lvi.SubItems.Add(person_list.Mode.ToString());
                                        lvi.SubItems.Add(person_list.Fp_Status.ToString());
                                        lvi.SubItems.Add(person_list.Rserved2.ToString());
                                        lvi.SubItems.Add(person_list.Special.ToString());

                                        DateTime date1 = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(person_list.start_date, 0)));

                                        DateTime date2 = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(person_list.end_date, 0)));
                                        //text_time2.Text = date2.ToString("yyyy-MM-dd HH:mm:ss");
                                        lvi.SubItems.Add(date1.ToString("yyyy-MM-dd HH:mm:ss"));
                                        lvi.SubItems.Add(date2.ToString("yyyy-MM-dd HH:mm:ss"));
                                        this.listView_person.Items.Add(lvi);
                                    }
                                    else if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.EmployeeType.DEV_TYPE_FLAG_MSG_ASCII_32)
                                    {                                                                                           ////this for NORMAL  SDK
                                        AnvizNew.CCHEX_RET_PERSON_INFO_STRU person_list;
                                        person_list = (AnvizNew.CCHEX_RET_PERSON_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_PERSON_INFO_STRU));



                                        // add person to listView_person
                                        int person_list_len = this.listView_person.Items.Count;
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = person_list_len;
                                        lvi.Text = (person_list_len + 1).ToString();
                                        //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                        byte[] temp = new byte[8];
                                        int i;
                                        for (i = 0; i < 5; i++)
                                        {
                                            //temp[8-5+i] = person_list.EmployeeId[i];
                                            temp[8 - 4 - i] = person_list.EmployeeId[i];
                                        }
                                        lvi.SubItems.Add(BitConverter.ToInt64(temp, 0).ToString());


                                        //lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeName));
                                        lvi.SubItems.Add(Encoding.Default.GetString(person_list.EmployeeName));

                                        if (person_list.password == 0xFFFFF)
                                        {
                                            lvi.SubItems.Add("");
                                        }
                                        else
                                        {
                                            //lvi.SubItems.Add(person_list.password.ToString());
                                            String tempstr = person_list.password.ToString();
                                            for (int i1 = 0; i1 < person_list.password_len - tempstr.Length; i1++)
                                            {
                                                tempstr = "0" + tempstr;
                                            }
                                            lvi.SubItems.Add(tempstr);
                                        }
                                        if (person_list.max_card_id == 6 && person_list.card_id == 0xFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else if (person_list.max_card_id == 10 && person_list.card_id == 0xFFFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else
                                        {
                                            lvi.SubItems.Add(person_list.card_id.ToString());
                                        }
                                        lvi.SubItems.Add(person_list.DepartmentId.ToString());
                                        lvi.SubItems.Add(person_list.GroupId.ToString());
                                        lvi.SubItems.Add(person_list.Mode.ToString());
                                        lvi.SubItems.Add(person_list.Fp_Status.ToString());
                                        lvi.SubItems.Add(person_list.Rserved2.ToString());
                                        lvi.SubItems.Add(person_list.Special.ToString());


                                        this.listView_person.Items.Add(lvi);

                                    }
                                    else
                                    {                                                                                           ////this for NORMAL  SDK
                                        AnvizNew.CCHEX_RET_PERSON_INFO_STRU person_list;
                                        person_list = (AnvizNew.CCHEX_RET_PERSON_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_PERSON_INFO_STRU));


                                        log_add_string(person_list.MachineId.ToString());
                                        log_add_string(person_list.CurIdx.ToString() + "/" + person_list.TotalCnt.ToString());
                                        // add person to listView_person
                                        int person_list_len = this.listView_person.Items.Count;
                                        ListViewItem lvi = new ListViewItem();
                                        lvi.ImageIndex = person_list_len;
                                        lvi.Text = (person_list_len + 1).ToString();
                                        //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                        byte[] temp = new byte[8];
                                        int i;
                                        for (i = 0; i < 5; i++)
                                        {
                                            //temp[8-5+i] = person_list.EmployeeId[i];
                                            temp[8 - 4 - i] = person_list.EmployeeId[i];
                                        }
                                        lvi.SubItems.Add(BitConverter.ToInt64(temp, 0).ToString());
                                        lvi.SubItems.Add(byte_to_unicode_string(person_list.EmployeeName));
                                        if (person_list.password == 0xFFFFF)
                                        {
                                            lvi.SubItems.Add("");
                                        }
                                        else
                                        {
                                            //lvi.SubItems.Add(person_list.password.ToString());
                                            String tempstr = person_list.password.ToString();
                                            for (int i1 = 0; i1 < person_list.password_len - tempstr.Length; i1++)
                                            {
                                                tempstr = "0" + tempstr;
                                            }
                                            lvi.SubItems.Add(tempstr);
                                        }
                                        if (person_list.max_card_id == 6 && person_list.card_id == 0xFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else if (person_list.max_card_id == 10 && person_list.card_id == 0xFFFFFFFF)
                                        {
                                            lvi.SubItems.Add("-1");
                                        }
                                        else
                                        {
                                            lvi.SubItems.Add(person_list.card_id.ToString());
                                        }
                                        lvi.SubItems.Add(person_list.DepartmentId.ToString());
                                        lvi.SubItems.Add(person_list.GroupId.ToString());
                                        lvi.SubItems.Add(person_list.Mode.ToString());
                                        lvi.SubItems.Add(person_list.Fp_Status.ToString());
                                        lvi.SubItems.Add(person_list.Rserved2.ToString());
                                        lvi.SubItems.Add(person_list.Special.ToString());

                                        // DR
                                        if ((DevTypeFlag[dev_idx[0]] & 0xFF) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_EMPLOYEE_FOR_DR_ADD_NAME2)
                                        {
                                            //add display , to add 
                                            // TODO: 
                                            //DevTypeFlag = 1;// DR
                                            dbg_info(BitConverter.ToInt64(temp, 0).ToString() + "EmployeeName2:" + byte_to_unicode_string(person_list.EmployeeName2) +
                                                     ",RFC:" + Encoding.ASCII.GetString(person_list.RFC) +
                                                     ",CURP:" + Encoding.ASCII.GetString(person_list.CURP));

                                        }
                                        else
                                        {
                                            //DevTypeFlag = 0;// NORMAL
                                        }
                                        this.listView_person.Items.Add(lvi);

                                    }
                                }

                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULEMPLOYEE_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULEMPLOYEE2_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULEMPLOYEE2W2_INFO_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULEMPLOYEE2_UNICODE_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU result;
                                result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Modify person OK Employeeid =" + Employee_array_to_srring(result.EmployeeId) + "  machineid :"+ result.MachineId.ToString());
                                    // TODO: modify listView_person.Items
                                }
                                else
                                {
                                    MessageBox.Show("Modify person Fail! Employeeid =" + Employee_array_to_srring(result.EmployeeId));
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULEMPLOYEE_VER_4_NEWID_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID result;
                                result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Modify person OK " + byte_to_unicode_string(result.EmployeeId));
                                    // TODO: modify listView_person.Items
                                }
                                else
                                {
                                    MessageBox.Show("Modify person Fail!" + byte_to_unicode_string(result.EmployeeId));
                                }
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_DEL_PERSON_INFO_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("Delete person OK  ID == " + byte_to_unicode_string(result.EmployeeId));
                                        listView_person.Items.RemoveAt(listView_person.FocusedItem.ImageIndex);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Delete person Fail!");
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU result;
                                    result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU));
                                    if (result.Result == 0)
                                    {
                                        byte[] temp = new byte[8];
                                        int i;
                                        for (i = 0; i < 5; i++)
                                        {
                                            temp[8 - 4 - i] = result.EmployeeId[i];
                                        }
                                        log_add_string(BitConverter.ToInt64(temp, 0).ToString());
                                        MessageBox.Show("Delete person OK  ID == " + BitConverter.ToInt64(temp, 0).ToString());
                                        listView_person.Items.RemoveAt(listView_person.FocusedItem.ImageIndex);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Delete person Fail!");
                                    }

                                }

                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DLFINGERPRT_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        //if ((int)result.fp_len == 338)
                                        if ((int)result.fp_len >0)
                                        {
                                            String msg = "";
                                            int i;

                                            //button_put_fp_raw_data.DataBindings = System.Text.Encoding.Default.GetString(result.Data);
                                            //button_put_fp_raw_data.text = System.Text.Encoding.Default.GetString(result.Data);
                                            Array.Copy(result.Data, buff_fin, (int)result.fp_len);
                                            len_fin = (int)result.fp_len;


                                            for (i = 0; i < result.fp_len; i++)
                                            {
                                                msg = msg + string.Format("{0:x2}", result.Data[i]).ToUpper();
                                            }
                                            MessageBox.Show(msg);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Download    Ok " + byte_to_unicode_string(result.EmployeeId));
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get FingerPrint Fail!" + byte_to_unicode_string(result.EmployeeId));
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU));
                                    if (result.Result == 0)
                                    {
                                        //if ((int)result.fp_len == 338)
                                            if ((int)result.fp_len > 0)
                                            {
                                            String msg = "";
                                            int i;

                                            //button_put_fp_raw_data.DataBindings = System.Text.Encoding.Default.GetString(result.Data);
                                            //button_put_fp_raw_data.text = System.Text.Encoding.Default.GetString(result.Data);
                                            Array.Copy(result.Data, buff_fin, (int)result.fp_len);
                                            len_fin = (int)result.fp_len;


                                            for (i = 0; i < result.fp_len; i++)
                                            {
                                                msg = msg + string.Format("{0:x2}", result.Data[i]).ToUpper();
                                            }
                                            MessageBox.Show(msg);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Download    Ok " + Employee_array_to_srring(result.EmployeeId));
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get FingerPrint Fail!" + Employee_array_to_srring(result.EmployeeId));
                                    }
                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_ULFINGERPRT_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("UPload FP OK" + byte_to_unicode_string(result.EmployeeId));
                                    }
                                    else
                                    {
                                        MessageBox.Show("UPload FP Fail!" + byte_to_unicode_string(result.EmployeeId));
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU));
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("UPload FP OK" + Employee_array_to_srring(result.EmployeeId));
                                    }
                                    else
                                    {
                                        MessageBox.Show("UPload FP Fail!" + Employee_array_to_srring(result.EmployeeId));
                                    }
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UPLOAD_RECORD_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("ADD record  OK  ID == " + byte_to_unicode_string(result.EmployeeId));
                                    }
                                    else
                                    {
                                        MessageBox.Show("ADD record  Fail!");
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU result;
                                    result = (AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_EMPLOYEE_INFO_STRU));
                                    if (result.Result == 0)
                                    {
                                        byte[] temp = new byte[8];
                                        int i;
                                        for (i = 0; i < 5; i++)
                                        {
                                            temp[8 - 4 - i] = result.EmployeeId[i];
                                        }
                                        log_add_string(BitConverter.ToInt64(temp, 0).ToString());
                                        MessageBox.Show("ADD record OK  ID == " + BitConverter.ToInt64(temp, 0).ToString());
                                    }
                                    else
                                    {
                                        MessageBox.Show("ADD record  Fail!");
                                    }

                                }

                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEL_RECORD_OR_FLAG_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEL_RECORD_STRU result;
                                result = (AnvizNew.CCHEX_RET_DEL_RECORD_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_RECORD_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Deleted count:" + result.deleted_count);
                                }
                                else
                                {
                                    MessageBox.Show("Delete Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GETTIME_TYPE:
                            {
                                AnvizNew.CCHEX_MSG_GETTIME_STRU_EXT_INF time_info;
                                time_info = (AnvizNew.CCHEX_MSG_GETTIME_STRU_EXT_INF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_MSG_GETTIME_STRU_EXT_INF));
                                if (time_info.Result == 0)
                                {
                                    this.Box6_year.Text = time_info.config.Year.ToString();
                                    this.Box6_month.Text = time_info.config.Month.ToString();
                                    this.Box6_day.Text = time_info.config.Day.ToString();

                                    this.Box6_hour.Text = time_info.config.Hour.ToString();
                                    this.Box6_min.Text = time_info.config.Min.ToString();
                                    this.Box6_sec.Text = time_info.config.Sec.ToString();

                                    string info_buff = "Get Time :Result:" + time_info.Result;
                                    log_add_string(info_buff);
                                    MessageBox.Show("Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_BASIC_CFG2_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF datainfo;

                                datainfo = (AnvizNew.CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_BASIC_CFG2_STRU_EXT_INF));
                                if (datainfo.Result == 0)
                                {
                                    config2_1.Text = datainfo.config.compare_level.ToString();
                                    config2_2.Text = datainfo.config.wiegand_range.ToString();
                                    config2_3.Text = datainfo.config.wiegand_type.ToString();
                                    config2_4.Text = datainfo.config.work_code.ToString();
                                    config2_5.Text = (datainfo.config.real_time_send & 0x01).ToString();
                                    config2_52.Text = ((datainfo.config.real_time_send >> 1) & (0x01)).ToString();

                                    config2_6.Text = datainfo.config.auto_update.ToString();
                                    config2_7.Text = datainfo.config.bell_lock.ToString();
                                    config2_8.Text = datainfo.config.lock_delay.ToString();

                                    config2_9.Text = datainfo.config.record_over_alarm.ToString();
                                    config2_10.Text = datainfo.config.re_attendance_delay.ToString();
                                    config2_11.Text = datainfo.config.door_sensor_alarm.ToString();
                                    config2_12.Text = datainfo.config.bell_delay.ToString();
                                    config2_13.Text = datainfo.config.correct_time.ToString();

                                    string info_buff = "Get Config2 :Result:" + datainfo.Result;
                                    log_add_string(info_buff + this.config2_1.Text.ToString());
                                    MessageBox.Show("Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_BASIC_CFG2_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Set Ok");
                                }
                                else
                                {
                                    MessageBox.Show("Set  Fail!");
                                }
                            }
                            break;


                        case (int)AnvizNew.MsgType.CCHEX_RET_INIT_USER_AREA_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Init Ok");
                                }
                                else
                                {
                                    MessageBox.Show("Init  Fail!");
                                }
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_INIT_SYSTEM_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Init Ok");
                                }
                                else
                                {
                                    MessageBox.Show("Init  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_PERIOD_TIME_TYPE:
                            {
                                AnvizNew.CCHEX_GET_PERIOD_TIME_STRU_EXT_INF datainfo;
                                datainfo = (AnvizNew.CCHEX_GET_PERIOD_TIME_STRU_EXT_INF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_GET_PERIOD_TIME_STRU_EXT_INF));

                                if (datainfo.Result == 0)
                                {
                                    time_p11.Text = datainfo.day_week[0].StartHour.ToString();
                                    time_p12.Text = datainfo.day_week[0].StartMin.ToString();
                                    time_p13.Text = datainfo.day_week[0].EndHour.ToString();
                                    time_p14.Text = datainfo.day_week[0].EndMin.ToString();

                                    time_p21.Text = datainfo.day_week[1].StartHour.ToString();
                                    time_p22.Text = datainfo.day_week[1].StartMin.ToString();
                                    time_p23.Text = datainfo.day_week[1].EndHour.ToString();
                                    time_p24.Text = datainfo.day_week[1].EndMin.ToString();

                                    time_p31.Text = datainfo.day_week[2].StartHour.ToString();
                                    time_p32.Text = datainfo.day_week[2].StartMin.ToString();
                                    time_p33.Text = datainfo.day_week[2].EndHour.ToString();
                                    time_p34.Text = datainfo.day_week[2].EndMin.ToString();

                                    time_p41.Text = datainfo.day_week[3].StartHour.ToString();
                                    time_p42.Text = datainfo.day_week[3].StartMin.ToString();
                                    time_p43.Text = datainfo.day_week[3].EndHour.ToString();
                                    time_p44.Text = datainfo.day_week[3].EndMin.ToString();

                                    time_p51.Text = datainfo.day_week[4].StartHour.ToString();
                                    time_p52.Text = datainfo.day_week[4].StartMin.ToString();
                                    time_p53.Text = datainfo.day_week[4].EndHour.ToString();
                                    time_p54.Text = datainfo.day_week[4].EndMin.ToString();

                                    time_p61.Text = datainfo.day_week[5].StartHour.ToString();
                                    time_p62.Text = datainfo.day_week[5].StartMin.ToString();
                                    time_p63.Text = datainfo.day_week[5].EndHour.ToString();
                                    time_p64.Text = datainfo.day_week[5].EndMin.ToString();

                                    time_p71.Text = datainfo.day_week[6].StartHour.ToString();
                                    time_p72.Text = datainfo.day_week[6].StartMin.ToString();
                                    time_p73.Text = datainfo.day_week[6].EndHour.ToString();
                                    time_p74.Text = datainfo.day_week[6].EndMin.ToString();

                                    string info_buff = "Get PeriodTime :Result:" + datainfo.Result + datainfo.MachineId;
                                    log_add_string(info_buff);
                                    MessageBox.Show("Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_PERIOD_TIME_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok");
                                }
                                else
                                {
                                    MessageBox.Show("SET  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_ADD_FINGERPRINT_ONLINE_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {

                                        Array.Copy(result.Data, buff_fin, (int)result.fp_len);
                                        len_fin = (int)result.fp_len;
                                        MessageBox.Show("Add FP OK!" + byte_to_unicode_string(result.EmployeeId));
                                    }
                                    else
                                    {
                                        MessageBox.Show("Add FP Fail!" + byte_to_unicode_string(result.EmployeeId));
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DLFINGERPRT_STRU result;
                                    result = (AnvizNew.CCHEX_RET_DLFINGERPRT_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DLFINGERPRT_STRU));
                                    if (result.Result == 0)
                                    {
                                        Array.Copy(result.Data, buff_fin, (int)result.fp_len);
                                        len_fin = (int)result.fp_len;

                                        MessageBox.Show("Add FP OK!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Add FP Fail!");
                                    }
                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_TEAM_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_GET_TEAM_INFO_STRU_EXT_INF result;
                                result = (AnvizNew.CCHEX_GET_TEAM_INFO_STRU_EXT_INF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_GET_TEAM_INFO_STRU_EXT_INF));
                                if (result.Result == 0)
                                {
                                    select_01.Text = result.PeriodTimeNumber[0].ToString();
                                    select_02.Text = result.PeriodTimeNumber[1].ToString();
                                    select_03.Text = result.PeriodTimeNumber[2].ToString();
                                    select_04.Text = result.PeriodTimeNumber[3].ToString();
                                    MessageBox.Show("GET Team Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Team Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_TEAM_INFO_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Team Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Team Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_CLEAR_ADMINISTRAT_FLAG_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Clear Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Clear Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_FORCED_UNLOCK_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("FORCED UNLOCK Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("FORCED UNLOCK Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UDP_SEARCH_DEV_TYPE:
                            {
                                AnvizNew.CCHEX_UDP_SEARCH_ALL_STRU_EXT_INF result;
                                result = (AnvizNew.CCHEX_UDP_SEARCH_ALL_STRU_EXT_INF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_UDP_SEARCH_ALL_STRU_EXT_INF));
                                IntPtr temptr;
                                if (result.DevNum > 0)
                                {
                                    MessageBox.Show("Search Dev > 0");
                                    temptr = pBuff + 4;
                                    log_add_string(result.DevNum.ToString());
                                    for (int i = 0; i < result.DevNum; i++)
                                    {
                                        AnvizNew.CCHEX_UDP_SEARCH_STRU_EXT_INF one_dev_info = result.dev_net_info[i];
                                        log_add_string(one_dev_info.DevHardwareType.ToString());
                                        if (one_dev_info.Result == 0)
                                        {
                                            //log_add_string(one_dev_info.DevHardwareType.ToString());
                                            //temptr = temptr;
                                            ListViewItem lvi = new ListViewItem();
                                            //log_add_string("#####################0001");
                                            switch (one_dev_info.DevHardwareType)
                                            {

                                                case (int)AnvizNew.NetCardType.NETCARD_WITHOUT_DNS:
                                                    {
                                                        AnvizNew.CCHEX_UDP_SEARCH_STRU withoutcard;
                                                        withoutcard = (AnvizNew.CCHEX_UDP_SEARCH_STRU)Marshal.PtrToStructure(temptr, typeof(AnvizNew.CCHEX_UDP_SEARCH_STRU));

                                                        lvi.Text = (i + 1).ToString();
                                                        lvi.SubItems.Add(((int)AnvizNew.NetCardType.NETCARD_WITHOUT_DNS).ToString());
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.DevNetInfo.IpAddr));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.DevNetInfo.IpMask));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.DevNetInfo.GwAddr));


                                                        string hexOutput = withoutcard.DevNetInfo.MacAddr[0].ToString("X2") + "." + withoutcard.DevNetInfo.MacAddr[1].ToString("X2") + "." + withoutcard.DevNetInfo.MacAddr[2].ToString("X2") + "." + withoutcard.DevNetInfo.MacAddr[3].ToString("X2") + "." + withoutcard.DevNetInfo.MacAddr[4].ToString("X2") + "." + withoutcard.DevNetInfo.MacAddr[5].ToString("X2"); ;

                                                        lvi.SubItems.Add(hexOutput);


                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.DevNetInfo.ServAddr));
                                                        lvi.SubItems.Add((withoutcard.DevNetInfo.Port[0] * 0x100 + withoutcard.DevNetInfo.Port[1]).ToString());
                                                        lvi.SubItems.Add(withoutcard.DevNetInfo.NetMode.ToString());
                                                        lvi.SubItems.Add(one_dev_info.MachineId.ToString());
                                                        lvi.SubItems.Add("");

                                                        lvi.SubItems.Add(System.Text.Encoding.Default.GetString(withoutcard.DevType));
                                                        lvi.SubItems.Add(System.Text.Encoding.Default.GetString(withoutcard.DevSerialNum));
                                                        lvi.SubItems.Add("");
                                                        lvi.SubItems.Add("");
                                                    }
                                                    break;
                                                case (int)AnvizNew.NetCardType.NETCARD_WITH_DNS:
                                                    {
                                                        AnvizNew.CCHEX_UDP_SEARCH_WITH_DNS_STRU withoutcard;
                                                        withoutcard = (AnvizNew.CCHEX_UDP_SEARCH_WITH_DNS_STRU)Marshal.PtrToStructure(temptr, typeof(AnvizNew.CCHEX_UDP_SEARCH_WITH_DNS_STRU));
                                                        //ListViewItem lvi = new ListViewItem();
                                                        lvi.Text = (i + 1).ToString();
                                                        lvi.SubItems.Add(((int)AnvizNew.NetCardType.NETCARD_WITH_DNS).ToString());
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.BasicSearch.DevNetInfo.IpAddr));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.BasicSearch.DevNetInfo.IpMask));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.BasicSearch.DevNetInfo.GwAddr));


                                                        string hexOutput = withoutcard.BasicSearch.DevNetInfo.MacAddr[0].ToString("X2") + "." + withoutcard.BasicSearch.DevNetInfo.MacAddr[1].ToString("X2") + "." + withoutcard.BasicSearch.DevNetInfo.MacAddr[2].ToString("X2") + "." + withoutcard.BasicSearch.DevNetInfo.MacAddr[3].ToString("X2") + "." + withoutcard.BasicSearch.DevNetInfo.MacAddr[4].ToString("X2") + "." + withoutcard.BasicSearch.DevNetInfo.MacAddr[5].ToString("X2");

                                                        lvi.SubItems.Add(hexOutput);

                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.BasicSearch.DevNetInfo.ServAddr));
                                                        lvi.SubItems.Add((withoutcard.BasicSearch.DevNetInfo.Port[0] * 0x100 + withoutcard.BasicSearch.DevNetInfo.Port[1]).ToString());
                                                        lvi.SubItems.Add(withoutcard.BasicSearch.DevNetInfo.NetMode.ToString());
                                                        lvi.SubItems.Add(one_dev_info.MachineId.ToString());
                                                        lvi.SubItems.Add("");

                                                        lvi.SubItems.Add(System.Text.Encoding.Default.GetString(withoutcard.BasicSearch.DevType));
                                                        lvi.SubItems.Add(System.Text.Encoding.Default.GetString(withoutcard.BasicSearch.DevSerialNum));

                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.Dns));
                                                        lvi.SubItems.Add(System.Text.Encoding.Default.GetString(withoutcard.Url));
                                                    }
                                                    break;
                                                case (int)AnvizNew.NetCardType.NETCARD_TWO_CARD:
                                                    {
                                                        log_add_string("~~~~~~~~~~~");
                                                        AnvizNew.CCHEX_UDP_SEARCH_TWO_CARD_STRU withoutcard;
                                                        withoutcard = (AnvizNew.CCHEX_UDP_SEARCH_TWO_CARD_STRU)Marshal.PtrToStructure(temptr, typeof(AnvizNew.CCHEX_UDP_SEARCH_TWO_CARD_STRU));
                                                        //ListViewItem lvi = new ListViewItem();
                                                        lvi.Text = (i + 1).ToString();
                                                        lvi.SubItems.Add(((int)AnvizNew.NetCardType.NETCARD_TWO_CARD).ToString());
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.CardInfo[0].IpAddr));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.CardInfo[0].IpMask));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.CardInfo[0].GwAddr));



                                                        string hexOutput = withoutcard.CardInfo[0].MacAddr[0].ToString("X2") + "." + withoutcard.CardInfo[0].MacAddr[1].ToString("X2") + "." + withoutcard.CardInfo[0].MacAddr[2].ToString("X2") + "." + withoutcard.CardInfo[0].MacAddr[3].ToString("X2") + "." + withoutcard.CardInfo[0].MacAddr[4].ToString("X2") + "." + withoutcard.CardInfo[0].MacAddr[5].ToString("X2");
                                                        lvi.SubItems.Add(hexOutput);
                                                        //lvi.SubItems.Add(System.Text.Encoding.ASCII.GetString(withoutcard.CardInfo[0].MacAddr));
                                                        lvi.SubItems.Add(bytetoipstr(withoutcard.ServAddr));
                                                        lvi.SubItems.Add((withoutcard.Port[0] * 0x100 + withoutcard.Port[1]).ToString());
                                                        lvi.SubItems.Add(withoutcard.NetMode.ToString());
                                                        lvi.SubItems.Add(one_dev_info.MachineId.ToString());
                                                        lvi.SubItems.Add("");

                                                        lvi.SubItems.Add("");
                                                        lvi.SubItems.Add("");
                                                        lvi.SubItems.Add("");
                                                        lvi.SubItems.Add("");
                                                        log_add_string("~~~~~~~~~~~" + bytetoipstr(withoutcard.CardInfo[0].IpAddr));
                                                    }
                                                    break;

                                            }
                                            this.listView1.Items.Add(lvi);

                                        }
                                        temptr = temptr + Marshal.SizeOf(one_dev_info);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Search Dev == 0!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UDP_SET_DEV_CONFIG_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("UDP  Set Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("UDP Set Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_BASIC_CFG5_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Config5  Set Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Config5 Set Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_MACHINE_ID_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_MACHINE_ID_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_MACHINE_ID_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_MACHINE_ID_STRU));
                                if (result.Result == 0)
                                {
                                    machineid_01.Text = result.MachineId.ToString();
                                    log_add_string(result.cur_machineid.ToString());
                                    MessageBox.Show("Get Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Get Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_MACHINE_ID_TYPE:
                            {
                                AnvizNew.CCHEX_RET_SET_MACHINE_ID_STRU result;
                                result = (AnvizNew.CCHEX_RET_SET_MACHINE_ID_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_SET_MACHINE_ID_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Set Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Set Fail!");
                                }
                            }
                            break;


                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_MACHINE_TYPE_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_MACHINE_TYPE_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_MACHINE_TYPE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_MACHINE_TYPE_STRU));
                                if (result.Result == 0)
                                {

                                    ListViewItem foundItem = this.listViewDevice.FindItemWithText(result.MachineId.ToString(), false, 0);
                                    if (foundItem != null)
                                    {
                                        DevTypeFlag[dev_idx[0]] = result.DevTypeFlag;
                                        MessageBox.Show("Set Ok! ");
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Set Fail!");
                                }
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_BASIC_CFG5_TYPE:
                            {
                                AnvizNew.CCHEX_GET_BASIC_CFG_INFO5_STRU result;
                                result = (AnvizNew.CCHEX_GET_BASIC_CFG_INFO5_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_GET_BASIC_CFG_INFO5_STRU));
                                if (result.Result == 0)
                                {
                                    config5_01.Text = result.param.fail_alarm_time.ToString();
                                    config5_02.Text = result.param.tamper_alarm.ToString();
                                    MessageBox.Show("GConfig5 get Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Config5 get Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_CARD_ID_TYPE:
                            {
                                AnvizNew.CCHEX_GET_CARD_ID_STRUF result;
                                result = (AnvizNew.CCHEX_GET_CARD_ID_STRUF)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_GET_CARD_ID_STRUF));
                                if (result.Result == 0)
                                {
                                    card_01.Text = result.card_id.ToString();
                                    MessageBox.Show("Card get Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Card get Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_DEV_CURRENT_STATUS_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_URL_TYPE:
                            {
                                AnvizNew.CCHEX_GET_URL_STRU result;
                                result = (AnvizNew.CCHEX_GET_URL_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_GET_URL_STRU));
                                if (result.Result == 0)
                                {
                                    url_dns.Text = bytetoipstr(result.Dns);
                                    url_01.Text = System.Text.Encoding.Default.GetString(result.Url);
                                    MessageBox.Show("GET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_URL_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UPLOADFILE_TYPE:
                            {
                                AnvizNew.CCHEX_RET_UPLOADFILE_STRU result;
                                result = (AnvizNew.CCHEX_RET_UPLOADFILE_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_UPLOADFILE_STRU));
                                if (result.Result == 0)
                                {
                                    //MessageBox.Show("UPLoad Ok!");
                                    //log_add_string(result.SendBytes.ToString()+"//"+ result.TotalBytes.ToString());
                                    baifenbi.Text = result.SendBytes.ToString() + "/" + result.TotalBytes.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UPDATEFILE_STATUS_TYPE:
                            {
                                AnvizNew.CCHEX_RET_UPDATEFILE_STATUS result;
                                result = (AnvizNew.CCHEX_RET_UPDATEFILE_STATUS)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_UPDATEFILE_STATUS));
                                if (result.Result == 0)
                                {
                                    log_add_string("Status: " + result.verify_status.ToString() + " RET: " + result.verify_ret.ToString());
                                    MessageBox.Show("UPDATE Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("UPDAATE Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_STATUS_SWITCH_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_STATUS_SWITCH_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU));
                                if (result.Result == 0)
                                {
                                    groupid01.Text = result.group_id.ToString();
                                    status00.Text = result.status_id.ToString();

                                    tg_001.Text = result.day_week[0].StartHour.ToString();
                                    tg_002.Text = result.day_week[0].StartMin.ToString();
                                    tg_003.Text = result.day_week[0].EndHour.ToString();
                                    tg_004.Text = result.day_week[0].EndMin.ToString();

                                    tg_005.Text = result.day_week[1].StartHour.ToString();
                                    tg_006.Text = result.day_week[1].StartMin.ToString();
                                    tg_007.Text = result.day_week[1].EndHour.ToString();
                                    tg_008.Text = result.day_week[1].EndMin.ToString();

                                    tg_009.Text = result.day_week[2].StartHour.ToString();
                                    tg_010.Text = result.day_week[2].StartMin.ToString();
                                    tg_011.Text = result.day_week[2].EndHour.ToString();
                                    tg_012.Text = result.day_week[2].EndMin.ToString();

                                    tg_013.Text = result.day_week[3].StartHour.ToString();
                                    tg_014.Text = result.day_week[3].StartMin.ToString();
                                    tg_015.Text = result.day_week[3].EndHour.ToString();
                                    tg_016.Text = result.day_week[3].EndMin.ToString();

                                    tg_017.Text = result.day_week[4].StartHour.ToString();
                                    tg_018.Text = result.day_week[4].StartMin.ToString();
                                    tg_019.Text = result.day_week[4].EndHour.ToString();
                                    tg_020.Text = result.day_week[4].EndMin.ToString();

                                    tg_021.Text = result.day_week[5].StartHour.ToString();
                                    tg_022.Text = result.day_week[5].StartMin.ToString();
                                    tg_023.Text = result.day_week[5].EndHour.ToString();
                                    tg_024.Text = result.day_week[5].EndMin.ToString();

                                    tg_025.Text = result.day_week[6].StartHour.ToString();
                                    tg_026.Text = result.day_week[6].StartMin.ToString();
                                    tg_027.Text = result.day_week[6].EndHour.ToString();
                                    tg_028.Text = result.day_week[6].EndMin.ToString();

                                    MessageBox.Show("GET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_STATUS_SWITCH_EXT_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_STATUS_SWITCH_EXT_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT result;
                                result = (AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_STATUS_SWITCH_STRU_EXT));
                                if (result.Result == 0)
                                {
                                    flagweek_01.Text = result.flag_week.ToString();

                                    tgj_001.Text = result.one_time[0].StartHour.ToString();
                                    tgj_002.Text = result.one_time[0].StartMin.ToString();
                                    tgj_003.Text = result.one_time[0].EndHour.ToString();
                                    tgj_004.Text = result.one_time[0].EndMin.ToString();
                                    tgj_005.Text = result.one_time[0].status_id.ToString();


                                    tgj_006.Text = result.one_time[1].StartHour.ToString();
                                    tgj_007.Text = result.one_time[1].StartMin.ToString();
                                    tgj_008.Text = result.one_time[1].EndHour.ToString();
                                    tgj_009.Text = result.one_time[1].EndMin.ToString();
                                    tgj_010.Text = result.one_time[1].status_id.ToString();

                                    tgj_011.Text = result.one_time[2].StartHour.ToString();
                                    tgj_012.Text = result.one_time[2].StartMin.ToString();
                                    tgj_013.Text = result.one_time[2].EndHour.ToString();
                                    tgj_014.Text = result.one_time[2].EndMin.ToString();
                                    tgj_015.Text = result.one_time[2].status_id.ToString();

                                    tgj_016.Text = result.one_time[3].StartHour.ToString();
                                    tgj_017.Text = result.one_time[3].StartMin.ToString();
                                    tgj_018.Text = result.one_time[3].EndHour.ToString();
                                    tgj_019.Text = result.one_time[3].EndMin.ToString();
                                    tgj_020.Text = result.one_time[3].status_id.ToString();

                                    tgj_021.Text = result.one_time[4].StartHour.ToString();
                                    tgj_022.Text = result.one_time[4].StartMin.ToString();
                                    tgj_023.Text = result.one_time[4].EndHour.ToString();
                                    tgj_024.Text = result.one_time[4].EndMin.ToString();
                                    tgj_025.Text = result.one_time[4].status_id.ToString();

                                    tgj_026.Text = result.one_time[5].StartHour.ToString();
                                    tgj_027.Text = result.one_time[5].StartMin.ToString();
                                    tgj_028.Text = result.one_time[5].EndHour.ToString();
                                    tgj_029.Text = result.one_time[5].EndMin.ToString();
                                    tgj_030.Text = result.one_time[5].status_id.ToString();

                                    tgj_031.Text = result.one_time[6].StartHour.ToString();
                                    tgj_032.Text = result.one_time[6].StartMin.ToString();
                                    tgj_033.Text = result.one_time[6].EndHour.ToString();
                                    tgj_034.Text = result.one_time[6].EndMin.ToString();
                                    tgj_035.Text = result.one_time[6].status_id.ToString();

                                    tgj_036.Text = result.one_time[7].StartHour.ToString();
                                    tgj_037.Text = result.one_time[7].StartMin.ToString();
                                    tgj_038.Text = result.one_time[7].EndHour.ToString();
                                    tgj_039.Text = result.one_time[7].EndMin.ToString();
                                    tgj_040.Text = result.one_time[7].status_id.ToString();


                                    MessageBox.Show("GET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_SET_BASIC_CFG3_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("SET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("SET Fail!");
                                }
                            }
                            break;

                        case (int)AnvizNew.MsgType.CCHEX_RET_CONNECTION_AUTHENTICATION_TYPE:
                            {
                                AnvizNew.CCHEX_RET_COMMON_STRU result;
                                result = (AnvizNew.CCHEX_RET_COMMON_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_COMMON_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Authentication Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("Authentication  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_BASIC_CFG3_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_BASIC_CFG_INFO3_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_BASIC_CFG_INFO3_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_BASIC_CFG_INFO3_STRU));
                                if (result.Result == 0)
                                {
                                    c3_01.Text = result.param.wiegand_type.ToString();
                                    c3_02.Text = result.param.online_mode.ToString();
                                    c3_03.Text = result.param.collect_level.ToString();
                                    c3_04.Text = result.param.pwd_status.ToString();
                                    c3_05.Text = result.param.sensor_status.ToString();
                                    c3_06.Text = result.param.reserved[7].ToString();
                                    c3_07.Text = result.param.independent_time.ToString();
                                    c3_08.Text = result.param.m5_t5_status.ToString();

                                    MessageBox.Show("GET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_SPECIAL_STATUS_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_SPECIAL_STATUS_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_SPECIAL_STATUS_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_SPECIAL_STATUS_STRU));
                                if (result.Result == 0)
                                {
                                    Special_01.Text = ((result.status >> 1) & 0x01).ToString();
                                    Special_05.Text = ((result.status >> 5) & 0x01).ToString();
                                    Special_06.Text = ((result.status >> 6) & 0x01).ToString();
                                    Special_07.Text = ((result.status >> 7) & 0x01).ToString();

                                    //位1：门报警状态 0-正常状态 1-报警状态,位5：门状态 0-关闭 1-打开,位6：门磁状态 0-关闭 1-打开位,7：锁状态 0-关闭 1-打开

                                    special_relay.Text = result.reserved[0].ToString();//Just For bolid
                                    MessageBox.Show("GET Ok!");
                                }
                                else
                                {
                                    MessageBox.Show("GET Fail!");
                                }
                            }
                            break;


                        case (int)AnvizNew.MsgType.CCHEX_RET_MANAGE_LOG_RECORD_TYPE:
                            {
                                AnvizNew.CCHEX_RET_MANAGE_LOG_RECORD result;
                                result = (AnvizNew.CCHEX_RET_MANAGE_LOG_RECORD)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_MANAGE_LOG_RECORD));
                                if (result.CmdType == 0x00) //get number for log
                                {
                                    if (result.Result == 0)
                                    {
                                        log_number.Text = result.TotalNum.ToString();
                                        MessageBox.Show("GET  Number Ok!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("GET  Number Fail!");
                                    }
                                }
                                else if (result.CmdType == 0x01) //download log
                                {
                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(result.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "LOG Info ----[ Mid:" + result.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + Employee_array_to_srring(result.EmployeeId)
                                        + " LogType:" + ((result.LogType[0] << 8) + result.LogType[1]).ToString()
                                        + " ]" + "(" + result.CurNum.ToString() + "/" + result.TotalNum.ToString() + ")";
                                    log_add_string(info_buff);
                                    //LogType[2];                   //日志类型 0x0001 开门 0x0002 关门 0x0003 门磁警报 0x0004 防拆警报 0x0005 出门按钮 0x0006 破门

                                }
                                else if (result.CmdType == 0x02) //remove log
                                {
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("Remove Log Ok!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Remove Log Fail!");
                                    }
                                }
                                else if (result.CmdType == 0x03)
                                {
                                    if (result.Result == 0)
                                    {
                                        if (result.IsAuto == 0)
                                        {
                                            MessageBox.Show("Close  Ok!");
                                        }
                                        else if (result.IsAuto == 1)
                                        {
                                            MessageBox.Show("Open  Ok!");
                                        }
                                        
                                    }
                                    else
                                    {
                                        if (result.IsAuto == 0)
                                        {
                                            MessageBox.Show("Close  Fail!");
                                        }
                                        else if (result.IsAuto == 1)
                                        {
                                            MessageBox.Show("Open  Fail!");
                                        }
                                    }
                                }
                                else if (result.CmdType == 0x04)
                                {
                                    if (result.Result == 0)
                                    {
                                        log_auto_01.Text = result.IsAuto.ToString();
                                        MessageBox.Show(" Ok!");
                                    }
                                    else
                                    {
                                        MessageBox.Show(" Fail!");
                                    }
                                }
                                else if (result.CmdType == 0x05)
                                {
                                    DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(result.Date, 0)));
                                    string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                                    string info_buff = "# Auto #LOG Info ----[ Mid:" + result.MachineId
                                        + " Date:" + dateStr
                                        + "PersonID: " + Employee_array_to_srring(result.EmployeeId)
                                        + " LogType:" + ((result.LogType[0] << 8) + result.LogType[1]).ToString()
                                        + " ]" + "(" + result.CurNum.ToString() + "/" + result.TotalNum.ToString() + ")";
                                    log_add_string(info_buff);
                                    //LogType[2];                   //日志类型 0x0001 开门 0x0002 关门 0x0003 门磁警报 0x0004 防拆警报 0x0005 出门按钮 0x0006 破门
                                }

                            }
                            break;
                        //case end

                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_COMMON_TYPE:
                            {
                                AnvizNew.SAC_DOWNLOAD_COMMON_RESULT download_temp;
                                download_temp = (AnvizNew.SAC_DOWNLOAD_COMMON_RESULT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_DOWNLOAD_COMMON_RESULT));
                                string info_buff = "LOG Info ----[ Mid:" + download_temp.MachineId.ToString()
                                        + " CmdPrincipa:" + download_temp.CmdPrincipa.ToString()
                                        + " Result:" + download_temp.Result.ToString()
                                        + " SeqNum:" + download_temp.SeqNum.ToString()
                                        + " MaxCount:" + download_temp.MaxCount.ToString()
                                        + " DataCurCount:" + download_temp.DataCurCount.ToString()
                                        + " ] ";
                                log_add_string(info_buff);
                                switch (download_temp.CmdPrincipa)
                                {
                                    case (int)AnvizNew.CMD_PRINCIPAL.CMD_Pri_Door:
                                        {
                                            AnvizNew.SAC_SET_DOOR_INFO_STRU door;
                                            int i = 0;
                                            for(i=0;i< download_temp.DataCurCount; i++)
                                            {
                                                door = (AnvizNew.SAC_SET_DOOR_INFO_STRU)Marshal.PtrToStructure(pBuff + Marshal.SizeOf(download_temp)+i* Marshal.SizeOf(new AnvizNew.SAC_SET_DOOR_INFO_STRU()), typeof(AnvizNew.SAC_SET_DOOR_INFO_STRU));
                                                info_buff ="doorid="+ door.DoorId.ToString() +"  doorname:" + byte_to_unicode_string(door.DoorName)
                                                +"  devname ="+byte_to_unicode_string(door.DevName)  +"  subtype"+ door.Anti_subType.ToString();
                                                log_add_string(info_buff);
                                            }
                                            //door = (AnvizNew.SAC_DOWNLOAD_COMMON_RESULT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_DOWNLOAD_COMMON_RESULT));
                                        }
                                        break;
                                    case (int)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo:
                                        {
                                            AnvizNew.SAC_EMPLOYEE_INFO_STRU data_temp;
                                            int i = 0;
                                            for (i = 0; i < download_temp.DataCurCount; i++)
                                            {
                                                data_temp = (AnvizNew.SAC_EMPLOYEE_INFO_STRU)Marshal.PtrToStructure(pBuff + Marshal.SizeOf(download_temp) + i * Marshal.SizeOf(new AnvizNew.SAC_EMPLOYEE_INFO_STRU()), typeof(AnvizNew.SAC_EMPLOYEE_INFO_STRU));
                                                ListViewItem lvi = new ListViewItem();
                                                int person_list_len = this.listsac_employee.Items.Count;
                                                lvi.ImageIndex = person_list_len;
                                                lvi.Text = (person_list_len + 1).ToString();
                                                //lvi.SubItems.Add(person_list.EmployeeId.ToString());
                                                lvi.SubItems.Add(Employee_array_to_srring(data_temp.EmployeeId));
                                                lvi.SubItems.Add(byte_to_unicode_string(data_temp.EmployeeName));

                                                if (data_temp.Password[0] == 0xFF && data_temp.Password[1] == 0xFF && data_temp.Password[2] == 0xFF)
                                                {
                                                    lvi.SubItems.Add("");
                                                }
                                                else
                                                {
                                                    int passwordlen = data_temp.Password[0] >> 4;
                                                    int password_ = ((data_temp.Password[0] & 0xf) << 16) + (data_temp.Password[1] << 8) + data_temp.Password[2];
                                                    String tempstr = password_.ToString();
                                                    for (int i1 = 0; i1 < passwordlen - tempstr.Length; i1++)
                                                    {
                                                        tempstr = "0" + tempstr;
                                                    }
                                                    lvi.SubItems.Add(tempstr);
                                                }
                                                if (data_temp.Card[0] == 0xFF && data_temp.Card[1] == 0xFF && data_temp.Card[2] == 0xFF && data_temp.Card[3] == 0xFF)
                                                {
                                                    lvi.SubItems.Add("");
                                                }
                                                else
                                                {
                                                    int cardid = (data_temp.Card[0] << 24) + (data_temp.Card[1] << 16) + (data_temp.Card[2] << 8) + data_temp.Card[3];
                                                    lvi.SubItems.Add(cardid.ToString());
                                                }


                                                lvi.SubItems.Add(data_temp.CardType.ToString());
                                                lvi.SubItems.Add(data_temp.GroupId.ToString());
                                                lvi.SubItems.Add(data_temp.AttendanceMode.ToString());
                                                lvi.SubItems.Add(((data_temp.FpStatus[0] << 8) + data_temp.FpStatus[1]).ToString());
                                                lvi.SubItems.Add(data_temp.HolidayGroup.ToString());
                                                lvi.SubItems.Add(data_temp.Special.ToString());
                                                this.listsac_employee.Items.Add(lvi);
                                            }
                                            

                                        }
                                        break;
                                }
                                
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_COMMON_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DELETE_COMMON_TYPE:
                        
                            {
                                AnvizNew.SAC_UPLOAD_COMMON_RESULT data_temp = (AnvizNew.SAC_UPLOAD_COMMON_RESULT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_UPLOAD_COMMON_RESULT));

                                string info_buff = "Upload Delete:: MachineId = "+ data_temp.MachineId.ToString()
                                    +"  CmdPrincipa = "+ data_temp.CmdPrincipa.ToString()
                                    +"  Result = "+ data_temp.Result.ToString()
                                    +"  DataCurCount ="+ data_temp.DataCurCount.ToString()
                                    +"  Byte:";

                                byte[] temp_byte = new byte[data_temp.DataCurCount];
                                Marshal.Copy( pBuff+ Marshal.SizeOf(data_temp), temp_byte, 0,(int)data_temp.DataCurCount);
                                for (int i = 0; i < data_temp.DataCurCount;i++)
                                {
                                    info_buff = info_buff + temp_byte[i].ToString();
                                }
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_INIT_COMMON_TYPE:
                            {
                                AnvizNew.SAC_INIT_COMMON_RESULT data_temp = (AnvizNew.SAC_INIT_COMMON_RESULT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_INIT_COMMON_RESULT));

                                string info_buff = "Upload Delete:: MachineId = " + data_temp.MachineId.ToString()
                                    + "  CmdPrincipa = " + data_temp.CmdPrincipa.ToString()
                                    + "  Result = " + data_temp.Result.ToString();

                                log_add_string(info_buff);
                                
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_PUSH_COMMON_TYPE:
                            {
                                {
                                    AnvizNew.SAC_PUSH_COMMON_RESULT data_temp = (AnvizNew.SAC_PUSH_COMMON_RESULT)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_PUSH_COMMON_RESULT));

                                    string info_buff = "Push:: MachineId = " + data_temp.MachineId.ToString()
                                        + "  CmdPrincipa = " + data_temp.CmdPrincipa.ToString()
                                        + "  Result = " + data_temp.Result.ToString()
                                    + "  DataCurCount = " + data_temp.DataCurCount.ToString();

                                    log_add_string(info_buff);
                                    switch (data_temp.CmdPrincipa)
                                    {
                                        case (int)AnvizNew.CMD_PRINCIPAL.CMD_Pri_AtEvent:
                                            {
                                                AnvizNew.SAC_DATA_Device_Event date = (AnvizNew.SAC_DATA_Device_Event)Marshal.PtrToStructure(pBuff + Marshal.SizeOf(data_temp), typeof(AnvizNew.SAC_DATA_Device_Event));
                                                DateTime datetime = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(date.dwTime, 0)));
                                                string dateStr = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                                                info_buff = date.uEventType.ToString()
                                                    + "  " + Employee_array_to_srring(date.uEmID)
                                                    + "   " + date.uEventType.ToString()
                                                     + "   " + date.uWiegandNo.ToString()
                                                      + "   " + date.uDoorNo.ToString()
                                                      + "   " + dateStr;
                                                log_add_string(info_buff);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            }

                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_DOOR_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_TIME_FRAME_GROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_GROUP_INFO_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_GROUP_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_GROUP_INFO_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " Group NUm:" + groupdata.GroupData.GroupId.ToString()
                                        + "Group Name: " + byte_to_unicode_string(groupdata.GroupData.GroupName)
                                        + " ]" + "(" + groupdata.CurIdx.ToString() + "/" + groupdata.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_DOOR_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_DOOR_WITH_DOORGROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_TIME_FRAME_WITH_TIME_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_TIME_FRAME_GROUP_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_ACCESS_CONTROL_GROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_UPLOAD_GROUP_INFO_STRU result;
                                result = (AnvizNew.SAC_RET_UPLOAD_GROUP_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_UPLOAD_GROUP_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("Upload group  Ok! GroupFlag = " + result.GroupFlag.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("Upload group  Fail!");
                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_GET_DOOR_INFO_TYPE:
                            {
                                AnvizNew.SAC_RET_GET_DOOR_INFO_STRU result;
                                result = (AnvizNew.SAC_RET_GET_DOOR_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_GET_DOOR_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    sac_1001.Text = result.Data.DoorId.ToString();
                                    sac_1002.Text = byte_to_unicode_string(result.Data.DoorName);
                                    sac_1003.Text = byte_to_unicode_string(result.Data.DevName);
                                    sac_1004.Text = result.Data.Anti_subType.ToString();
                                    sac_1005.Text = result.Data.InterlockFlag.ToString();
                                    sac_1006.Text = result.Data.InterlockDoorId.ToString();
                                    sac_1007.Text = result.Data.DoorStatus.ToString();
                                    MessageBox.Show("get door  Ok! GroupFlag = ");
                                }
                                else
                                {
                                    MessageBox.Show("get door  Fail!");
                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_SET_DOOR_INFO_TYPE:
                            {
                                AnvizNew.SAC_RET_SET_DOOR_INFO_STRU result;
                                result = (AnvizNew.SAC_RET_SET_DOOR_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_SET_DOOR_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("set door  Ok!   DoorId = "+ result.DoorId.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("set door  Fail! DoorId = " + result.DoorId.ToString());
                                }


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_ACCESS_CONTROL_GROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_CONTROL_GROUP_INFO_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_CONTROL_GROUP_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_CONTROL_GROUP_INFO_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " SAC_GroupId:" + groupdata.Data.SAC_GroupId.ToString()
                                        + " GroupName:" + byte_to_unicode_string(groupdata.Data.GroupName)
                                        + " EmployeeGroupId:" + groupdata.Data.EmployeeGroupId.ToString()
                                        + " DoorGroupId:" + groupdata.Data.DoorGroupId.ToString()
                                        + " TimeGroupId:" + groupdata.Data.TimeGroupId.ToString()
                                        + " ]" + "(" + groupdata.CurIdx.ToString() + "/" + groupdata.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_DOOR_WITH_DOORGROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_DOOR_WITH_DOORGROUP_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_DOOR_WITH_DOORGROUP_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_DOOR_WITH_DOORGROUP_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " CombinationId:" + groupdata.Data.CombinationId.ToString()
                                        + " DoorId:" + groupdata.Data.DoorId.ToString()
                                        + " DoorGroupId:" + groupdata.Data.DoorGroupId.ToString()
                                        + " ]" + "(" + groupdata.CurIdx.ToString() + "/" + groupdata.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_TIME_FRAME_WITH_TIME_GROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_TimeFrame_WITH_TimeGROUP_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_TimeFrame_WITH_TimeGROUP_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_TimeFrame_WITH_TimeGROUP_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " CombinationId:" + groupdata.Data.CombinationId.ToString()
                                        + " TimeFrameId:" + groupdata.Data.TimeFrameId.ToString()
                                        + " TimeFrameGroupId:" + groupdata.Data.TimeFrameGroupId.ToString()
                                        + " ]" + "(" + groupdata.CurIdx.ToString() + "/" + groupdata.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_TIME_FRAME_INFO_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_TIME_FRAME_INFO_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_TIME_FRAME_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_TIME_FRAME_INFO_STRU));
                                if (groupdata.Result == 0)
                                {
                                    int i = 0;
                                    sac_time00.Text = groupdata.TimeFrameNum.ToString();
                                    string strtemp ="";
                                    for (i = 0; i < 10; i++)
                                    {
                                        if (i == 0)
                                        {
                                            strtemp += groupdata.Data.date[i].StartHour.ToString();

                                        }
                                        else
                                        {
                                            strtemp += "-"+groupdata.Data.date[i].StartHour.ToString();
                                        }
                                        strtemp += "-" + groupdata.Data.date[i].StartMin.ToString();
                                        strtemp += "-" + groupdata.Data.date[i].EndHour.ToString();
                                        strtemp += "-" + groupdata.Data.date[i].EndMin.ToString();
                                    }
                                    sac_time01.Text = strtemp;
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_TIME_FRAME_INFO_TYPE:
                            {
                                AnvizNew.SAC_RET_UPLOAD_TIME_FRAME_INFO_STRU result;
                                result = (AnvizNew.SAC_RET_UPLOAD_TIME_FRAME_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_UPLOAD_TIME_FRAME_INFO_STRU));
                                if (result.Result == 0)
                                {
                                    MessageBox.Show("set door  Ok!   DoorId = " + result.TimeFrameNum.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("set door  Fail! DoorId = " + result.TimeFrameNum.ToString());
                                }


                            }
                            break;


                        case (int)AnvizNew.MsgType.CCHEX_SAC_DOWNLOAD_EMPLOYEE_WITH_GROUP_TYPE:
                            {
                                AnvizNew.SAC_RET_DOWNLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU groupdata;
                                groupdata = (AnvizNew.SAC_RET_DOWNLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_RET_DOWNLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " Group NUm:" + groupdata.Data.GroupId.ToString()
                                        + "Group PersonId: " + Employee_array_to_srring(groupdata.Data.EmployeeId)
                                        + " ]" + "(" + groupdata.CurIdx.ToString() + "/" + groupdata.TotalCnt.ToString() + ")";
                                log_add_string(info_buff);


                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_SAC_UPLOAD_EMPLOYEE_WITH_GROUP_TYPE:
                            {
                                AnvizNew.SAC_UPLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU result;
                                result = (AnvizNew.SAC_UPLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.SAC_UPLOAD_EMPLOYEE_WITH_GROUP_INFO_STRU));
                                if (result.Result == 0)
                                {

                                    MessageBox.Show("Upload employee with group  Ok! GroupFlag = " );
                                }
                                else
                                {
                                    MessageBox.Show("Upload employee with group  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_PICTURE_GET_TOTAL_NUMBER_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_PICTURE_NUMBER_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_PICTURE_NUMBER_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_NUMBER_STRU));
                                if (result.Result == 0)
                                {
                                    pic_num.Text = result.PictureTotal.ToString();
                                    MessageBox.Show("Picture get number  OK! ");
                                }
                                else
                                {
                                    MessageBox.Show("Picture get number  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_PICTURE_GET_ALL_HEAD_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(result.DateTime, 0)));
                                        string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                                        string info_buff = "Get PicTure Head ----[ Mid:" + result.MachineId
                                        + " PersonId: " + byte_to_unicode_string(result.EmployeeId)
                                        + " ]" +"DateTime:"+ dateStr + "(" + result.CurIdx.ToString() + "/" + result.TotalCnt.ToString() + ")";
                                        log_add_string(info_buff);

                                        //MessageBox.Show("Upload employee with group  Ok! GroupFlag = ");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get head  Fail!");
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU result;
                                    result = (AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_HEAD_INFO_STRU));
                                    if (result.Result == 0)
                                    {
                                        DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(result.DateTime, 0)));
                                        string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                                        string info_buff = "Get PicTure Head ----[ Mid:" + result.MachineId
                                        + " PersonId: " + Employee_array_to_srring(result.EmployeeId)
                                        + " ]" + "DateTime:" + dateStr + "(" + result.CurIdx.ToString() + "/" + result.TotalCnt.ToString() + ")";
                                        log_add_string(info_buff);

                                        //MessageBox.Show("Upload employee with group  Ok! GroupFlag = ");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get head  Fail!");
                                    }
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_PICTURE_GET_DATA_BY_EID_TIME_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME_VER_4_NEWID));
                                    if (result.Result == 0 && result.DataLen != 0)
                                    {
                                        string info_buff = "Get PicTure Head ----[ Mid:" + result.MachineId
                                        + " PersonId: " + byte_to_unicode_string(result.EmployeeId)
                                        + " ]" +"  Len =" + result.DataLen.ToString() + "   Len = " + ret.ToString();
                                        log_add_string(info_buff);
                                        byte[] buffer = new byte[result.DataLen];
                                        Marshal.Copy(pBuff + Marshal.SizeOf(result), buffer, 0, (int)result.DataLen);
                                        MemoryStream ms = new MemoryStream(buffer);
                                        System.Drawing.Image result1 = System.Drawing.Image.FromStream(ms);
                                        pictureBox1.Image = result1;
                                        
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get  Fail!");
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME result;
                                    result = (AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_BY_EID_AND_TIME));
                                    if (result.Result == 0 && result.DataLen !=0)
                                    {
                                        string info_buff = "Get PicTure Head ----[ Mid:" + result.MachineId
                                        + " PersonId: " + Employee_array_to_srring(result.EmployeeId)
                                        + " ]" + "  Len =" + result.DataLen.ToString() + "   Len = " + ret.ToString();
                                        log_add_string(info_buff);
                                        byte[] buffer = new byte[result.DataLen];
                                        Marshal.Copy(pBuff+ Marshal.SizeOf(result), buffer, 0, (int)result.DataLen);
                                        MemoryStream ms = new MemoryStream(buffer);
                                        System.Drawing.Image result1 = System.Drawing.Image.FromStream(ms);
                                        pictureBox1.Image = result1;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Get  Fail!");
                                    }
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_PICTURE_DEL_DATA_BY_EID_TIME_TYPE:
                            {
                                if ((DevTypeFlag[dev_idx[0]] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                                {
                                    AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID result;
                                    result = (AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME_VER_4_NEWID));
                                    if (result.Result == 0)
                                    {
                                        MessageBox.Show("Del   Ok!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Del  Fail!");
                                    }
                                }
                                else
                                {
                                    AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME result;
                                    result = (AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_PICTURE_BY_EID_AND_TIME));
                                    if (result.Result == 0)
                                    {

                                        MessageBox.Show("Del  OK!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Del  Fail!");
                                    }
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_T_RECORD_NUMBER_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_T_RECORD_NUMBER_STRU groupdata;
                                groupdata = (AnvizNew.CCHEX_RET_GET_T_RECORD_NUMBER_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_T_RECORD_NUMBER_STRU));
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " Result:" + groupdata.Result
                                        + " Num: " + groupdata.RecordNum
                                        + " Type " + groupdata.RecordTpye.ToString();
                                log_add_string(info_buff);
                                MessageBox.Show(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_T_RECORD_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_T_RECORD_STRU groupdata;
                                groupdata = (AnvizNew.CCHEX_RET_GET_T_RECORD_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_T_RECORD_STRU));
                                DateTime date = new DateTime(2000, 1, 2).AddSeconds(swapInt32(BitConverter.ToUInt32(groupdata.Date, 0)));
                                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " Result:" + groupdata.Result
                                        +" ID" + byte_to_uint64(groupdata.RecoradId)
                                        +" Date:"+ dateStr
                                        +" Type" + groupdata.TemperatureType
                                        + " Temp.." + ((groupdata.Temperature[0]<<8)+ groupdata.Temperature[1])
                                        + " Num: " + groupdata.CurIdx
                                        + " Type " + groupdata.TotalCnt.ToString();
                                log_add_string(info_buff);
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_GET_T_PICTURE_BY_RECORD_ID_TYPE:
                            {
                                AnvizNew.CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU result;
                                result = (AnvizNew.CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_GET_PICTURE_BY_RECORD_ID_STRU));
                                if (result.Result == 0 && result.DataLen != 0)
                                {
                                    string path = "NULL";
                                    string info_buff = "LOG Info ----[ Mid:" + result.MachineId
                                        + " Result:" + result.Result
                                        + " ID" + byte_to_uint64(result.RecoradId);
                                    log_add_string(info_buff);
                                    byte[] buffer = new byte[result.DataLen];
                                    Marshal.Copy(pBuff + Marshal.SizeOf(result), buffer, 0, (int)result.DataLen);
                                    MemoryStream ms = new MemoryStream(buffer);
                                    System.Drawing.Image result1 = System.Drawing.Image.FromStream(ms);
                                    
                                    pictureBox2.Image = result1;
                                    {                           //保存人脸图片
                                        byte[] save_buffer = new byte[result.DataLen];
                                        Marshal.Copy(pBuff + Marshal.SizeOf(result), save_buffer, 0, (int)result.DataLen);
                                        //MemoryStream ms = new MemoryStream(buffer);
                                        //System.Drawing.Image result1 = System.Drawing.Image.FromStream(ms);

                                        //pictureBox2.Image = result1;   
                                        //之前获取的是图片文件，现在获取的是模板
                                        path = Environment.CurrentDirectory + "/" + byte_to_uint64(result.RecoradId)+".jpg";
                                        FileStream fs = new FileStream(path, FileMode.Create);
                                        fs.Write(save_buffer, 0, (int)result.DataLen);
                                        fs.Dispose();
                                    }
                                    MessageBox.Show("Get Face picture Success!" + path);
                                }
                                else
                                {
                                    MessageBox.Show("Get  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_UPLOAD_FACE_PICTURE_MODULE_TYPE:
                            {
                                AnvizNew.CCHEX_UPLOAD_FACE_PICTURE_MODULE result;
                                result = (AnvizNew.CCHEX_UPLOAD_FACE_PICTURE_MODULE)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_UPLOAD_FACE_PICTURE_MODULE));
                                if (result.Result == 0)
                                {
                                    string info_buff = "LOG Info ----[ Mid:" + result.MachineId
                                        + " Result:" + result.Result
                                        + " PersonId: " + Employee_array_to_srring(result.EmployeeId);
                                    log_add_string(info_buff);
                                    MessageBox.Show(info_buff);
                                }
                                else
                                {
                                    MessageBox.Show("Upload  Fail!");
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DOWNLOAD_FACE_PICTURE_MODULE_TYPE:
                        case (int)AnvizNew.MsgType.CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE:
                            {
                                AnvizNew.CCHEX_DOWNLOAD_FACE_PICTURE_MODULE result;
                                result = (AnvizNew.CCHEX_DOWNLOAD_FACE_PICTURE_MODULE)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_DOWNLOAD_FACE_PICTURE_MODULE));
                                if (result.Result == 0 && result.DataLen != 0)
                                {
                                    string info_buff = "LOG Info ----[ Mid:" + result.MachineId
                                        + " Result:" + result.Result
                                        + " PersonId: " + Employee_array_to_srring(result.EmployeeId) + result.DataLen;
                                    log_add_string(info_buff);

                                    if (true)
                                    {
                                        Marshal.Copy(pBuff + Marshal.SizeOf(result), buff_fP_picture, 0, (int)result.DataLen);
                                        len_fP_picture = (int)result.DataLen;
                                        if ((int)AnvizNew.MsgType.CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE == Type[0])
                                        {
                                            MessageBox.Show("add FP online  Success!");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Download  Success!");
                                        }
                                    }
                                    else  //保存为文件
                                    {
                                        byte[] buffer = new byte[result.DataLen];
                                        Marshal.Copy(pBuff + Marshal.SizeOf(result), buffer, 0, (int)result.DataLen);
                                        //MemoryStream ms = new MemoryStream(buffer);
                                        //System.Drawing.Image result1 = System.Drawing.Image.FromStream(ms);

                                        //pictureBox2.Image = result1;   
                                        //之前获取的是图片文件，现在获取的是模板
                                        string path = Environment.CurrentDirectory + "/" + Employee_array_to_srring(result.EmployeeId);
                                        FileStream fs = new FileStream(path, FileMode.Create);
                                        fs.Write(buffer, 0, (int)result.DataLen);
                                        fs.Dispose();
                                    }


                                }
                                else
                                {
                                    if ((int)AnvizNew.MsgType.CCHEX_RET_ADD_PICTURE_FINGERPRINT_ONLINE_TYPE == Type[0])
                                    {
                                        MessageBox.Show("add FP online  Fail!");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Download  Fail!");
                                    }
                                }
                            }
                            break;
                        case (int)AnvizNew.MsgType.CCHEX_RET_DEL_T_PICTURE_BY_RECORD_ID_TYPE:
                            {
                                AnvizNew.CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU groupdata;
                                groupdata = (AnvizNew.CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU)Marshal.PtrToStructure(pBuff, typeof(AnvizNew.CCHEX_RET_DEL_PICTURE_BY_RECORD_ID_STRU));

                                string info_buff = "LOG Info ----[ Mid:" + groupdata.MachineId
                                        + " Result:" + groupdata.Result
                                        + "ID" + byte_to_uint64(groupdata.RecoradId)
                                        + "Type" + groupdata.TemperatureType;
                                log_add_string(info_buff);
                            }
                            break;

                    }
                }
                else if (0 == ret)
                {
                    //MY_SLEEP(10);
                    //Marshal.FreeHGlobal(pBuff); // free the memory  
                    break;
                }
                else if (0 > ret)
                {
                    //buff to small
                    len = len * 2;
                    Marshal.FreeHGlobal(pBuff);
                    pBuff = Marshal.AllocHGlobal(len);
                }
                else
                {
                    //没有找到消息类型
                    //Marshal.FreeHGlobal(pBuff); // free the memory  
                    break;
                }
                //Marshal.FreeHGlobal(pBuff); // free the memory  
            }
            //Marshal.FreeHGlobal(buff_fin);
            Marshal.FreeHGlobal(pBuff); // free the memory  
        }

        private void textBoxMode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '3'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }

        private void listViewDevice_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxIp.Clear();
            textBoxMask.Clear();
            textBoxMac.Clear();
            textBoxPort.Clear();
            textBoxGw.Clear();
            textBoxServIp.Clear();
            textBoxMode.Clear();
            textBoxRemote.Clear();
        }

        private void listBoxLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush mybsh = Brushes.Black;
                // 判断是什么类型的标签  
                if (listBoxLog.Items[e.Index].ToString().IndexOf("Dev Login") != -1)
                {
                    mybsh = Brushes.Green;
                }
                else if (listBoxLog.Items[e.Index].ToString().IndexOf("Dev Logout") != -1)
                {
                    mybsh = Brushes.Red;
                }
                // 焦点框  
                e.DrawFocusRectangle();
                //文本
                e.Graphics.DrawString(listBoxLog.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds,StringFormat.GenericDefault);
                //解决重绘后水平进度条问题
                this.listBoxLog.HorizontalExtent = Math.Max(this.listBoxLog.HorizontalExtent, (int)this.listBoxLog.CreateGraphics().MeasureString(listBoxLog.Items[e.Index].ToString().Replace('\n', '\r'), this.listBoxLog.Font).Width + 10);
            }

        }

        

        private void button_get_sn_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetSNConfig(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_dl_all_record_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_DownloadAllRecords(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_restart_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                this.timer1.Enabled = false;
                IntPtr anviz_handle_tmp = anviz_handle;
                anviz_handle = IntPtr.Zero;
                AnvizNew.CChex_Stop(anviz_handle_tmp);

                listBoxLog.Items.Clear();
                listViewDevice.Items.Clear();

                anviz_handle = AnvizNew.CChex_Start();

                

                DevCount = 0;
                

                if (anviz_handle != IntPtr.Zero)
                {
                    
                    this.timer1.Enabled = true;
                    log_add_string(AnvizNew.CChex_Get_Service_Port(anviz_handle).ToString());
                }
                else
                {
                    MessageBox.Show("Startup errors,Please restart the program.");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_get_basic_cfg_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetBasicConfigInfo(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_set_basic_cfg_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_BASIC_CFG_INFO_STRU set_basic_cfg = new AnvizNew.CCHEX_SET_BASIC_CFG_INFO_STRU();
                // init to OxFF
                set_basic_cfg.delay_for_sleep = 0xFF;
                set_basic_cfg.volume = 0xFF;
                set_basic_cfg.language = 0xFF;
                set_basic_cfg.date_format = 0xFF;
                set_basic_cfg.time_format = 0xFF;
                set_basic_cfg.machine_status = 0xFF;
                set_basic_cfg.modify_language = 0x01;
                set_basic_cfg.reserved = 0x05; 
                set_basic_cfg.password = 0xFFFFFFFF;
                if (textBox_pwd.Text.Trim() != string.Empty && textBox4_sleep_time.Text.Trim() != string.Empty)
                {
                    if (IsNumeric(textBox_pwd.Text.ToString()) && IsNumeric(textBox4_sleep_time.Text.ToString()))
                    {
                        byte pwd_len = (byte)textBox_pwd.Text.Length;
                        uint pwd = Convert.ToUInt32(textBox_pwd.Text.ToString());
                        uint sleep_time = Convert.ToUInt32(textBox4_sleep_time.Text.ToString());
                        /*set_basic_cfg.password[0] = (byte)(pwd_len << 4);
                        set_basic_cfg.password[0] += (byte)((pwd & 0x0F0000) >> 16);
                        set_basic_cfg.password[1] = (byte)((pwd & 0xFF00) >> 8);
                        set_basic_cfg.password[2] = (byte)(pwd & 0xFF);*/
                        set_basic_cfg.password = pwd;
                        set_basic_cfg.pwd_len = pwd_len;
                        set_basic_cfg.volume = (byte)this.comboBox1.SelectedIndex;
                        /*set_basic_cfg.date_time_format = (byte)(this.comboBox2.SelectedIndex << 4);
                        set_basic_cfg.date_time_format += (byte)(this.comboBox3.SelectedIndex);*/
                        set_basic_cfg.date_format = (byte)this.comboBox2.SelectedIndex;
                        set_basic_cfg.time_format = (byte)this.comboBox3.SelectedIndex;
                        if (sleep_time <= 250)
                        {
                            set_basic_cfg.delay_for_sleep = (byte)sleep_time; 
                            ret = AnvizNew.CChex_SetBasicConfigInfo(anviz_handle, dev_idx, ref set_basic_cfg);

                        }
                        else
                        {
                            MessageBox.Show("Sleep time <= 250", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter Numeric char at password or sleep time", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Empty,Please enter admin password or sleep time", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_get_all_person_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_ListPersonInfo(anviz_handle, dev_idx);
                listView_person.Items.Clear();

                // enable button and textbox
                //button_del_all_record.Enabled = true;
                //button_del_flag_count.Enabled = true;
                //textBox_del_flag_count.Enabled = true; 
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_modify_person_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0x7f000000) == (int)AnvizNew.CustomType.DEV_TYPE_FLAG_CARDNO_BYTE_7)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    AnvizNew.CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU item = new AnvizNew.CCHEX_EMPLOYEE2UNICODE_INFO_CARDID_7_STRU();
                    item.EmployeeId = new byte[5];
                    string_to_byte(this.textBox_personID.Text, item.EmployeeId, 5);
                    byte[] name = Encoding.Unicode.GetBytes(textBox_name.Text);
                    //item.EmployeeId = Convert.ToUInt64(textBox_personID.Text);

                    item.EmployeeName = new byte[20];// unicode
                    int i = 0;                             
                    for (i = 0; i < 20; i += 2)
                    {
                        if (i < name.Length)
                        {
                            item.EmployeeName[i] = name[i + 1];
                            item.EmployeeName[i + 1] = name[i];
                            continue;
                        }
                        item.EmployeeName[i] = 0;
                    }
                    int password;
                    int password_len = 0;
                    item.Passwd = new byte[3];
                    if (textBox_password.Text.Trim().CompareTo("") == 0)
                    {
                        item.Passwd[0] = 0xff;
                        item.Passwd[1] = 0xff;
                        item.Passwd[2] = 0xff;
                    }
                    else
                    {
                        password_len = (byte)textBox_password.Text.Length;
                        password = Convert.ToInt32(textBox_password.Text);
                        password = (password & 0xfffff) + ((password_len & 0xf) << 20);

                        uint64_to_byte((UInt64)password, item.Passwd, 3);
                    }
                    item.CardId = new byte[7];
                    UInt64  card_id = Convert.ToUInt64(textBox_cardID.Text);
                    if (card_id == 0)
                    {
                        card_id = 0xffffffffffffff;
                    }
                    uint64_to_byte((UInt64)card_id, item.CardId, 7);

                    item.DepartmentId = Convert.ToByte(textBox_dept.Text);
                    item.GroupId = Convert.ToByte(textBox_group.Text);
                    item.Mode = Convert.ToByte(textBox_mode.Text);
                    item.FpStatus = new byte[2];
                    int temp_fp_num= Convert.ToInt32(textBox_fp_status.Text);
                    uint64_to_byte((UInt64)temp_fp_num, item.FpStatus, 2);

                    item.Special = Convert.ToByte(textBox_special.Text);
                    item.Rserved1 = 0xFF;  // do not modify
                    item.Rserved2 = Convert.ToByte(textBox_reserved2.Text);
                    AnvizNew.CChex_UploadEmployee2UnicodeInfo_CardIdLen7(anviz_handle, dev_idx, ref item, 1);
                }
                else
                {
                    int ret = 0;
                    if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                    {
                        int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                        //this  ANVIZ_CUSTOM_FOR_ComsecITech  W2
                        AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4 item = new AnvizNew.CCHEX_RET_DLEMPLOYEE_INFO_STRU_EXT_INF_FOR_VER_4();

                        int i = 0;
                        //byte[] name = System.Text.Encoding.Unicode.GetBytes(textBox_name.Text);
                        byte[] name = Encoding.Unicode.GetBytes(textBox_name.Text);
                        //item.EmployeeId = Convert.ToUInt64(textBox_personID.Text);

                        item.EmployeeName = new byte[64];// unicode
                                                         // TODO: check textBox_name do not exceed max_EmployeeName digitals number
                        for (i = 0; i < 64; i += 2)
                        {
                            if (i < name.Length)
                            {
                                item.EmployeeName[i] = name[i + 1];
                                item.EmployeeName[i + 1] = name[i];
                                continue;
                            }
                            item.EmployeeName[i] = 0;
                        }

                        item.EmployeeId = string_to_my_unicodebyte(28, textBox_personID.Text);
                        //byte[] id_byte = Encoding.ASCII.GetBytes(string_id.Text);
                        //Array.Copy(id_byte, item.employeeId_string, id_byte.Length > 28 ? 28 : id_byte.Length);


                        // TODO: check textBox_password do not exceed max_password digitals number
                        if (textBox_password.Text.Trim().CompareTo("") == 0)
                        {
                            item.password = 0xFFFFF;
                        }
                        else
                        {
                            item.password_len = (byte)textBox_password.Text.Length;
                            item.password = Convert.ToInt32(textBox_password.Text);
                        }
                        // TODO: check textBox_cardID do not exceed max_card_id digitals number
                        if (textBox_cardID.Text.CompareTo("-1") == 0)
                        {
                            item.card_id = 0xFFFFFFFF;// no card
                        }
                        else
                        {
                            item.card_id = Convert.ToUInt32(textBox_cardID.Text);
                        }
                        item.DepartmentId = Convert.ToByte(textBox_dept.Text);
                        item.GroupId = Convert.ToByte(textBox_group.Text);
                        item.Mode = Convert.ToByte(textBox_mode.Text);
                        item.Fp_Status = Convert.ToUInt32(textBox_fp_status.Text);
                        item.Special = Convert.ToByte(textBox_special.Text);
                        item.Rserved1 = 0xFF;  // do not modify
                        item.Rserved2 = Convert.ToByte(textBox_reserved2.Text);

                        DateTime date1 = Convert.ToDateTime(text_time1.Text.ToString());
                        DateTime date2 = Convert.ToDateTime(text_time2.Text.ToString());
                        DateTime date3 = new DateTime(2000, 1, 2);
                        int sec1 = (int)(date1 - date3).TotalSeconds;
                        int sec2 = (int)(date2 - date3).TotalSeconds;
                        item.start_date = new byte[4];
                        item.end_date = new byte[4];
                        item.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                        item.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                        item.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                        item.start_date[3] = (byte)((sec1 >> 0) & 0xff);
                        item.end_date[0] = (byte)((sec2 >> 24) & 0xff);
                        item.end_date[1] = (byte)((sec2 >> 16) & 0xff);
                        item.end_date[2] = (byte)((sec2 >> 8) & 0xff);
                        item.end_date[3] = (byte)((sec2 >> 0) & 0xff);


                        ret = AnvizNew.CChex_ModifyPersonInfo_VER_4_NEWID(anviz_handle, dev_idx, ref item, 1);
                    }
                    else if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_EMPLOYEE_FOR_W2_ADD_TIME)
                    {
                        int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                        //this  ANVIZ_CUSTOM_FOR_ComsecITech  W2
                        AnvizNew.CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2 item = new AnvizNew.CCHEX_EMPLOYEE_INFO_STRU_EXT_INF_FOR_W2();

                        int i = 0;
                        //byte[] name = System.Text.Encoding.Unicode.GetBytes(textBox_name.Text);
                        byte[] name = Encoding.Unicode.GetBytes(textBox_name.Text);
                        //item.EmployeeId = Convert.ToUInt64(textBox_personID.Text);
                        item.EmployeeId = new byte[5];
                        // TODO: check textBox_personID do not exceed 12 digitals number
                        string_to_byte(this.textBox_personID.Text, item.EmployeeId, 5);
                        item.EmployeeName = new byte[64];// unicode
                                                         // TODO: check textBox_name do not exceed max_EmployeeName digitals number
                        for (i = 0; i < 64; i += 2)
                        {
                            if (i < name.Length)
                            {
                                item.EmployeeName[i] = name[i + 1];
                                item.EmployeeName[i + 1] = name[i];
                                continue;
                            }
                            item.EmployeeName[i] = 0;
                        }
                        // TODO: check textBox_password do not exceed max_password digitals number
                        if (textBox_password.Text.Trim().CompareTo("") == 0)
                        {
                            item.password = 0xFFFFF;
                        }
                        else
                        {
                            item.password_len = (byte)textBox_password.Text.Length;
                            item.password = Convert.ToInt32(textBox_password.Text);
                        }
                        // TODO: check textBox_cardID do not exceed max_card_id digitals number
                        if (textBox_cardID.Text.CompareTo("-1") == 0)
                        {
                            item.card_id = 0xFFFFFFFF;// no card
                        }
                        else
                        {
                            item.card_id = Convert.ToUInt32(textBox_cardID.Text);
                        }
                        item.DepartmentId = Convert.ToByte(textBox_dept.Text);
                        item.GroupId = Convert.ToByte(textBox_group.Text);
                        item.Mode = Convert.ToByte(textBox_mode.Text);
                        item.Fp_Status = Convert.ToUInt32(textBox_fp_status.Text);
                        item.Special = Convert.ToByte(textBox_special.Text);
                        item.Rserved1 = 0xFF;  // do not modify
                        item.Rserved2 = Convert.ToByte(textBox_reserved2.Text);

                        DateTime date1 = Convert.ToDateTime(text_time1.Text.ToString());
                        DateTime date2 = Convert.ToDateTime(text_time2.Text.ToString());
                        DateTime date3 = new DateTime(2000, 1, 2);
                        uint sec1 = (uint)(date1 - date3).TotalSeconds;
                        uint sec2 = (uint)(date2 - date3).TotalSeconds;
                        log_add_string(sec1.ToString());
                        log_add_string(sec2.ToString());
                        item.start_date = new byte[4];
                        item.end_date = new byte[4];
                        item.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                        item.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                        item.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                        item.start_date[3] = (byte)((sec1 >> 0) & 0xff);
                        item.end_date[0] = (byte)((sec2 >> 24) & 0xff);
                        item.end_date[1] = (byte)((sec2 >> 16) & 0xff);
                        item.end_date[2] = (byte)((sec2 >> 8) & 0xff);
                        item.end_date[3] = (byte)((sec2 >> 0) & 0xff);

                        ret = AnvizNew.CChex_ModifyPersonW2Info(anviz_handle, dev_idx, ref item, 1);
                    }
                    else if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.EmployeeType.DEV_TYPE_FLAG_MSG_ASCII_32)
                    {

                        int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                        AnvizNew.CCHEX_RET_PERSON_INFO_STRU item = new AnvizNew.CCHEX_RET_PERSON_INFO_STRU();
                        //byte[] name = System.Text.Encoding.Unicode.GetBytes(textBox_name.Text);
                        byte[] name = Encoding.Default.GetBytes(textBox_name.Text);
                        //item.EmployeeId = Convert.ToUInt64(textBox_personID.Text);
                        item.EmployeeId = new byte[5];
                        // TODO: check textBox_personID do not exceed 12 digitals number
                        string_to_byte(this.textBox_personID.Text, item.EmployeeId, 5);
                        item.EmployeeName = new byte[64];// unicode
                                                         // TODO: check textBox_name do not exceed max_EmployeeName digitals number

                        Array.Copy(name, item.EmployeeName, name.Length);
                        // TODO: check textBox_password do not exceed max_password digitals number
                        if (textBox_password.Text.Trim().CompareTo("") == 0)
                        {
                            item.password = 0xFFFFF;
                        }
                        else
                        {
                            item.password_len = (byte)textBox_password.Text.Length;
                            item.password = Convert.ToInt32(textBox_password.Text);
                        }
                        // TODO: check textBox_cardID do not exceed max_card_id digitals number
                        if (textBox_cardID.Text.CompareTo("-1") == 0)
                        {
                            item.card_id = 0xFFFFFFFF;// no card
                        }
                        else
                        {
                            item.card_id = Convert.ToUInt32(textBox_cardID.Text);
                        }
                        item.DepartmentId = Convert.ToByte(textBox_dept.Text);
                        item.GroupId = Convert.ToByte(textBox_group.Text);
                        item.Mode = Convert.ToByte(textBox_mode.Text);
                        item.Fp_Status = Convert.ToUInt32(textBox_fp_status.Text);
                        item.Special = Convert.ToByte(textBox_special.Text);
                        item.Rserved1 = 0x00;  // do not modify
                        item.Rserved2 = Convert.ToByte(textBox_reserved2.Text);


                        ret = AnvizNew.CChex_ModifyPersonInfo(anviz_handle, dev_idx, ref item, 1);
                    }
                    else
                    {

                        int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                        {
                            AnvizNew.CCHEX_RET_PERSON_INFO_STRU item = new AnvizNew.CCHEX_RET_PERSON_INFO_STRU();
                            int i = 0;
                            //byte[] name = System.Text.Encoding.Unicode.GetBytes(textBox_name.Text);
                            byte[] name = Encoding.Unicode.GetBytes(textBox_name.Text);
                            //item.EmployeeId = Convert.ToUInt64(textBox_personID.Text);
                            item.EmployeeId = new byte[5];
                            // TODO: check textBox_personID do not exceed 12 digitals number
                            string_to_byte(this.textBox_personID.Text, item.EmployeeId, 5);
                            item.EmployeeName = new byte[64];// unicode
                                                             // TODO: check textBox_name do not exceed max_EmployeeName digitals number

                            for (i = 0; i < 64; i += 2)
                            {
                                if (i < name.Length)
                                {
                                    item.EmployeeName[i] = name[i + 1];
                                    item.EmployeeName[i + 1] = name[i];
                                    continue;
                                }
                                item.EmployeeName[i] = 0;
                            }
                            // TODO: check textBox_password do not exceed max_password digitals number
                            if (textBox_password.Text.Trim().CompareTo("") == 0)
                            {
                                item.password = 0xFFFFF;
                            }
                            else
                            {
                                item.password_len = (byte)textBox_password.Text.Length;
                                item.password = Convert.ToInt32(textBox_password.Text);
                            }
                            // TODO: check textBox_cardID do not exceed max_card_id digitals number
                            if (textBox_cardID.Text.CompareTo("-1") == 0)
                            {
                                item.card_id = 0xFFFFFFFF;// no card
                            }
                            else
                            {
                                item.card_id = Convert.ToUInt32(textBox_cardID.Text);
                            }
                            item.DepartmentId = Convert.ToByte(textBox_dept.Text);
                            item.GroupId = Convert.ToByte(textBox_group.Text);
                            item.Mode = Convert.ToByte(textBox_mode.Text);
                            item.Fp_Status = Convert.ToUInt32(textBox_fp_status.Text);
                            item.Special = Convert.ToByte(textBox_special.Text);
                            item.Rserved1 = 0x00;  // do not modify
                            item.Rserved2 = Convert.ToByte(textBox_reserved2.Text);

                            // add modify DR info
                            if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_EMPLOYEE_FOR_DR_ADD_NAME2)
                            {
                                // add code to full EmployeeName2,RFC,CURP
                                // string to byte
                                // TODO:
                                item.EmployeeName2 = new byte[160];// unicode
                                byte[] temp = Encoding.Unicode.GetBytes("abcd");
                                for (i = 0; i < 160; i += 2)
                                {
                                    if (i < temp.Length)
                                    {
                                        item.EmployeeName2[i] = temp[i + 1];
                                        item.EmployeeName2[i + 1] = temp[i];
                                        continue;
                                    }
                                    item.EmployeeName2[i] = 0;
                                }
                                item.RFC = new byte[13];
                                temp = Encoding.ASCII.GetBytes("123456");
                                for (i = 0; i < 13; i++)
                                {
                                    if (i < temp.Length)
                                    {
                                        item.RFC[i] = temp[i];
                                        continue;
                                    }
                                    item.RFC[i] = 0;
                                }
                                item.CURP = new byte[18];
                                temp = Encoding.ASCII.GetBytes("987654321");
                                for (i = 0; i < 18; i++)
                                {
                                    if (i < temp.Length)
                                    {
                                        item.CURP[i] = temp[i];
                                        continue;
                                    }
                                    item.CURP[i] = 0;
                                }
                            }
                            ret = AnvizNew.CChex_ModifyPersonInfo(anviz_handle, dev_idx, ref item, 1);
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("Person List NULL!");
                    //}
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_delete_person_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_DEL_EMPLOYEE_INFO_STRU_EXT_INF_ID_VER_4_NEWID delete_item;
                    delete_item.EmployeeId = string_to_my_unicodebyte(28,this.textBox_personID.Text);
                    delete_item.operation = 0xFF; // bit 0:fingerprint1,1:fingerprint2,3:passwork; 4:card; 0xFF all person info
                    ret = AnvizNew.CChex_DeletePersonInfo_VER_4_NEWID(anviz_handle, dev_idx, ref delete_item);
                }
                else
                {
                    AnvizNew.CCHEX_DEL_PERSON_INFO_STRU delete_item;
                    delete_item.EmployeeId = new byte[5];
                    string_to_byte(this.textBox_personID.Text, delete_item.EmployeeId, 5);
                    delete_item.operation = 0xFF; // bit 0:fingerprint1,1:fingerprint2,3:passwork; 4:card; 0xFF all person info
                    ret = AnvizNew.CChex_DeletePersonInfo(anviz_handle, dev_idx, ref delete_item);
                }
                
                    
                
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView_person_Click(object sender, EventArgs e)
        {
            if (listView_person.SelectedItems.Count > 0)
            {
                this.textBox_personID.Text = listView_person.FocusedItem.SubItems[1].Text;
                this.textBox_name.Text = listView_person.FocusedItem.SubItems[2].Text;
                this.textBox_password.Text = listView_person.FocusedItem.SubItems[3].Text;
                this.textBox_cardID.Text = listView_person.FocusedItem.SubItems[4].Text;
                this.textBox_dept.Text = listView_person.FocusedItem.SubItems[5].Text;
                this.textBox_group.Text = listView_person.FocusedItem.SubItems[6].Text;
                this.textBox_mode.Text = listView_person.FocusedItem.SubItems[7].Text;
                this.textBox_fp_status.Text = listView_person.FocusedItem.SubItems[8].Text;
                this.textBox_reserved2.Text = listView_person.FocusedItem.SubItems[9].Text;
                this.textBox_special.Text = listView_person.FocusedItem.SubItems[10].Text;
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.CustomType.ANVIZ_CUSTOM_EMPLOYEE_FOR_W2_ADD_TIME)
                {
                    this.text_time1.Text = listView_person.FocusedItem.SubItems[11].Text;
                    this.text_time2.Text = listView_person.FocusedItem.SubItems[12].Text;
                }
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    this.text_time1.Text = listView_person.FocusedItem.SubItems[11].Text;
                    this.text_time2.Text = listView_person.FocusedItem.SubItems[12].Text;
                }

                //button7_modify_person.Enabled = true;
                button8_delete_person.Enabled = true;
                button_get_fp_raw_data.Enabled = true;
                button_put_fp_raw_data.Enabled = true;
            }
        }

        

    private void button_get_fp_raw_data_Click(object sender, EventArgs e)
        {

            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    byte[] PersonID = string_to_my_unicodebyte(28,this.textBox_personID.Text);
                    byte FingerIdx = Convert.ToByte(fptext_01.Text.ToString());
                    AnvizNew.CChex_DownloadFingerPrint_VER_4_NEWID(anviz_handle, dev_idx, PersonID, FingerIdx); // FingerIdx:(1,10)
                }
                else
                {
                    byte[] PersonID = new byte[5];
                    string_to_byte(this.textBox_personID.Text, PersonID, 5);
                    byte FingerIdx = Convert.ToByte(fptext_01.Text.ToString());
                    AnvizNew.CChex_DownloadFingerPrint(anviz_handle, dev_idx, PersonID, FingerIdx); // FingerIdx:(1,10)
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        static int dev_type_to_fp_len(int dev_type_flag)
        {
            int fp_len = 338;

            switch (dev_type_flag & 0xFF000)
            {
                // 
                //case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_1200:
                //    fp_len = 1200;
                //    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_2400:
                    fp_len = 2400;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_2048:
                    fp_len = 2048;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_6144:
                    fp_len = 6144;
                    break;
                //case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_10240:
                //    fp_len = 10240;
                //    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_15360:
                    fp_len = 15360;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_2056:
                    fp_len = 2056;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_1028:
                    fp_len = 1028;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_338:
                    fp_len = 338;
                    break;
                case (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FP_LEN_2052:
                    fp_len = 2052;
                    break;
            }

            return fp_len;
}
        private void button_put_fp_raw_data_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int fp_len = dev_type_to_fp_len(DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())]);
                
                byte[] Section = new byte[fp_len];

                if (fp_len == len_fin)
                {
                    Array.Copy(buff_fin,0, Section,0, fp_len);
                }
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    byte[] PersonID = string_to_my_unicodebyte(28,this.textBox_personID.Text);
                    byte FingerIdx = Convert.ToByte(fptext_01.Text.ToString());
                    AnvizNew.CChex_UploadFingerPrint_VER_4_NEWID(anviz_handle, dev_idx, PersonID, FingerIdx, Section, fp_len); // FingerIdx:(1~10)
                }
                else
                {
                    byte[] PersonID = new byte[5]; // string to byte
                    string_to_byte(this.textBox_personID.Text, PersonID, 5);
                    byte FingerIdx = Convert.ToByte(fptext_01.Text.ToString());
                    AnvizNew.CChex_UploadFingerPrint(anviz_handle, dev_idx, PersonID, FingerIdx, Section, fp_len); // FingerIdx:(1~10)
                }
                    

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_del_all_record_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_DEL_RECORD_INFO_STRU delete_record;
                System.Windows.Forms.DialogResult ret_msg = MessageBox.Show("Are you sure to delete all records!!!",
                                "Delete Record",
                                 MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning
                                );
                if (ret_msg == System.Windows.Forms.DialogResult.OK)
                {
                    delete_record.del_type = 0;// delete all record;
                    delete_record.del_count = 0; // skip

                    ret = AnvizNew.CChex_DeleteRecordInfo(anviz_handle, dev_idx, ref delete_record);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_del_flag_count_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_DEL_RECORD_INFO_STRU delete_record;

                delete_record.del_type = 2;// delete new flag;
                delete_record.del_count = Convert.ToUInt32(textBox_del_flag_count.Text.ToString());

                ret = AnvizNew.CChex_DeleteRecordInfo(anviz_handle, dev_idx, ref delete_record);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int ret = 0;
            byte[] Ipstr = new byte[16];
            //string_to_byte(this.Add_dev_Ip.Text,Ipstr, (byte)Add_dev_Ip.Text.Length);
            Ipstr = System.Text.Encoding.Default.GetBytes(Add_dev_Ip.Text);
            int Port = Convert.ToInt32(Add_dev_port.Text);
            
                ret = AnvizNew.CCHex_ClientConnect(anviz_handle, Ipstr, Port);
                
                
            //MessageBox.Show("Connect");
            //System.Diagnostics.Debug.WriteLine(Ipstr);
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void Add_dev_Ip_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Add_dev_port_TextChanged(object sender, EventArgs e)
        {

        }

        private void but_disconnect_Click(object sender, EventArgs e)
        {
            int ret = 0;
            int Devidx = Convert.ToInt32(text_disconnect.Text);
            ret = AnvizNew.CCHex_ClientDisconnect(anviz_handle, Devidx);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void gettime_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetTime(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxIp_TextChanged(object sender, EventArgs e)
        {

        }

        private void listViewDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void getconfig2_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetBasicConfigInfo2(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setconfig2_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF getconfig ;

                getconfig.compare_level = Convert.ToByte(config2_1.Text);
                getconfig.wiegand_range = Convert.ToByte(config2_2.Text);
                getconfig.wiegand_type = Convert.ToByte(config2_3.Text);
                getconfig.work_code = Convert.ToByte(config2_4.Text);
                getconfig.real_time_send = Convert.ToByte((Convert.ToUInt32(config2_5.Text)==1?1:0 )+ ((Convert.ToUInt32(config2_52.Text)==1?1:0)<<1));
                getconfig.auto_update = Convert.ToByte(config2_6.Text);
                getconfig.bell_lock = Convert.ToByte(config2_7.Text);
                getconfig.lock_delay = Convert.ToByte(config2_8.Text);
                getconfig.record_over_alarm = Convert.ToUInt32(config2_9.Text);
                getconfig.re_attendance_delay = Convert.ToByte(config2_10.Text);
                getconfig.door_sensor_alarm = Convert.ToByte(config2_11.Text);
                getconfig.bell_delay = Convert.ToByte(config2_12.Text);
                getconfig.correct_time = Convert.ToByte(config2_13.Text);


                ret = AnvizNew.CChex_SetBasicConfigInfo2(anviz_handle, dev_idx,ref getconfig);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void inituserarea_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_InitUserArea(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void init_system_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_InitSystem(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void get_period_time_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetPeriodTime(anviz_handle, dev_idx, Convert.ToByte(time_number.Text));
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void set_period_time_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                AnvizNew.CCHEX_SET_PERIOD_TIME_STRU_EXT_INF datainfo;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                datainfo.SerialNumbe = Convert.ToByte(time_number.Text);
                datainfo.day_week = new AnvizNew.CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[7];
                datainfo.day_week[0].StartHour = Convert.ToByte(time_p11.Text);
                datainfo.day_week[0].StartMin = Convert.ToByte(time_p12.Text);
                datainfo.day_week[0].EndHour = Convert.ToByte(time_p13.Text);
                datainfo.day_week[0].EndMin = Convert.ToByte(time_p14.Text);
                datainfo.day_week[1].StartHour = Convert.ToByte(time_p21.Text);
                datainfo.day_week[1].StartMin = Convert.ToByte(time_p22.Text);
                datainfo.day_week[1].EndHour = Convert.ToByte(time_p23.Text);
                datainfo.day_week[1].EndMin = Convert.ToByte(time_p24.Text);
                datainfo.day_week[2].StartHour = Convert.ToByte(time_p31.Text);
                datainfo.day_week[2].StartMin = Convert.ToByte(time_p32.Text);
                datainfo.day_week[2].EndHour = Convert.ToByte(time_p33.Text);
                datainfo.day_week[2].EndMin = Convert.ToByte(time_p34.Text);
                datainfo.day_week[3].StartHour = Convert.ToByte(time_p41.Text);
                datainfo.day_week[3].StartMin = Convert.ToByte(time_p42.Text);
                datainfo.day_week[3].EndHour = Convert.ToByte(time_p43.Text);
                datainfo.day_week[3].EndMin = Convert.ToByte(time_p44.Text);
                datainfo.day_week[4].StartHour = Convert.ToByte(time_p51.Text);
                datainfo.day_week[4].StartMin = Convert.ToByte(time_p52.Text);
                datainfo.day_week[4].EndHour = Convert.ToByte(time_p53.Text);
                datainfo.day_week[4].EndMin = Convert.ToByte(time_p54.Text);
                datainfo.day_week[5].StartHour = Convert.ToByte(time_p61.Text);
                datainfo.day_week[5].StartMin = Convert.ToByte(time_p62.Text);
                datainfo.day_week[5].EndHour = Convert.ToByte(time_p63.Text);
                datainfo.day_week[5].EndMin = Convert.ToByte(time_p64.Text);
                datainfo.day_week[6].StartHour = Convert.ToByte(time_p71.Text);
                datainfo.day_week[6].StartMin = Convert.ToByte(time_p72.Text);
                datainfo.day_week[6].EndHour = Convert.ToByte(time_p73.Text);
                datainfo.day_week[6].EndMin = Convert.ToByte(time_p74.Text);

                ret = AnvizNew.CChex_SetPeriodTime(anviz_handle, dev_idx, ref datainfo);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void add_fp_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & 0xFF) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    
                    AnvizNew.CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF_ID_VER_4_NEWID datainfo;
                    datainfo.EmployeeId = string_to_my_unicodebyte(28,addfp_person_id.Text);
                    datainfo.BackupNum = 1;
                    ret = AnvizNew.CCHex_AddFingerprintOnline_VER_4_NEWID(anviz_handle, dev_idx, ref datainfo);
                }
                else
                {
                    AnvizNew.CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF datainfo;
                    datainfo.EmployeeId = new byte[5];//1B 8063 72E6
                    string_to_byte(addfp_person_id.Text, datainfo.EmployeeId, 5);
                    datainfo.BackupNum = 1;
                    ret = AnvizNew.CCHex_AddFingerprintOnline(anviz_handle, dev_idx, ref datainfo);
                }
                
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getteam_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                byte teamidx = Convert.ToByte(teamid.Text);
                ret = AnvizNew.CChex_GetTeamInfo(anviz_handle, dev_idx, teamidx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setteam_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_TEAM_INFO_STRU_EXT_INF setteam;
                setteam.TeamNumbe = Convert.ToByte(teamid.Text);
                setteam.PeriodTimeNumber = new byte[4];
                setteam.PeriodTimeNumber[0] = Convert.ToByte(select_01.Text);
                setteam.PeriodTimeNumber[1] = Convert.ToByte(select_02.Text);
                setteam.PeriodTimeNumber[2] = Convert.ToByte(select_03.Text);
                setteam.PeriodTimeNumber[3] = Convert.ToByte(select_04.Text);
                ret = AnvizNew.CChex_SetTeamInfo(anviz_handle, dev_idx,ref setteam);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void forcedunlock_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //AnvizNew.CCHEX_FORCED_UNLOCK_STRU_EXT_INF 
                ret = AnvizNew.CCHex_ForcedUnlock(anviz_handle, dev_idx,IntPtr.Zero);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            int ret = 0;
                ret = AnvizNew.CCHex_Udp_Search_Dev(anviz_handle);
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (listView1.SelectedItems.Count > 0)
            {
                udp_01.Text = listView1.FocusedItem.SubItems[2].Text;
                udp_02.Text = listView1.FocusedItem.SubItems[3].Text;
                udp_03.Text = listView1.FocusedItem.SubItems[4].Text;
                udp_04.Text = listView1.FocusedItem.SubItems[5].Text;
                udp_05.Text = listView1.FocusedItem.SubItems[6].Text;
                udp_06.Text = listView1.FocusedItem.SubItems[7].Text;
                udp_07.Text = listView1.FocusedItem.SubItems[8].Text;
                udp_08.Text = listView1.FocusedItem.SubItems[9].Text;


                udp_09.Text = listView1.FocusedItem.SubItems[1].Text;


                //udp_10.Text = listView1.FocusedItem.SubItems[1].Text;
                //udp_11.Text = listView1.FocusedItem.SubItems[1].Text;
                udp_12.Text = listView1.FocusedItem.SubItems[13].Text;
                udp_13.Text = listView1.FocusedItem.SubItems[14].Text;

                Add_dev_Ip.Text = listView1.FocusedItem.SubItems[2].Text;
                Add_dev_port.Text = listView1.FocusedItem.SubItems[7].Text;


            }
    }

        private void button8_Click(object sender, EventArgs e)
        {
            if (udp_01.Text.Trim() != string.Empty && udp_02.Text.Trim() != string.Empty && udp_03.Text.Trim() != string.Empty && udp_04.Text.Trim() != string.Empty
                 && udp_06.Text.Trim() != string.Empty && udp_07.Text.Trim() != string.Empty
                && udp_08.Text.Trim() != string.Empty && udp_09.Text.Trim() != string.Empty && udp_10.Text.Trim() != string.Empty
                )
            {
                AnvizNew.CCHEX_UDP_SET_DEV_CONFIG_STRU_EXT_INF config;
                //config.DevNetInfo = AnvizNew.CCHEX_DEV_NET_INFO_STRU;
                string[] ip_array = udp_01.Text.Split('.');
                config.DevNetInfo.IpAddr = new byte[4];
                config.DevNetInfo.IpAddr[0] = Convert.ToByte(ip_array[0]);
                config.DevNetInfo.IpAddr[1] = Convert.ToByte(ip_array[1]);
                config.DevNetInfo.IpAddr[2] = Convert.ToByte(ip_array[2]);
                config.DevNetInfo.IpAddr[3] = Convert.ToByte(ip_array[3]);
                log_add_string(config.DevNetInfo.IpAddr[0].ToString());
                ip_array = udp_02.Text.Split('.');
                config.DevNetInfo.IpMask = new byte[4];
                config.DevNetInfo.IpMask[0] = Convert.ToByte(ip_array[0]);
                config.DevNetInfo.IpMask[1] = Convert.ToByte(ip_array[1]);
                config.DevNetInfo.IpMask[2] = Convert.ToByte(ip_array[2]);
                config.DevNetInfo.IpMask[3] = Convert.ToByte(ip_array[3]);


                ip_array = udp_03.Text.Split('.');
                config.DevNetInfo.GwAddr = new byte[4];
                config.DevNetInfo.GwAddr[0] = Convert.ToByte(ip_array[0]);
                config.DevNetInfo.GwAddr[1] = Convert.ToByte(ip_array[1]);
                config.DevNetInfo.GwAddr[2] = Convert.ToByte(ip_array[2]);
                config.DevNetInfo.GwAddr[3] = Convert.ToByte(ip_array[3]);
                ip_array = udp_04.Text.Split('.');
                config.DevNetInfo.MacAddr = new byte[6];
                config.DevNetInfo.MacAddr[0] = Convert.ToByte(Convert.ToInt64(ip_array[0], 16));
                config.DevNetInfo.MacAddr[1] = Convert.ToByte(Convert.ToInt64(ip_array[1], 16));
                config.DevNetInfo.MacAddr[2] = Convert.ToByte(Convert.ToInt64(ip_array[2], 16));
                config.DevNetInfo.MacAddr[3] = Convert.ToByte(Convert.ToInt64(ip_array[3], 16));
                config.DevNetInfo.MacAddr[4] = Convert.ToByte(Convert.ToInt64(ip_array[4], 16));
                config.DevNetInfo.MacAddr[5] = Convert.ToByte(Convert.ToInt64(ip_array[5], 16));

                
                config.DevNetInfo.ServAddr = new byte[4];
                if (udp_05.Text.Trim() != string.Empty)
                {
                    ip_array = udp_05.Text.Split('.');
                    config.DevNetInfo.ServAddr[0] = Convert.ToByte(ip_array[0]);
                    config.DevNetInfo.ServAddr[1] = Convert.ToByte(ip_array[1]);
                    config.DevNetInfo.ServAddr[2] = Convert.ToByte(ip_array[2]);
                    config.DevNetInfo.ServAddr[3] = Convert.ToByte(ip_array[3]);
                }
                int portint = Convert.ToInt32(udp_06.Text);
                config.DevNetInfo.Port = new byte[2];
                config.DevNetInfo.Port[0] = Convert.ToByte((portint >> 8) & 0xff);
                config.DevNetInfo.Port[1] = Convert.ToByte(portint & 0xff);

                config.DevNetInfo.NetMode = Convert.ToByte(udp_07.Text);

                config.Padding = new byte[3];
                config.Padding[0] = 0;
                config.Padding[1] = 0;
                config.Padding[2] = 0;
                config.NewMachineId = Convert.ToUInt32(udp_08.Text);

                config.Reserved = new byte[4];

                config.DevUserName = new byte[12];
                byte[] tempbname = System.Text.Encoding.Default.GetBytes(udp_10.Text.ToString());
                Array.Copy(tempbname, config.DevUserName, tempbname.Length);
                config.DevPassWord = new byte[12];
                byte[] tempbpwd = System.Text.Encoding.Default.GetBytes(udp_11.Text.ToString());
                Array.Copy(tempbpwd, config.DevPassWord, tempbpwd.Length);

                config.DevHardwareType = Convert.ToUInt32(udp_09.Text)!=167?0:1;
                //byte pwd_len = (byte)udp_11.Text.Length;
                //uint pwd = Convert.ToUInt32(udp_11.Text.ToString());


                config.Dns = new byte[4];
                if (config.DevHardwareType == 1)
                {
                    ip_array = udp_12.Text.Split('.');
                    config.Dns[0] = Convert.ToByte(ip_array[0]);
                    config.Dns[1] = Convert.ToByte(ip_array[1]);
                    config.Dns[2] = Convert.ToByte(ip_array[2]);
                    config.Dns[3] = Convert.ToByte(ip_array[3]);
                }

                //config.DevNetInfo.ServAddr = new byte[4];
                //if (udp_05.Text.Trim() != string.Empty)
                //{
                //    ip_array = udp_05.Text.Split('.');
                //    config.Dns[0] = Convert.ToByte(ip_array[0]);
                //    config.Dns[1] = Convert.ToByte(ip_array[1]);
                //    config.Dns[2] = Convert.ToByte(ip_array[2]);
                //    config.Dns[3] = Convert.ToByte(ip_array[3]);
                //}
                config.Url = new byte[100];
                byte[] tempb = System.Text.Encoding.Default.GetBytes(udp_13.Text.ToString());
                Array.Copy(tempb,config.Url,tempb.Length);

                int ret = 0;
                //log_add_string(Marshal.SizeOf(config).ToString());
                ret = AnvizNew.CCHex_Udp_Set_Dev_Config(anviz_handle, ref config);
            }
            else
            {
                MessageBox.Show("Check Dev  Info");
            }
        }

        private void label69_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetBasicConfigInfo5(anviz_handle,dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_BASIC_CFG_INFO5_STRU param;
                param.fail_alarm_time = Convert.ToByte(config5_01.Text);
                param.tamper_alarm = Convert.ToByte(config5_02.Text);
                param.reserved = new byte[94];

                ret = AnvizNew.CChex_SetBasicConfigInfo5(anviz_handle, dev_idx,ref param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_getcard_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetCardNo(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_DEV_CURRENT_STATUS_STRU param;
                param.alarm_stop = Convert.ToByte(devstatus_01.Text);
                param.door_status = Convert.ToByte(devstatus_02.Text);
                param.reserved = new byte[94];
                ret = AnvizNew.CChex_SetDevCurrentStatus(anviz_handle, dev_idx,ref param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void geturl_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetServiceURL(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void seturl_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                
                byte[] url = new byte[104];
                byte[] tempb = System.Text.Encoding.Default.GetBytes(url_01.Text.ToString());

                string[] ip_array = url_dns.Text.Split('.');
                url[0] = Convert.ToByte(ip_array[0]);
                url[1] = Convert.ToByte(ip_array[1]);
                url[2] = Convert.ToByte(ip_array[2]);
                url[3] = Convert.ToByte(ip_array[3]);

                Array.Copy(tempb,0, url,4, tempb.Length);

                ret = AnvizNew.CChex_SetServiceURL(anviz_handle, dev_idx, url, (uint)url.Length);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            baifenbi.Text = "0/100";
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                int Len;
                byte[] Buff;
                byte[] filename = System.Text.Encoding.Default.GetBytes("OA1100.zip");

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Title = "Choose File";
                dialog.Filter = "所有文件(*.*)|*.*";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = dialog.FileName;
                    FileStream fs = new FileStream(file, FileMode.Open);
                    Len = (int)fs.Length;
                    Buff = new byte[Len];
                    fs.Read(Buff, 0, Buff.Length);
                    fs.Close();
                    ret = AnvizNew.CChex_UploadFile(anviz_handle, dev_idx, 0, filename, Buff, Len);
                }
                //FileType 0: firmware 1:pic 2: audio 3: language file
                
                
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                
                    int ret = 0;
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    ret = AnvizNew.CChex_UpdateDevStatus(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getstatus01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0x200000) != 0)
                {
                    int ret = 0;
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    byte groupid = Convert.ToByte(groupid01.Text);
                    ret = AnvizNew.CChex_GetStatusSwitch(anviz_handle, dev_idx, groupid);
                }
                else
                {
                    MessageBox.Show("Type is error");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setstatus01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0x200000) != 0)
                {
                    int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_STATUS_SWITCH_STRU datainfo;
                datainfo.group_id = Convert.ToByte(groupid01.Text);
                datainfo.status_id = Convert.ToByte(status00.Text);
                datainfo.padding = new byte[2];

                datainfo.day_week = new AnvizNew.CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[7];
                datainfo.day_week[0].StartHour = Convert.ToByte(tg_001.Text);
                datainfo.day_week[0].StartMin = Convert.ToByte(tg_002.Text);
                datainfo.day_week[0].EndHour = Convert.ToByte(tg_003.Text);
                datainfo.day_week[0].EndMin = Convert.ToByte(tg_004.Text);
                datainfo.day_week[1].StartHour = Convert.ToByte(tg_005.Text);
                datainfo.day_week[1].StartMin = Convert.ToByte(tg_006.Text);
                datainfo.day_week[1].EndHour = Convert.ToByte(tg_007.Text);
                datainfo.day_week[1].EndMin = Convert.ToByte(tg_008.Text);
                datainfo.day_week[2].StartHour = Convert.ToByte(tg_009.Text);
                datainfo.day_week[2].StartMin = Convert.ToByte(tg_010.Text);
                datainfo.day_week[2].EndHour = Convert.ToByte(tg_011.Text);
                datainfo.day_week[2].EndMin = Convert.ToByte(tg_012.Text);
                datainfo.day_week[3].StartHour = Convert.ToByte(tg_013.Text);
                datainfo.day_week[3].StartMin = Convert.ToByte(tg_014.Text);
                datainfo.day_week[3].EndHour = Convert.ToByte(tg_015.Text);
                datainfo.day_week[3].EndMin = Convert.ToByte(tg_016.Text);
                datainfo.day_week[4].StartHour = Convert.ToByte(tg_017.Text);
                datainfo.day_week[4].StartMin = Convert.ToByte(tg_018.Text);
                datainfo.day_week[4].EndHour = Convert.ToByte(tg_019.Text);
                datainfo.day_week[4].EndMin = Convert.ToByte(tg_020.Text);
                datainfo.day_week[5].StartHour = Convert.ToByte(tg_021.Text);
                datainfo.day_week[5].StartMin = Convert.ToByte(tg_022.Text);
                datainfo.day_week[5].EndHour = Convert.ToByte(tg_023.Text);
                datainfo.day_week[5].EndMin = Convert.ToByte(tg_024.Text);
                datainfo.day_week[6].StartHour = Convert.ToByte(tg_025.Text);
                datainfo.day_week[6].StartMin = Convert.ToByte(tg_026.Text);
                datainfo.day_week[6].EndHour = Convert.ToByte(tg_027.Text);
                datainfo.day_week[6].EndMin = Convert.ToByte(tg_028.Text);
                ret = AnvizNew.CChex_SetStatusSwitch(anviz_handle, dev_idx, ref datainfo);
                }
                else
                {
                    MessageBox.Show("Type is error");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getstatus02_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0x100000)!=0)
                {
                    int ret = 0;

                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    byte flag_week = Convert.ToByte(flagweek_01.Text);
                    ret = AnvizNew.CChex_GetStatusSwitch_EXT(anviz_handle, dev_idx, flag_week);
                }
                else
                {
                    MessageBox.Show("Type is error");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setstatus02_Click(object sender, EventArgs e)
        {
            log_add_string("000");
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if ((DevTypeFlag[int.Parse(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString())] & 0x100000) != 0)
                {
                    log_add_string("000");
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_STATUS_SWITCH_STRU_EXT datainfo;
                datainfo.flag_week = Convert.ToByte(flagweek_01.Text);
                datainfo.padding = new byte[3];
                log_add_string("001");
                datainfo.one_time = new AnvizNew.CCHEX_ONE_TIMER_STATUS[8];
                datainfo.one_time[0].StartHour = Convert.ToByte(tgj_001.Text);
                datainfo.one_time[0].StartMin = Convert.ToByte(tgj_002.Text);
                datainfo.one_time[0].EndHour = Convert.ToByte(tgj_003.Text);
                datainfo.one_time[0].EndMin = Convert.ToByte(tgj_004.Text);
                datainfo.one_time[0].status_id = Convert.ToByte(tgj_005.Text);

                datainfo.one_time[1].StartHour = Convert.ToByte(tgj_006.Text);
                datainfo.one_time[1].StartMin = Convert.ToByte(tgj_007.Text);
                datainfo.one_time[1].EndHour = Convert.ToByte(tgj_008.Text);
                datainfo.one_time[1].EndMin = Convert.ToByte(tgj_009.Text);
                datainfo.one_time[1].status_id = Convert.ToByte(tgj_010.Text);

                datainfo.one_time[2].StartHour = Convert.ToByte(tgj_011.Text);
                datainfo.one_time[2].StartMin = Convert.ToByte(tgj_012.Text);
                datainfo.one_time[2].EndHour = Convert.ToByte(tgj_013.Text);
                datainfo.one_time[2].EndMin = Convert.ToByte(tgj_014.Text);
                datainfo.one_time[2].status_id = Convert.ToByte(tgj_015.Text);

                datainfo.one_time[3].StartHour = Convert.ToByte(tgj_016.Text);
                datainfo.one_time[3].StartMin = Convert.ToByte(tgj_017.Text);
                datainfo.one_time[3].EndHour = Convert.ToByte(tgj_018.Text);
                datainfo.one_time[3].EndMin = Convert.ToByte(tgj_019.Text);
                datainfo.one_time[3].status_id = Convert.ToByte(tgj_020.Text);

                datainfo.one_time[4].StartHour = Convert.ToByte(tgj_021.Text);
                datainfo.one_time[4].StartMin = Convert.ToByte(tgj_022.Text);
                datainfo.one_time[4].EndHour = Convert.ToByte(tgj_023.Text);
                datainfo.one_time[4].EndMin = Convert.ToByte(tgj_024.Text);
                datainfo.one_time[4].status_id = Convert.ToByte(tgj_025.Text);

                datainfo.one_time[5].StartHour = Convert.ToByte(tgj_026.Text);
                datainfo.one_time[5].StartMin = Convert.ToByte(tgj_027.Text);
                datainfo.one_time[5].EndHour = Convert.ToByte(tgj_028.Text);
                datainfo.one_time[5].EndMin = Convert.ToByte(tgj_029.Text);
                datainfo.one_time[5].status_id = Convert.ToByte(tgj_030.Text);

                datainfo.one_time[6].StartHour = Convert.ToByte(tgj_031.Text);
                datainfo.one_time[6].StartMin = Convert.ToByte(tgj_032.Text);
                datainfo.one_time[6].EndHour = Convert.ToByte(tgj_033.Text);
                datainfo.one_time[6].EndMin = Convert.ToByte(tgj_034.Text);
                datainfo.one_time[6].status_id = Convert.ToByte(tgj_035.Text);

                datainfo.one_time[7].StartHour = Convert.ToByte(tgj_036.Text);
                datainfo.one_time[7].StartMin = Convert.ToByte(tgj_037.Text);
                datainfo.one_time[7].EndHour = Convert.ToByte(tgj_038.Text);
                datainfo.one_time[7].EndMin = Convert.ToByte(tgj_039.Text);
                datainfo.one_time[7].status_id = Convert.ToByte(tgj_040.Text);
                log_add_string("002");
                ret = AnvizNew.CChex_SetStatusSwitch_EXT(anviz_handle, dev_idx, ref datainfo);
                }
                else
                {
                    MessageBox.Show("Type is error");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closeallalarm_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_GET_BASIC_CFG_INFO2_STRU_EXT_INF Config;


                Config.compare_level = 0xff;
                Config.wiegand_range = 0xff;
                Config.wiegand_type = 0xff;
                Config.work_code = 0xff;
                Config.real_time_send = 0xff;
                Config.auto_update = 0xff;
                Config.bell_lock = 0xff;
                Config.lock_delay = 0xff;
                Config.record_over_alarm = 0xffffff;
                Config.re_attendance_delay = 0xff;
                Config.door_sensor_alarm = 0x00;
                Config.bell_delay = 0xff;
                Config.correct_time = 0xff;


                ret = AnvizNew.CChex_SetBasicConfigInfo2(anviz_handle, dev_idx, ref Config);

                AnvizNew.CCHEX_SET_BASIC_CFG_INFO5_STRU param;
                param.fail_alarm_time = 0xff;
                param.tamper_alarm = 0x00;
                param.reserved = new byte[94];
                ret = AnvizNew.CChex_SetBasicConfigInfo5(anviz_handle, dev_idx, ref param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (service_port.Text.Trim() != string.Empty&& iscloseservice.Text.Trim() != string.Empty)
            {
                //AnvizNew.CChex_Set_Service_Port(Convert.ToUInt16(service_port.Text));
                //if (Convert.ToUInt16(iscloseservice.Text) == 1)
                //{
                //    AnvizNew.CChex_Set_Service_Disenable();
                //}
                this.config3.Hide();

                AnvizNew.CChex_Init();

                anviz_handle = AnvizNew.CChex_Start_With_Param(Convert.ToUInt16(iscloseservice.Text),Convert.ToUInt16(service_port.Text));
                DevCount = 0;
                if (anviz_handle != null)
                {
                    this.timer1.Enabled = true;
                    log_add_string(AnvizNew.CChex_Get_Service_Port(anviz_handle).ToString());

                    this.Text += ("Ver:" + AnvizNew.CChex_Version().ToString() + "Service port: " + AnvizNew.CChex_Get_Service_Port(anviz_handle).ToString());

                    groupBox15.Hide();
                }
                else
                {
                    MessageBox.Show("Startup errors,Please restart the program.");
                }
                if (choose.CheckState == CheckState.Checked)
                {
                    AnvizNew.CCHEX_CONNECTION_AUTHENTICATION_STRU param;
                    byte[] tempb = Encoding.Default.GetBytes(a_name.Text.ToString());
                    param.username = new byte[12];
                    Array.Copy(tempb, param.username, tempb.Length > 12 ? 12 : tempb.Length);
                    param.password = new byte[12];
                    tempb = Encoding.Default.GetBytes(a_password.Text.ToString());
                    Array.Copy(tempb, param.password, tempb.Length > 12 ? 12 : tempb.Length);

                    IntPtr paramptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));
                    Marshal.StructureToPtr(param, paramptr, false);

                    AnvizNew.CChex_Set_Connect_Authentication(anviz_handle, paramptr);
                    //AnvizNew.CChex_Set_Connect_Authentication(IntPtr.Zero);
                    Marshal.FreeHGlobal(paramptr);
                }

            }
            else
            {
                MessageBox.Show("Param   error");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_BASIC_CFG_INFO3_STRU Param;
                Param.wiegand_type = 0xff;
                Param.online_mode = 0xff;
                Param.collect_level = 0xff;
                Param.pwd_status = 0xff;
                Param.sensor_status = 0xff;
                Param.independent_time = 0xff;
                Param.m5_t5_status = 0xff;
                Param.reserved = new byte[8];

                Param.reserved[0] = 0xff;
                Param.reserved[1] = 0xff;
                Param.reserved[2] = 0xff;
                Param.reserved[3] = 0xff;
                Param.reserved[4] = 0xff;
                Param.reserved[5] = 0xff;
                Param.reserved[6] = 0xff;
                Param.reserved[7] = 0x00;

                ret = AnvizNew.CChex_SetBasicConfigInfo3(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_BASIC_CFG_INFO3_STRU Param;
                Param.wiegand_type = 0xff;
                Param.online_mode = 0xff;
                Param.collect_level = 0xff;
                Param.pwd_status = 0xff;
                Param.sensor_status = 0xff;
                Param.independent_time = 0xff;
                Param.m5_t5_status = 0xff;
                Param.reserved = new byte[8];

                Param.reserved[0] = 0xff;
                Param.reserved[1] = 0xff;
                Param.reserved[2] = 0xff;
                Param.reserved[3] = 0xff;
                Param.reserved[4] = 0xff;
                Param.reserved[5] = 0xff;
                Param.reserved[6] = 0xff;
                Param.reserved[7] = 0x01;

                ret = AnvizNew.CChex_SetBasicConfigInfo3(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetConfig3_01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_SET_BASIC_CFG_INFO3_STRU Param;
                Param.wiegand_type = Convert.ToByte(c3_01.Text);
                Param.online_mode = Convert.ToByte(c3_02.Text);
                Param.collect_level = Convert.ToByte(c3_03.Text);
                Param.pwd_status = Convert.ToByte(c3_04.Text);
                Param.sensor_status = Convert.ToByte(c3_05.Text);
                Param.independent_time = Convert.ToByte(c3_07.Text);
                Param.m5_t5_status = Convert.ToByte(c3_08.Text);
                Param.reserved = new byte[8];

                Param.reserved[0] = 0xff;
                Param.reserved[1] = 0xff;
                Param.reserved[2] = 0xff;
                Param.reserved[3] = 0xff;
                Param.reserved[4] = 0xff;
                Param.reserved[5] = 0xff;
                Param.reserved[6] = 0xff;
                Param.reserved[7] = Convert.ToByte(Convert.ToUInt32(c3_06.Text) == 1 ? 1 : 0);

                ret = AnvizNew.CChex_SetBasicConfigInfo3(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetConfig3_01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                ret = AnvizNew.CChex_GetBasicConfigInfo3(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            AnvizNew.CChex_SetSdkConfig(anviz_handle, Convert.ToInt32(sdk_01.Text) == 1 ? 1 : 0, Convert.ToInt32(sdk_02.Text) == 1 ? 1 : 0, Convert.ToInt32(sdk_03.Text) == 1 ? 1 : 0);
        }

        

        private void getmachineid_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                ret = AnvizNew.CChex_GetMachineId(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setmachine_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                uint MachineId = Convert.ToUInt32(machineid_01.Text.ToString());
                ret = AnvizNew.CChex_SetMachineId(anviz_handle, dev_idx, MachineId);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void authentication_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                AnvizNew.CCHEX_CONNECTION_AUTHENTICATION_STRU param;
                param.username = new byte[12];
                param.password = new byte[12];
                byte[] temp = System.Text.Encoding.Default.GetBytes(c_username.Text.ToString());
                Array.Copy(temp, param.username,temp.Length>12?12: temp.Length);
                temp = System.Text.Encoding.Default.GetBytes(c_password.Text.ToString());
                Array.Copy(temp, param.password, temp.Length > 12 ? 12 : temp.Length);
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                uint MachineId = Convert.ToUInt32(machineid_01.Text.ToString());
                ret = AnvizNew.CChex_ConnectionAuthentication(anviz_handle, dev_idx, ref param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearadmin_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_ClearAdministratFlag(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newrecord_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_DownloadAllNewRecords(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void button12_Click_2(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int ret = 0;

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                ret = AnvizNew.CChex_GetSpecialStatus(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            uint MachineId = Convert.ToUInt32(machine_z.Text.ToString());
            int ret = AnvizNew.CChex_Find_DevIdx_By_MachineId(anviz_handle,MachineId);
            devidx_z.Text = ret.ToString();
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void set_a_Click(object sender, EventArgs e)
        {
            if (choose02.CheckState == CheckState.Checked)
            {
                AnvizNew.CCHEX_CONNECTION_AUTHENTICATION_STRU param;
                byte[] tempb = Encoding.Default.GetBytes(a_name_02.Text.ToString());
                param.username = new byte[12];
                Array.Copy(tempb, param.username, tempb.Length > 12 ? 12 : tempb.Length);
                param.password = new byte[12];
                tempb = Encoding.Default.GetBytes(a_password_02.Text.ToString());
                Array.Copy(tempb, param.password, tempb.Length > 12 ? 12 : tempb.Length);

                IntPtr paramptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));
                Marshal.StructureToPtr(param, paramptr, false);

                AnvizNew.CChex_Set_Connect_Authentication(anviz_handle, paramptr);
                //AnvizNew.CChex_Set_Connect_Authentication(anviz_handle,IntPtr.Zero);
                Marshal.FreeHGlobal(paramptr);
            }
            else
            {
                AnvizNew.CChex_Set_Connect_Authentication(anviz_handle, IntPtr.Zero);
            }
            
        }

        private void upload_003_TextChanged(object sender, EventArgs e)
        {

        }

        private void upload_record_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_UPLOAD_RECORD_INFO_STRU_VER_4_NEWID record;
                    record.EmployeeId = string_to_my_unicodebyte(28, upload_001.Text);

                    record.date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(upload_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    record.date[0] = (byte)((sec1 >> 24) & 0xff);
                    record.date[1] = (byte)((sec1 >> 16) & 0xff);
                    record.date[2] = (byte)((sec1 >> 8) & 0xff);
                    record.date[3] = (byte)((sec1 >> 0) & 0xff);

                    //record.date = new byte[4];
                    //string_to_byte(upload_002.Text, record.date, 4);

                    record.back_id = (byte)int.Parse(upload_003.Text);
                    record.record_type = (byte)int.Parse(upload_004.Text);
                    record.work_type = new byte[3];
                    string_to_byte(upload_005.Text, record.work_type, 3);

                    AnvizNew.CChex_UploadRecord_VER_4_NEWID(anviz_handle, dev_idx, ref record);
                }
                else
                {
                    AnvizNew.CCHEX_UPLOAD_RECORD_INFO_STRU record;
                    record.EmployeeId = new byte[5];
                    string_to_byte(upload_001.Text, record.EmployeeId, 5);
                    //record.date = new byte[4];
                    //string_to_byte(upload_002.Text, record.date, 4);
                    record.date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(upload_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    record.date[0] = (byte)((sec1 >> 24) & 0xff);
                    record.date[1] = (byte)((sec1 >> 16) & 0xff);
                    record.date[2] = (byte)((sec1 >> 8) & 0xff);
                    record.date[3] = (byte)((sec1 >> 0) & 0xff);

                    record.back_id = (byte)int.Parse(upload_003.Text);
                    record.record_type = (byte)int.Parse(upload_004.Text);
                    record.work_type = new byte[3];
                    string_to_byte(upload_005.Text, record.work_type, 3);

                    AnvizNew.CChex_UploadRecord(anviz_handle, dev_idx, ref record);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox4_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID param;
                    param.EmployeeId = string_to_my_unicodebyte(28, p_record_001.Text);
                    //param.start_date = new byte[4];
                    //string_to_byte(p_record_002.Text, param.start_date, 4);
                    //param.end_date = new byte[4];
                    //string_to_byte(p_record_003.Text, param.end_date, 4);

                    param.start_date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(p_record_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                    param.end_date = new byte[4];
                    datetemp_cur = Convert.ToDateTime(p_record_003.Text.ToString());
                    datetemp_begin = new DateTime(2000, 1, 2);
                    sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.end_date[3] = (byte)((sec1 >> 0) & 0xff);


                    AnvizNew.CChex_DownloadRecordByEmployeeIdAndTime_VER_4_NEWID(anviz_handle, dev_idx, ref param);
                }
                else
                {
                    AnvizNew.CCHEX_GET_RECORD_INFO_BY_TIME param;
                    param.EmployeeId = new byte[5];
                    string_to_byte(p_record_001.Text, param.EmployeeId, 5);
                    //param.start_date = new byte[4];
                    //string_to_byte(p_record_002.Text, param.start_date, 4);

                    //param.end_date = new byte[4];
                    //string_to_byte(p_record_003.Text, param.end_date, 4);
                    param.start_date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(p_record_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                    param.end_date = new byte[4];
                    datetemp_cur = Convert.ToDateTime(p_record_003.Text.ToString());
                    datetemp_begin = new DateTime(2000, 1, 2);
                    sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.end_date[3] = (byte)((sec1 >> 0) & 0xff);


                    AnvizNew.CChex_DownloadRecordByEmployeeIdAndTime(anviz_handle, dev_idx, ref param);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_GET_RECORD_INFO_BY_TIME_VER_4_NEWID param;
                    param.EmployeeId = string_to_my_unicodebyte(28, p_record_001.Text);
                    //param.start_date = new byte[4];
                    //string_to_byte(p_record_002.Text, param.start_date, 4);

                    //param.end_date = new byte[4];
                    //string_to_byte(p_record_003.Text, param.end_date, 4);

                    param.start_date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(p_record_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                    param.end_date = new byte[4];
                    datetemp_cur = Convert.ToDateTime(p_record_003.Text.ToString());
                    datetemp_begin = new DateTime(2000, 1, 2);
                    sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.end_date[3] = (byte)((sec1 >> 0) & 0xff);

                    AnvizNew.CChex_GetRecordNumByEmployeeIdAndTime_VER_4_NEWID(anviz_handle, dev_idx, ref param);
                }
                else
                {
                    AnvizNew.CCHEX_GET_RECORD_INFO_BY_TIME param;
                    param.EmployeeId = new byte[5];
                    string_to_byte(p_record_001.Text, param.EmployeeId, 5);
                    //param.start_date = new byte[4];
                    //string_to_byte(p_record_002.Text, param.start_date, 4);

                    //param.end_date = new byte[4];
                    //string_to_byte(p_record_003.Text, param.end_date, 4);
                    param.start_date = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(p_record_002.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                    param.end_date = new byte[4];
                    datetemp_cur = Convert.ToDateTime(p_record_003.Text.ToString());
                    datetemp_begin = new DateTime(2000, 1, 2);
                    sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                    param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                    param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                    param.end_date[3] = (byte)((sec1 >> 0) & 0xff);


                    AnvizNew.CChex_GetRecordNumByEmployeeIdAndTime(anviz_handle, dev_idx, ref param);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                listView_person.Items.Clear();
                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_GET_ONE_EMPLOYEE_INFO_STRU_ID_VER_4_NEWID param;
                    param.EmployeeId = string_to_my_unicodebyte(28, get_p_001.Text);

                    AnvizNew.CChex_GetOnePersonInfo_VER_4_NEWID(anviz_handle, dev_idx, ref param);
                }
                else
                {
                    AnvizNew.CCHEX_GET_ONE_EMPLOYEE_INFO_STRU param;
                    param.EmployeeId = new byte[5];
                    string_to_byte(get_p_001.Text, param.EmployeeId, 5);


                    AnvizNew.CChex_GetOnePersonInfo(anviz_handle, dev_idx, ref param);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CChex_GetRecordInfoStatus(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            anviz_handle = AnvizNew.CChex_Start();
            DevCount = 0;


            if (anviz_handle != IntPtr.Zero)
            {

                this.timer1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Startup errors,Please restart the program.");
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            DevTypeFlag.Clear();
            IntPtr anviz_handle_tmp = anviz_handle;
            anviz_handle = IntPtr.Zero;
            AnvizNew.CChex_Stop(anviz_handle_tmp);

            listBoxLog.Items.Clear();
            listViewDevice.Items.Clear();
        }

        private void button_del_all_record_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_DEL_RECORD_INFO_STRU delete_record;
                System.Windows.Forms.DialogResult ret_msg = MessageBox.Show("Are you sure to delete all records!!!",
                                "Delete Record",
                                 MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning
                                );
                if (ret_msg == System.Windows.Forms.DialogResult.OK)
                {
                    delete_record.del_type = 0;// delete all record;
                    delete_record.del_count = 0; // skip

                    ret = AnvizNew.CChex_DeleteRecordInfo(anviz_handle, dev_idx, ref delete_record);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_del_flag_count_Click_1(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_DEL_RECORD_INFO_STRU delete_record;

                delete_record.del_type = 2;// delete new flag;
                delete_record.del_count = Convert.ToUInt32(textBox_del_flag_count.Text.ToString());

                ret = AnvizNew.CChex_DeleteRecordInfo(anviz_handle, dev_idx, ref delete_record);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_get_employee_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                listsac_employee.Items.Clear();

                //ret = AnvizNew.CChex_SAC_DownloadAllEmployeeInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_manage_01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //Param.start_date =  new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.CmdType = 0x00;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log */
                Param.AutoFlag = 0;

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_manage_02_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //Param.start_date = new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.CmdType = 0x01;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log */
                Param.AutoFlag = 0;

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_manage_03_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //Param.start_date = new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.CmdType = 0x02;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log */
                Param.AutoFlag = 0;

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void select_sac_employee(object sender, EventArgs e)
        {
            if (listsac_employee.SelectedItems.Count > 0)
            {
                this.sac_e_01.Text = listsac_employee.FocusedItem.SubItems[1].Text;
                this.sac_e_02.Text = listsac_employee.FocusedItem.SubItems[2].Text;
                this.sac_e_03.Text = listsac_employee.FocusedItem.SubItems[3].Text;
                this.sac_e_04.Text = listsac_employee.FocusedItem.SubItems[4].Text;
                this.sac_e_05.Text = listsac_employee.FocusedItem.SubItems[5].Text;
                this.sac_e_06.Text = listsac_employee.FocusedItem.SubItems[6].Text;
                this.sac_e_07.Text = listsac_employee.FocusedItem.SubItems[7].Text;
                this.sac_e_08.Text = listsac_employee.FocusedItem.SubItems[8].Text;
                this.sac_e_09.Text = listsac_employee.FocusedItem.SubItems[9].Text;
                this.sac_e_10.Text = listsac_employee.FocusedItem.SubItems[10].Text;

            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                if (sac_e_03.Text.Length <= 6 || sac_e_04.Text.Length <= 10)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    int falg = 1;
                    if(falg == 1)
                    {
                        AnvizNew.SAC_EMPLOYEE_INFO_STRU item = new AnvizNew.SAC_EMPLOYEE_INFO_STRU();
                        item.EmployeeId = new byte[5];
                        string_to_byte(sac_e_01.Text, item.EmployeeId, 5);

                        item.EmployeeName = string_to_my_unicodebyte(20, sac_e_02.Text);

                        item.Password = new byte[3];
                        uint pwd = Convert.ToUInt32(sac_e_03.Text.ToString());
                        item.Password[0] = (byte)(((sac_e_03.Text.Length) << 4) + ((pwd >> 16) & 0x0f));
                        item.Password[1] = (byte)((pwd >> 8) & 0xff);
                        item.Password[2] = (byte)(pwd & 0xff);

                        item.Card = new byte[4];
                        string_to_byte(sac_e_04.Text, item.Card, 4);



                        item.CardType = Convert.ToByte(sac_e_05.Text);
                        item.GroupId = Convert.ToByte(sac_e_06.Text);
                        item.AttendanceMode = Convert.ToByte(sac_e_07.Text);
                        item.FpStatus = new byte[2];
                        uint fpint = Convert.ToUInt32(sac_e_08.Text.ToString());
                        item.FpStatus[0] = (byte)((fpint >> 8) & 0xff);
                        item.FpStatus[1] = (byte)((fpint) & 0xff);

                        item.PwdH8bit = 0x00;
                        item.HolidayGroup = Convert.ToByte(sac_e_09.Text);
                        item.Special = Convert.ToByte(sac_e_10.Text);
                        int nSizeOfPerson = Marshal.SizeOf(new AnvizNew.SAC_EMPLOYEE_INFO_STRU());
                        AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
                    }
                    else
                    {
                        int nSizeOfPerson = Marshal.SizeOf(new AnvizNew.SAC_EMPLOYEE_INFO_STRU());
                        byte[] buff = new byte[nSizeOfPerson * 100];
                        for (int j = 0; j < 100; j++)
                        {
                            AnvizNew.SAC_EMPLOYEE_INFO_STRU item = new AnvizNew.SAC_EMPLOYEE_INFO_STRU();
                            //item[j] = new AnvizNew.SAC_EMPLOYEE_INFO_STRU();
                            item.EmployeeId = new byte[5];
                            //string_to_byte(sac_e_01.Text, item.EmployeeId, 5);
                            string_to_byte_add_num(sac_e_01.Text, item.EmployeeId, 5, j);

                            item.EmployeeName = string_to_my_unicodebyte(20, sac_e_02.Text);

                            item.Password = new byte[3];
                            uint pwd = Convert.ToUInt32(sac_e_03.Text.ToString());
                            item.Password[0] = (byte)(((sac_e_03.Text.Length) << 4) + ((pwd >> 16) & 0x0f));
                            item.Password[1] = (byte)((pwd >> 8) & 0xff);
                            item.Password[2] = (byte)(pwd & 0xff);

                            item.Card = new byte[4];
                            string_to_byte(sac_e_04.Text, item.Card, 4);



                            item.CardType = Convert.ToByte(sac_e_05.Text);
                            item.GroupId = Convert.ToByte(sac_e_06.Text);
                            item.AttendanceMode = Convert.ToByte(sac_e_07.Text);
                            item.FpStatus = new byte[2];
                            uint fpint = Convert.ToUInt32(sac_e_08.Text.ToString());
                            item.FpStatus[0] = (byte)((fpint >> 8) & 0xff);
                            item.FpStatus[1] = (byte)((fpint) & 0xff);

                            item.PwdH8bit = 0x00;
                            item.HolidayGroup = Convert.ToByte(sac_e_09.Text);
                            item.Special = Convert.ToByte(sac_e_10.Text);
                            Array.Copy(StructToBytes(item),0,buff,j*nSizeOfPerson,nSizeOfPerson);
                        }
                        //AnvizNew.CChex_SAC_UploadEmployeeInfo(anviz_handle, dev_idx, ref item, 1);
                        //int nSizeOfPerson = Marshal.SizeOf(new AnvizNew.SAC_EMPLOYEE_INFO_STRU())*100;
                        AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo, 100, buff, (uint)nSizeOfPerson*100);
                    }
                }
                else
                {
                    MessageBox.Show("Password Max len id 6,Crad max is 10");
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_delete_people_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
               // if (sac_e_01.Text.Length <= 6 || sac_e_04.Text.Length <= 10)
                {
                    int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                    byte[] EmployeeId = new byte[12];
                    string_to_byte(sac_delete01.Text, EmployeeId, 5);
                    EmployeeId[5] = 255;
                    string_to_byte_begin(sac_delete02.Text, EmployeeId, 5,6);
                    EmployeeId[11] = 255;

                    AnvizNew.CChex_SAC_Delete_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo, 2, EmployeeId, 12);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void sac_init_people_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                AnvizNew.CChex_SAC_Init_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffInfo);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_get_group_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffGroup);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_upload_group_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_GROUP_INFO_STRU item = new AnvizNew.SAC_GROUP_INFO_STRU();
                item.GroupId = Convert.ToByte(sac_up_01.Text);
                item.GroupName = string_to_my_unicodebyte(100, sac_up_02.Text);
                item.uGroupType = 0;
                //AnvizNew.CChex_SAC_UploadGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffGroup, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void p_g_btn1_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllEmployeeWithGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffCombi);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void p_g_btn2_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_EMPLOYEE_WITH_GROUP_INFO_STRU item = new AnvizNew.SAC_EMPLOYEE_WITH_GROUP_INFO_STRU();
                item.GroupId = Convert.ToByte(p_g_01.Text);
                item.EmployeeId = new byte[5];
                // TODO: check textBox_personID do not exceed 12 digitals number
                string_to_byte(p_g_02.Text, item.EmployeeId, 5);
                //AnvizNew.CChex_SAC_UploadEmployeeWithGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_StaffCombi, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_Door);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_door_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_SET_DOOR_INFO_STRU item = new AnvizNew.SAC_SET_DOOR_INFO_STRU();
                item.DoorId = Convert.ToByte(sac_1001.Text);
                item.DoorName = string_to_my_unicodebyte(100, sac_1002.Text);
                item.DevName = string_to_my_unicodebyte(40, sac_1003.Text);
                item.Anti_subType = Convert.ToByte(sac_1004.Text);
                item.InterlockFlag = Convert.ToByte(sac_1005.Text);
                item.InterlockDoorId = Convert.ToByte(sac_1006.Text);
                item.DoorStatus = Convert.ToByte(sac_1007.Text);
                int nSizeOfPerson = Marshal.SizeOf(item);
                //IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_Door,1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downdoorgroup_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllDoorGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorGroup);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updoorgroup_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_GROUP_INFO_STRU item = new AnvizNew.SAC_GROUP_INFO_STRU();
                item.GroupId = Convert.ToByte(door_g_01.Text);
                item.GroupName = string_to_my_unicodebyte(100, door_g_02.Text);
                //AnvizNew.CChex_SAC_UploadDoorGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorGroup, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void door_group_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllDoorWithDoorGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorCombi);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updoor_group_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_DOOR_WITH_DOORGROUP_INFO_STRU item = new AnvizNew.SAC_DOOR_WITH_DOORGROUP_INFO_STRU();
                item.CombinationId = Convert.ToByte(door01.Text);
                item.DoorId = Convert.ToByte(door02.Text);
                item.DoorGroupId = Convert.ToByte(door03.Text);
                //AnvizNew.CChex_SAC_UploadDoorWithDoorGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorCombi, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void downtimegroup_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllTimeGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpaceGroup);

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void uptimegroup_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_GROUP_INFO_STRU item = new AnvizNew.SAC_GROUP_INFO_STRU();
                item.GroupId = Convert.ToByte(time_g_01.Text);
                item.GroupName = string_to_my_unicodebyte(100, time_g_02.Text);
                //AnvizNew.CChex_SAC_UploadTimeGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpaceGroup, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAllTimeFrameWithTimeGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpaceCombi);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_TimeFrame_WITH_TimeGROUP_INFO_STRU item = new AnvizNew.SAC_TimeFrame_WITH_TimeGROUP_INFO_STRU();
                item.CombinationId = Convert.ToByte(time01.Text);
                item.TimeFrameId = Convert.ToByte(time02.Text);
                item.TimeFrameGroupId = Convert.ToByte(time03.Text);
                //AnvizNew.CChex_SAC_UploadTimeFrameWithTimeGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpaceCombi, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_get_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                //ret = AnvizNew.CChex_SAC_DownloadAccessControlGroupInfo(anviz_handle, dev_idx);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorTimeSpaceCombi);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sac_set_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_ACCESS_CONTROL_GROUP_INFO_STRU item = new AnvizNew.SAC_ACCESS_CONTROL_GROUP_INFO_STRU();
                item.SAC_GroupId = Convert.ToByte(sac_sac01.Text);
                item.GroupName = string_to_my_unicodebyte(100, sac_sac02.Text);
                item.EmployeeGroupId = Convert.ToByte(sac_sac03.Text);
                item.DoorGroupId = Convert.ToByte(sac_sac04.Text);
                item.TimeGroupId = Convert.ToByte(sac_sac05.Text);
                //AnvizNew.CChex_SAC_UploadAccessControlGroupInfo(anviz_handle, dev_idx, ref item, 1);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_DoorTimeSpaceCombi, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gettimeframe_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                byte TimeFrameNum = Convert.ToByte(sac_time00.Text);
                //ret = AnvizNew.CChex_SAC_DownloadTimeFrameInfo(anviz_handle, dev_idx, TimeFrameNum);
                AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpace);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settimeframe_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.SAC_UPLOAD_TIME_FRAME_INFO_STRU item = new AnvizNew.SAC_UPLOAD_TIME_FRAME_INFO_STRU();
                item.date = new AnvizNew.CCHEX_GET_PERIOD_TIME_ONE_STRU_EXT_INF[10];
                item.date[0].StartHour = 1;
                item.date[0].StartMin = 2;
                item.date[0].EndHour = 3;
                item.date[0].EndMin = 4;
                item.date[8].StartHour = 1;
                item.date[8].StartMin = 2;
                item.date[8].EndHour = 3;
                item.date[8].EndMin = 4;
                byte TimeFrameNum  = Convert.ToByte(sac_time00.Text);
                //AnvizNew.CChex_SAC_UploadTimeFrameInfo(anviz_handle, dev_idx, ref item, TimeFrameNum);
                int nSizeOfPerson = Marshal.SizeOf(item);
                AnvizNew.CChex_SAC_Upload_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_TimeSpace, 1, StructToBytes(item, nSizeOfPerson), (uint)nSizeOfPerson);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_auto_btn1_Click(object sender, EventArgs e)
        {

            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //Param.start_date = new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff); /* time is not use */

                Param.CmdType = 0x04;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log 0x04 get is auto flag */
                Param.AutoFlag = 0;

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_auto_btn2_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //Param.start_date = new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff); /* time is not use */

                Param.CmdType = 0x03;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log 0x03 set auto flag*/
                Param.AutoFlag = 0x01;                     /* AutoFlag : 0x01   open auto     0x00 close auto*/

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void log_auto_btn3_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                AnvizNew.CCHEX_MANAGE_LOG_RECORD Param;
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                //Param.start_date = new byte[4];
                //string_to_byte(log_begintime.Text, Param.start_date, 4);
                //Param.end_date = new byte[4];
                //string_to_byte(log_endtime.Text, Param.end_date, 4);

                Param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(log_begintime.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                Param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(log_endtime.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                Param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                Param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                Param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                Param.end_date[3] = (byte)((sec1 >> 0) & 0xff);
                                                                /* time is not use */
                Param.CmdType = 0x03;                   /*　0x00 get number for log;  0x01 download log; 0x02  remove log 0x03 set auto*/
                Param.AutoFlag = 0x00;                   /* AutoFlag : 0x01   open auto     0x00 close auto*/

                ret = AnvizNew.CChex_ManageLogRecord(anviz_handle, dev_idx, ref Param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void upload_002_TextChanged(object sender, EventArgs e)
        {

        }

        private void pic_getnum_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetPictureNumber(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pic_gethead_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetPictureAllHeadInfo(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pic_getpic_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());


                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID Param;
                    Param.EmployeeId = string_to_my_unicodebyte(28, pic_eid.Text);
                    Param.DateTime = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(pic_time.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    Param.DateTime[0] = (byte)((sec1 >> 24) & 0xff);
                    Param.DateTime[1] = (byte)((sec1 >> 16) & 0xff);
                    Param.DateTime[2] = (byte)((sec1 >> 8) & 0xff);
                    Param.DateTime[3] = (byte)((sec1 >> 0) & 0xff);

                    ret = AnvizNew.CChex_GetPictureByEmployeeIdAndTime_VER_4_NEWID(anviz_handle, dev_idx, ref Param);
                }
                else
                {
                    AnvizNew.CCHEX_PICTURE_BY_EID_AND_TIME Param;
                    Param.EmployeeId = new byte[5];
                    string_to_byte(pic_eid.Text, Param.EmployeeId, 5);
                    Param.DateTime = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(pic_time.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    Param.DateTime[0] = (byte)((sec1 >> 24) & 0xff);
                    Param.DateTime[1] = (byte)((sec1 >> 16) & 0xff);
                    Param.DateTime[2] = (byte)((sec1 >> 8) & 0xff);
                    Param.DateTime[3] = (byte)((sec1 >> 0) & 0xff);
                    ret = AnvizNew.CChex_GetPictureByEmployeeIdAndTime(anviz_handle, dev_idx, ref Param);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pic_delpic_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());


                if ((DevTypeFlag[dev_idx] & 0xff) == (int)AnvizNew.CustomType.DEV_TYPE_VER_4_NEWID)
                {
                    AnvizNew.CCHEX_PICTURE_BY_EID_AND_TIME_VER_4_NEWID Param;
                    Param.EmployeeId = string_to_my_unicodebyte(28, pic_eid.Text);
                    Param.DateTime = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(pic_time.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    Param.DateTime[0] = (byte)((sec1 >> 24) & 0xff);
                    Param.DateTime[1] = (byte)((sec1 >> 16) & 0xff);
                    Param.DateTime[2] = (byte)((sec1 >> 8) & 0xff);
                    Param.DateTime[3] = (byte)((sec1 >> 0) & 0xff);

                    ret = AnvizNew.CChex_DelPictureByEmployeeIdAndTime_VER_4_NEWID(anviz_handle, dev_idx, ref Param);
                }
                else
                {
                    AnvizNew.CCHEX_PICTURE_BY_EID_AND_TIME Param;
                    Param.EmployeeId = new byte[5];
                    string_to_byte(pic_eid.Text, Param.EmployeeId, 5);
                    Param.DateTime = new byte[4];
                    DateTime datetemp_cur = Convert.ToDateTime(pic_time.Text.ToString());
                    DateTime datetemp_begin = new DateTime(2000, 1, 2);
                    int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                    Param.DateTime[0] = (byte)((sec1 >> 24) & 0xff);
                    Param.DateTime[1] = (byte)((sec1 >> 16) & 0xff);
                    Param.DateTime[2] = (byte)((sec1 >> 8) & 0xff);
                    Param.DateTime[3] = (byte)((sec1 >> 0) & 0xff);
                    ret = AnvizNew.CChex_DelPictureByEmployeeIdAndTime(anviz_handle, dev_idx, ref Param);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Test_Bell_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_GetBellInfo(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                byte number = Convert.ToByte(bell0.Text.ToString());
                byte hour = Convert.ToByte(bell1.Text.ToString());
                byte min = Convert.ToByte(bell2.Text.ToString());
                byte week = Convert.ToByte(bell3.Text.ToString());

                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_SetBellInfo(anviz_handle, dev_idx, number, hour, min, week);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label172_Click(object sender, EventArgs e)
        {

        }

        private void sac_getevent_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_SAC_Download_Common(anviz_handle, dev_idx, (uint)AnvizNew.CMD_PRINCIPAL.CMD_Pri_AtEvent);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_TM_DownloadAllRecords(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                ret = AnvizNew.CChex_TM_DownloadAllNewRecords(anviz_handle, dev_idx);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());

                AnvizNew.CCHEX_GET_RECORD_INFO_BY_TIME param;
                param.EmployeeId = new byte[5];
                string_to_byte(tm_employee_id.Text, param.EmployeeId, 5);
                //param.start_date = new byte[4];
                //string_to_byte(p_record_002.Text, param.start_date, 4);

                //param.end_date = new byte[4];
                //string_to_byte(p_record_003.Text, param.end_date, 4);
                param.start_date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(tm_start_time.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                param.start_date[0] = (byte)((sec1 >> 24) & 0xff);
                param.start_date[1] = (byte)((sec1 >> 16) & 0xff);
                param.start_date[2] = (byte)((sec1 >> 8) & 0xff);
                param.start_date[3] = (byte)((sec1 >> 0) & 0xff);

                param.end_date = new byte[4];
                datetemp_cur = Convert.ToDateTime(tm_stop_time.Text.ToString());
                datetemp_begin = new DateTime(2000, 1, 2);
                sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                param.end_date[0] = (byte)((sec1 >> 24) & 0xff);
                param.end_date[1] = (byte)((sec1 >> 16) & 0xff);
                param.end_date[2] = (byte)((sec1 >> 8) & 0xff);
                param.end_date[3] = (byte)((sec1 >> 0) & 0xff);
                ret = AnvizNew.CChex_TM_DownloadRecordByEmployeeIdAndTime(anviz_handle, dev_idx, ref param);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int temp01 = 368;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                AnvizNew.CCHEX_TM_UPLOAD_RECORD_INFO_STRU record;
                record.EmployeeId = new byte[5];
                string_to_byte(upload_001.Text, record.EmployeeId, 5);
                //record.date = new byte[4];
                //string_to_byte(upload_002.Text, record.date, 4);
                record.date = new byte[4];
                DateTime datetemp_cur = Convert.ToDateTime(upload_002.Text.ToString());
                DateTime datetemp_begin = new DateTime(2000, 1, 2);
                int sec1 = (int)(datetemp_cur - datetemp_begin).TotalSeconds;
                record.date[0] = (byte)((sec1 >> 24) & 0xff);
                record.date[1] = (byte)((sec1 >> 16) & 0xff);
                record.date[2] = (byte)((sec1 >> 8) & 0xff);
                record.date[3] = (byte)((sec1 >> 0) & 0xff);

                record.back_id = (byte)int.Parse(upload_003.Text);
                record.record_type = (byte)int.Parse(upload_004.Text);
                record.work_type = new byte[3];
                string_to_byte(upload_005.Text, record.work_type, 3);
                record.Temperature = new byte[2];
                record.Temperature[0] = (byte)((temp01 >> 8) & 0xff);
                record.Temperature[1] = (byte)(temp01 & 0xff);
                record.IsMask = 1;
                record.OpenType = 2;
                ret = AnvizNew.CChex_TM_UploadRecord(anviz_handle, dev_idx,ref record);
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void t_p_n01_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_Dev_Type.DEV_TYPE_FLAG_RECORD_TEMPERATURE_T) != 0)
                {
                    int ret = 0;
                    int flag = Convert.ToInt32(t_p_01.Text.ToString());
                    
                    ret = AnvizNew.CChex_GetTRecordNumberByType(anviz_handle, dev_idx, flag);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_Dev_Type.DEV_TYPE_FLAG_RECORD_TEMPERATURE_T) != 0)
                {
                    int ret = 0;
                    int flag = Convert.ToInt32(t_p_02.Text.ToString());
                    
                    ret = AnvizNew.CChex_GetTRecordByType(anviz_handle, dev_idx, flag);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_Dev_Type.DEV_TYPE_FLAG_RECORD_TEMPERATURE_T) != 0)
                {
                    int ret = 0;
                    UInt64 id_temp = Convert.ToUInt64(t_p_03.Text.ToString());

                    AnvizNew.CCHEX_PICTURE_BY_RECORD_ID_STRU Data;
                    Data.TemperatureType = 10;
                    Data.RecoradId = new byte[8];
                    Data.RecoradId[0] = (byte)((id_temp >> 56) & 0xff);
                    Data.RecoradId[1] = (byte)((id_temp >> 48) & 0xff);
                    Data.RecoradId[2] = (byte)((id_temp >> 40) & 0xff);
                    Data.RecoradId[3] = (byte)((id_temp >> 32) & 0xff);
                    Data.RecoradId[4] = (byte)((id_temp >> 24) & 0xff);
                    Data.RecoradId[5] = (byte)((id_temp >> 16) & 0xff);
                    Data.RecoradId[6] = (byte)((id_temp >> 8) & 0xff);
                    Data.RecoradId[7] = (byte)((id_temp >> 0) & 0xff);
                    string info_buff = "byte ----[ 0:" + Data.RecoradId[0]
                                            + " 1:" + Data.RecoradId[1]
                                            + "2: " + Data.RecoradId[2];
                    log_add_string(info_buff);

                    
                    ret = AnvizNew.CChex_GetPictureByTRecordIdType(anviz_handle, dev_idx, ref Data);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_Dev_Type.DEV_TYPE_FLAG_RECORD_TEMPERATURE_T) != 0)
                {
                    int ret = 0;
                    UInt64 id_temp = Convert.ToUInt64(t_p_04.Text.ToString());
                    AnvizNew.CCHEX_PICTURE_BY_RECORD_ID_STRU Data;
                    Data.TemperatureType = 10;
                    Data.RecoradId = new byte[8];
                    Data.RecoradId[0] = (byte)((id_temp >> 56) & 0xff);
                    Data.RecoradId[1] = (byte)((id_temp >> 48) & 0xff);
                    Data.RecoradId[2] = (byte)((id_temp >> 40) & 0xff);
                    Data.RecoradId[3] = (byte)((id_temp >> 32) & 0xff);
                    Data.RecoradId[4] = (byte)((id_temp >> 24) & 0xff);
                    Data.RecoradId[5] = (byte)((id_temp >> 16) & 0xff);
                    Data.RecoradId[6] = (byte)((id_temp >> 8) & 0xff);
                    Data.RecoradId[7] = (byte)((id_temp >> 0) & 0xff);

                    string info_buff = "byte ----[ 0:" + Data.RecoradId[0]
                                            + " 1:" + Data.RecoradId[1]
                                            + "2: " + Data.RecoradId[2];
                    log_add_string(info_buff);

                    
                    ret = AnvizNew.CChex_DelPictureByTRecordIdType(anviz_handle, dev_idx, ref Data);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pic_face_btn_dl_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_PICTURE) != 0)
                {
                    int ret = 0;
                    AnvizNew.CCHEX_DEL_PERSON_INFO_STRU Data;

                    Data.EmployeeId = new byte[5];
                    string_to_byte(this.employee_face_PId.Text, Data.EmployeeId, 5);
                    Data.operation = 11;
                    

                    ret = AnvizNew.CChex_DownloadFacePictureModule(anviz_handle, dev_idx, ref Data);
                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void pic_face_btn_ul_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_PICTURE) != 0)
                {
                    int ret = 0;
                    AnvizNew.CCHEX_DEL_PERSON_INFO_STRU Data;

                    Data.EmployeeId = new byte[5];
                    string_to_byte(this.upload_face_PId.Text, Data.EmployeeId, 5);
                    Data.operation = 11;
                    
                    byte[] Buff;
                    int Len = 0;
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.Title = "Choose File";
                    dialog.Filter = "所有文件(*.*)|*.*";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string file = dialog.FileName;
                        FileStream fs = new FileStream(file, FileMode.Open);
                        Len = (int)fs.Length;
                        Buff = new byte[Len];
                        fs.Read(Buff, 0, Buff.Length);
                        fs.Close();
                        ret = AnvizNew.CChex_UploadFacePictureModule(anviz_handle, dev_idx, ref Data, Buff, Len);
                    }

                }
            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pic_face_fp_btn_ul_Click(object sender, EventArgs e)
        {
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_PICTURE) != 0)
                {
                    if (len_fP_picture > 0)
                    {
                        int ret = 0;
                        AnvizNew.CCHEX_DEL_PERSON_INFO_STRU Data;

                        Data.EmployeeId = new byte[5];
                        string_to_byte(this.upload_face_PId.Text, Data.EmployeeId, 5);
                        Data.operation = 11;
                        

                        ret = AnvizNew.CChex_UploadFacePictureModule(anviz_handle, dev_idx, ref Data, buff_fP_picture, len_fP_picture);
                    }
                    else
                    {
                        MessageBox.Show("Please download FP_picture");
                    }
                }

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label174_Click(object sender, EventArgs e)
        {

        }

        private void button29_Click(object sender, EventArgs e)
        {
            ;
            if (listViewDevice.SelectedItems.Count != 0)
            {
                int ret = 0;
                int dev_idx = Convert.ToInt32(listViewDevice.SelectedItems[0].SubItems[1].Text.ToString());
                if ((DevTypeFlag[dev_idx] & (int)AnvizNew.MODE_FP_Type.DEV_TYPE_FLAG_FACEPASS_PICTURE) != 0)
                {
                    AnvizNew.CCHEX_ADD_FINGERPRINT_ONLINE_STRU_EXT_INF datainfo;
                    datainfo.EmployeeId = new byte[5];//1B 8063 72E6
                    string_to_byte(e_add_fp_pic.Text, datainfo.EmployeeId, 5);
                    datainfo.BackupNum = 1;
                    ret = AnvizNew.CCHex_AddFingerprintOnline_FacePicture(anviz_handle, dev_idx, ref datainfo);
                }

            }
            else
            {
                MessageBox.Show("Please select the device", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tm_stop_time_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
