using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public string type;
    public string description;
    public int energy;
    public int attack;
    public int shield;
    public bool needTarget;
    public bool burnAtUse;
    public List<string> specialActions;
    public List<int> specialActionsValue;
    // <nombre de la accion, valor de la accion>
    // ejemplo: <"StealCard", 1> == roba una carta de la pila.
    // ver cada accion especial en el script specialActions.cs

    public Sprite artwork;
}
