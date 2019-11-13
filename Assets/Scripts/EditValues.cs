using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditValues : MonoBehaviour
{
	public static List<InputField> allSelectedInputFields = new List<InputField>();

	[SerializeField] GameObject panel = null;
	[SerializeField] InputField inputField = null;
	[SerializeField] Toggle toggle = null;

	private bool readyToApply = false;

	private void ApplyValue ()
	{
		if (toggle.isOn)
		{
			foreach (InputField i in allSelectedInputFields)
			{
				string newValue = string.Empty;

				if (inputField.text == string.Empty)
				{
					newValue = "0";
				}
				else
				{
					newValue = inputField.text;
				}

				i.text = newValue;
				i.textComponent.text = newValue;

				Selectable newSelectable = i.GetComponent<Selectable>();
				newSelectable.ChangeColor();
			}

			Selectable.currentlySelected.Clear();
			allSelectedInputFields.Clear();
			inputField.text = string.Empty;
			panel.SetActive(false);
		}
		else
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
				}

				i.text = newValue.ToString();
				i.textComponent.text = newValue.ToString();
			}

			Selectable.currentlySelected.Clear();
			allSelectedInputFields.Clear();
			toggle.isOn = true;
			inputField.text = string.Empty;
			panel.SetActive(false);
		}
	}

	// Update is called once per frame
	private void Update()
    {
		if (inputField.isFocused)
		{
			readyToApply = true;
		}

		if (Input.GetKeyDown(KeyCode.Return) && readyToApply && inputField.text != string.Empty || Input.GetKeyDown(KeyCode.KeypadEnter) && readyToApply && inputField.text != string.Empty)
		{
			ApplyValue();
			readyToApply = false;
		}

		if (Selectable.currentlySelected.Count != 0 && Input.GetKeyDown(KeyCode.Return) && !panel.activeInHierarchy || Input.GetKeyDown(KeyCode.KeypadEnter) && Selectable.currentlySelected.Count != 0 && !panel.activeInHierarchy)
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
}
