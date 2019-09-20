using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class COM : MonoBehaviour
{
	private SerialPort serialPort = new SerialPort("COM3", 1000000, Parity.None, 8, StopBits.One);

	float x = 2;

    // Start is called before the first frame update
    void Start()
    {
		serialPort.Open();

		Thread sampleThread = new Thread(new ThreadStart(sampleFunction));
		sampleThread.IsBackground = true;
		// start thread
		sampleThread.Start();

		//serialPort.Open();
		//serialPort.ReadTimeout = 5000;

		//try
		//{
		//	Debug.Log(serialPort.ReadChar());
		//	// do other stuff with the data
		//}
		//catch (TimeoutException e)
		//{
		//	Debug.Log(e);
		//	// no-op, just to silence the timeouts. 
		//	// (my arduino sends 12-16 byte packets every 0.1 secs)
		//}
	}

	public void sampleFunction()
	{
		Debug.Log(serialPort.ReadLine());
	}
}
