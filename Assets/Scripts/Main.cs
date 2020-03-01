using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.EventSystems;
using XModemProtocol;
using System;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour
{
	public static float minInputFieldValue = 0f;
	public static float maxInputFieldValue = 5000f;

	public InputField mainInput;
	public int channelUpdated = 0;

	public InputField[] currentDataTable = null;

	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

	[SerializeField] CanvasGroup T12 = null;

	[SerializeField] GameObject editValuesPanel = null;

	[SerializeField] EventSystem eventSystem = null;

	private BaseEventData eventData = null;

	private COM com = null;

	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 15;
	private int lineCount = 0;

	private string output;


	private string xAndYOutput = string.Empty;
	private string graphStartValueOutput = string.Empty;
	private string graphEndValueOutput = string.Empty;
	private int graphOutputValueCount = 0;

	private int xValue = 0;
	private int yValue = 0;

	[SerializeField] Text connectButtonText = null;

	[SerializeField] InputField currentSelected = null;

	public void Deselect()
	{
		if (!editValuesPanel.activeInHierarchy)
		{
			Selectable.DeselectAll(eventData);
		}
	}

	public void CheckTheActiveDataTable()
	{
		if (T12.alpha == 1) // Improve this if more tabs get added
		{
			currentDataTable = dataTableX;
		}
		else
		{
			currentDataTable = dataTableY;
		}
	}

	public void Export(string exportPath, bool writingFromOneDataTable)
	{

		if (!writingFromOneDataTable)
		{
			for (int x = 0; x < dataTableX.Length; x++)
			{
				output += dataTableX[x].text + ",";

				if ((x % inputFieldOffset) == 0 && x != 0)
				{
					output += "\n";
					inputFieldOffset += 16;
				}
			}

			inputFieldOffset = 15;
			output += "\n";

			for (int x = 0; x < dataTableY.Length; x++)
			{
				output += dataTableY[x].text + ",";

				if ((x % inputFieldOffset) == 0 && x != 0 && x != 255)
				{
					output += "\n";
					inputFieldOffset += 16;
				}
			}
		}
		else
		{
			CheckTheActiveDataTable();

			for (int x = 0; x < currentDataTable.Length; x++)
			{
				output += currentDataTable[x].text + ",";

				if ((x % inputFieldOffset) == 0 && x != 0 && x != 255)
				{
					output += "\n";
					inputFieldOffset += 16;
				}
			}
		}

		File.WriteAllText(exportPath, output);

		Reset();
	}

	public void Import(string importPath)
	{
		reader = new StreamReader(importPath);

		while (reader.ReadLine() != null)
		{
			lineCount++;
		}

		reader.Close();

		if (lineCount == 33)
		{
			reader = new StreamReader(importPath);

			for (int x = 0; x < 16; x++)
			{
				string line = reader.ReadLine();

				foreach (char c in line.ToCharArray())
				{
					if (c.ToString() == "," || c.ToString() == " ")
					{
						dataTableX[inputFieldSpot].text = output;
						output = string.Empty;
						inputFieldSpot++;
					}
					else
					{
						output += c;
					}
				}
			}

			inputFieldSpot = 0;

			for (int x = 0; x < 17; x++)
			{
				string line = reader.ReadLine();

				if (line != string.Empty)
				{
					foreach (char c in line.ToCharArray())
					{
						if (c.ToString() == "," || c.ToString() == " ")
						{
							dataTableY[inputFieldSpot].text = output;
							output = string.Empty;
							inputFieldSpot++;
						}
						else
						{
							output += c;
						}
					}
				}
			}
			reader.Close();
		}

		Reset();
	}

	public void TryWritingToDevice()
	{
		Debug.Log("Trying to write to comport");
		StartCoroutine(DelayedWriteToComport());
	}

	IEnumerator DelayedWriteToComport()
	{
		StartCoroutine(com.DisableBenchmark());
		yield return new WaitUntil(() => !The.benchmarkRunning);
		Debug.Log("Writing to comport as benchmark is not running!");
		WriteComport();
		com.serialPort.DiscardOutBuffer();
	}

	public void WriteComport()
	{
		if (!com.serialPort.IsOpen)
		{
			com.statusManager.statusText.text = "Please connect the device and try again.";
			return;
		}


		com.statusManager.statusText.text = "Preparing data for transfer to device...";
		Reset();
		

		for (int x = 0; x < dataTableX.Length; x++)
		{
			output += dataTableX[x].text + ",";

			if ((x % inputFieldOffset) == 0 && x != 0)
			{
				output += "\n";
				inputFieldOffset += 16;
			}
		}

		inputFieldOffset = 15;
		output += "\n";

		for (int x = 0; x < dataTableY.Length; x++)
		{
			output += dataTableY[x].text + ",";

			if ((x % inputFieldOffset) == 0 && x != 0 && x != 255)
			{
				output += "\n";
				inputFieldOffset += 16;
			}
		}
		
		byte[] bytes = Encoding.ASCII.GetBytes(output);

		var xmodem = new XModemCommunicator();
		xmodem.Port = com.serialPort;
		xmodem.Data = bytes;

		// Subscribe to events.
		xmodem.Completed += (s, e) => {
			Debug.Log($"Operation completed.\n");
			com.statusManager.statusText.text = "Writing data to device has successfully completed!";
		};
		xmodem.Aborted += (s, e) => {
			Debug.Log("Operation Aborted.\n");
			com.statusManager.statusText.text = "Writing data to device has failed!";
		};

		com.writeMessages[1] = com.channelSwitchers[The.currentChannel];
		com.currentMessage = 0;
		
		while (com.currentMessage < com.writeMessages.Length)
		{
			com.serialPort.Write(com.writeMessages[com.currentMessage]);
			com.currentMessage++;
		}
		
		
		xmodem.Send();

		if (xmodem.State != XModemStates.Idle)
		{
			xmodem.CancelOperation();
		}

		//com.EnableDataRead();
		Reset();
		com.EnableDataRead();
	}

	public void RefreshArray(int array, int channel)
	{
		// array 0 - T12
		// array 1 - T3
		// channels 0-8

		if (array == 0)
		{
			string[] values = com.channelData[array, channel].Split(',');
			foreach (string value in values)
			{
				dataTableX[inputFieldSpot].text = value;
				inputFieldSpot++;
			}
			channelUpdated = channel;
		}
		if (array == 1)
		{
			string[] values = com.channelData[array, channel].Split(',');
			foreach (string value in values)
			{
				dataTableY[inputFieldSpot].text = value;
				inputFieldSpot++;
			}
			channelUpdated = channel;
		}

		Reset();
	}

	

	private void Awake()
	{
		The.main = this;
	}

	private void Start()
	{
		com = GetComponent<COM>();
		com.FillComPortNames();
	}

	private void Update()
	{
		if (The.currentChannel != channelUpdated && com.hasConnected && com.isDataRead)
		{
			if (!The.arrayChangedLocally[The.currentArray, The.currentChannel])
			{
				RefreshArray(The.currentArray, The.currentChannel);
			}
				
		}

		if (Selectable.currentlySelected.Count >= 2)
		{
			eventSystem.sendNavigationEvents = false;
		}
		else
		{
			eventSystem.sendNavigationEvents = true;
		}

		if (com.hasConnected)
		{
			connectButtonText.text = "Disconnect";
		}
		else
		{
			connectButtonText.text = "Connect";
		}
	}

	private void Reset()
	{
		output = "";
		inputFieldOffset = 15;
		inputFieldSpot = 0;
		lineCount = 0;
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

			CheckTheActiveDataTable();
			InputField currentInputField = currentDataTable[xValue + 16 * yValue];

			if (currentSelected != null)
			{
				currentSelected.GetComponent<Selectable>().GraphSelected();
			}

			currentSelected = currentInputField;
			currentSelected.GetComponent<Selectable>().GraphSelected();
		}
	}

	public void GetValuesForGraph()
	{
		while (true)
		{
			if (com.message.Contains("$B,C,"))
			{
				com.message = com.message.Replace("$B,C,", "");

				foreach (char c in com.message.ToCharArray())
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

			if (com.message.Contains("$B,D,"))
			{
				com.message = com.message.Replace("$B,D,", "");

				foreach (char c in com.message.ToCharArray())
				{
					if (c.ToString() == ",")
					{
						graphOutputValueCount++;

						if (graphOutputValueCount <= 2)
						{
							if (T12.alpha == 1 && graphOutputValueCount == 1)
							{
								if (The.graph.startValue.Count >= The.graph.maxObjectsInList)
								{
									The.graph.startValue.RemoveAt(0);

								}

								The.graph.startValue.Add(float.Parse(graphStartValueOutput));
							}
							else if (T12.alpha == 0 && graphOutputValueCount == 2)
							{
								if (The.graph.startValue.Count >= The.graph.maxObjectsInList)
								{
									The.graph.startValue.RemoveAt(0);
								}

								The.graph.startValue.Add(float.Parse(graphStartValueOutput));
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

			if (com.message.Contains("$B,A,"))
			{
				com.message = com.message.Replace("$B,A,", "");

				graphEndValueOutput = string.Empty;

				foreach (char c in com.message.ToCharArray())
				{
					if (c.ToString() == ",")
					{
						if (T12.alpha == 1)
						{
							if (The.graph.endValue.Count >= The.graph.maxObjectsInList)
							{
								The.graph.endValue.RemoveAt(0);
							}

							The.graph.endValue.Add(float.Parse(graphEndValueOutput));

							return;
						}

						graphEndValueOutput = string.Empty;
					}
					else
					{
						graphEndValueOutput += c;
					}

					if (com.message.IndexOf(c) == com.message.Length - 1)
					{
						if (T12.alpha == 0)
						{
							if (The.graph.endValue.Count >= The.graph.maxObjectsInList)
							{
								The.graph.endValue.RemoveAt(0);
							}

							The.graph.endValue.Add(float.Parse(graphEndValueOutput));

							return;
						}

						graphEndValueOutput = string.Empty;
					}
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		//DisableBenchmark();
		com.serialPort.Close();
	}
}
