using JetBrains.Annotations;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


// Keeping all relevant info about a unit on a Scriptable means we can easily access that info.
// EX: Imagine wanting to display them on MenuScreen.
public abstract class ScriptableExampleUnitBase : ScriptableObject
{
    public Faction Faction;

    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    // Used in game
    public HeroUnitBase Prefab;

    // Used in Menus:
    public string Description;
    public Sprite MenuSprite;
}


// Storing base stats in a struct keeps it flexible and easily editable.
// Within the prefab, one can change them depending on conditions.
[Serializable]
public struct Stats
{
    public int Health;
    public int AttackPower;
    public int TravelDistance;
}

[Serializable]
public enum Faction
{
    Heroes = 0,
    Enemies = 1,
}