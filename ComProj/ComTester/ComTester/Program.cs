using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComTester
{
	class Program
	{
		public static SerialPort serialPort = new SerialPort();

		static void Main(string[] args)
		{
			Console.WriteLine("wsup");

			var portNames = SerialPort.GetPortNames();
			for (int i = 0; i < portNames.Length; i++)
			{
				Console.WriteLine($"[{i}] Port {portNames[i]}");
			}
			int portIdx = -1;
			while (false == int.TryParse(Console.ReadLine(), out portIdx) || portIdx < 0 || portIdx >= portNames.Length) { Console.Write("Port idx: "); }

			string port = portNames[portIdx];

			serialPort.PortName = port;
			serialPort.BaudRate = 1000000;
			serialPort.DataBits = 8;
			serialPort.Parity = Parity.None;
			serialPort.StopBits = StopBits.One;

			try
			{
				serialPort.Open();
				Console.WriteLine("Opened port");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.ToString());
				return;
			}
		}
	}
}
