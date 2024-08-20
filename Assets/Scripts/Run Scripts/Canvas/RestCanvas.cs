using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestCanvas : CanvasScript
{
    public GameObject Button1;
    public GameObject Button2;
    public GameObject endGameContainer;

    void Start()
    {
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
    }

    public void RestHeal(bool action)
    {
        runManager.RestHeal(action);

        Button1.SetActive(false);
        Button2.SetActive(false);
        endGameContainer.SetActive(true);
    }
}
