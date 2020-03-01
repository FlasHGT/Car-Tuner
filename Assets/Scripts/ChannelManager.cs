using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChannelManager : MonoBehaviour
{
	public int currentActiveChannel = 0;
	public int currentActiveArray = 0;

	[SerializeField] COM com = null;
	[SerializeField] Main main = null;

	[SerializeField] Button[] channelButtons = null;

	private int previousChannel = 0;
	private string startingValues = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";

	public void ChangeProfile(int i)
	{
		//while (The.benchmarkRunning)
		//{
		//	Debug.Log("Disabling benchmark mode & switching profile");
		//	com.DisableBenchmark();
		//}
		
		previousChannel = The.currentChannel;
		SaveDataToFile(previousChannel);
		switch (i)
		{
			case 0:
				com.importMessages[1] = "b";
				com.importMessagesChannel[1] = "b";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 1:
				com.importMessages[1] = "e";
				com.importMessagesChannel[1] = "e";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 2:
				com.importMessages[1] = "p";
				com.importMessagesChannel[1] = "p";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 3:
				com.importMessages[1] = "0";
				com.importMessagesChannel[1] = "0";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 4:
				com.importMessages[1] = "1";
				com.importMessagesChannel[1] = "1";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 5:
				com.importMessages[1] = "2";
				com.importMessagesChannel[1] = "1";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 6:
				com.importMessages[1] = "3";
				com.importMessagesChannel[1] = "3";
				The.currentChannel = i;
				PerformActions(i);
				break;
			case 7:
				com.importMessages[1] = "4";
				com.importMessagesChannel[1] = "4";
				The.currentChannel = i;
				PerformActions(i);
				break;
			default:
				break;
		}
		if (com.hasConnected)
		{
			StartCoroutine(com.TryChangeChannel());
		}
		

		//com.EnableBenchmark();
	}

	private void Awake()
	{
		The.channelManager = this;
	}

	private void Start()
	{
		if (!Directory.Exists(Application.dataPath + "\\ChannelData\\"))
		{
			Directory.CreateDirectory(Application.dataPath + "\\ChannelData\\");
			CreateFilledFiles();
		}
		previousChannel = The.currentChannel;
		PerformActions(0);
	}

	private void CreateFilledFiles()
	{
		for (int x = 0; x < 8; x++)
		{
			string path = Application.dataPath + "\\ChannelData\\" + x + "_" + "T12T3" + ".csv";

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

				newOutput += "\n\n";

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
		currentActiveArray = The.currentArray;

		currentActiveChannel = i;
		channelButtons[i].interactable = false;

		if (!com.isDataRead)
		{
			ReadDataFromFile(i);
		}
		else if (com.hasConnected && com.isDataRead)
		{
			ReadDataFromFile(i);
			if (!The.arrayChangedLocally[0, i])
			{
				//Debug.Log("Array changed locally is: " + The.arrayChangedLocally[0, i] + " refreshing array 0");
				main.RefreshArray(0, The.currentChannel);
			}
			if (!The.arrayChangedLocally[1, i])
			{
				//Debug.Log("Array changed locally is: " + The.arrayChangedLocally[1, i] + " refreshing array 1");
				main.RefreshArray(1, The.currentChannel);
			}
		}
		else
		{
			ReadDataFromFile(i);
		}
		ResetChannels(i);

	}

	public void SaveDataToFile(int i)
	{
		string path = Application.dataPath + "\\ChannelData\\" + i + "_" + "T12T3" + ".csv";

		main.Export(path, false);
	}

	private void ReadDataFromFile(int i)
	{
		string path = Application.dataPath + "\\ChannelData\\" + i + "_" + "T12T3" + ".csv";

		main.Import(path);
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
