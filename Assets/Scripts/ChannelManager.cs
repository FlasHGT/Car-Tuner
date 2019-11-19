using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChannelManager : MonoBehaviour
{
	[SerializeField] COM com = null;
	[SerializeField] Main main = null;

	[SerializeField] Button[] channelButtons = null;

	[SerializeField] string currentDataTableName = string.Empty;

	private string path = string.Empty;
	private int previousChannel;
	private bool profileHasBeenSelected = false;

	public void Start()
	{
		//File.WriteAllBytes(Application.dataPath + "\\ChannelData\\")
	}

	public void ChangeProfile(int i)
	{
		switch (i)
		{
			case 0:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "b";
				channelButtons[0].interactable = false;
				ReadDataFromFile(0);
				ResetChannels(0);
				previousChannel = 0;
				break;
			case 1:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "e";
				channelButtons[1].interactable = false;
				ReadDataFromFile(1);
				ResetChannels(1);
				previousChannel = 1;
				break;
			case 2:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "p";
				channelButtons[2].interactable = false;
				ReadDataFromFile(2);
				ResetChannels(2);
				previousChannel = 2;
				break;
			case 3:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "0";
				channelButtons[3].interactable = false;
				ReadDataFromFile(3);
				ResetChannels(3);
				previousChannel = 3;
				break;
			case 4:
				//SaveDataToFile(previousChannel);
				com.importMessage[1] = "1";
				channelButtons[4].interactable = false;
				//ReadDataFromFile(4);
				ResetChannels(4);
				previousChannel = 4;
				break;
			case 5:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "2";
				channelButtons[5].interactable = false;
				ReadDataFromFile(5);
				ResetChannels(5);
				previousChannel = 5;
				break;
			case 6:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "3";
				channelButtons[6].interactable = false;
				ReadDataFromFile(6);
				ResetChannels(6);
				previousChannel = 6;
				break;
			case 7:
				SaveDataToFile(previousChannel);
				com.importMessage[1] = "4";
				channelButtons[7].interactable = false;
				ReadDataFromFile(7);
				ResetChannels(7);
				previousChannel = 7;
				break;
			default:
				break;
		}

		profileHasBeenSelected = true;
	}

	private void SaveDataToFile(int i)
	{
		if (profileHasBeenSelected)
		{
			path = Application.dataPath + "\\ChannelData\\" + i + "_" + currentDataTableName + ".csv";

			main.Export(path, true);
		}
	}

	private void ReadDataFromFile(int i)
	{
		path = Application.dataPath + "\\ChannelData\\" + i + "_" + currentDataTableName + ".csv";

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
