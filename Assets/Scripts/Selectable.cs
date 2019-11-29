using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
	public static HashSet<Selectable> allMySelectables = new HashSet<Selectable>();
	public static HashSet<Selectable> currentlySelected = new HashSet<Selectable>();

	public Image selectedImage = null;
	public Color selectedColor;

	// Main object
	private InputField mainInputField;

	// This object
	private bool dontChangeValue = false;
	private InputField thisInputField = null;

	private void Awake()
	{
		allMySelectables.Add(this);
	}

	private void Start()
	{
		thisInputField = GetComponent<InputField>();

		if (The.main.mainInput != null)
		{
			mainInputField = The.main.mainInput;
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{ 
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			dontChangeValue = false;
		}	
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !mainInputField.gameObject.activeInHierarchy)
		{
			DeselectAll(eventData);
		}

		if (!mainInputField.gameObject.activeInHierarchy)
		{
			OnSelect(eventData);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (!mainInputField.gameObject.activeInHierarchy)
		{
			if (!currentlySelected.Contains(this))
			{
				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
				{
					dontChangeValue = true;
					currentlySelected.Clear();
					EditValues.allSelectedInputFields.Clear();
				}

				currentlySelected.Add(this);
				EditValues.allSelectedInputFields.Add(thisInputField);
			}
		}
	}

	public static void DeselectAll(BaseEventData eventData)
	{
		foreach (Selectable selectable in currentlySelected)
		{
			selectable.OnDeselect(eventData);
		}

		EditValues.allSelectedInputFields.Clear();
		currentlySelected.Clear();
	}

	public void ChangeColor ()
	{
		if (currentlySelected.Contains(this))
		{
			selectedColor.a = 0.2f;
			selectedImage.color = selectedColor;
		}
		else
		{
			selectedColor.a = 0f;
			selectedImage.color = selectedColor;
		}
	}

	private void Update()
	{
		thisInputField.DeactivateInputField();

		if (mainInputField.gameObject.activeInHierarchy)
		{
			thisInputField.interactable = false;
		}
		else
		{
			thisInputField.interactable = true;
		}

		ChangeColor();

		if (!dontChangeValue)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow) && !mainInputField.gameObject.activeInHierarchy && currentlySelected.Count >= 2 && currentlySelected.Contains(this) && thisInputField.text != "500")
			{
				float newFloat = 0f;

				if (thisInputField.text == string.Empty)
				{
					newFloat = 0f + 1f;
				}
				else
				{
					newFloat = float.Parse(thisInputField.text) + 1f;
				}

				thisInputField.text = newFloat.ToString();
				thisInputField.textComponent.text = newFloat.ToString();
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow) && !mainInputField.gameObject.activeInHierarchy && currentlySelected.Count >= 2 && currentlySelected.Contains(this) && thisInputField.text != "0")
			{
				float newFloat = 0f;

				if (thisInputField.text == string.Empty)
				{
					newFloat = 0f - 1f;
				}
				else
				{
					newFloat = float.Parse(thisInputField.text) - 1f;
				}

				thisInputField.text = newFloat.ToString();
				thisInputField.textComponent.text = newFloat.ToString();
			}
		}
	}
}
