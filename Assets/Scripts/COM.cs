using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class COM : MonoBehaviour
{
	public SerialPort serialPort = new SerialPort();

	[HideInInspector] public string[] graphMessage = { "a", "b", "b" };
	[HideInInspector] public string[] importMessage = { "a", "b", "d" };
	[HideInInspector] public string[] writeMessage = { "a", "b", "u" };
	[HideInInspector] public string[] channelSwitcher = { "b", "e", "p", "0", "1", "2", "3", "4" };

	public float updateGraphEverySeconds = 1f;
	public float currentGraphTime = 0f;
	[HideInInspector] public bool benchmarkNeedsReset = true;

	// channelData[0][0] - Array T12 and Channel 1
	// channelData[1][0] - Array T3 and Channel 1
	public string[,] channelData = new string[2, 8];

	public bool hasConnected = false;
	public StatusManager statusManager;

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;

	[SerializeField] Dropdown portName = null;
	[SerializeField] CanvasGroup T12 = null;
	[SerializeField] Graph graph = null;

	private string readMessage = string.Empty;
	public bool isDataRead = false;
	public int currentMessage = 0;

	public string message;

	private List<string> portNames = new List<string>();

	private List<string> benchmarkValues = new List<string>();

	private Main main = null;

	private Coroutine readCoroutine = null;

	public void ConnectButton()
	{
		if (hasConnected)
		{
			COMDisconnect();
		}
		else
		{
			SaveSettings();
		}
	}

	public void SaveSettings()
	{
		serialPort.PortName = portNames[portName.value];
		serialPort.BaudRate = 1000000;
		serialPort.DataBits = 8;
		serialPort.Parity = Parity.None;
		serialPort.StopBits = StopBits.One;

		OpenPort();
	}

	public void ClosePort()
	{
		serialPort.Close();
		hasConnected = false;
		//DisableBenchmark();
	}

	public void OpenPort()
	{
		try
		{
			serialPort.Open();
			hasConnected = true;
			statusManager.statusText.text = "Opening " + serialPort.PortName + " port.";
		}
		catch (Exception ex)
		{
			Debug.Log("Error opening my port: " + ex.Message);
			statusManager.statusText.text = "Error oppening the " + serialPort.PortName + " port, maybe it's already opened?";
			return;
		}
		statusManager.statusText.text = $"Port {serialPort.PortName} sucessfully opened.";

		//EnableBenchmark();
		EnableDataRead();
	}

	public void COMDisconnect()
	{
		statusManager.statusText.text = "Disconnected from " + serialPort.PortName + " port.";
		ClosePort();
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
			OpenPort();
		}

		StartCoroutine(ReadNewData());
	}

	public IEnumerator ReadNewData()
	{
		ExitBenchmarkMode();
		yield return null;
		ReadNew();
	}

	private void ExitBenchmarkMode()
	{
		if (The.benchmarkRunning)
		{
			serialPort.Write("b");
			The.benchmarkRunning = false;
		}
	}

	public void GetValuesForGraph()
	{
		if (benchmarkNeedsReset)
		{
			currentMessage = 0;
			while (currentMessage < graphMessage.Length)
			{
				serialPort.Write(graphMessage[currentMessage]);
				currentMessage++;
			}
			currentMessage = 0;

			serialPort.ReadTo("$B,B");
			serialPort.ReadTo("\n");

			benchmarkNeedsReset = false;
		}

		//StartCoroutine("ReadSerialPort");

	}

	public IEnumerator ReadBenchmarkData()
	{
		int bytesToRead = 0;
		if (hasConnected)
		{
			while (true)
			{
				bytesToRead = serialPort.BytesToRead;
				if (bytesToRead > 0)
				{
					byte[] input = new byte[bytesToRead];
					serialPort.Read(input, 0, bytesToRead);

					message = System.Text.Encoding.UTF8.GetString(input);
					Debug.Log("reading");
				}
				bytesToRead = 0;
				yield return null;
			}
		}
	}

	public IEnumerator ReadSerialPort()
	{
		readMessage = serialPort.ReadTo("\n");
		Debug.Log(readMessage);
		yield return null;
	}

	public void EnableBenchmark()
	{
		currentGraphTime = Time.time;
		Debug.Log("Enabling benchmark");
		graph.Reset();
		serialPort.Write("b");
		The.benchmarkRunning = true;
	}

	public void EnableDataRead()
	{
		Debug.Log("Enabling Benchmark on device");
		if (serialPort.IsOpen)
		{
			Debug.Log("Starting benchmark data read");
			serialPort.Write("b");
			The.benchmarkRunning = true;
			readCoroutine = StartCoroutine(ReadBenchmarkData());
		}
	}

	public void DisableBenchmark()
	{
		if (readCoroutine != null)
		{
			StopCoroutine(readCoroutine);
			readCoroutine = null;
		}

		The.benchmarkRunning = false;
		serialPort.Write("b");
		//benchmarkNeedsReset = true;
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
			}
		}
		readMessage = string.Empty;
		isDataRead = true;
		The.main.RefreshArray(0, The.currentChannel);
		The.main.RefreshArray(1, The.currentChannel);

		EnableDataRead();
	}

	public void FillComPortNames()
	{
		portName.ClearOptions();

		foreach (string s in SerialPort.GetPortNames())
		{
			portNames.Add(s);
		}

		portName.AddOptions(portNames);
	}


	public void ReadComport()
	{
		if (!hasConnected)
		{
			ManualStart();
			return;
		}

		statusManager.statusText.text = "Reading data from the device...";
		ManualStart();


		statusManager.statusText.text = "Data reading from device has completed!";
		The.channelManager.SaveDataToFile(The.currentChannel);

	}
}
