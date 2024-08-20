using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector3 initialPosition;
    private CardDisplay cardDisplay;
    private GameManager gameManager;

    private float dropZoneY = -30f; // dropZone cartas sin objetivo
    private float dragLeftLimit = -270f;
    private float dragRightLimit = 310f;  // contenedor donde se podra mover la carta
    private float dragTopLimit = 95f;

    private bool needTarget;


    void Start()
    {
        
        cardDisplay = GetComponent<CardDisplay>();
        gameManager = cardDisplay.gameManager;
        needTarget = cardDisplay.card.needTarget;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardDisplay.isInteractable && !cardDisplay.isShowed)
        {
            //if (cardDisplay.isHighlited) cardDisplay.HighlightCard(false);
            cardDisplay.HighlightCard(true);
            if (needTarget && gameManager.enemies.Count > 1) gameManager.cardWithTargetDragging = true;
            cardDisplay.isDragged = true;
            initialPosition = transform.position;
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cardDisplay.isInteractable && !cardDisplay.isShowed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));

            /*if (needTarget)
            {
                
            }
            else
            {*/
                SetCardPositionInContainer(mousePos);

                if (transform.position.y > dropZoneY && cardDisplay.canUse)
                {
                    cardDisplay.border.GetComponent<Image>().color = new Color(0, 0.45f, 1, 1);
                }
                else
                {
                    cardDisplay.SetBorder(cardDisplay.canUse);
                }
            /*} */
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cardDisplay.isDragged = false;
        if (cardDisplay.isInteractable && !cardDisplay.isShowed)
        {
            /*if (needTarget)
            {

            }
            else
            {*/
            if (needTarget) cardDisplay.gameManager.cardWithTargetDragging = false;

            if(cardDisplay.canUse && transform.position.y > dropZoneY && !(needTarget && cardDisplay.gameManager.enemyPointed == null && gameManager.enemies.Count > 1))
            {
                cardDisplay.UseCard();
            }
            
            else if (transform.position.y < dropZoneY || (needTarget && cardDisplay.gameManager.enemyPointed == null && gameManager.enemies.Count > 1))
                 {
                    if(needTarget && cardDisplay.gameManager.enemyPointed == null && gameManager.enemies.Count > 1) gameManager.ShowPlayerMessage("Target an Enemy...");
                    cardDisplay.soundManager.PlayCardSound(4, 0.1f);
                    transform.position = initialPosition;
                    cardDisplay.HighlightCard(false);
                    cardDisplay.SetBorder(cardDisplay.canUse);
                   
                 }
                else if(!cardDisplay.canUse)
                {
                    cardDisplay.soundManager.PlayCardSound(4, 0.1f);
                    transform.position = initialPosition;
                    cardDisplay.HighlightCard(false);
                    cardDisplay.SetBorder(cardDisplay.canUse);
                    cardDisplay.UseCard();
            }
            
           /* }*/
        }
    }

    

    void SetCardPositionInContainer(Vector3 mousePos) // no permite mover la carta fuera del contenedor
    {
        if (mousePos.x > dragLeftLimit && mousePos.x < dragRightLimit && mousePos.y < dragTopLimit)
        {
            transform.position = mousePos;
        }
        else
        {
            if (mousePos.y > dragTopLimit)
            {
                if (mousePos.x < dragLeftLimit)
                {
                    transform.position = new Vector3(dragLeftLimit, dragTopLimit, 10f);
                }
                else if (mousePos.x > dragRightLimit)
                {
                    transform.position = new Vector3(dragRightLimit, dragTopLimit, 10f);
                }
                else transform.position = new Vector3(mousePos.x, dragTopLimit, 10f);
            }
            else if (mousePos.x < dragLeftLimit)
            {
                transform.position = new Vector3(dragLeftLimit, mousePos.y, 10f);
            }
            else if (mousePos.x > dragRightLimit)
            {
                transform.position = new Vector3(dragRightLimit, mousePos.y, 10f);
            }
        }
    }
}
