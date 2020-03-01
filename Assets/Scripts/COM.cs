using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;

public class COM : MonoBehaviour
{
	public GraphChart graphChart;

	public SerialPort serialPort = new SerialPort();

	[HideInInspector] public string[] importMessages = {"a","b","d"};
	[HideInInspector] public string[] writeMessages = { "a", "b", "u" };
	[HideInInspector] public string[] channelSwitchers = { "b", "e", "p", "0", "1", "2", "3", "4" };

	[HideInInspector] public char[] startBenchmark = { 'b' };

	public float updateGraphEverySeconds = 1f;
	public float currentGraphTime = 0f;
	private bool benchmarkNeedsReset = false;

	private float tempBAvalue = 0f;

	// channelData[0][0] - Array T12 and Channel 1
	// channelData[1][0] - Array T3 and Channel 1
	public string[,] channelData = new string[2, 8];

	public bool hasConnected = false;
	public StatusManager statusManager;

	[HideInInspector] public string output = string.Empty;
	[HideInInspector] public string output2 = string.Empty;
	[SerializeField] Dropdown portName = null;

	private string readMessage = string.Empty;
	public bool isDataRead = false;
	public int currentMessage = 0;

	public string message;

	private List<string> portNames = new List<string>();

	private Coroutine readCoroutine = null;

	private string graphEndValueOutput = string.Empty;
	private bool messageSent = false;

	private int bdX = 0;
	private int baX = 0;

	private List<float> dataValuesBA = new List<float>();

	private Thread readingThread;
	private Thread threadStatus;

	public void ThreadStatus()
	{
		string data = serialPort.ReadLine();
		Debug.Log("Data is" + data);
	}

	//private int dataCount = 0;
	//private long secondStart = 0;

	private long nextDataTicks = 0;
	private static readonly long graphFps = TimeSpan.TicksPerSecond / 15;

