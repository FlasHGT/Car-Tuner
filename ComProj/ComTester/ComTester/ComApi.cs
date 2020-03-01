using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComTester
{
	public class ComApi
	{
		public SerialPort serialPort = null;

		public ComApi(string port)
		{
			serialPort.PortName = "COM3";
			serialPort.BaudRate = 1000000;
			serialPort.DataBits = 8;
			serialPort.Parity = Parity.None;
			serialPort.StopBits = StopBits.One;

			serialPort.Open();

			serialPort.Handshake = Handshake.None;
			serialPort.DiscardInBuffer();
			serialPort.DiscardOutBuffer();
			serialPort.DtrEnable = true;

			new Thread(WorkerThread).Start();
		}

		private List<char> commandBuffer = new List<char>();
		private object listLocker = new object();

		public void SendCommand (char cmd, Action onResult)
		{
			lock (listLocker)
			{
				commandBuffer.Add(cmd);
			}
		}

		private void WorkerThread()
		{
			serialPort.DiscardInBuffer();

			while (true)
			{
				char? cmd = null;
				lock (listLocker)
				{
					if (commandBuffer.Count == 0)
						goto AfterCheck;

					cmd = commandBuffer[0];
					commandBuffer.RemoveAt(0);
				}

				serialPort.DiscardOutBuffer();
				serialPort.Write(new char[] { cmd.Value }, 0, 1);



				AfterCheck:
				Thread.Sleep(1);
			}
		}
	}
}
