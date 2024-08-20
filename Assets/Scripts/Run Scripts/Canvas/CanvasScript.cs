using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    public RunManager runManager;
    public GameObject optionsContainer;
    public GameObject optionsLayer;
    public TMP_Text floorText;
    public bool optionsOpen = false;

    void Start()
    {
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
    }

    public void ContinueGame()
    {
        runManager.ContinueGame();
        runManager.soundManager.PlayGameSound(2, 0.1f);
    }

    public void Options()
    {
        runManager.soundManager.PlayGameSound(2, 0.1f);
        optionsOpen = !optionsOpen;
        optionsContainer.SetActive(optionsOpen);
        optionsLayer.SetActive(optionsOpen);
    }

    public void QuitGame()
    {
        Destroy(GameObject.Find("RunManager"));
        Destroy(GameObject.Find("PathManager"));
        SceneManager.LoadScene("MainMenu");
    }

    
    
}
