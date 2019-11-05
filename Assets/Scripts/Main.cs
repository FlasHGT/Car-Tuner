using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO.Ports;
using XModemProtocol;
using System.Linq;

public class Main : MonoBehaviour
{
	[SerializeField] GameObject EditValuesPanel = null;

	[SerializeField] Toggle xToggle = null;
	[SerializeField] Toggle yToggle = null;

	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

	[SerializeField] EventSystem eventSystem = null;
	private BaseEventData eventData = null;

	private COM com = null;

	private InputField[] currentTable = null;
	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 15;
	private int lineCount = 0;

	private string output;

	public void Deselect()
	{
		if (!EditValuesPanel.activeInHierarchy)
		{
			Selectable.DeselectAll(eventData);
		}
	}

	public void Export(string exportPath)
	{
		if (xToggle.isOn && yToggle.isOn)
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

			File.WriteAllText(exportPath, output);

			Reset();
		}
		else
		{
			if (!xToggle.isOn && !yToggle.isOn)
			{
				Debug.Log("Error"); // Make a pop up
				return;
			}

			if (xToggle.isOn)
			{
				currentTable = dataTableX;
			}
			else
			{
				currentTable = dataTableY;
			}

			for (int x = 0; x < currentTable.Length; x++)
			{
				output += currentTable[x].text + ",";

				if ((x % inputFieldOffset) == 0 && x != 0 && x != 255)
				{
					output += "\n";
					inputFieldOffset += 16;
				}
			}

			File.WriteAllText(exportPath, output);

			Reset();
		}
	}

	public void Import(string importPath)
	{
		if (xToggle.isOn && yToggle.isOn)
		{
			reader = new StreamReader(importPath);

			while (reader.ReadLine() != null)
			{
				lineCount++;
			}

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
			}
			else
			{
				Debug.Log("Error"); // Make a pop up
			}

			Reset();
		}
		else
		{
			if (!xToggle.isOn && !yToggle.isOn)
			{
				Debug.Log("Error"); // Make a pop up
				return;
			}

			if (xToggle.isOn)
			{
				currentTable = dataTableX;
			}
			else
			{
				currentTable = dataTableY;
			}

			reader = new StreamReader(importPath);

			while (reader.ReadLine() != null)
			{
				lineCount++;
			}

			if (lineCount == 16)
			{
				reader = new StreamReader(importPath);

				for (int x = 0; x < lineCount; x++)
				{
					string line = reader.ReadLine();

					foreach (char c in line.ToCharArray())
					{
						if (c.ToString() == "," || c.ToString() == " ")
						{
							currentTable[inputFieldSpot].text = output;
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
			else
			{
				Debug.Log("Error"); // Make a pop up
			}

			Reset();
		}
	}

	public void WriteComport()
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
		
		byte[] bytes = Encoding.ASCII.GetBytes(output);

		var xmodem = new XModemCommunicator();
		xmodem.Port = com.serialPort;
		xmodem.Data = bytes;

		// Subscribe to events.
		xmodem.Completed += (s, e) => {
			Debug.Log($"Operation completed.\n");
		};
		xmodem.Aborted += (s, e) => {
			Debug.Log("Operation Aborted.\n");
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
		com.ManualStart();

		foreach (char c in com.output.ToCharArray())
		{
			if (c.ToString() == ",")
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

		Reset();

		foreach (char c in com.output2.ToCharArray())
		{
			if (c.ToString() == ",")
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

		Reset();

		com.serialPort.Close();
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
