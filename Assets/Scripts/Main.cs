using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SFB;
using System.Collections;
using UnityEngine.Networking;

public class Main : MonoBehaviour
{
	//public string importPath = null;
	//public string exportPath = null;

	[SerializeField] InputField[] inputFields = null;

	private StreamReader reader = null;

	private int inputFieldSpot = 0;
	private int inputFieldOffset = 14;
	private int lineCount = 0;

	private string output;

	public void FileExplorerImport()
	{
		var paths = StandaloneFileBrowser.OpenFilePanel("Import", "", "csv", false);
		if (paths.Length > 0)
		{
			StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
		}
	}

	public void FileExplorerExport()
	{
		var path = StandaloneFileBrowser.SaveFilePanel("Export", "", "sample", "csv");
		if (!string.IsNullOrEmpty(path))
		{
			Write(path);
		}
	}

	private void Write(string exportPath)
	{
		for (int x = 0; x < inputFields.Length; x++)
		{
			output += inputFields[x].text + ",";

			if ((x % inputFieldOffset) == 0 && x != 0)
			{
				output += "\n";
				inputFieldOffset += 15;
			}
		}	

		File.WriteAllText(exportPath, output);

		Reset();
	}

	private void Read(string importPath)
    {
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

		Reset();
	}

	private void Reset()
	{
		output = "";
		inputFieldOffset = 14;
		inputFieldSpot = 0;
		lineCount = 0;
	}

	private IEnumerator OutputRoutine(string url)
	{
		var loader = new UnityWebRequest(url);
		yield return loader;
		Read(loader.url.Replace("file:///", ""));
	}
}

