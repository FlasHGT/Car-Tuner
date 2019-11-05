using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSelection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	[SerializeField] Image selectionBoxImage = null;

	private Image image = null;
	private float temps = 0f;
	private Vector2 startPosition;
	private Rect selectionRect;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			Selectable.DeselectAll(new BaseEventData(EventSystem.current));
		}

		selectionBoxImage.gameObject.SetActive(true);
		startPosition = eventData.position;
		selectionRect = new Rect();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.position.x < startPosition.x)
		{
			selectionRect.xMin = eventData.position.x;
			selectionRect.xMax = startPosition.x;
		}
		else
		{
			selectionRect.xMin = startPosition.x;
			selectionRect.xMax = eventData.position.x;
		}

		if (eventData.position.y < startPosition.y)
		{
			selectionRect.yMin = eventData.position.y;
			selectionRect.yMax = startPosition.y;
		}
		else
		{
			selectionRect.yMin = startPosition.y;
			selectionRect.yMax = eventData.position.y;
		}

		selectionBoxImage.rectTransform.offsetMin = selectionRect.min;
		selectionBoxImage.rectTransform.offsetMax = selectionRect.max;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		selectionBoxImage.gameObject.SetActive(false);

		foreach (Selectable selectable in Selectable.allMySelectables)
		{
			if (selectionRect.Contains(Camera.main.WorldToScreenPoint(selectable.transform.position)))
			{
				selectable.OnSelect(eventData);
			}
		}

		image.raycastTarget = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, results);

		float myDistance = 0;

		foreach (RaycastResult result in results)
		{
			if (result.gameObject == gameObject)
			{
				myDistance = result.distance;
				break;
			}
		}

		GameObject nextObject = null;
		float maxDistance = Mathf.Infinity;

		foreach (RaycastResult result in results)
		{
			if (result.distance > myDistance && result.distance < maxDistance)
			{
				nextObject = result.gameObject;
				maxDistance = result.distance;
			}
		}

		if (nextObject)
		{
			ExecuteEvents.Execute<IPointerClickHandler>(nextObject, eventData, (x, y) => { x.OnPointerClick((PointerEventData)y); });
		}
	}
	
	private void Start ()
	{
		image = GetComponent<Image>();
	}

	private void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			temps = Time.time;
		}

		if (Input.GetMouseButton(0) && (Time.time - temps) > 0.2)
		{
			image.raycastTarget = true;
		}
	}
}
