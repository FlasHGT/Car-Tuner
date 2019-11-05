using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    public GameObject checkActive;
    public GameObject graphicActive;
    public GameObject graphicPassive;

    [Header("Tabs to Disable")]
    public GameObject tab1;

    private void Update()
    {
        if (checkActive.activeSelf)
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
            checkActive.SetActive(true);
            tab1.SetActive(false);
    }
}
