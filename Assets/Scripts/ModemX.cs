using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XModemProtocol;
using System.IO.Ports;
using System.IO;
using System.Linq;

public class ModemX : MonoBehaviour
{
    public void WriteModem()
	{
		var port = new SerialPort
		{
			BaudRate = 1000000,
			DataBits = 8,
			Parity = Parity.None,
			StopBits = StopBits.One,
			PortName = "COM3",
		};

		var xmodem = new XModemCommunicator();
		xmodem.Port = port;
		xmodem.Data = File.ReadAllBytes(@"C:\sample.csv");



		// Subscribe to events.
		xmodem.Completed += (s, e) => {
			Debug.Log($"Operation completed.\nPress enter to exit.");
		};
		xmodem.Aborted += (s, e) => {
			Debug.Log("Operation Aborted.\nPress enter to exit.");
		};

		port.Open();
		port.Write("u");
		xmodem.Send();

		if (xmodem.State != XModemStates.Idle)
		{
			xmodem.CancelOperation();
		}

		// Dispose of port.
		port.Close();
	}
}
