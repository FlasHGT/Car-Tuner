using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO.Ports;

public class Main : MonoBehaviour
{
	[HideInInspector] public InputField currentlyActiveInputField = null;

	private string[] message = { "a", "b", "u" }; // Move this to COM.cs as exportMessage
	private int currentMessage = 0; // Remove this

	[SerializeField] Toggle xToggle = null;
	[SerializeField] Toggle yToggle = null;

	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

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
		Selectable.DeselectAll(eventData);
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
		//for (int x = 0; x < dataTableX.Length; x++)
		//{
		//	output += dataTableX[x].text + ",";

		//	if ((x % inputFieldOffset) == 0 && x != 0)
		//	{
		//		output += "\n";
		//		inputFieldOffset += 16;
		//	}
		//}

		//inputFieldOffset = 15;
		//output += "\n";

		//for (int x = 0; x < dataTableY.Length; x++)
		//{
		//	output += dataTableY[x].text + ",";

		//	if ((x % inputFieldOffset) == 0 && x != 0 && x != 255)
		//	{
		//		output += "\n";
		//		inputFieldOffset += 16;
		//	}
		//}

		//com.serialPort.ReadTimeout = 1000;
		//com.serialPort.WriteTimeout = 1000;

		//if (!com.serialPort.IsOpen)
		//{
		//	com.serialPort.Open();
		//}

		//while (currentMessage < message.Length)
		//{
		//	com.serialPort.Write(message[currentMessage]);
		//	currentMessage++;
		//}

		//currentMessage = 0;

		//while (currentMessage < 30)
		//{
		//	Debug.Log(com.serialPort.ReadLine());
		//	currentMessage++;
		//}

		//Reset();

		if (!com.serialPort.IsOpen)
		{
			com.serialPort.Open();
		}

		while (currentMessage < message.Length)
		{
			com.serialPort.Write(message[currentMessage]);
			currentMessage++;
		}

		const byte SOH = 1;  // Start of Header
		const byte EOT = 4;  // End of Transmission

		const byte dataSize = 128;

		int packetNumber = 0;
		int invertedPacketNumber = 255;
		byte[] data = new byte[dataSize];
		int checkSum = 0;

		FileStream fileStream = new FileStream(@"D:\@Projects\Unity Projects\Windows APP\Assets\CSV\sample2.csv", FileMode.Open, FileAccess.Read);

		int fileReadCount;
		do
		{
			fileReadCount = fileStream.Read(data, 0, dataSize);
			if (fileReadCount == 0) break;
			if (fileReadCount != dataSize)
				for (int i = fileReadCount; i < dataSize; i++)
					data[i] = 0;

			packetNumber++;
			if (packetNumber > 255)
				packetNumber -= 256;

			invertedPacketNumber = 255 - packetNumber;

			checkSum = 1;
			checkSum += packetNumber;
			checkSum += invertedPacketNumber;
			for (int i = 0; i < dataSize; i++)
				checkSum += data[i];

			com.serialPort.Write(new byte[] { SOH }, 0, 1);
			com.serialPort.Write(new byte[] { (byte)packetNumber }, 0, 1);
			com.serialPort.Write(new byte[] { (byte)invertedPacketNumber }, 0, 1);
			com.serialPort.Write(data, 0, dataSize);
			com.serialPort.Write(new byte[] { (byte)checkSum }, 0, 1);

		} while (dataSize == fileReadCount);

		com.serialPort.Write(new byte[] { EOT }, 0, 1);

		//for (int y = 0; y < 16; y++)
		//{
		//	for (int x = 0; x < 16; x++)
		//	{
		//		com.serialPort.Write("e");
		//		output = "0 " + "" + x + " " + "" + y + " " + "" + dataTableX[inputFieldSpot].text;
		//		com.serialPort.Write(output);
		//		com.serialPort.Write("\r\n"); // ENTER
		//		inputFieldSpot++;
		//	}
		//}

		//Reset();

		//for (int y = 0; y < 16; y++)
		//{
		//	for (int x = 0; x < 16; x++)
		//	{
		//		com.serialPort.Write("e");
		//		output = "1 " + "" + x + " " + "" + y + " " + "" + dataTableY[inputFieldSpot].text;
		//		com.serialPort.Write(output);
		//		com.serialPort.Write("\r\n"); // ENTER
		//		inputFieldSpot++;
		//	}
		//}

		//Reset();

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

	private void Reset()
	{
		output = "";
		inputFieldOffset = 15;
		inputFieldSpot = 0;
		lineCount = 0;
	}
}
