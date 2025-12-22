using UnityEngine;

public class ExampleUnitManager : Singleton<ExampleUnitManager>
{
    public void SpawnHeroes()
    {

    }


    private void SpawnUnit(ExampleHeroType t, Vector3 pos)
    {
        var legoSoldierScriptable = ResourceSystem.Instance.GetExampleHero(t);

        var spawned = Instantiate(legoSoldierScriptable.Prefab, pos, Quaternion.identity, transform);

        // Apply possible modifications here such as potion boosts, team synergies, etc.
        var stats = legoSoldierScriptable.BaseStats;
        stats.Health += 20;

        spawned.SetStats(stats);
    }

}
