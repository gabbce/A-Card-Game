using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card card;

    public GameObject cardName;
    public GameObject description;
    public GameObject type;
    public GameObject energy;
    public GameObject artWork;
    public GameObject border;
    public int handID;

    public GameManager gameManager;
    public GameObject cardCanvas;

    public bool canUse;

    public bool isInteractable;
    public bool canShow;
    public bool isHighlited;
    public bool isDragged;
    public bool isShowed;
    private Vector3 cardPosition;
    private Vector3 cardSize;
    private Vector3 normalScale = new Vector3(2f, 2f, 1);
    private Vector3 highlightScale = new Vector3(2.5f, 2.5f, 1);
    private Vector3 showScale = new Vector3(3.2f, 3.2f, 1);


    public SoundManager soundManager;

    void Start()
    {
        canUse = true;
        isHighlited = false;
        isInteractable = false;
        isDragged = false;
        isShowed = false;
        cardName.GetComponent<TMP_Text>().text = card.cardName;
        description.GetComponent<TMP_Text>().text = card.description;
        type.GetComponent<TMP_Text>().text = card.type;
        energy.GetComponent<TMP_Text>().text = card.energy.ToString();
        artWork.GetComponent<Image>().sprite = card.artwork;
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    } 

    public void CheckEnergy(int currentEnergy)
    {
        if (card.energy > currentEnergy)
        {
            canUse = false;
            SetBorder(canUse);
        }
        else
        {
           canUse = true;
           SetBorder(canUse);
        }
    }

    public void SetBorder(bool state)
    {
        if (state)
        {
            border.GetComponent<Image>().color = new Color(0, 255, 227, 201);
            energy.GetComponent<TMP_Text>().color = Color.black;
        }
        else
        {
            border.GetComponent<Image>().color = Color.black;
            energy.GetComponent<TMP_Text>().color = Color.red;
        }
    }

    public void SetNormalScale()
    {
        transform.localScale = normalScale;
    }

    public void SetCanShow(bool state)
    {
        canShow = state;
    }

    public void UseCard()
    {
        StartCoroutine(gameManager.UseCard(handID, gameManager.enemyPointed));
    }

    public void HighlightCard(bool state)
    {
        
        if(state && !isHighlited && !isShowed)
        {
            cardCanvas.GetComponent<Canvas>().sortingOrder = 3;
            transform.localScale = highlightScale;
            cardPosition = transform.localPosition;
            transform.localPosition = cardPosition + new Vector3(0f, 35f, 0f);
            isHighlited = true;
           
        }
        else if (!state && isHighlited && !isDragged)
        {
            cardCanvas.GetComponent<Canvas>().sortingOrder = 1;
            transform.localScale = normalScale;
            transform.localPosition = cardPosition;
            isHighlited = false;
            
        }
        
    }

    public void ShowCard(bool show)
    {
        if (show && !isShowed)
        {
            if(isHighlited) HighlightCard(false);
            isShowed = true;
            gameManager.cardShowed = transform.gameObject;
            soundManager.PlayCardSound(4, 0.1f);

            gameManager.InteractuableButtons(false);
            cardPosition = transform.position;
            cardSize = transform.localScale;
            transform.position = new Vector3(0, 19, transform.position.z);
            transform.localScale = showScale;
            cardCanvas.GetComponent<Canvas>().sortingOrder = 5;
            GameObject.Find("GameCanvas").GetComponent<GameCanvasManager>().ShowCardLayer.SetActive(true);
        }
        else
        {
            soundManager.PlayCardSound(4, 0.1f);
            GameObject.Find("GameCanvas").GetComponent<GameCanvasManager>().ShowCardLayer.SetActive(false);
            
            if(canShow == isInteractable) cardCanvas.GetComponent<Canvas>().sortingOrder = 2;
            else cardCanvas.GetComponent<Canvas>().sortingOrder = 3;

            gameManager.cardShowed = null;
            transform.position = cardPosition;
            transform.localScale = cardSize;
            isShowed = false;
            gameManager.InteractuableButtons(true);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isInteractable) HighlightCard(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInteractable) HighlightCard(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canShow && !isDragged)
        {
            ShowCard(true);
        }
    }
}
