using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	private UIManager uiManager = null;

	[SerializeField] InputField[] dataTableX = null;
	[SerializeField] InputField[] dataTableY = null;

	private InputField[] currentTable = null;
	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 14;
	private int lineCount = 0;

	private string output;

	public void Write(string exportPath)
	{
		CheckWhichDataTableIsActive();

		for (int x = 0; x < currentTable.Length; x++)
		{
			output += currentTable[x].text + ",";

			if ((x % inputFieldOffset) == 0 && x != 0)
			{
				output += "\n";
				inputFieldOffset += 15;
			}
		}	

		File.WriteAllText(exportPath, output);

		Reset();
	}

	public void Read(string importPath)
    {
		CheckWhichDataTableIsActive();

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
					currentTable[inputFieldSpot].text = output;
					output = "";
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

	private void Start()
	{
		uiManager = GetComponent<UIManager>();
	}

	private void CheckWhichDataTableIsActive () // Change this so you can view both data tables on one screen
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
		inputFieldOffset = 14;
		inputFieldSpot = 0;
		lineCount = 0;
	}
}

