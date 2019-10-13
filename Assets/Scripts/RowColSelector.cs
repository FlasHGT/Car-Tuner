using UnityEngine;
using UnityEngine.UI;

public class RowColSelector : MonoBehaviour
{
	public Main main = null;

	[SerializeField] Button button = null;
	[SerializeField] InputField valueInputField = null;

	[SerializeField] InputField[] inputFields = null; 

    public void ActivateInputField ()
	{
		if(button.gameObject.activeInHierarchy && !main.inputFieldBeingEdited)
		{
			button.gameObject.SetActive(false);
			valueInputField.gameObject.SetActive(true);
			main.inputFieldBeingEdited = true;

			valueInputField.text = 0.ToString();
			valueInputField.textComponent.text = 0.ToString();

			for (int x = 0; x < inputFields.Length; x++)
			{
				inputFields[x].GetComponent<Image>().color = Color.yellow;
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
		}

		valueInputField.text = string.Empty;
		valueInputField.gameObject.SetActive(false);
		button.gameObject.SetActive(true);
		main.inputFieldBeingEdited = false;
	}

	private void Update ()
	{
		if(valueInputField.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow))
		{
			float newFloat = 0f;

			if(valueInputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
			}else
			{
				newFloat = float.Parse(valueInputField.text) + 0.1f;
			}

			valueInputField.text = newFloat.ToString();
			valueInputField.textComponent.text = newFloat.ToString();
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			float newFloat = 0f;

			if (valueInputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
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
