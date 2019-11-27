using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.EventSystems;
using XModemProtocol;

public class Main : MonoBehaviour
{
	public InputField mainInput;
	public int channelUpdated = 0;

	[SerializeField] GameObject editValuesPanel = null;

	[SerializeField] ChannelManager[] channelManagers = null;

	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

	[SerializeField] CanvasGroup T12 = null;

	[SerializeField] EventSystem eventSystem = null;

	private InputField[] currentDataTable = null;

	private BaseEventData eventData = null;

	private COM com = null;

	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 15;
	private int lineCount = 0;

	private string output;

	public void Deselect()
	{
		if (!editValuesPanel.activeInHierarchy)
		{
			Selectable.DeselectAll(eventData);
		}
	}

	public void Export(string exportPath, bool writingFromOneDataTable)
	{
		if(!writingFromOneDataTable)
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
		else
		{
			CheckTheActiveDataTable();

			reader = new StreamReader(importPath);

			for (int x = 0; x < 16; x++)
			{
				string line = reader.ReadLine();

				foreach (char c in line.ToCharArray())
				{
					if (c.ToString() == "," || c.ToString() == " ")
					{
						currentDataTable[inputFieldSpot].text = output;
						output = string.Empty;
						inputFieldSpot++;
					}
					else
					{
						output += c;
					}
				}
			}
			reader.Close();
		}

		Reset();
	}

	public void WriteComport()
	{
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

		if (!com.serialPort.IsOpen)
		{
			com.serialPort.Open();
		}
		

		com.writeMessage[1] = com.channelSwitcher[The.currentChannel];
		com.currentMessage = 0;
		while (com.currentMessage < com.writeMessage.Length)
		{
			com.serialPort.Write(com.writeMessage[com.currentMessage]);
			com.currentMessage++;
		}
		xmodem.Send();

		if (xmodem.State != XModemStates.Idle)
		{
			xmodem.CancelOperation();
		}

		// Dispose of port.
		com.serialPort.Close();
		Reset();
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

	public void ReadComport()
	{
		if (!com.hasConnected)
		{
			com.ManualStart();
			return;
		}

		com.statusManager.statusText.text = "Reading data from the device...";
		com.ManualStart();

		
		com.statusManager.statusText.text = "Data reading from device has completed!";
		com.serialPort.Close();
		channelManagers[0].SaveDataToFile(channelManagers[0].currentActiveChannel);		
	}

	private void CheckTheActiveDataTable()
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

	private void Awake()
	{
		The.main = this;
	}

	private void Start()
	{
		com = GetComponent<COM>();
		
	}

	private void Update()
	{
		if (The.currentChannel != channelUpdated && com.hasConnected && com.isDataRead)
		{
			if (!The.arrayChangedLocally[The.currentArray, The.currentChannel])
			{
				print("dude, wtf");
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
	}

	private void Reset()
	{
		output = "";
		inputFieldOffset = 15;
		inputFieldSpot = 0;
		lineCount = 0;
	}
}
