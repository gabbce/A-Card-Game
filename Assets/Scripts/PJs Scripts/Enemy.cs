using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public int shield;

    public List<string> intentionType;
    public List<int> intentionValue;

    public GameObject isObjetiveMark;
    public TMP_Text shieldText;
    public TMP_Text HPText;
    public TMP_Text intention;
    public Scrollbar HPScrollbar;

    GameManager gameManager;
    private SoundManager soundManager;

    Animator anim;

    void Start()
    {
        HPText.text = maxHP.ToString();
        currentHP = maxHP;
        HPScrollbar.size = 1;
        shieldText.text = shield.ToString();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill()
    {
        if(currentHP < 0)
        {
            Destroy(gameObject);
        }
    }

    public bool GetDamage(int damage) // retorna falso si el enemigo MUERE
    {
        if (damage > 0)
        {
            anim.Rebind();
            anim.Play("EnemyDamage");

            if (damage > shield)
            {
                damage -= shield;
                shield = 0;
                shieldText.text = "0";
            }
            else
            {
                shield -= damage;
                damage = 0;
                shieldText.text = shield.ToString();
            }

            currentHP -= damage;

            if (currentHP < 1)
            {
                HPText.text = "0";
                HPScrollbar.size = 0;
                return false;
            }
        }

        HPText.text = currentHP.ToString();
        HPScrollbar.size = currentHP / maxHP;
        return true;
    }

    public IEnumerator GetShield(int size)
    {
        soundManager.PlayEnemySound(1, 0.3f);
        anim.Rebind();
        anim.Play("EnemyShield");
        shield += size;
        shieldText.text = shield.ToString();

        yield return new WaitForSeconds(2f);
    }

    public void RefreshIntention(int turn)
    {
        if (intentionType[turn] == "A")
        {
            intention.text = "Attack: " + (intentionValue[turn]).ToString();
        }
        else if (intentionType[turn] == "D")
            intention.text = "Defense";
    }

    public void RefreshShield()
    {
        shield = 0;
        shieldText.text = "0";
    }

    private void OnMouseEnter()
    {
        if(gameManager.cardWithTargetDragging)
        {
            gameManager.enemyPointed = gameObject;
            if (!isObjetiveMark.activeSelf) isObjetiveMark.SetActive(true);
        }
    }

    private void OnMouseOver()
    {
        if (!gameManager.cardWithTargetDragging)
        {
            gameManager.enemyPointed = null;
            if (isObjetiveMark.activeSelf) isObjetiveMark.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        if (gameManager.cardWithTargetDragging)
        {
            gameManager.enemyPointed = null;
            if (isObjetiveMark.activeSelf) isObjetiveMark.SetActive(false);
        }
    }
}
