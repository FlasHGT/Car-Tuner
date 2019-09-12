using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ReadFile : MonoBehaviour
{
	[SerializeField] string filePath = null;
	[SerializeField] InputField[] inputFields = null;

	private int inputFieldSpot = 0;
	private StreamReader reader = null;
	private int lineCount = 0;
	private string output;

    // Start is called before the first frame update
    private void Start()
    {
		Read();
	}

    private void Read()
    {
		filePath = @"Assets\CSV\test.csv";

		reader = new StreamReader(filePath);

		while (reader.ReadLine() != null)
		{
			lineCount++;
		}

		reader = new StreamReader(filePath);

		for (int x = 0; x < lineCount; x++)
		{
			string line = reader.ReadLine();
			output = "";

			foreach(char c in line.ToCharArray())
			{
				if (c.ToString() == "," || c.ToString() == " ")
				{
					inputFields[inputFieldSpot].textComponent.fontSize = 9; // Remove this later on
					inputFields[inputFieldSpot].text = output;
					output = "";
					inputFieldSpot++;		
				}
				else
				{
					output += c;
				}
			}
		}
	}

	private void Write()
	{

	}
}

