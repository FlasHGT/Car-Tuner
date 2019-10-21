using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;

public class COM : MonoBehaviour
{
	public SerialPort serialPort = new SerialPort("COM4", 1000000, Parity.None, 8, StopBits.One);

	public string output = string.Empty;
	public string output2 = string.Empty;

	private string[] importMessage = {"a", "b", "d"};
	private string readMessage = string.Empty;
	private int currentMessage = 0;
	private int amountOfLinesNotToRead = 6;
	private bool hasReadFirstArray = false;
	private bool continueReading = true;

	public void ManualStart()
    {
		serialPort.ReadTimeout = 1000;
		serialPort.WriteTimeout = 1000;
		serialPort.Handshake = Handshake.None;

		if(!serialPort.IsOpen)
		{
			serialPort.Open();
		}

		while (currentMessage < importMessage.Length)
		{
			serialPort.Write(importMessage[currentMessage]);
			currentMessage++;
		}

		Read();
	}

	private void Read()
	{
		while (continueReading)
		{
			readMessage = serialPort.ReadLine();

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

				readMessage = serialPort.ReadLine();

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
				readMessage = readMessage.Replace("   " + x + " |   ", "");

				for (int y = 0; y < 10; y++)
				{
					if (readMessage.Substring(0, 2) == " " + y)
					{
						readMessage = readMessage.Remove(0, 1);
					}
				}
			}
			else
			{
				if(x == 9)
				{
					readMessage = readMessage.Replace("    " + x + " |   ", "");

					for (int y = 0; y < 10; y++)
					{
						if (readMessage.Substring(0, 2) == " " + y)
						{
							
							readMessage = readMessage.Remove(0, 1);
						}
					}
				}
				else
				{
					for (int y = 10; y < 17; y++)
					{
						if (readMessage.Contains("" + y))
						{
							readMessage = readMessage.Replace("    " + x + " |   ", "");
						}
					}

					readMessage = readMessage.Replace("    " + x + " |    ", "");
				}
			}

			if (x > 9)
			{
				readMessage = readMessage.Replace("    ", ",");

				for (int y = 0; y < 10; y++)
				{
					if (readMessage.Contains("" + y))
					{
						readMessage = readMessage.Replace(", ", ",");
					}
				}
			}
			else
			{
				if(x == 9)
				{
					readMessage = readMessage.Replace("    ", ",");

					for (int y = 0; y < 10; y++)
					{
						if (readMessage.Contains("" + y))
						{
							readMessage = readMessage.Replace(", ", ",");
						}
					}
				}
				else
				{
					readMessage = readMessage.Replace("     ", ",");

					for (int y = 10; y < 17; y++)
					{
						if (readMessage.Contains("" + y))
						{
							readMessage = readMessage.Replace("    ", ",");
						}
					}
				}
			}

			readMessage = readMessage.Replace(" ", ",");

			if (!hasReadFirstArray)
			{
				if(x != 15)
				{
					output += readMessage + "\n";
				}
				else
				{
					output += readMessage;
				}
			}
			else
			{
				if (x != 15)
				{
					output2 += readMessage + "\n";
				}
				else
				{
					output2 += readMessage;
				}
			}
	
			if (x != 15)
			{
				readMessage = serialPort.ReadLine();
			}
		}
		hasReadFirstArray = true;
	}
}
