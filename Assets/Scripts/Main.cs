using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;
using CSVWrite; // remove this
using System.Threading;

public class Main : MonoBehaviour
{
	[SerializeField] GameObject dataTableX1 = null; // Remove this
	[SerializeField] GameObject dataTableY1 = null; // Remove this
	private string[] message = {"a", "b"}; // Move this to COM.cs as exportMessage
	private string[] message2 = {"a", "b", "d"}; // Remove this
	private int currentMessage = 0; // Remove this
	
	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

	private UIManager uiManager = null;
	private COM com = null;

	private InputField[] currentTable = null;
	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 15;
	private int lineCount = 0;

	private string output;

	public void Export(string exportPath)
	{
		//CheckWhichDataTableIsActive(); // Uncomment this
		CheckCurrentTable(); // Remove this

		for (int x = 0; x < currentTable.Length; x++)
		{
			output += currentTable[x].text + ",";

			if ((x % inputFieldOffset) == 0 && x != 0)
			{
				output += "\n";
				inputFieldOffset += 16;
			}
		}	

		File.WriteAllText(exportPath, output);

		Reset();
	}

	public void Import(string importPath)
    {
		//CheckWhichDataTableIsActive(); // Uncomment this
		CheckCurrentTable(); // Remove this

		reader = new StreamReader(importPath);

		while (reader.ReadLine() != null)
		{
			lineCount++;
		}

		reader = new StreamReader(importPath);

		for (int x = 0; x < lineCount; x++)
		{
			string line = reader.ReadLine();

			foreach(char c in line.ToCharArray())
			{
				if (c.ToString() == "," || c.ToString() == " ")
				{
					currentTable[inputFieldSpot].textComponent.fontSize = 9; // Remove this
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

		Reset();
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

		for (int y = 0; y < 16; y++)
		{
			for (int x = 0; x < 16; x++)
			{
				Thread.Sleep(30);
				com.serialPort.Write("e");
				com.serialPort.Write("0 " + "" + x + " " + "" + y + " " + "" + dataTableX[inputFieldSpot].text);
				com.serialPort.Write("\r"); // ENTER
				inputFieldSpot++;
			}
		}

		Reset();

		for (int y = 0; y < 16; y++)
		{
			for (int x = 0; x < 16; x++)
			{
				Thread.Sleep(30);
				com.serialPort.Write("e");
				com.serialPort.Write("1 " + "" + x + " " + "" + y + " " + "" + dataTableY[inputFieldSpot].text);
				com.serialPort.Write("\r"); // ENTER
				inputFieldSpot++;
			}
		}

		Reset();

		//while (currentMessage < 80)
		//{
		//	Debug.Log(com.serialPort.ReadLine());
		//	currentMessage++;
		//}
	}

	public void ReadComport ()
	{
		com.ManualStart();

		foreach(char c in com.output.ToCharArray())
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
	}

	public void ChangeDataTable () // Remove this
	{
		if (dataTableX1.activeInHierarchy)
		{
			dataTableX1.SetActive(false);
			dataTableY1.SetActive(true);
		}
		else
		{
			dataTableY1.SetActive(false);
			dataTableX1.SetActive(true);
		}
	}

	private void CheckCurrentTable()
	{
		if(dataTableX1.activeInHierarchy)
		{
			currentTable = dataTableX;
		}
		else
		{
			currentTable = dataTableY;
		}
	}

	private void Start()
	{
		uiManager = GetComponent<UIManager>();
		com = GetComponent<COM>();

		for (int x = 0; x < dataTableX.Length; x++)
		{
			dataTableX[x].textComponent.text = dataTableX[x].text;
			Debug.Log("HELLO");
		}

		for (int y = 0; y < dataTableY.Length; y++)
		{
			dataTableY[y].textComponent.text = dataTableY[y].text;
		}
	}

	private void CheckWhichDataTableIsActive ()
	{
		if (uiManager.dataTableLeftPanel.activeInHierarchy)
		{
			currentTable = dataTableY;
		}
		else
		{
			currentTable = dataTableX;
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
