using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	[SerializeField] GameObject dataTableX1 = null; // Remove this
	[SerializeField] GameObject dataTableY1 = null; // Remove this

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

