using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class PathManager : MonoBehaviour
{
    public static PathManager pathManager;
    RunManager runManager;

    [SerializeField] private Sprite[] pathSprites;
    // 1 = enemy, 2 = scroll, 3 = campfire, 4 = boss, 5 = random
    Dictionary<string, int> spriteType = new Dictionary<string, int> { { "enemy1.1", 1 }, { "enemy1.2", 1 }, { "scroll", 2 }, { "campfire", 3 }, { "boss1", 4 }, { "random", 5 } };

    List<List<string>> levelPosibleEncounters = new List<List<string>> { new List<string> { "enemy1.1" }, new List<string> { "enemy1.1" }, 
                                                                           new List<string> {/*"scroll",*/ "campfire" },  new List<string>{ "enemy1.1" }, new List<string>{ "enemy1.2" }, 
                                                                            new List<string> { "campfire" }, new List<string>{ "enemy1.2" }, new List<string>{ "enemy1.2" },  
                                                                               new List<string> { "campfire" }, new List<string> { "boss1" }};

    // no mas de 6 posibles encuentro por nivel
    List<int> levelMaxEncounters = new List<int> { 5, 6, 4, 6, 5, 4, 6, 5, 3, 1 }; // las etapas contiguas deben tener distintos valores para que el path quede bien

    List<List<string>> pathLevels = new List<List<string>>();
    List<List<bool>> hasChild = new List<List<bool>>();

    List<List<List<int>>> pathLevelsFathers = new List<List<List<int>>>();
    
    List<List<GameObject>> activePathButtons = new List<List<GameObject>>();
    List<int> actualRunPath = new List<int>();
    

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject lineActive;
    [SerializeField] private GameObject lineInactive; 

    
    Vector3 initialPos = new Vector3(51f, 155f, 1f);
    int offsetX = 100;
    int offsetY = 75;
    int levels = 10;

    void Awake()
    {
        if (pathManager == null)
        {
            pathManager = this;
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
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
        
        /*spriteType.Add("enemy1.1", 1);
        spriteType.Add("enemy1.2", 1);
        spriteType.Add("scroll", 2);
        spriteType.Add("campfire", 3);
        spriteType.Add("boss1", 4);
        spriteType.Add("random", 5);*/
    }

    public void GeneratePath(int stage) {
       for (int i = 0; i < levels; i++)
        {
            hasChild.Add(new List<bool>());
            for (int j = 0; j < levelMaxEncounters[i]; j++)
            {
                hasChild[i].Add(false);
            }
            pathLevelsFathers.Add(new List<List<int>>());
        }

        pathLevels.Add(new List<string>());
        pathLevelsFathers[0].Add(new List<int>());

        int initialPaths = Random.Range(2, levelMaxEncounters[0]+1);

        for (int i = 0; i < levelMaxEncounters[0]; i++)
        {
            if(i < initialPaths) pathLevels[0].Add(levelPosibleEncounters[0][Random.Range(0, levelPosibleEncounters[0].Count)]);
            else pathLevels[0].Add("null");
        }
        ShuffleList(pathLevels[0]);

        for (int i = (stage - 1) * levels; i < (levels * stage) - 1; i++)
        {
            pathLevels.Add(new List<string>());
            for(int j = 0; j < levelMaxEncounters[i+1]; j++)
            {
                pathLevels[i + 1].Add("null");
                pathLevelsFathers[i + 1].Add(new List<int>());
            }
            CreateChildrenPath(0, levelMaxEncounters[i] - 1, i, 0, levelMaxEncounters[i+1] - 1);  
        }
    }

    void CreateChildrenPath(int parentFirst, int parentSize, int level, int childFirst, int childSize)
    {
        if (parentSize == 0) // tamaño == 1
        {
            if (!HasEncounter(level, parentFirst)) return;

            for (int i = childFirst; i < childSize + 1 + childFirst; i++)
            {
                if (PathProbability(level))
                {
                    AssignEncounter(level, parentFirst, i);
                } 
            }

            if (!hasChild[level][parentFirst])
            {
                int rand = Random.Range(childFirst, childSize + 1);
                AssignEncounter(level, parentFirst, rand);
            }
        }

        else if (childSize == 0) // tamaño == 1
        {
            for (int i = parentFirst; i < (parentSize + 1 + parentFirst); i++)
            {
                if (!HasEncounter(level, i)) continue;

                if (hasChild[level][i])
                {
                    if (PathProbability(level))
                    {
                        AssignEncounter(level, i, childFirst);
                    }
                }
                else
                {
                    AssignEncounter(level, i, childFirst);
                }
            }
        }

        else if(parentSize == 1 && childSize == 1) 
        {
            CreateChildrenPath(parentFirst, 0, level, childFirst, 0);
            CreateChildrenPath(parentFirst + 1, 0, level, childFirst + 1, 0);
        }

        else if(parentSize >= childSize)
        {
            if (HasEncounter(level, parentFirst))
            {
                if (hasChild[level][parentFirst])
                {
                    if (PathProbability(level))
                    {
                        AssignEncounter(level, parentFirst, childFirst);
                    }
                }
                else
                {
                    AssignEncounter(level, parentFirst, childFirst);
                }
            }
            
            if(HasEncounter(level, parentSize + parentFirst))
            {
                if (hasChild[level][parentSize + parentFirst])
                {
                    if (PathProbability(level))
                    {
                        AssignEncounter(level, parentSize + parentFirst, childSize + childFirst);
                    }
                }
                else
                {
                    AssignEncounter(level, parentSize + parentFirst, childSize + childFirst);
                }
            }

            CreateChildrenPath(parentFirst + 1, parentSize - 2, level, childFirst, childSize);
        }

        else if(parentSize < childSize)
        {
            if (HasEncounter(level, parentFirst))
            {
                if (PathProbability(level))
                {
                    // vincularlos y asignar encuentro
                    AssignEncounter(level, parentFirst, childFirst);
                }
            }
            
            if(HasEncounter(level, parentSize + parentFirst))
            {
                if (PathProbability(level))
                {
                    AssignEncounter(level, parentSize + parentFirst, childSize + childFirst);
                }
            }

            CreateChildrenPath(parentFirst, parentSize, level, childFirst + 1, childSize - 2);
        }   
    }

    public void AssignEncounter(int parentLevel, int parentIndex, int childIndex)
    {
        if (pathLevels[parentLevel+1][childIndex] == "null") pathLevels[parentLevel + 1][childIndex] = levelPosibleEncounters[parentLevel + 1][Random.Range(0, levelPosibleEncounters[parentLevel + 1].Count)];

        pathLevelsFathers[parentLevel + 1][childIndex].Add(parentIndex);

        hasChild[parentLevel][parentIndex] = true;
    }

    bool HasEncounter(int level, int index)
    {
        if (pathLevels[level][index] == "null") return false;
        return true;
    }


    bool PathProbability(int level)
    {
        if(Random.Range(0, 100) <= (60 + level * 3)) return true;
        return false;
    }
    

    // MOSTRAR EL PATH

    public void DisplayPath()
    {
       
        activePathButtons.Clear();
        for (int i = 0; i < levels; i++)
        {
            activePathButtons.Add(new List<GameObject>());

            for(int j = 0; j < pathLevels[i].Count; j++)
            {
                
                DisplayButton(i, j);
            }
        }
    }

    public void DisplayButton(int level, int index)
    {
        if (pathLevels[level][index] == "null")
        {
            activePathButtons[level].Add(null);
            return;
        }

        buttonPrefab.GetComponent<PathButton>().pathLevel = level;
        buttonPrefab.GetComponent<PathButton>().pathLevelIndex = index;
        buttonPrefab.GetComponent<PathButton>().pathManager = this;
        buttonPrefab.GetComponent<SpriteRenderer>().sprite = pathSprites[spriteType[pathLevels[level][index]]];


        int posX, posY;
        if (pathLevels[level].Count % 2 == 0) posX = (offsetX / 2) - ((pathLevels[level].Count / 2) * offsetX) + offsetX * index;
        else posX = -((pathLevels[level].Count / 2) * offsetX) + offsetX * index;

        posY = -offsetY * level;

        activePathButtons[level].Add(Instantiate(buttonPrefab, initialPos + new Vector3(posX, posY, 0f), Quaternion.identity, GameObject.FindGameObjectWithTag("ButtonContainer").transform));




        if (level == 0 && runManager.actualFloor == level)
        {
            activePathButtons[level][index].GetComponent<PathButton>().isInteractable = true;
            activePathButtons[level][index].GetComponent<Animator>().Play("Active");
            return;
        }
        if (level > 0 && runManager.actualFloor == 0) 
        {
            if(level == 1)
            {
                foreach (int father in pathLevelsFathers[level][index])
                {
                    DrawLine(level, index, father, true);
                }
            }
            else
            {
                foreach (int father in pathLevelsFathers[level][index])
                {
                    DrawLine(level, index, father, false);
                }
            }
            return;
        }

        if (runManager.actualFloor > level)
        {
            if (actualRunPath[level] == index)
            {
                activePathButtons[level][index].GetComponent<Animator>().Play("Passed");

                if (level > 0) 
                    foreach (int father in pathLevelsFathers[level][index])
                    {
                        if (actualRunPath[level - 1] == father) DrawLine(level, index, father, true);
                        else DrawLine(level, index, father, false);
                    }
            }
            else
            {
                activePathButtons[level][index].GetComponent<Animator>().Play("Inactive");
                if (level > 0) 
                    foreach (int father in pathLevelsFathers[level][index])
                    {
                        DrawLine(level, index, father, false);
                    }
            }
            activePathButtons[level][index].GetComponent<PathButton>().isInteractable = false;
        }
        else if (runManager.actualFloor == level && pathLevelsFathers[level][index].Contains(actualRunPath[level - 1]))
        {
            activePathButtons[level][index].GetComponent<PathButton>().isInteractable = true;
            activePathButtons[level][index].GetComponent<Animator>().Play("Active");
            foreach (int father in pathLevelsFathers[level][index])
            {
                if (actualRunPath[level - 1] == father) DrawLine(level, index, father, true);
                else DrawLine(level, index, father, false);
            }
        }
        else
        {
            activePathButtons[level][index].GetComponent<Animator>().Play("Inactive");
            activePathButtons[level][index].GetComponent<PathButton>().isInteractable = false;
            foreach (int father in pathLevelsFathers[level][index])
            {
                DrawLine(level, index, father, false);
            }
        }

        

    }

    void DrawLine(int childLevel, int childIndex, int parentIndex, bool active)
    {
        LineRenderer line;
        if (active) line = (Instantiate(lineActive, new Vector3(0f, 0f, 0f), Quaternion.identity, GameObject.FindGameObjectWithTag("ButtonContainer").transform)).GetComponent<LineRenderer>();
        else line = (Instantiate(lineInactive, new Vector3(0f, 0f, 0f), Quaternion.identity, GameObject.FindGameObjectWithTag("ButtonContainer").transform)).GetComponent<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, activePathButtons[childLevel-1][parentIndex].transform.position);
        line.SetPosition(1, activePathButtons[childLevel][childIndex].transform.position);
    }

    public void SelectPath(int level, int index)
    {
        actualRunPath.Add(index);
        runManager.actualFloor++;
        runManager.GoToLevel(pathLevels[level][index], runManager.actualFloor);
    }

    public void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
