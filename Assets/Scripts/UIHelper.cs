using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    public GameObject checkActiveTabs;
    public GameObject graphicActive;
    public GameObject graphicPassive;

    [Header("Tabs to Disable")]
    public GameObject[] tabs;

    private void Update()
    {
        if (checkActiveTabs.activeSelf)
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
        checkActiveTabs.SetActive(true);
		foreach(GameObject obj in tabs)
		{
			obj.SetActive(false);
		}
    }
}
