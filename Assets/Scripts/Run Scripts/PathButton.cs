using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PathButton : MonoBehaviour
{
    public PathManager pathManager;
    public int pathLevel;
    public int pathLevelIndex;
    public bool isInteractable;
    // Start is called before the first frame update
    private void OnMouseDown()
    {
        if (isInteractable)
        {
            pathManager.SelectPath(pathLevel, pathLevelIndex);
        }
    }

    private void OnMouseEnter()
    {
        if (isInteractable)
        {
            
        }
    }

    private void OnMouseExit()
    {
        if (isInteractable)
        {
           
        }
    }
}
