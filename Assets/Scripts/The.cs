﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class The : MonoBehaviour
{
	public static Main main;
	public static ChannelManager channelManager;
	public static int currentChannel = 0;
	public static int currentArray = 0;
	public static bool[,] arrayChangedLocally = new bool[2,8];
}
