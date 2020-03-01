using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComTesterForms
{
	public partial class Form1 : Form
	{
		public static SerialPort serialPort = new SerialPort();

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			serialPort.PortName = "COM3";
			serialPort.BaudRate = 1000000;
			serialPort.DataBits = 8;
			serialPort.Parity = Parity.None;
			serialPort.StopBits = StopBits.One;

			try
			{
				serialPort.Open();
				MessageBox.Show("Opened port");

				//serialPort.DiscardInBuffer();
				//serialPort.DiscardOutBuffer();
				//serialPort.BaseStream.Flush();

				serialPort.Handshake = Handshake.None;
				serialPort.DiscardInBuffer();
				serialPort.DiscardOutBuffer();
				serialPort.DtrEnable = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.ToString());
				return;
			}

			new Thread(read).Start();
			new Thread(write).Start();
		}

		private void read()
		{
			try
			{
				while (true)
				{
					//char c = (char)serialPort.ReadByte();
					string line = serialPort.ReadLine();
					///this.Invoke(() => { textBox1.Text += line + "\n"; });
					//this.Invoke(new Action<string>(AppendTextBox), new object[] {value});
					this.Invoke((MethodInvoker)delegate () {
						textBox1.Text += line + "\n";
						//textBox1.Text += c + "\n";
						textBox1.SelectionStart = textBox1.TextLength;
						textBox1.ScrollToCaret();
					});
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		private void write()
		{
			try
			{
				while (true)
				{
					if (commands.Count > 0 && lastReadCommand < commands.Count)
					{
						serialPort.Write(commands[lastReadCommand]);
						serialPort.BaseStream.Flush();
						lastReadCommand++;
					}
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private int lastReadCommand = 0;
		private List<string> commands = new List<string>();

		private void textBox3_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (string.IsNullOrEmpty(textBox3.Text) == false)
				{
					textBox2.Text += "\r\n" + textBox3.Text;
					textBox2.SelectionStart = textBox2.TextLength;
					textBox2.ScrollToCaret();
					commands.Add(textBox3.Text);
				}
			}
		}
	}
}
