using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : MonoBehaviour
{
    RunManager runManager;
    float maxHP;
    float currentHP;
    public int shield;

    public TMP_Text shieldText;
    public TMP_Text HPText;
    public Scrollbar HPScrollbar;

    Animator anim;

    void Start()
    {
        runManager = GameObject.Find("RunManager").GetComponent<RunManager>();
        RefreshHP();
        RefreshShield(runManager.shieldAtTurnBegin);
        shieldText.text = shield.ToString();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetDamage (int damage) // retorna falso si el jugador MUERE
    {
        anim.Rebind();
        anim.Play("CharacterDamage");

        if (damage > shield)
        {
            damage -= shield;
            shieldText.text = "0";
        }
        else
        {
            shield -= damage;
            damage = 0;
            shieldText.text = shield.ToString();
        }

        runManager.currentHP -= damage;
        currentHP -= damage;

        if (currentHP < 1)
        {
            HPText.text = "0";
            HPScrollbar.size = 0;
            return false;
        }

        RefreshHP();
        return true;
    }

    public void GetShield(int size)
    {
        anim.Rebind();
        anim.Play("CharacterShield");
        shield += size;
        shieldText.text = shield.ToString();
    }

    public void RefreshHP()
    {
        maxHP = runManager.maxHP;
        currentHP = runManager.currentHP;
        HPText.text = currentHP.ToString();
        HPScrollbar.size = currentHP / maxHP;
    }

    public void RefreshShield(int shieldAtTurnBegin)
    {
        shield = shieldAtTurnBegin;
        shieldText.text = shieldAtTurnBegin.ToString();
    }
}
