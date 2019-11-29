using UnityEngine;
using UnityEngine.UI;

public class DragSelection : MonoBehaviour
{
	[SerializeField] Main main = null;

	[SerializeField] GUIStyle style = null;

	private bool draggingHasBegun = false;

	private Vector2 startPos = Vector2.zero;
	private Vector2 endPos = Vector2.zero;

	private Vector2 boxStart = Vector2.zero;
	private Vector2 boxFinish = Vector2.zero;

	private float boxWidth = 0f;
	private float boxHeight = 0f;
	private float boxLeft = 0f;
	private float boxTop = 0f;

	private Rect selectionBox = new Rect();

	private void Update ()
	{
		if (Input.GetMouseButton(0) && draggingHasBegun)
		{
			Dragging();
		}

		if (Input.GetMouseButtonDown(0))
		{
			startPos = Input.mousePosition;
			draggingHasBegun = true;
		}

		if (Input.GetMouseButtonUp(0))
		{
			selectionBox = Rect.zero;
			SelectSelectables();
			draggingHasBegun = false;
		}
	}

	private void SelectSelectables ()
	{
		main.CheckTheActiveDataTable();

		foreach (InputField i in main.currentDataTable)
		{
			Vector3[] corners = new Vector3[4];

			i.GetComponent<RectTransform>().GetWorldCorners(corners);

			foreach (Vector3 v in corners)
			{
				if ((v.x > boxStart.x && v.y < boxStart.y) && (v.x < boxFinish.x && v.y > boxFinish.y))
				{
					Selectable.currentlySelected.Add(i.GetComponent<Selectable>());
					EditValues.allSelectedInputFields.Add(i.gameObject.GetComponent<InputField>());
				}
			}
		}
	}

	private void Dragging()
	{
		endPos = Input.mousePosition;

		boxWidth = startPos.x - endPos.x;
		boxHeight = startPos.y - endPos.y;
		boxLeft = endPos.x;
		boxTop = (Screen.height - endPos.y) - boxHeight;

		if (boxWidth > 0f && boxHeight < 0f)
		{
			boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}
		else if (boxWidth > 0f && boxHeight > 0f)
		{
			boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
		}
		else if (boxWidth < 0f && boxHeight < 0f)
		{
			boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
		}
		else if (boxWidth < 0f && boxHeight > 0f)
		{
			boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
		}

		boxFinish = new Vector2(boxStart.x + Unsigned(boxWidth), boxStart.y - Unsigned(boxHeight));

		selectionBox = new Rect(boxLeft, boxTop, boxWidth, boxHeight);
	}

	float Unsigned(float val)
	{
		if (val < 0f)
		{
			val *= -1;
		}

		return val;
	}

	private void OnGUI()
	{
		if (draggingHasBegun)
		{
			GUI.Box(selectionBox, "", style);
		}
	}
}
