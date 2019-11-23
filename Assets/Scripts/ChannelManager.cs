using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChannelManager : MonoBehaviour
{
	public int currentActiveChannel = 0;

	[SerializeField] COM com = null;
	[SerializeField] Main main = null;
	[SerializeField] EditValues editValues = null;

	[SerializeField] Button[] channelButtons = null;
	[SerializeField] CanvasGroup canvasGroup = null;

	private int previousChannel = 0;
	private string startingValues = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";

	public void ChangeProfile(int i)
	{
		switch (i)
		{
			case 0:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "b";
				PerformActions(0);
				break;
			case 1:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "e";
				PerformActions(1);
				break;
			case 2:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "p";
				PerformActions(2);
				break;
			case 3:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "0";
				PerformActions(3);
				break;
			case 4:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "1";
				PerformActions(4);
				break;
			case 5:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "2";
				PerformActions(5);
				break;
			case 6:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "3";
				PerformActions(6);
				break;
			case 7:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "4";
				PerformActions(7);
				break;
			default:
				break;
		}
	}

	private void Start()
	{
		com.importMessage[1] = "b";
		PerformActions(0);

		if (!Directory.Exists(Application.dataPath + "\\ChannelData\\"))
		{
			Directory.CreateDirectory(Application.dataPath + "\\ChannelData\\");
			CreateFilledFiles();
		}
	}

	private void CreateFilledFiles()
	{
		for (int x = 0; x < 8; x++)
		{
			string path = Application.dataPath + "\\ChannelData\\" + x + "_" + canvasGroup.name + ".csv";

			if (!File.Exists(path))
			{
				string newOutput = string.Empty;

				for (int y = 0; y < 16; y++)
				{
					newOutput += startingValues;
					
					if (y != 15)
					{
						newOutput += "\n";
					}
				}

				File.WriteAllText(path, newOutput);
			}
		}
	}

	private void PerformActions(int i)
	{
		currentActiveChannel = i;
		channelButtons[i].interactable = false;
		ReadDataFromFile(i);
		ResetChannels(i);
		previousChannel = i;
	}

	public void SaveDataToFile(int i)
	{
		string path = Application.dataPath + "\\ChannelData\\" + i + "_" + canvasGroup.name + ".csv";

		main.Export(path, true);
	}

	private void ReadDataFromFile(int i)
	{
		string path = Application.dataPath + "\\ChannelData\\" + i + "_" + canvasGroup.name + ".csv";

		main.Import(path);
	}

	private void Update ()
	{
		if (canvasGroup.alpha == 1)
		{
			editValues.currentChannelManager = this;
		}
	}

	private void ResetChannels(int i)
	{
		for (int x = 0; x < channelButtons.Length; x++)
		{
			if (x != i)
			{
				channelButtons[x].interactable = true;
			}
		}
	}
}
