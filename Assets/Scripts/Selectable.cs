using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
	public static HashSet<Selectable> allMySelectables = new HashSet<Selectable>();
	public static HashSet<Selectable> currentlySelected = new HashSet<Selectable>();

	// Main object
	[SerializeField] InputField mainInputField = null;

	// This object
	private bool focused = false;
	private Image thisImage = null;
	private InputField thisInputField = null;
	private Color startingColor;

	private void Awake()
	{
		allMySelectables.Add(this);
	}

	private void Start()
	{
		thisImage = GetComponent<Image>();
		thisInputField = GetComponent<InputField>();
		startingColor = thisImage.color;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			thisImage.color = Color.white;
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
				currentlySelected.Add(this);
				EditValues.allSelectedInputFields.Add(thisInputField);
			}

			thisImage.color = Color.yellow;
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

	private void Update()
	{
		if(currentlySelected.Contains(this))
		{
			thisImage.color = Color.yellow;
		}
		else
		{
			thisImage.color = startingColor;
		}

		if (mainInputField.gameObject.activeInHierarchy)
		{
			thisInputField.interactable = false;
		}
		else
		{
			thisInputField.interactable = true;
		}

		//if (currentlySelected.Count == 1)
		//{
		//	if (Input.GetKeyDown(KeyCode.Return) && thisImage.color == Color.yellow && !thisInputField.isFocused && !focused)
		//	{
		//		thisInputField.ActivateInputField();
		//		thisInputField.Select();
		//		focused = true;
		//	}
		//	else if (Input.GetKeyDown(KeyCode.Return) && thisInputField.isFocused || !focused)
		//	{
		//		thisInputField.DeactivateInputField();
		//		focused = false;
		//	}
		//}

		if (float.Parse(thisInputField.text) < 0)
		{
			thisInputField.text = "0";
		}
		else if (float.Parse(thisInputField.text) > 500)
		{
			thisInputField.text = "500";
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) && thisImage.color == Color.yellow && !mainInputField.gameObject.activeInHierarchy && thisInputField.isFocused
			|| Input.GetKeyDown(KeyCode.UpArrow) && thisImage.color == Color.yellow && currentlySelected.Count >= 2)
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
		else if (Input.GetKeyDown(KeyCode.DownArrow) && thisImage.color == Color.yellow && !mainInputField.gameObject.activeInHierarchy && thisInputField.isFocused
				 || Input.GetKeyDown(KeyCode.DownArrow) && thisImage.color == Color.yellow && currentlySelected.Count >= 2)
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
