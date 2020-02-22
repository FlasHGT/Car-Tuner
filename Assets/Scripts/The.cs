using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class The
{
	public static Main main;
	public static ChannelManager channelManager;
	public static bool benchmarkRunning;
	public static int currentChannel = 0;
	public static int currentArray = 0;
	public static bool[,] arrayChangedLocally = new bool[2,8];
	public static Graph graph;
}
