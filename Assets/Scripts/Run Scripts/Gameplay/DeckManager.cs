using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DeckManager : MonoBehaviour
{
    public static DeckManager deckManager;

    public List<Card> cardsList;
    public List<Card> initialCards;
    public List<Card> commonCards;
    public List<Card> specialCards;
    public List<Card> epicCards;
    public List<Card> legendaryCards;

    public List<Card> initialDeck;
    public List<Card> deckList;


    List<List<Card>> cardList = new List<List<Card>>();

    // Start is called before the first frame update
    void Awake()
    {
        if (deckManager == null)
        {
            deckManager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        InitialDeck();
    }

    // Update is called once per frame
    void Start()
    {
        cardList.Add(initialCards);
        cardList.Add(commonCards);
        cardList.Add(specialCards);
        cardList.Add(epicCards);
        cardList.Add(legendaryCards);
    }

    void InitialDeck()
    {
        deckList = new List<Card>(initialDeck);
    }

    public void AddCard(int rarity, int index)
    {
        deckList.Add(cardList[rarity][index]);
    }

}
