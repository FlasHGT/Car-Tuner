using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowColSelector : MonoBehaviour
{
	[SerializeField] RowColInput rowColInput = null;
	[SerializeField] List<InputField> inputFields = null;

	private Button button;

	public void ActivateInputField()
	{
		if (!rowColInput.gameObject.activeInHierarchy)
		{
			rowColInput.gameObject.SetActive(true);
		}

		foreach (InputField _field in inputFields)
		{
			_field.GetComponent<Selectable>().selectedBySelector = true;
			_field.GetComponent<Image>().color = Color.yellow;
		}

		rowColInput.allSelectedInputFields.AddRange(inputFields);
	}

	private void Start ()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(ActivateInputField);
	}
}
