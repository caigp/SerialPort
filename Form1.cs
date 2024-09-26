using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsComApp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < portNames.Length; i++)
            {
                comboBox1.Items.Add(portNames[i]);
            }
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            if (portNames.Length < 1)
            {
                button1.Enabled = false;
            }

            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 2;

            comboBox3.Items.Add("0");
            comboBox3.Items.Add("1");
            comboBox3.Items.Add("2");
            comboBox3.Items.Add("3");
            comboBox3.Items.Add("4");
            comboBox3.SelectedIndex = 0;

            comboBox4.Items.Add("5");
            comboBox4.Items.Add("6");
            comboBox4.Items.Add("7");
            comboBox4.Items.Add("8");
            comboBox4.SelectedIndex = 3;

            comboBox5.Items.Add("0");
            comboBox5.Items.Add("1");
            comboBox5.Items.Add("2");
            comboBox5.Items.Add("3");
            comboBox5.SelectedIndex = 1;

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            serialPort.ReadBufferSize = 1024;

            textBox2.KeyPress += new KeyPressEventHandler(textBox2_KeyPress);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton1.Checked)
            {
                return;
            }

            // 检查是否按下了Control键（允许复制粘贴等操作）  
            if (char.IsControl(e.KeyChar))
            {
                return; // 如果是Control键，则不做处理  
            }

            // 检查是否是有效的十六进制字符  
            if (!char.IsDigit(e.KeyChar) && !(e.KeyChar >= 'A' && e.KeyChar <= 'F') && !(e.KeyChar >= 'a' && e.KeyChar <= 'f'))
            {
                e.Handled = true; // 阻止输入非十六进制字符  
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button click");

            if (serialPort.IsOpen)
            {
                button1.Text = "打开";

                serialPort.Close();

                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            } else
            {
                button1.Text = "关闭";

                serialPort.PortName = comboBox1.SelectedItem.ToString();
                serialPort.BaudRate = int.Parse(comboBox2.SelectedItem.ToString());

                Parity parity;
                string temp = comboBox3.SelectedItem.ToString();
                switch(temp)
                {
                    case "1":
                        parity = Parity.Odd;
                        break;
                    case "2":
                        parity = Parity.Even;
                        break;
                    case "3":
                        parity = Parity.Mark;
                        break;
                    case "4":
                        parity = Parity.Space;
                        break;
                    case "0":
                    default:
                        parity = Parity.None;
                        break;
                }

                serialPort.Parity = parity;
                serialPort.DataBits = int.Parse(comboBox4.SelectedItem.ToString());

                StopBits stopBits;
                temp = comboBox5.SelectedItem.ToString();
                switch (temp)
                {
                    case "1":
                        stopBits = StopBits.One;
                        break;
                    case "2":
                        stopBits = StopBits.Two;
                        break;
                    case "3":
                        stopBits = StopBits.OnePointFive;
                        break;
                    case "0":
                    default:
                        stopBits = StopBits.None;
                        break;
                }

                serialPort.StopBits = stopBits;
                serialPort.Open();

                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            //string data = serialPort.ReadExisting();
            string data;
            byte[] buffer = new byte[2048];
            int c = 0;

            int i = serialPort.BytesToRead;
            while (i > 0)
            {
                Console.WriteLine(i);
                c += serialPort.Read(buffer, c, buffer.Length - c);
                if (c >= buffer.Length)
                {
                    break;
                }
                System.Threading.Thread.Sleep(10);
                i = serialPort.BytesToRead;
            }

            if (radioButton2.Checked)
            {
                data = TextUtils.ByteArrayToHex(buffer, c);
            } else
            {
                data = Encoding.UTF8.GetString(buffer, 0, c);
            }

            textBox1.BeginInvoke(new MethodInvoker(delegate
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Text += "\r\n\r\n";
                }
                textBox1.Text += "[";
                textBox1.Text += DateTime.Now.ToString("HH:mm:ss");
                textBox1.Text += "]";
                textBox1.Text += data;

                textBox1.SelectionStart = textBox1.Text.Length; // 将光标移动到文本的末尾  
                textBox1.ScrollToCaret(); // 滚动到光标位置（即文本的末尾）
            }));
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string text = textBox2.Text;
            if (string.IsNullOrEmpty(text.Trim()))
            {
                return;
            }

            if (serialPort.IsOpen)
            {
                byte[] buffer;
                if (radioButton2.Checked)
                {
                    buffer = TextUtils.HexStringToByteArray(text);

                } else
                {
                    buffer = Encoding.UTF8.GetBytes(text);
                }
                serialPort.Write(buffer, 0, buffer.Length);
                textBox2.Clear();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
