using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
	public InputField mainInputField = null;
	public bool selectedBySelector = false;
	public Main main = null;

	public static HashSet<Selectable> allMySelectables = new HashSet<Selectable>();
	public static HashSet<Selectable> currentlySelected = new HashSet<Selectable>();

	private Image image = null;
	private InputField inputField = null;
	private Color startingColor;

	private void Awake ()
	{
		allMySelectables.Add(this);
	}

	private void Start ()
	{
		image = GetComponent<Image>();
		inputField = GetComponent<InputField>();
		startingColor = image.color;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if(!mainInputField.gameObject.activeInHierarchy && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			image.color = Color.white;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			DeselectAll(eventData);
		}

		if(!mainInputField.gameObject.activeInHierarchy)
		{
			OnSelect(eventData);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if(!mainInputField.gameObject.activeInHierarchy)
		{
			currentlySelected.Add(this);
			image.color = Color.yellow;
		}
	}

	public static void DeselectAll (BaseEventData eventData)
	{
		foreach (Selectable selectable in currentlySelected)
		{
			selectable.OnDeselect(eventData);
		}
		currentlySelected.Clear();
	}

	private void Update ()
	{
		if(!selectedBySelector && main.currentlyActiveInputField && main.currentlyActiveInputField.gameObject.activeInHierarchy)
		{
			image.color = startingColor;
		}

		if (mainInputField.gameObject.activeInHierarchy)
		{
			if(image.color == Color.yellow && !selectedBySelector)
			{
				image.color = startingColor;
			}

			inputField.interactable = false;
		}
		else if(image.color != Color.yellow)
		{
			image.color = startingColor;
			inputField.interactable = true;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) && image.color == Color.yellow && !mainInputField.gameObject.activeInHierarchy)
		{
			float newFloat = 0f;

			if (inputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
			}
			else
			{
				newFloat = float.Parse(inputField.text) + 0.1f;
			}

			inputField.text = newFloat.ToString();
			inputField.textComponent.text = newFloat.ToString();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) && image.color == Color.yellow && !mainInputField.gameObject.activeInHierarchy)
		{
			float newFloat = 0f;

			if (inputField.text == string.Empty)
			{
				newFloat = 0f + 0.1f;
			}
			else
			{
				newFloat = float.Parse(inputField.text) - 0.1f;
			}

			inputField.text = newFloat.ToString();
			inputField.textComponent.text = newFloat.ToString();
		}
	}
}
