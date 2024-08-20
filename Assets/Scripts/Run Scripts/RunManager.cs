using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    [Header("Player stats")]
    public static RunManager runManager;
    public int energy;
    //public int hp;
    public int cardsHurt;
    public int shieldAtTurnBegin;
    public float maxHP;
    public float currentHP;
    // public int attackBuff;
    // public list<gem> gemsList;

    // CADA LISTA REPRESENTA LOS INDICES DE LOS ENEMIGOS DE ESA PELEA (REPRESENTADOS POR SU PREFAB, ALMACENADO EN (ETAPA)PREFABS)
    public List<GameObject> enemy11Prefabs;
    List<List<int>> enemy11PrefabsIndex = new List<List<int>> { new List<int> { 0 }, new List<int> { 1 }, new List<int> { 2 , 3 }, new List<int> { 4, 5, 6 }, new List<int> { 7 } };
    
    public List<GameObject> enemy12Prefabs;
    List<List<int>> enemy12PrefabsIndex = new List<List<int>> { new List<int> { 0 }, new List<int> { 1 }, new List<int> { 2 }, new List<int> { 3 }, new List<int> { 4, 5 } };
    
    public GameObject boss1Prefab;
    public GameObject carta;

    public List<GameObject> actualEnemies = new List<GameObject>();

    public string scrollScene;
    public string restScene;
    public GameObject finalBoss;

    public int actualFloor;
    public int actualStage;
    public DeckManager deckManager;
    private PathManager pathManager;
    public SoundManager soundManager;

    void Awake()
    {
        if (runManager == null)
        {
            runManager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        pathManager = GameObject.Find("PathManager").GetComponent<PathManager>();
        actualFloor = 0;
        actualStage = 1;
        currentHP = maxHP;
        pathManager.GeneratePath(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            // MANTENER ACTUALIZADO EL GAMEBAR PARA TODAS LAS ESCENAS QUE LO CONTENGAN
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().floorText.text = actualFloor.ToString();
        }
        if(scene.name == "RunNavigator")
        {
            if (actualFloor >= 3 && actualFloor < 7) Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -65 - 75 * (actualFloor - 3), 0);
            else if(actualFloor >= 7) Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -65 - 75 * 3, 0);
            Invoke(nameof(DisplayPath), 0.01f);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void DisplayPath()
    {
        pathManager.DisplayPath();
    }


    public void BattleContinueGame()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(gameManager.battleState == BattleState.WON && actualFloor != 10)
        {
            SceneManager.LoadScene("RunNavigator");

        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(GameObject.Find("PathManager"));
            Destroy(GameObject.Find("RunManager"));
        }
        actualEnemies.Clear();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("RunNavigator");
    }

    public void GoToLevel(string type, int level)
    {
        actualEnemies.Clear();
        if (type == "enemy1.1")
        {
            //actualEnemy = enemy11Prefabs[Random.Range(0, enemy11Prefabs.Count)];
            int enemy = Random.Range(0, enemy11PrefabsIndex.Count);
            foreach (int i in enemy11PrefabsIndex[enemy])
            {
                actualEnemies.Add(enemy11Prefabs[i]);
            }
            enemy11PrefabsIndex.RemoveAt(enemy);

            SceneManager.LoadScene("enemyStage1");
        }
        else if (type == "enemy1.2")
        {
            //actualEnemy = enemy12Prefabs[Random.Range(0, enemy12Prefabs.Count)];
            int enemy = Random.Range(0, enemy11PrefabsIndex.Count);
            foreach (int i in enemy12PrefabsIndex[enemy])
            {
                actualEnemies.Add(enemy12Prefabs[i]);
            }
            enemy12PrefabsIndex.RemoveAt(enemy);

            SceneManager.LoadScene("enemyStage1");
        }
        else if(type == "boss1")
        {
            actualEnemies.Add(boss1Prefab);
            SceneManager.LoadScene("enemyStage1");
        }
        else if(type == "scroll")
        {
            SceneManager.LoadScene(scrollScene);
        }
        else if(type == "campfire")
        {
            SceneManager.LoadScene(restScene);
        }
    }


    public void RestHeal(bool option)
    {
        if(option == true)
        {
            maxHP += 10f;
            currentHP += 10f;
        }
        else
        {
            currentHP += 30f;
        }

        GameObject.Find("Character").GetComponent<Animator>().Rebind();
        GameObject.Find("Character").GetComponent<Animator>().Play("CharacterHeal");
        GameObject.Find("Character").GetComponent<PlayerCharacter>().RefreshHP();
        
    }

    public void EndGameButton(string type)
    {
        switch (type)
        {
            case "addCard":
                AddCard();
                break;
        }
    }

    void AddCard()
    {
        //deckManager.AddCard(1, 1);
    }
}
