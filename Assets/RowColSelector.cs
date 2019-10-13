using UnityEngine;
using UnityEngine.UI;

public class RowColSelector : MonoBehaviour
{
	[SerializeField] Button button = null;
	[SerializeField] InputField valueInputField = null;

	[SerializeField] InputField[] inputFields = null; 

    public void ActivateInputField ()
	{
		if(button.gameObject.activeInHierarchy)
		{
			//button.gameObject.SetActive(false);
			valueInputField.gameObject.SetActive(true);
		}

        foreach(InputField _field in inputFields)
        {
            _field.GetComponent<Image>().color = Color.yellow;
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
		//valueInputField.gameObject.SetActive(false);
	}
}
