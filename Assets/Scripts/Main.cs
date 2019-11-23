using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.EventSystems;
using XModemProtocol;

public class Main : MonoBehaviour
{
	public InputField mainInput;

	[SerializeField] GameObject editValuesPanel = null;

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
			reader.Close();

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
				reader.Close();
			}
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

		com.serialPort.Open();
		com.serialPort.Write("u");
		xmodem.Send();

		if (xmodem.State != XModemStates.Idle)
		{
			xmodem.CancelOperation();
		}

		// Dispose of port.
		com.serialPort.Close();
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

		foreach (char c in com.output.ToCharArray())
		{
			if (c.ToString() == ",")
			{
				if (output == null || output == "" || output == "\n") continue;
				dataTableX[inputFieldSpot].text = output;
				output = string.Empty;
				inputFieldSpot++;
			}
			else
			{
				output += c;
			}
		}

		Reset();

		foreach (char c in com.output2.ToCharArray())
		{
			if (c.ToString() == ",")
			{
				if (output == null || output == "" || output == "\n") continue;
				dataTableY[inputFieldSpot].text = output;
				output = string.Empty;
				inputFieldSpot++;
			}
			else
			{
				output += c;
			}
		}

		Reset();

		com.statusManager.statusText.text = "Data reading from device has completed!";
		com.serialPort.Close();
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
