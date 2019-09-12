using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	public string importPath = null;
	public string exportPath = null;

	[SerializeField] InputField[] inputFields = null;

	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int lineCount = 0;

	private string importOutput;
	private string exportOutput;

	// Start is called before the first frame update
	private void Start()
    {
		Read();
	}

	public void Write()
	{
		exportPath = @"Assets\CSV\writetest.csv";

		for (int x = 0; x < inputFields.Length; x++)
		{
			exportOutput += inputFields[x].text + ",";

			if ((x % 14) == 0 && x != 0) // Fix this line break
			{
				exportOutput += "\n";
			}
		}

		File.WriteAllText(exportPath, exportOutput);
	}

	private void Read()
    {
		importPath = @"Assets\CSV\readtest.csv";

		reader = new StreamReader(importPath);

		while (reader.ReadLine() != null)
		{
			lineCount++;
		}

		reader = new StreamReader(importPath);

		for (int x = 0; x < lineCount; x++)
		{
			string line = reader.ReadLine();
			exportOutput = "";

			foreach(char c in line.ToCharArray())
			{
				if (c.ToString() == "," || c.ToString() == " ")
				{
					inputFields[inputFieldSpot].textComponent.fontSize = 9; // Remove this later on
					inputFields[inputFieldSpot].text = exportOutput;
					exportOutput = "";
					inputFieldSpot++;
				}
				else
				{
					exportOutput += c;
				}
			}
		}
	}
}

