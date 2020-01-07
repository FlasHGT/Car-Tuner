using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
	public List<RectTransform> startLines = new List<RectTransform>();
	public List<RectTransform> endLines = new List<RectTransform>();

	[HideInInspector] public List<float> startValue = new List<float>();
	[HideInInspector] public List<float> endValue = new List<float>();

	[HideInInspector] public List<Vector2> startConnectorVectors = new List<Vector2>();
	[HideInInspector] public List<Vector2> endConnectorVectors = new List<Vector2>();

	[HideInInspector] public List<float> start0 = new List<float>();
	[HideInInspector] public List<float> start1 = new List<float>();

	public int maxObjectsInList = 30;

	[SerializeField] COM com = null;

	[SerializeField] int xSeparatorCount = 30;
	[SerializeField] int ySeparatorCount = 20;

	[SerializeField] float xMax = 300f;
	[SerializeField] float xMin = 0f;

	[SerializeField] float yMax = 5f;
	[SerializeField] float yMin = 0f;

	[SerializeField] RectTransform graphContainer = null;
	[SerializeField] RectTransform labelTemplate = null;
	[SerializeField] RectTransform xLines = null;
	[SerializeField] RectTransform yLines = null;
	[SerializeField] RectTransform lineTemplate = null;

	private Vector2 lastStartCircle = Vector2.zero;
	private Vector2 lastEndCircle = Vector2.zero;

	private float graphLength = 0f;
	private float graphHeight = 0f;

	private int lastValue = 0;

	public void Reset ()
	{
		foreach (RectTransform sl in startLines)
		{
			sl.gameObject.SetActive(false);
		}

		foreach (RectTransform el in endLines)
		{
			el.gameObject.SetActive(false);
		}

		startConnectorVectors.Clear();
		endConnectorVectors.Clear();
		start0.Clear();
		start1.Clear();
		startValue.Clear();
		endValue.Clear();

		lastValue = 0;
		lastStartCircle = Vector2.zero;
		lastEndCircle = Vector2.zero;
	}

	private void Start()
	{
		graphLength = graphContainer.sizeDelta.x;
		graphHeight = graphContainer.sizeDelta.y;

		CreateLabels();
		CreateLines();
	}

	private void Update ()
	{
		if (lastValue == maxObjectsInList)
		{
			Reset();
			//lastValue--; // UNCOMMENT SO LINIJU, LAI STRADATU APAKSEJAIS KODS
		}

		if (lastValue < startValue.Count && com.benchmarkRunning)
		{
			ShowGraph(startValue, 0, startLines, startConnectorVectors, start0);
			ShowGraph(endValue, 1, endLines, endConnectorVectors, start1);

			lastValue++;
		}
	}

	private void ShowGraph(List<float> valueList, int start0End1, List<RectTransform> dotConnectors, List<Vector2> connectorVectors, List<float> startPos)
	{
		for (int i = lastValue; i < valueList.Count; i++)
		{
			float xPos = (i * 1f - xMin) / (xMax - xMin) * graphLength;

			float yPos = ((float)Math.Round(valueList[i] / 1000, 1) - yMin) / (yMax - yMin) * graphHeight;

			Vector2 newVector = new Vector2(xPos, yPos);

			// KODS, KAS IR APAKSA PADARA GRAFIKU DINAMISKU, BET ED LOTI DAUDZ FRAMES

			//if (lastValue == maxObjectsInList - 1) 
			//{
			//	dotConnectors[0].gameObject.SetActive(false);
			//	dotConnectors.Insert(dotConnectors.Count, dotConnectors[0]);
			//	dotConnectors.RemoveAt(0);

			//	foreach (RectTransform rt in dotConnectors)
			//	{
			//		if (startPos.Count - 1 >= dotConnectors.IndexOf(rt) && rt.gameObject.activeInHierarchy)
			//		{
			//			rt.anchoredPosition = new Vector2(startPos[dotConnectors.IndexOf(rt)], rt.anchoredPosition.y);
			//		}
			//	}

			//	if (connectorVectors.Count != 0)
			//	{
			//		connectorVectors.RemoveAt(0);
			//	}

			//	if (start0End1 == 0)
			//	{
			//		lastStartCircle = connectorVectors[connectorVectors.Count - 1];
			//		CreateDotConnection(lastStartCircle, newVector, dotConnectors, i, connectorVectors, startPos);
			//	}
			//	else
			//	{
			//		lastEndCircle = connectorVectors[connectorVectors.Count - 1];
			//		CreateDotConnection(lastEndCircle, newVector, dotConnectors, i, connectorVectors, startPos);
			//	}

			//	return;
			//}

			if (start0End1 == 0)
			{
				if (lastStartCircle != Vector2.zero)
				{
					CreateDotConnection(lastStartCircle, newVector, dotConnectors, i, connectorVectors, startPos);
				}
				else if (i == 1)
				{
					CreateDotConnection(new Vector2(0f , newVector.y), newVector, dotConnectors, i, connectorVectors, startPos);
				}

				lastStartCircle = newVector;
			}
			else
			{
				if (lastEndCircle != Vector2.zero)
				{
					CreateDotConnection(lastEndCircle, newVector, dotConnectors, i, connectorVectors, startPos);
				}

				lastEndCircle = newVector;
			}
		}
	}

	private void CreateLines ()
	{
		for (int x = 0; x <= xSeparatorCount; x++)
		{
			if (x != 0 && x != xSeparatorCount)
			{
				float normalizedValue = x * 1f / xSeparatorCount;

				RectTransform lineX = Instantiate(lineTemplate);
				lineX.SetParent(xLines);
				lineX.sizeDelta = new Vector2(1f, graphHeight);
				lineX.localScale = Vector3.one;
				lineX.anchoredPosition = new Vector2(normalizedValue * graphLength, graphHeight / 2f);
			}
		}

		for (int y = 0; y <= ySeparatorCount; y++)
		{
			if (y != 0 && y != ySeparatorCount)
			{
				float normalizedValue = y * 1f / ySeparatorCount;

				RectTransform lineY = Instantiate(lineTemplate);
				lineY.SetParent(yLines);
				lineY.eulerAngles = new Vector3(0f, 0f, 90f);
				lineY.sizeDelta = new Vector2(1f, graphLength);
				lineY.localScale = Vector3.one;
				lineY.anchoredPosition = new Vector2(graphLength / 2f, normalizedValue * graphHeight);
			}
		}
	}
	
	private void CreateLabels()
	{
		for (int x = 0; x <= xSeparatorCount; x++)
		{
			float normalizedValue = x * 1f / xSeparatorCount;

			RectTransform labelX = Instantiate(labelTemplate);
			labelX.SetParent(graphContainer, false);
			labelX.anchoredPosition = new Vector2(normalizedValue * graphLength, -15f);
			labelX.GetComponent<Text>().text = (normalizedValue * xSeparatorCount).ToString();
		}

		for (int y = 0; y <= ySeparatorCount; y++)
		{
			float normalizedValue = y * 1f / ySeparatorCount;

			RectTransform labelY = Instantiate(labelTemplate);
			labelY.SetParent(graphContainer, false);
			labelY.anchoredPosition = new Vector2(-15f, normalizedValue * graphHeight);
			labelY.GetComponent<Text>().text = (normalizedValue * yMax).ToString();
		}
	}

	private void CreateDotConnection (Vector2 dotPosA, Vector2 dotPosB, List<RectTransform> dotConnectors, int indexOfDotConnector, List<Vector2> connectorVectors, List<float> startPos)
	{
		float distance = Vector2.Distance(dotPosA, dotPosB);
		Vector2 dir = (dotPosB - dotPosA).normalized;

		RectTransform currentTransform = dotConnectors[indexOfDotConnector - 1];

		currentTransform.sizeDelta = new Vector2(distance, 3f);
		currentTransform.anchoredPosition = dotPosA + dir * distance * 0.5f;
		currentTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

		currentTransform.gameObject.SetActive(true);

		//if (lastValue <= maxObjectsInList)
		//{
		//	startPos.Add(dotPosA.x); // UNCOMMENT, LAI STRADATU AUGSEJAIS KODS
		//}

		connectorVectors.Add(dotPosA);
	}
}
