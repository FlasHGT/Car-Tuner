using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class COM : MonoBehaviour
{
	private Main main = null;

	private static SerialPort serialPort = new SerialPort("COM3", 1000000, Parity.None, 8, StopBits.One);

	private string[] message = {"a", "b", "d"};
	private string readMessage = string.Empty;
	private string output = string.Empty;
	private string output2 = string.Empty;
	private int currentMessage = 0;
	private int amountOfLinesNotToRead = 6;
	private bool hasReadFirstArray = false;
	private bool continueReading = true;

	private void Start()
    {
		main = GetComponent<Main>();

		serialPort.ReadTimeout = 1000;
		serialPort.WriteTimeout = 1000;
		serialPort.Handshake = Handshake.None;

		serialPort.Open();

		while (currentMessage < message.Length)
		{
			serialPort.Write(message[currentMessage]);
			currentMessage++;
		}

		Read();
	}

	private void Read()
	{
		while (continueReading)
		{
			readMessage = serialPort.ReadLine();
			//Debug.Log(readMessage);

			if (readMessage == "> d")
			{
				for (int x = 0; x < amountOfLinesNotToRead; x++)
				{
					serialPort.ReadLine();
				}

				amountOfLinesNotToRead = 5;
			}
			else
			{
				if (amountOfLinesNotToRead == 5 && output == string.Empty)
				{
					ReadArray();
				}
			}

			if (output != string.Empty && output2 == string.Empty)
			{
				for (int x = 0; x < amountOfLinesNotToRead; x++)
				{
					serialPort.ReadLine();
				}

				ReadArray();
				continueReading = false;
			}
		}
	}

	private void ReadArray()
	{
		for (int x = 0; x < 16; x++)
		{
			if (x > 9)
			{
				readMessage = readMessage.Replace("   " + x + " |    ", "");
			}
			else
			{
				readMessage = readMessage.Replace("    " + x + " |    ", "");
			}

			readMessage = readMessage.Replace("     ", ",");
			readMessage = readMessage.Replace(" ", ","); // Read this and add to an array

			if(!hasReadFirstArray)
			{
				output += readMessage + "\n";
			}
			else
			{
				output2 += readMessage + "\n";
			}

			if(x != 15 && !hasReadFirstArray)
			{
				readMessage = serialPort.ReadLine();
			}
		}
		Debug.Log(output);
		Debug.Log(output2);
		hasReadFirstArray = true;
	}
}
