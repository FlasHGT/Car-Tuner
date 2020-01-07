using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class COM : MonoBehaviour
{
	public SerialPort serialPort = new SerialPort();

	[HideInInspector] public string[] graphMessage = { "a", "b", "b" };
	[HideInInspector] public string[] importMessage = { "a", "b", "d" };
	[HideInInspector] public string[] writeMessage = { "a", "b", "u" };
	[HideInInspector] public string[] channelSwitcher = { "b", "e", "p", "0", "1", "2", "3", "4" };

	public float updateGraphEverySeconds = 1f;
	public float currentGraphTime = 0f;
	[HideInInspector] public bool benchmarkRunning = false;
	[HideInInspector] public bool benchmarkNeedsReset = true;

	private int xValue = 0;
	private int yValue = 0;

	// channelData[0][0] - Array T12 and Channel 1
	// channelData[1][0] - Array T3 and Channel 1
	public string[,] channelData = new string[2, 8];

	public bool hasConnected = false;
	public StatusManager statusManager;

	[SerializeField] InputField currentSelected = null;

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;

	[SerializeField] Dropdown portName = null;
	[SerializeField] Text connectButtonText = null;
	[SerializeField] CanvasGroup T12 = null;
	[SerializeField] Graph graph = null;

	private string readMessage = string.Empty;
	public bool isDataRead = false;
	public int currentMessage = 0;

	private string xAndYOutput = string.Empty;
	private string graphStartValueOutput = string.Empty;
	private string graphEndValueOutput = string.Empty;
	private int graphOutputValueCount = 0;

	private List<string> portNames = new List<string>();

	private Main main = null;

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
		statusManager.statusText.text = "Port " + serialPort.PortName + " successfully opened.";
		
		GetValuesForGraph();
		benchmarkRunning = true;
		currentGraphTime = Time.time;
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

		ReadNew();
	}

	public void GetValuesForGraph()
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

		while (true)
		{
			readMessage = serialPort.ReadTo("\n");

			if (readMessage.Contains("$B,C,"))
			{
				readMessage = readMessage.Replace("$B,C,", "");

				foreach (char c in readMessage.ToCharArray())
				{
					if (c.ToString() == "," || c.ToString() == " ")
					{
						graphOutputValueCount++;

						if (graphOutputValueCount <= 4)
						{
							if (T12.alpha == 1 && graphOutputValueCount <= 2)
							{
								CalculateInputField(1);
							}
							else if (T12.alpha == 0 && graphOutputValueCount > 2)
							{
								CalculateInputField(3);
							}

							xAndYOutput = string.Empty;
						}
						else
						{
							break;
						}
					}
					else
					{
						xAndYOutput += c;
					}
				}

				xAndYOutput = string.Empty;
				graphOutputValueCount = 0;
			}

			if (readMessage.Contains("$B,D,"))
			{
				readMessage = readMessage.Replace("$B,D,", "");

				foreach (char c in readMessage.ToCharArray())
				{
					if (c.ToString() == ",")
					{
						graphOutputValueCount++;

						if (graphOutputValueCount <= 2)
						{
							if (T12.alpha == 1 && graphOutputValueCount == 1)
							{
								if (graph.startValue.Count >= graph.maxObjectsInList)
								{
									graph.startValue.RemoveAt(0);

								}

								graph.startValue.Add(float.Parse(graphStartValueOutput));
							}
							else if (T12.alpha == 0 && graphOutputValueCount == 2)
							{
								if (graph.startValue.Count >= graph.maxObjectsInList)
								{
									graph.startValue.RemoveAt(0);
								}

								graph.startValue.Add(float.Parse(graphStartValueOutput));
							}

							graphStartValueOutput = string.Empty;
						}
						else
						{
							break;
						}
					}
					else
					{
						graphStartValueOutput += c;
					}
				}

				graphStartValueOutput = string.Empty;
				graphOutputValueCount = 0;
			}

			if (readMessage.Contains("$B,A,"))
			{
				readMessage = readMessage.Replace("$B,A,", "");

				graphEndValueOutput = string.Empty;

				foreach (char c in readMessage.ToCharArray())
				{
					if (c.ToString() == ",")
					{
						if (T12.alpha == 1)
						{
							if (graph.endValue.Count >= graph.maxObjectsInList)
							{
								graph.endValue.RemoveAt(0);
							}

							graph.endValue.Add(float.Parse(graphEndValueOutput));

							return;
						}

						graphEndValueOutput = string.Empty;
					}
					else
					{
						graphEndValueOutput += c;
					}

					if (readMessage.IndexOf(c) == readMessage.Length - 1)
					{
						if (T12.alpha == 0)
						{
							if (graph.endValue.Count >= graph.maxObjectsInList)
							{
								graph.endValue.RemoveAt(0);
							}

							graph.endValue.Add(float.Parse(graphEndValueOutput));

							return;
						}

						graphEndValueOutput = string.Empty;
					}
				}
			}
		}
	}

	private void CalculateInputField(int valueToCompareTo)
	{
		float newValue = float.Parse(xAndYOutput);

		if (newValue >= 16) // Making sure that the value is not higher than 15, so the value is not higher than 255.
		{
			return;
		}

		Math.Floor(newValue);

		if (graphOutputValueCount == valueToCompareTo)
		{
			xValue = (int)newValue;
		}
		else
		{
			yValue = (int)newValue;

			main.CheckTheActiveDataTable();
			InputField currentInputField = main.currentDataTable[xValue + 16 * yValue];

			if (currentSelected != null)
			{
				currentSelected.GetComponent<Selectable>().GraphSelected();
			}

			currentSelected = currentInputField;
			currentSelected.GetComponent<Selectable>().GraphSelected();
		}
	}

	public void EnableBenchmark()
	{
		if (hasConnected)
		{
			graph.Reset();
			serialPort.Write("b");
			benchmarkRunning = true;
		}
	}

	public void DisableBenchmark()
	{
		if (hasConnected)
		{
			benchmarkRunning = false;
			serialPort.Write("b");
			benchmarkNeedsReset = true;
		}
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
	}

	private void Start ()
	{
		main = GetComponent<Main>();

		FillComPortNames();
	}

	private void Update()
	{
		if (hasConnected)
		{
			connectButtonText.text = "Disconnect";
		}
		else
		{
			connectButtonText.text = "Connect";
		}

		if (Time.time - currentGraphTime > updateGraphEverySeconds && benchmarkRunning)
		{
			GetValuesForGraph();
			currentGraphTime = Time.time;
		}
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

	private void OnApplicationQuit()
	{
		DisableBenchmark();
		serialPort.Close();
	}

}
