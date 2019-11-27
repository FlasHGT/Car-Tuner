using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class COM : MonoBehaviour
{
	public SerialPort serialPort = new SerialPort();

	[HideInInspector] public string[] importMessage = { "a", "b", "d" };
	[HideInInspector] public string[] writeMessage = { "a", "b", "u" };
	[HideInInspector] public string[] channelSwitcher = { "b", "e", "p", "0", "1", "2", "3", "4" };
	
	// channelData[0][0] - Array T12 and Channel 1
	// channelData[1][0] - Array T3 and Channel 1
	public string[,] channelData = new string[2,8];

	public bool hasConnected = false;
	public StatusManager statusManager;

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;

	[SerializeField] Dropdown portName = null;
	[SerializeField] Text connectButtonText = null;
	[SerializeField] Button connectButton = null;

	private string readMessage = string.Empty;
	public bool isDataRead = false;
	public int currentMessage = 0;

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
			connectButtonText.text = "Disconnect";
			connectButton.onClick.AddListener(COMDisconnect);
		}
		else
		{
			connectButtonText.text = "Connect";
		}
	}

	public void COMDisconnect ()
	{
		serialPort.Close();
		statusManager.statusText.text = "Disconnected from " + serialPort.PortName + " port.";
		hasConnected = false;
		connectButton.onClick.AddListener(SaveSettings);
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

		ReadNew();
	}

	private void ReadNew()
	{
		currentMessage = 0;
		for (int i = 0; i < 8; i++)
		{
			importMessage[1] = channelSwitcher[i];
			while (currentMessage < importMessage.Length)
			{
				serialPort.Write(importMessage[currentMessage]);
				currentMessage++;
			}
			print(importMessage[0] + " " + importMessage[1] + " " + importMessage[2]);
			currentMessage = 0;

			for (int k = 0; k < 2; k++)
			{
				channelData[k, i] = string.Empty;
				The.arrayChangedLocally[k, i] = false;
				
				while (true)
				{
					readMessage = serialPort.ReadLine();
					if (readMessage.StartsWith("-"))
					{
						for (int x = 0; x < 16; x++)
						{
							readMessage = serialPort.ReadLine();
							channelData[k, i] += readMessage.Substring(readMessage.IndexOf('|') + 1) + "\n";
						}
						channelData[k, i] = Regex.Replace(channelData[k, i], @"\s+", " ");
						channelData[k, i] = channelData[k, i].Replace(' ', ',');
						channelData[k, i] = channelData[k, i].Substring(1);
						channelData[k, i] = channelData[k, i].Remove(channelData[k, i].Length - 1);
						break;
					}
				}
				print(channelData[k, i]);
			}
		}
		readMessage = string.Empty;
		isDataRead = true;
		The.main.RefreshArray(0, The.currentChannel);
		The.main.RefreshArray(1, The.currentChannel);
		serialPort.Close();
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
