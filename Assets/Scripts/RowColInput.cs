using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowColInput : MonoBehaviour
{
	public List<InputField> allSelectedInputFields = null;
	public InputField thisInputField = null;

	[SerializeField] Main main = null;

	public void ApplyValue()
	{
		foreach (InputField i in allSelectedInputFields)
		{
			float newValue = float.Parse(i.text) + float.Parse(thisInputField.text);
			i.text = newValue.ToString();
			i.textComponent.text = newValue.ToString();
			i.GetComponent<Image>().color = Color.white;
			i.GetComponent<Selectable>().selectedBySelector = false;
		}

		allSelectedInputFields.Clear();
		thisInputField.text = string.Empty;
		thisInputField.gameObject.SetActive(false);
	}

	// Start is called before the first frame update
	void Start()
    {
		thisInputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
		if (thisInputField.isFocused)
		{
			main.currentlyActiveInputField = thisInputField;
		}

		if (Input.GetKeyDown(KeyCode.Return) && main.currentlyActiveInputField == thisInputField)
		{
			ApplyValue();
		}

		if (thisInputField.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow) && thisInputField.isFocused)
		{
			float newFloat = 0f;

			if (thisInputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
			}
			else
			{
				newFloat = float.Parse(thisInputField.text) + 0.1f;
			}

			thisInputField.text = newFloat.ToString();
			thisInputField.textComponent.text = newFloat.ToString();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) && thisInputField.isFocused)
		{
			float newFloat = 0f;

			if (thisInputField.text == string.Empty)
			{
				newFloat = 0f - 0.1f;
			}
			else
			{
				newFloat = float.Parse(thisInputField.text) - 0.1f;
			}

			thisInputField.text = newFloat.ToString();
			thisInputField.textComponent.text = newFloat.ToString();
		}
	}
}
