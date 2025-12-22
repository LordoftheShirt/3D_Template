using System;
using UnityEngine;


// creates a scriptable hero.
[CreateAssetMenu(fileName = "New Scriptable Example")]
public class ScriptableExampleHero : ScriptableExampleUnitBase
{
    public ExampleHeroType HeroType;

}

[Serializable]
public enum ExampleHeroType
{
    SoldierBoy = 0,
    MeleeBoy = 1,
    ShieldBoy = 2,
}