using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Collections.Generic;

public class COM : MonoBehaviour
{
	//public SerialPort serialPort = new SerialPort("COM4", 1000000, Parity.None, 8, StopBits.One);
	public SerialPort serialPort = new SerialPort();

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;

	[SerializeField] InputField baudRate = null;
	[SerializeField] InputField dataBits = null;
	[SerializeField] Dropdown portName = null;
	[SerializeField] Dropdown parity = null;
	[SerializeField] Dropdown stopBits = null;

	private string[] importMessage = {"a", "b", "d"};
	private string readMessage = string.Empty;
	private int currentMessage = 0;
	private int amountOfLinesNotToRead = 6;
	private bool hasReadFirstArray = false;
	private bool continueReading = true;

	private List<string> portNames = new List<string>();
	private List<string> parityOptions = new List<string>() { Parity.Even.ToString(), Parity.Mark.ToString(), Parity.None.ToString(), Parity.Odd.ToString(), Parity.Space.ToString() };
	private List<string> stopBitOptions = new List<string>() { StopBits.None.ToString(), StopBits.One.ToString(), StopBits.OnePointFive.ToString(), StopBits.Two.ToString() };

	public void SaveSettings()
	{
		serialPort.PortName = portNames[portName.value];
		serialPort.BaudRate = int.Parse(baudRate.text);
		serialPort.DataBits = int.Parse(dataBits.text);
		SetParity();
		SetStopBits();
		Debug.Log(serialPort.PortName);
		Debug.Log(serialPort.BaudRate);
		Debug.Log(serialPort.DataBits);
		Debug.Log(serialPort.Parity);
		Debug.Log(serialPort.StopBits);
	}

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

	private void Start ()
	{
		FillComPortNames();
		FillParityOptions();
		FillStopBitOptions();
	}

	private void FillComPortNames ()
	{
		portName.ClearOptions();

		foreach (string s in SerialPort.GetPortNames())
		{
			portNames.Add(s);
		}

		portName.AddOptions(portNames);
	}

	private void FillParityOptions ()
	{
		parity.ClearOptions();
		parity.AddOptions(parityOptions);
	}

	private void FillStopBitOptions ()
	{
		stopBits.ClearOptions();
		stopBits.AddOptions(stopBitOptions);
	}

	private void SetParity ()
	{
		switch (parity.value)
		{
			case 0:
				serialPort.Parity = Parity.Even;
				break;
			case 1:
				serialPort.Parity = Parity.Mark;
				break;
			case 2:
				serialPort.Parity = Parity.None;
				break;
			case 3:
				serialPort.Parity = Parity.Odd;
				break;
			case 4:
				serialPort.Parity = Parity.Space;
				break;
			default:
				break;
		}
	}

	private void SetStopBits ()
	{
		switch (stopBits.value)
		{
			case 0:
				serialPort.StopBits = StopBits.None;
				break;
			case 1:
				serialPort.StopBits = StopBits.One;
				break;
			case 2:
				serialPort.StopBits = StopBits.OnePointFive;
				break;
			case 3:
				serialPort.StopBits = StopBits.Two;
				break;
			default:
				break;
		}
	}
}
