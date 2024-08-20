using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameButton : MonoBehaviour
{
    public RunManager runManager;
    public string type;
    // Start is called before the first frame update
    public void Press()
    {
        runManager.EndGameButton(type);
    }
}
