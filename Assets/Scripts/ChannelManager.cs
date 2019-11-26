using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChannelManager : MonoBehaviour
{
	public int currentActiveChannel = 0;
	public int currentActiveArray = 0;

	[SerializeField] COM com = null;
	[SerializeField] Main main = null;
	[SerializeField] EditValues editValues = null;

	[SerializeField] Button[] channelButtons = null;
	[SerializeField] CanvasGroup canvasGroup = null;

	private int previousChannel = 0;
	private bool firstValuesHaveBeenSet = false;
	private string startingValues = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";

	public void ChangeProfile(int i)
	{
		switch (i)
		{
			case 0:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "b";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 1:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "e";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 2:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "p";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 3:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "0";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 4:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "1";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 5:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "2";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 6:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "3";
				PerformActions(i);
				The.currentChannel = i;
				break;
			case 7:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "4";
				PerformActions(i);
				The.currentChannel = i;
				break;
			default:
				break;
		}
	}

	private void Start()
	{
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
		Debug.Log(com.hasConnected);
		if (!com.hasConnected)
		{
			currentActiveChannel = i;
			channelButtons[i].interactable = false;
			ReadDataFromFile(i);
			ResetChannels(i);
			previousChannel = i;
		}
		else
		{
			currentActiveChannel = i;
			channelButtons[i].interactable = false;
			
			ResetChannels(i);
			previousChannel = i;
			main.RefreshArray(The.currentArray, The.currentChannel);
		}
		
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
		if (canvasGroup.alpha == 1 && !firstValuesHaveBeenSet)
		{
			PerformActions(0);
			firstValuesHaveBeenSet = true;
		}

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
