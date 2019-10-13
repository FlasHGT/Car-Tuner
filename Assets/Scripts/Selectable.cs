using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
	public Main main = null;

	public static HashSet<Selectable> allMySelectables = new HashSet<Selectable>();
	public static HashSet<Selectable> currentlySelected = new HashSet<Selectable>();

	private Image image = null;

	private void Awake ()
	{
		allMySelectables.Add(this);
	}

	private void Start ()
	{
		image = GetComponent<Image>();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if(!main.inputFieldBeingEdited)
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

		if(!main.inputFieldBeingEdited)
		{
			OnSelect(eventData);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if(!main.inputFieldBeingEdited)
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
}
