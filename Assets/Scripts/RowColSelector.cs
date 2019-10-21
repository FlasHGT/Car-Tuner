using UnityEngine;
using UnityEngine.UI;

public class RowColSelector : MonoBehaviour
{
	public Main main = null;

	[SerializeField] InputField valueInputField = null;

	[SerializeField] InputField[] inputFields = null;

	private Button button = null;

	public void ActivateInputField ()
	{
		if(!valueInputField.gameObject.activeInHierarchy)
		{
			valueInputField.gameObject.SetActive(true);

			foreach (InputField _field in inputFields)
			{
				_field.GetComponent<Selectable>().selectedBySelector = true;
				_field.GetComponent<Image>().color = Color.yellow;
			}
		}    
	}

	public void ApplyValue ()
	{
		foreach(InputField i in inputFields)
		{
			float newValue = float.Parse(i.text) + float.Parse(valueInputField.text);
			i.text = newValue.ToString();
			i.textComponent.text = newValue.ToString();
			i.GetComponent<Image>().color = Color.white;
			i.GetComponent<Selectable>().selectedBySelector = false;
		}

		valueInputField.text = string.Empty;
		valueInputField.gameObject.SetActive(false);
	}

	private void Start ()
	{
		button = GetComponent<Button>();
	}

	private void Update()
	{
		if(valueInputField.isFocused)
		{
			main.currentlyActiveInputField = valueInputField;
		}

		if (Input.GetKeyDown(KeyCode.Return) && inputFields[0].GetComponent<Image>().color == Color.yellow && inputFields[1].GetComponent<Image>().color == Color.yellow && main.currentlyActiveInputField == valueInputField)
		{
			ApplyValue();
		}

		if (valueInputField.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow) && inputFields[0].GetComponent<Image>().color == Color.yellow && inputFields[1].GetComponent<Image>().color == Color.yellow && valueInputField.isFocused)
		{
			float newFloat = 0f;

			if (valueInputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
			}
			else
			{
				newFloat = float.Parse(valueInputField.text) + 0.1f;
			}

			valueInputField.text = newFloat.ToString();
			valueInputField.textComponent.text = newFloat.ToString();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) && inputFields[0].GetComponent<Image>().color == Color.yellow && inputFields[1].GetComponent<Image>().color == Color.yellow && valueInputField.isFocused)
		{
			float newFloat = 0f;

			if (valueInputField.text == string.Empty)
			{
				newFloat = 0f - 0.1f;
			}
			else
			{
				newFloat = float.Parse(valueInputField.text) - 0.1f;
			}

			valueInputField.text = newFloat.ToString();
			valueInputField.textComponent.text = newFloat.ToString();
		}
	}
}
