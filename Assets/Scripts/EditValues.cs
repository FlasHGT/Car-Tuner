using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditValues : MonoBehaviour
{
	public static List<InputField> allSelectedInputFields = new List<InputField>();

	public ChannelManager currentChannelManager = null;

	[SerializeField] GameObject panel = null;
	[SerializeField] InputField inputField = null;
	[SerializeField] Toggle toggle = null;

	private RectTransform panelRT = null;

	private float currentTime = 0f;
	private bool readyToApply = false;

	private void ApplyValue ()
	{
		if (toggle.isOn)
		{
			if(allSelectedInputFields.Count != 1)
			{
				foreach (InputField i in allSelectedInputFields)
				{
					float newValue = 0f;

					if (inputField.text == string.Empty)
					{
						newValue = 0f;
					}
					else
					{
						newValue = float.Parse(inputField.text);
						newValue = CheckIfValueIsBetweenBounds(newValue);
					}

					i.text = newValue.ToString();
					i.textComponent.text = newValue.ToString();
				}
			}
			else
			{
				float newValue = 0f;

				if (inputField.text == string.Empty)
				{
					newValue = 0f;
				}
				else
				{
					newValue = float.Parse(inputField.text);
					newValue = CheckIfValueIsBetweenBounds(newValue);
				}

				allSelectedInputFields[0].text = newValue.ToString();
				allSelectedInputFields[0].textComponent.text = newValue.ToString();
			}
		}
		else
		{
			if (allSelectedInputFields.Count != 1)
			{
				foreach (InputField i in allSelectedInputFields)
				{
					float newValue = 0f;

					if (inputField.text == string.Empty)
					{
						newValue = float.Parse(i.text) + 0f;
					}
					else
					{
						newValue = float.Parse(i.text) + float.Parse(inputField.text);
						newValue = CheckIfValueIsBetweenBounds(newValue);
					}

					i.text = newValue.ToString();
					i.textComponent.text = newValue.ToString();
				}
			}
			else
			{
				float newValue = 0f;

				if (inputField.text == string.Empty)
				{
					newValue = float.Parse(allSelectedInputFields[0].text) + 0f;
				}
				else
				{
					newValue = float.Parse(allSelectedInputFields[0].text) + float.Parse(inputField.text);
					newValue = CheckIfValueIsBetweenBounds(newValue);
				}

				allSelectedInputFields[0].text = newValue.ToString();
				allSelectedInputFields[0].textComponent.text = newValue.ToString();
			}

			toggle.isOn = true;
		}

		if (allSelectedInputFields.Count != 1)
		{
			Selectable.currentlySelected.Clear();
			allSelectedInputFields.Clear();
		}
		else
		{
			allSelectedInputFields[0].Select();
		}

		currentTime = Time.time;
		inputField.text = string.Empty;
		panel.SetActive(false);
		The.channelManager.SaveDataToFile(The.currentChannel);
	}

	private float CheckIfValueIsBetweenBounds(float newValue)
	{
		if (newValue < Main.minInputFieldValue)
		{
			newValue = Main.minInputFieldValue;
		}
		else if (newValue > Main.maxInputFieldValue)
		{
			newValue = Main.maxInputFieldValue;
		}

		return newValue;
	}

	private void Start ()
	{
		currentTime = Time.time;
		panelRT = panel.GetComponent<RectTransform>();
	}

	// Update is called once per frame
	private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && !GetScreenCoordinates(panelRT).Contains(Input.mousePosition))
		{
			panel.SetActive(false);
		}

		if (inputField.isFocused)
		{
			readyToApply = true;
		}

		if (readyToApply && inputField.text != string.Empty && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
		{
			ApplyValue();
			The.arrayChangedLocally[The.currentArray, The.currentChannel] = true;
			readyToApply = false;
		}

		if(Selectable.currentlySelected.Count != 0 && !panel.activeInHierarchy && Time.time - currentTime > 0.5f &&
			(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
		{
			panel.SetActive(true);
			inputField.ActivateInputField();
		}

		if (inputField.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow) && inputField.isFocused)
		{
			float newFloat = 0f;

			if (inputField.text == string.Empty)
			{
				newFloat = 0f + 1f;
			}
			else
			{
				newFloat = float.Parse(inputField.text) + 1f;
			}

			inputField.text = newFloat.ToString();
			inputField.textComponent.text = newFloat.ToString();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) && inputField.isFocused)
		{
			float newFloat = 0f;

			if (inputField.text == string.Empty)
			{
				newFloat = 0f - 1f;
			}
			else
			{
				newFloat = float.Parse(inputField.text) - 1f;
			}

			inputField.text = newFloat.ToString();
			inputField.textComponent.text = newFloat.ToString();
		}
	}

	private Rect GetScreenCoordinates(RectTransform uiElement)
	{
		var worldCorners = new Vector3[4];
		uiElement.GetWorldCorners(worldCorners);
		var result = new Rect(
					  worldCorners[0].x,
					  worldCorners[0].y,
					  worldCorners[2].x - worldCorners[0].x,
					  worldCorners[2].y - worldCorners[0].y);
		return result;
	}
}