	public void ThreadReadData()
	{
		try
		{	
			while (true)
			{
				if (benchmarkNeedsReset)
				{
					readingThread = null;
					return;
				}

				long ticks = DateTime.UtcNow.Ticks;
				bool skip = nextDataTicks > ticks;

				string s = serialPort.ReadTo("\n");
				Debug.LogWarning(s);

				if (skip) continue;
				else nextDataTicks = ticks + graphFps;

				//continue;
				if (s.Contains("$B,A,"))
				{
					s = s.Replace("$B,A,", "");

					string temp = string.Empty;

					foreach (char c in s.ToCharArray())
					{
						if (c == ',')
						{
							float.TryParse(temp, out tempBAvalue);
							tempBAvalue = tempBAvalue / 1000;
							Debug.Log("BA " + tempBAvalue);
							graphChart.DataSource.AddPointToCategoryRealtime("BA", baX, tempBAvalue);
							//Debug.Log("Temp BA value:" + tempBAvalue);
							baX++;
							//return;
							break;
						}
						else
						{
							temp += c;
						}
					}
				}
				else if (s.Contains("$B,D,"))
				{
					s = s.Replace("$B,D,", "");

					string temp = string.Empty;

					foreach (char c in s.ToCharArray())
					{
						if (c == ',')
						{
							float tempBDvalue;
							float.TryParse(temp, out tempBDvalue);
							tempBDvalue = tempBDvalue / 1000;
							Debug.Log("BD " + tempBDvalue);
							graphChart.DataSource.AddPointToCategoryRealtime("BD", baX, tempBAvalue - tempBDvalue);
							//Debug.Log("Temp BD value:" + tempBDvalue);
							baX++;
							//return;
							break;
						}
						else
						{
							temp += c;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

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
		//serialPort.ReadTimeout = 500;

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

		EnableDataRead();
		//StartCoroutine(CheckDeviceStatus());

	}

	IEnumerator CheckDeviceStatus()
	{
		Debug.Log("Check device status");
		while (true)
		{
			if (serialPort.BytesToRead > 0)
			{
				try
				{
					string data = serialPort.ReadLine();
					Debug.Log("Data: " + data);

					if(data.Contains("$B"))
					{
						Debug.Log("DEVICE STATUS: Device in benchmark mode!");
					}
					if (data.Contains("Available"))
					{
						Debug.Log("DEVICE STATUS: Device in commands menu!");
					}
					if (data.Contains("tag:"))
					{
						Debug.Log("DEVICE STATUS: Device is version menu!");
					}

				}
				catch (TimeoutException e)
				{

				}

				yield return new WaitForSeconds(0.1f);
			}
			yield return null;
		}
		
	}



	public void COMDisconnect()
	{
		StartCoroutine(DisableBenchmark());
		StartCoroutine(CloseSerialPort());
	}

	IEnumerator CloseSerialPort()
	{

		yield return new WaitUntil(() => !The.benchmarkRunning);
		serialPort.Close();
		hasConnected = false;
		statusManager.statusText.text = "Disconnected from " + serialPort.PortName + " port.";
	}

	public IEnumerator ReadNewData()
	{
		//while (true)
		//{
		//	int bytesToRead;
		//	bytesToRead = serialPort.BytesToRead;
		//	if (bytesToRead > 0)
		//	{
		//		Debug.LogError("Device still sending DATA!!");
		//		serialPort.DiscardInBuffer();

		//		if (bytesToRead > 0)
		//		{
		//			Debug.LogError("Device still still still still sending DATA!!");
		//			serialPort.Write("b");
		//		}
		//		else
		//		{
		//			break;
		//		}
		//	}
		//	else
		//	{
		//		break;
		//	}

		//	yield return null;
		//}
		ClearSerialPortBuffer();
		yield return null;
		//ReadNew();

	}

	public string RemoveBefore(string value, string character)
	{
		int index = value.IndexOf(character);
		if (index > 0)
		{
			value = value.Substring(index + 1);
		}
		return value;
	}

	public string RemoveAfter(string value, string character)
	{
		int index = value.IndexOf(character);
		if (index > 0)
		{
			value = value.Substring(0, index);
		}
		return value;
	}

	public void GetValuesForGraph(string someMessage)
	{	
  
		if (someMessage.Contains("$B,A,"))
		{
			someMessage = someMessage.Replace("$B,A,", "");

			graphEndValueOutput = string.Empty;

			foreach (char c in someMessage.ToCharArray())
			{
				if (c == ',')
				{
					float.TryParse(graphEndValueOutput, out tempBAvalue);
					tempBAvalue = tempBAvalue / 1000;
					graphChart.DataSource.AddPointToCategoryRealtime("BA", baX, tempBAvalue);
					Debug.Log("Temp BA value:" + tempBAvalue);
					baX++;
					return;
				}
				else
				{
					graphEndValueOutput += c;
				}
			}
		}
		else if (someMessage.Contains("$B,D,"))
		{
			someMessage = someMessage.Replace("$B,D,", "");

			graphEndValueOutput = string.Empty;

			foreach (char c in someMessage.ToCharArray())
			{
				if (c == ',')
				{
					float tempBDvalue;
					float.TryParse(graphEndValueOutput, out tempBDvalue);
					tempBDvalue = tempBDvalue / 1000;
					graphChart.DataSource.AddPointToCategoryRealtime("BD", bdX, tempBAvalue - tempBDvalue);
					bdX++;
					return;
				}
				else
				{
					graphEndValueOutput += c;
				}
			}
		}
		serialPort.BaseStream.Flush();
	}

	public IEnumerator ReadBenchmarkData()
	{
		Debug.Log("Starting data read thread.");
		readingThread = new Thread(ThreadReadData);
		readingThread.Start();
		//readingThread.IsBackground = true;
		//int i = 0;			
		//GetValuesForGraph(s);
		
		//Debug.Log(s);
		//ClearSerialPortBuffer();
		yield return null;	
		//i++;
	}

	public void ClearSerialPortBuffer()
	{
		serialPort.BaseStream.Flush();
		serialPort.DiscardInBuffer();
		serialPort.DiscardOutBuffer();
	}

	public void EnableDataRead()
	{

		string temp = string.Empty;
		byte[] buffer = new byte[1000];
		serialPort.ReadTimeout = 500;
		try
		{
			serialPort.Read(buffer, 0, 1000);
		}
		catch
		{
			Debug.Log("Timeout has occured");
		}

		Debug.Log("Do I continue?");
		if (buffer != null)
		{
			temp = String.Join("", buffer);
		}
		serialPort.ReadTimeout = -1;

		//string temp = serialPort.ReadExisting();
		Debug.Log("Temp value is: " + temp);
		while (!temp.Equals(string.Empty))
		{
			temp = serialPort.ReadExisting();
			Debug.Log("Temp value is: " + temp);
			if (temp.Contains("$B"))
			{
				Debug.Log("Benchmark already running!");
				The.benchmarkRunning = true;
				if (readCoroutine == null)
				{
					readCoroutine = StartCoroutine(ReadBenchmarkData());
					return;
				}
			}
		}

		Debug.Log("Enabling Benchmark on device");
		if (serialPort.IsOpen && !The.benchmarkRunning)
		{
			Debug.Log("Starting benchmark data read");
			ClearSerialPortBuffer();
			serialPort.Write(startBenchmark, 0, startBenchmark.Length);
			The.benchmarkRunning = true;
			readCoroutine = StartCoroutine(ReadBenchmarkData());
		}
	}

	public IEnumerator DisableBenchmark()
	{
		if (The.benchmarkRunning)
		{
			bool checkEnd = false;
			string temp = string.Empty;
			benchmarkNeedsReset = true;
			yield return new WaitUntil(() => readingThread == null);
			serialPort.DiscardOutBuffer();
			serialPort.Write(startBenchmark, 0, startBenchmark.Length);
			temp = serialPort.ReadExisting();
			while (!temp.Equals(string.Empty))
			{
				temp = serialPort.ReadExisting();
				Debug.Log("Still checking");
			}
			Debug.Log("Theres no data coming in!");			
			The.benchmarkRunning = false;
			benchmarkNeedsReset = false;
			Debug.Log(The.benchmarkRunning);
		}

	}

	IEnumerator DataDumpWrite(int i)
	{
		currentMessage = 0;
		importMessages[1] = channelSwitchers[i];
		while (currentMessage < importMessages.Length)
		{
			Debug.Log(importMessages[currentMessage]);
			serialPort.Write(importMessages[currentMessage]);
			yield return null;
			currentMessage++;
		}
		messageSent = true;
	}

	IEnumerator ReadNew()
	{
		
		for (int i = 0; i < 8; i++)
		{
			StartCoroutine(DataDumpWrite(i));
			serialPort.BaseStream.Flush();
			serialPort.DiscardInBuffer();
			yield return new WaitUntil(() => messageSent);
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
			messageSent = false;
		}
		readMessage = string.Empty;
		isDataRead = true;
		The.main.RefreshArray(0, The.currentChannel);
		The.main.RefreshArray(1, The.currentChannel);
		yield return null;
		EnableDataRead();
	}
	private IEnumerator ReadNewSequence()
	{
		StartCoroutine(DisableBenchmark());
		yield return new WaitUntil(() => !The.benchmarkRunning);
		StartCoroutine(ReadNew());
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
			statusManager.statusText.text = "Please connect the device and try again.";
			return;
		}
		//DisableBenchmark();

		statusManager.statusText.text = "Reading data from the device...";
		//ReadNew();
		StartCoroutine(ReadNewSequence());


		statusManager.statusText.text = "Data reading from device has completed!";
		The.channelManager.SaveDataToFile(The.currentChannel);

	}
}
