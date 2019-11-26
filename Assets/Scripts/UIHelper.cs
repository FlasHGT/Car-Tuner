using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{

    public CanvasGroup checkActiveTabs;
    public GameObject graphicActive;
    public GameObject graphicPassive;

    [Header("Tabs to Disable")]
    public CanvasGroup[] tabsCanvasGroup;

	[SerializeField] Main main = null;

    private void Update()
    {
        if (checkActiveTabs.alpha == 1f)
        {
            graphicActive.SetActive(true);
            graphicPassive.SetActive(false);
            
        }
        else
        {
            graphicActive.SetActive(false);
            graphicPassive.SetActive(true);
        }
    }

    public void EnableMenu()
    {
		main.Deselect();
        checkActiveTabs.alpha = 1f;
		checkActiveTabs.blocksRaycasts = true;
		checkActiveTabs.interactable = true;

		if (checkActiveTabs.name == "T12")
		{
			The.currentArray = 0;
		}
		if (checkActiveTabs.name == "T3")
		{
			The.currentArray = 1;
		}
		
		foreach (CanvasGroup cgroup in tabsCanvasGroup)
		{
			cgroup.alpha = 0f;
			cgroup.blocksRaycasts = false;
			cgroup.interactable = false;
		}
    }
}
