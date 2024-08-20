using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvasManager : CanvasScript
{
    GameManager gameManager;
    public GameObject ShowCardLayer;
    public GameObject ShowThingLayer;
    public GameObject stackShowContainer;
    public TMP_Text stackShowType;
    public GameObject stackShowCardContainer;
    void Start()
    {
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void EndTurn()
    {
       StartCoroutine(gameManager.EndTurn());
    }

    public new void Options()
    {
       /* if(!optionsOpen)
        {
            StopShowing();
        }   */

        optionsOpen =! optionsOpen;
        optionsContainer.SetActive(optionsOpen);
        optionsLayer.SetActive(optionsOpen);
        gameManager.InteractuableButtons(!optionsOpen);
    }

    public void BattleContinueGame()
    {
        runManager.BattleContinueGame();
    }

    public void StopShowing()
    {
        if(gameManager.cardShowed) gameManager.StopShowingCard();
        else if(stackShowContainer.activeSelf)
        {
            stackShowContainer.SetActive(false);
            ShowThingLayer.SetActive(false);
            gameManager.InteractuableButtons(true);
            foreach (Transform child in stackShowCardContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ShowStack(int type)
    {
        

        List<Card> stack = new List<Card>();
        if (type == 1)
        {
         stack  = gameManager.GetExtractStack();
         stackShowType.text = "Extract Stack";
        }
        else if(type == 2)
        {
           stack = gameManager.GetDiscardStack();
           stackShowType.text = "Discard Stack";
        }

        if (stack.Count == 0)
        {
            gameManager.ShowPlayerMessage("Empty Stack");
            return;
        }

        float initialX = -630f;
        gameManager.InteractuableButtons(false);

        stack = stack.OrderByDescending(card => card.cardName).ToList();

        for (int i = 0; i < stack.Count; i++)
        {
            CardDisplay cartaSeleccionada = gameManager.carta.GetComponent<CardDisplay>(); // cambiar la carta en el prefab
            cartaSeleccionada.card = stack[i];

            GameObject newCard = Instantiate(gameManager.carta, new Vector3(0, 0, 0), Quaternion.identity, stackShowCardContainer.transform);

            newCard.transform.localPosition = new Vector3(initialX + ((i % 8) * 180), -(i / 8 * 240), 0);
            newCard.GetComponent<CardDisplay>().cardCanvas.GetComponent<Canvas>().sortingOrder = 3;
            newCard.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            newCard.GetComponent<CardDisplay>().SetCanShow(true);

            stackShowContainer.SetActive(true);
            ShowThingLayer.SetActive(true);
        }
    }

   
}
