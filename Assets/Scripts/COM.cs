using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Collections.Generic;

public class COM : MonoBehaviour
{
	public SerialPort serialPort = new SerialPort();

	public bool hasConnected = false;
	public StatusManager statusManager;

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;

	[SerializeField] Dropdown portName = null;
	[SerializeField] Text connectButtonText = null;
	[SerializeField] Button connectButton = null;
	[SerializeField] Button[] channelButtons = null;

	private string[] importMessage = {"a", "b", "d"};
	private string readMessage = string.Empty;
	private int currentMessage = 0;
	private int amountOfLinesNotToRead = 6;
	private bool hasReadFirstArray = false;
	private bool continueReading = true;

	private List<string> portNames = new List<string>();

	public void SaveSettings()
	{
		serialPort.PortName = portNames[portName.value];
		serialPort.BaudRate = 1000000;
		serialPort.DataBits = 8;
		serialPort.Parity = Parity.None;
		serialPort.StopBits = StopBits.One;

		hasConnected = true;
		statusManager.statusText.text = "Sucessfuly connected to " + serialPort.PortName + " port.";
	}

	private void Update()
	{
		if (portName.options[portName.value].text.Equals(serialPort.PortName) && hasConnected)
		{
			connectButtonText.text = "Connected";
			connectButton.interactable = false;
		}
		else
		{
			connectButtonText.text = "Connect";
			connectButton.interactable = true;
		}
	}

	public void ChangeProfile (int i)
	{
		switch (i)
		{
			case 0:
				importMessage[1] = "b";
				channelButtons[0].interactable = false;
				ResetChannels(0);
				break;
			case 1:
				importMessage[1] = "e";
				channelButtons[1].interactable = false;
				ResetChannels(1);
				break;
			case 2:
				importMessage[1] = "p";
				channelButtons[2].interactable = false;
				ResetChannels(2);
				break;
			case 3:
				importMessage[1] = "0";
				channelButtons[3].interactable = false;
				ResetChannels(3);
				break;
			case 4:
				importMessage[1] = "1";
				channelButtons[4].interactable = false;
				ResetChannels(4);
				break;
			case 5:
				importMessage[1] = "2";
				channelButtons[5].interactable = false;
				ResetChannels(5);
				break;
			case 6:
				importMessage[1] = "3";
				channelButtons[6].interactable = false;
				ResetChannels(6);
				break;
			case 7:
				importMessage[1] = "4";
				channelButtons[7].interactable = false;
				ResetChannels(7);
				break;
			default:
				break;
		}
	}

	public void ManualStart()
	{
		if (!hasConnected)
		{
			statusManager.statusText.text = "Please connect the device and try again.";
			return;
		}

		if (!serialPort.IsOpen)
		{
			serialPort.Open();
		}

		while (currentMessage < importMessage.Length)
		{
			serialPort.Write(importMessage[currentMessage]);
			currentMessage++;
		}

		Reset();

		Read();
	}

	private void ResetChannels (int i)
	{
		for (int x = 0; x < channelButtons.Length; x++)
		{
			if (x != i)
			{
				channelButtons[x].interactable = true;
			}
		}
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
				if (x != 15)
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

	private void Reset()
	{
		amountOfLinesNotToRead = 6;
		currentMessage = 0;
		output = string.Empty;
		output2 = string.Empty;
		hasReadFirstArray = false;

		continueReading = true;
	}

	private void Start ()
	{
		FillComPortNames();
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
}
