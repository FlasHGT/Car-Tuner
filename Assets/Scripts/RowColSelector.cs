using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowColSelector : MonoBehaviour
{
	public bool pressed = false;
	public int inputFieldCount = 0;

	[SerializeField] List<InputField> inputFields = null;
	[SerializeField] GameObject mainPanel = null;

	private Button button = null;

	public void ActivateInputFields()
	{
		foreach (InputField i in inputFields)
		{
			if (EditValues.allSelectedInputFields.Contains(i))
			{
				inputFieldCount += 1;
			}
		}

		if (inputFieldCount != 16)
		{
			foreach (InputField i in inputFields)
			{
				Selectable selectable = i.GetComponent<Selectable>();

				if (!Selectable.currentlySelected.Contains(selectable))
				{
					Selectable.currentlySelected.Add(selectable);
					EditValues.allSelectedInputFields.Add(i);
				}
			}
		}
		else
		{
			foreach (InputField i in inputFields)
			{
				Selectable.currentlySelected.Remove(i.GetComponent<Selectable>());
				EditValues.allSelectedInputFields.Remove(i);
			}
		}

		inputFieldCount = 0;
	}

	private void Start ()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(ActivateInputFields);
	}

	private void Update ()
	{
		if (mainPanel.activeInHierarchy)
		{
			button.interactable = false;
		}
		else
		{
			button.interactable = true;
		}
	}
}