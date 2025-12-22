using UnityEngine;

// This will share logic for any unit on the field, friend or foe, controlled or not.
// Damage, dying, animation triggers etc.
public class UnitBase : MonoBehaviour
{
    public Stats Stats {  get; private set; }
    public virtual void SetStats(Stats stats) => Stats = stats;

    public virtual void TakeDamage(int dmg) 
    {

    }
}
