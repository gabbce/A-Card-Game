using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Drawing;
using Color = UnityEngine.Color;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class GameManager : MonoBehaviour
{
    public static GameManager instanceManager;

    [Header("GameObjects")]
    public GameObject carta;
    public RunManager runManager;
    public DeckManager deckManager;
    public GameObject character;
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject cardContainer;
    public GameObject EndGameButton;

    [Header("UI")]
    public TMP_Text currentEnergyUI;
    public TMP_Text totalEnergyUI;
    public TMP_Text turnUI;
    public Button endTurnButton;
    public GameObject endTurnContainer;
    public TMP_Text endTurnText;
    public TMP_Text endGameUI;
    public GameObject endGameContainer;
    public GameObject endButtonContainer;
    public TMP_Text extractStackUI;
    public Button extractStackButton;
    public TMP_Text discardStackUI;
    public Button discardStackButton;
    public TMP_Text playerMessage;
    


    List<Card> extractStack;
    List<Card> discardStack;
    List<GameObject> cardsHand = new List<GameObject>();
    int totalEnergy;
    int currentEnergy;
    int cardsHurt;
    GameObject actualCardGO;

    Vector3 initialPosition = new Vector3(0, -124, 1); // posicion primer carta (en el medio)

    private int turn;
    public BattleState battleState;
    public GameObject cardShowed;
    public bool cardWithTargetDragging;
    public GameObject enemyPointed;

    private Animator animCanvas;
    private SoundManager soundManager;
    private SpecialActions specialActions;
    public int shieldAtTurnBegin;
    private int handID;

    private void Awake()
    {
        instanceManager = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        specialActions = GameObject.Find("RunManager").GetComponent<SpecialActions>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
        deckManager = GameObject.Find("RunManager").GetComponent<DeckManager>();

        shieldAtTurnBegin = runManager.shieldAtTurnBegin;
        cardsHurt = runManager.cardsHurt;
        totalEnergy = runManager.energy;

        extractStackUI.text = deckManager.deckList.Count.ToString();
        discardStackUI.text = "0";

        animCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animator>();
        
        cardShowed = null;
        enemyPointed = null;
        cardWithTargetDragging = false;

        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public IEnumerator SetupBattle()
    {
        battleState = BattleState.START;
        handID = 0;
        turn = 0;
        RefreshUI();
        discardStack = deckManager.deckList.ToList<Card>();
        enemies.Clear();

        foreach (GameObject enemy in runManager.actualEnemies)
        {
            enemies.Add(Instantiate(enemy));
        }

        InteractuableButtons(false);

        yield return new WaitForSeconds(0.5f);

        endTurnText.text = "START BATTLE";
        endTurnContainer.SetActive(true);
        battleState = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(3f);
        endTurnContainer.SetActive(false);

        turn++;

        RefreshUI();
        currentEnergy = totalEnergy;

        yield return StartCoroutine(nameof(ShuffleStack));


        cardsHand.Clear();

        yield return new WaitForSeconds(0.5f);

        
        StartCoroutine(BeginTurnStealCards());

        
        

    }

    public IEnumerator EndTurn()
    {

        if (animCanvas.GetCurrentAnimatorStateInfo(0).IsName("ShowPlayerMessage"))
        {
            animCanvas.Rebind();
        }

        soundManager.PlayGameSound(2, 0.1f);
        InteractuableButtons(false);

        foreach (GameObject card in cardsHand)
        {

            Discard(card, card.GetComponent<CardDisplay>().card);
            yield return new WaitForSeconds(0.25f);

        }

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().RefreshShield();
        }

        endTurnText.text = "ENEMY TURN";
        endTurnContainer.SetActive(true);

        yield return new WaitForSeconds(2f);

        endTurnContainer.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(nameof(EnemyTurn));


        if (battleState == BattleState.LOST)
        {
            yield break;
        }

        character.GetComponent<PlayerCharacter>().RefreshShield(0);

        yield return new WaitForSeconds(1f);

        battleState = BattleState.PLAYERTURN;
        endTurnText.text = "PLAYER TURN";
        endTurnContainer.SetActive(true);
        character.GetComponent<PlayerCharacter>().RefreshShield(shieldAtTurnBegin);

        yield return new WaitForSeconds(2f);

        turn++;

        RefreshUI();
        currentEnergy = totalEnergy;

        cardsHand.Clear();
        handID = 0;

        endTurnContainer.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(BeginTurnStealCards());

       
        
    }

    IEnumerator BeginTurnStealCards()
    {
        InteractuableButtons(false);
        for (int i = 0; i < cardsHurt; i++)
        {
            yield return StartCoroutine(nameof(StealCard));
        }
        InteractuableButtons(true);
    }

    public void RefreshUI()
    {
        currentEnergyUI.text = totalEnergy.ToString();
        totalEnergyUI.text = totalEnergy.ToString();
        turnUI.text = turn.ToString();

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().RefreshIntention(turn);
        }
    }

    public IEnumerator StealCard()
    {
        CardDisplay cartaSeleccionada = carta.GetComponent<CardDisplay>(); // cambiar la carta en el prefab
        cartaSeleccionada.card = extractStack.First();
        cartaSeleccionada.gameManager = this;
        cartaSeleccionada.handID = handID;
        handID++;

        soundManager.PlayGameSound(0, 0.5f);

        yield return new WaitForSeconds(0.1f);

        extractStack.RemoveAt(0);
        extractStackUI.text = extractStack.Count.ToString();

        GameObject newCard = Instantiate(carta, new Vector3(-500f, -300f, 0), Quaternion.identity, cardContainer.transform);

        newCard.GetComponent<CardDisplay>().SetCanShow(false);
        newCard.GetComponent<CardDisplay>().SetNormalScale();
        cardsHand.Add(newCard);

        // play animation

        ArrangeCards();
        newCard.GetComponent<CardDisplay>().CheckEnergy(currentEnergy);

        yield return new WaitForSeconds(0.2f);

        if (extractStack.Count == 0)
        {
            yield return StartCoroutine(nameof(ShuffleStack));
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ShuffleStack()
    {
        //extractStack.Clear();
        soundManager.PlayGameSound(1, 0.5f);
        extractStack = ShuffleList(discardStack).ToList<Card>();
        discardStack.Clear();
        discardStackUI.text = discardStack.Count.ToString();
        yield return new WaitForSeconds(0.5f);
        extractStackUI.text = extractStack.Count.ToString();
    }


    public IEnumerator UseCard(int handID, GameObject enemyObjetive)
    {
        if (animCanvas.GetCurrentAnimatorStateInfo(0).IsName("ShowPlayerMessage"))
        {
            animCanvas.Rebind();
        }

        int i = 0;
        while (cardsHand[i].GetComponent<CardDisplay>().handID != handID)
        {
            i++;
        }

        actualCardGO = cardsHand[i];
        Card actualCard = actualCardGO.GetComponent<CardDisplay>().card;
        
        if(actualCardGO.GetComponent<CardDisplay>().canUse)
        {
            currentEnergy -= actualCard.energy;
            currentEnergyUI.text = currentEnergy.ToString();


            if(actualCard.type == "Attack")
                UseAttackCard(actualCard, enemyObjetive);
                
            else if(actualCard.type == "Ability")
                UseAbilityCard(actualCard);
            else
                UsePowerCard(actualCard);


            cardsHand.Remove(actualCardGO);

            if (actualCard.burnAtUse) BurnCard(actualCardGO, actualCard);
            else Discard(actualCardGO, actualCard);
            
            actualCardGO = null;

            //yield return new WaitForSeconds(0.2f);

            CheckEnergy();

            ArrangeCards();

            if(enemies.Count == 0)
            {
                FinalizeGame(true);
            }

        }
        
        else
        {
            ShowPlayerMessage("Insufficient energy...");
        }
        yield break;
    }

    void UseAttackCard(Card card, GameObject enemy)
    {
        soundManager.PlayCardSound(0, 0.4f);
        if(card.shield > 0) character.GetComponent<PlayerCharacter>().GetShield(card.shield);
        
        if(card.needTarget && enemies.Count > 1)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            bool alive = enemyScript.GetDamage(card.attack);
            if (!alive)
            {
                enemyScript.Kill();
                enemies.Remove(enemy);
            }

            PlaySpecialActions(card);
        }
        else
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                Enemy enemyScript = enemies[i].GetComponent<Enemy>();

                bool alive = enemyScript.GetDamage(card.attack);
                if (!alive)
                {
                    enemyScript.Kill();
                    enemies.Remove(enemies[i]);
                    i--;
                }

                PlaySpecialActions(card);
            }
        }

        
    }

    void UseAbilityCard(Card card)
    {
        soundManager.PlayCardSound(1, 0.25f);
        character.GetComponent<PlayerCharacter>().GetShield(card.shield);
        PlaySpecialActions(card);
    }

    void UsePowerCard(Card card)
    {
        soundManager.PlayCardSound(2, 0.5f);
        PlaySpecialActions(card);
    }

    void Discard(GameObject cardGO, Card card)
    {
        soundManager.PlayCardSound(4, 0.3f);
        discardStack.Add(card);
        discardStackUI.text = discardStack.Count.ToString();
        Destroy(cardGO); 
    }

    void BurnCard(GameObject cardGO, Card card)
    {
        Destroy(cardGO);
    }

    void PlaySpecialActions(Card card)
    {
        for (int i = 0; i < card.specialActions.Count; i++)
        {
            specialActions.PlaySpecialAction(card.specialActions[i], card.specialActionsValue[i]);
        }
    }

    void ArrangeCards()
    {
        float offsetX = 240f - 10 * cardsHand.Count;
        float offsetY = 10f;

        if (cardsHand.Count == 1) cardsHand[0].transform.localPosition = new Vector3(0, 0, 0);
        else if(cardsHand.Count == 2)
        {
            cardsHand[0].transform.localPosition = new Vector3(-offsetX / 2f, 0, 0);
            cardsHand[1].transform.localPosition = new Vector3(offsetX / 2f, 0, 0);
        }
        else if(cardsHand.Count % 2 == 0)
        {
            
            for (int i = 0; i < cardsHand.Count; i++)
            {
                float posX = (offsetX / 2) - ((cardsHand.Count / 2) * offsetX) + (offsetX * i);

                float posY;
                if (i < (cardsHand.Count / 2)) posY = offsetY - (((cardsHand.Count / 2) - i - 1) * offsetY);
                else posY = offsetY - ((i - (cardsHand.Count / 2)) * offsetY);

                cardsHand[i].transform.localPosition = new Vector3(posX, posY, 0) ;
            }
        }
        else
        {
            for (int i = 0; i < cardsHand.Count; i++)
            {
                float posX = - ((cardsHand.Count / 2) * offsetX) + (offsetX * i);
                float posY = offsetY - (Mathf.Abs((cardsHand.Count / 2) - i) * offsetY);
                cardsHand[i].transform.localPosition = new Vector3(posX, posY, 0);

            }
        }
    }

    void CheckEnergy()
    {
        foreach (GameObject card in cardsHand)
        {
            card.GetComponent<CardDisplay>().CheckEnergy(currentEnergy);
        }   
    }

    public IEnumerator EnemyTurn()
    {
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            string enemyIntention = enemyScript.intentionType[turn];
            int enemyActionValue = enemyScript.intentionValue[turn];

            // mensaje ENEMY TURN
            battleState = BattleState.ENEMYTURN;

            enemyScript.intention.text = "";

            if (enemyIntention == "A")
               yield return StartCoroutine(nameof(EnemyAttack), enemyActionValue);

            else if (enemyIntention == "D")
                yield return StartCoroutine(enemyScript.GetShield(enemyActionValue));

          
            if (battleState == BattleState.LOST)
            {
                yield break ;
            }
        }

        
    }


    IEnumerator EnemyAttack(int damage)
    {
        PlayerCharacter characterScript = character.GetComponent<PlayerCharacter>();

        soundManager.PlayEnemySound(0, 0.4f);

        bool alive = characterScript.GetDamage(damage);

       
        yield return new WaitForSeconds(1.5f);

        if(!alive)
        {
            FinalizeGame(false);
        }
    }

    void FinalizeGame(bool result)
    {
        if (result)
        {
            battleState = BattleState.WON;
        }
        else
        {
            battleState = BattleState.LOST;
        }
        
        InteractuableButtons(false);

        if (result)
        {
            if (runManager.actualFloor == 10) endGameUI.text = "Enemy defeat\nYou win!!";
            else
            {
                endGameUI.text = "Enemy defeat";
               /* EndGameButton.GetComponent<EndGameButton>().type = "AddCard";
                EndGameButton.GetComponent<EndGameButton>().runManager = runManager;
                Instantiate(EndGameButton, new Vector3(0, 20f, 0), Quaternion.identity, endButtonContainer.transform);*/
            }

        }
        else {
            endGameUI.text = "You died";
        }

        endGameContainer.SetActive(true);
    }

    private void DestroyGO()
    {
        Destroy(gameObject);
    }

    public List<Card> ShuffleList(List<Card> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Card temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

    public void InteractuableButtons(bool state)
    {
        foreach (GameObject card in cardsHand)
        {
            if (card != cardShowed)
            {
                card.GetComponent<CardDisplay>().isInteractable = state;
                card.GetComponent<CardDisplay>().canShow = state;

                if (state) card.GetComponent<CardDisplay>().cardCanvas.GetComponent<Canvas>().sortingOrder = 1;
                else card.GetComponent<CardDisplay>().cardCanvas.GetComponent<Canvas>().sortingOrder = 0;
            }
        }
        endTurnButton.interactable = state;
        extractStackButton.interactable = state;
        discardStackButton.interactable = state;
    }

    
    public void StopShowingCard()
    {
        if(cardShowed != null)
        {
            cardShowed.GetComponent<CardDisplay>().ShowCard(false);
        }
    }

    public List<Card> GetExtractStack()
    {
        return extractStack.ToList();
    }
    public List<Card> GetDiscardStack()
    {
        return discardStack.ToList();
    }

    public void ShowPlayerMessage(string message)
    {
        playerMessage.text = message;
        animCanvas.Rebind();
        animCanvas.Play("ShowPlayerMessage");
    }
}
