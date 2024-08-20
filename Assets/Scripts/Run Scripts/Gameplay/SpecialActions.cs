using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialActions : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
    {
        
    }

    void GetGameManager()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void PlaySpecialAction(string action, int value)
    {
        GetGameManager();
        switch (action)
        {
            case "StealCard":
                StartCoroutine("StealCard", value);
                break;
            case "IncrementShieldAtTurnBegin":
                IncrementShieldAtTurnBegin(value);
                break;
            default:
                break;
        }
    }

    IEnumerator StealCard(int value)
    {
        for (int i = 0; i < value; i++)
        {
            gameManager.InteractuableButtons(false);
            yield return StartCoroutine(gameManager.StealCard());
            gameManager.InteractuableButtons(true);
        }   
    }
    
    void IncrementShieldAtTurnBegin(int value)
    {
        gameManager.shieldAtTurnBegin += value;
    }
}
