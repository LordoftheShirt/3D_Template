using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// One repository for all scriptable objects. Create your Query methods here to keep your business logic clean
public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableExampleHero> ExampleHeroes {  get; private set; }
    private Dictionary<ExampleHeroType, ScriptableExampleHero> _ExampleHeroesDict;

    /* Attempt 1
    public List<WFC_2D_Scriptable> WaveFunctionParts2D { get; private set; }
    public Dictionary<WFCType, WFC_2D_Scriptable> _WFCPartsDict;
    */

    public List<ScriptableModules> WFCModuleList { get; private set; }
    public Dictionary<WFCModuleEnum2D, ScriptableModules> _WFCModulesDict;


    protected override void Awake()
    {
        base.Awake();
        AssembleResources();

    }

    private void AssembleResources()
    {
        ExampleHeroes = Resources.LoadAll<ScriptableExampleHero>("ExampleHeroes").ToList();
        _ExampleHeroesDict = ExampleHeroes.ToDictionary(r => r.HeroType, r => r);

        WFCModuleList = Resources.LoadAll<ScriptableModules>("WaveFunctionParts2D").ToList();
        _WFCModulesDict = WFCModuleList.ToDictionary(r => r.module, r => r);

        /* Attempt 1
        WaveFunctionParts2D = Resources.LoadAll<WFC_2D_Scriptable>("WaveFunctionParts2D").ToList();
        _WFCPartsDict = WaveFunctionParts2D.ToDictionary(r => r.WFCType, r => r);
        */
    }

    public ScriptableExampleHero GetExampleHero(ExampleHeroType t) => _ExampleHeroesDict[t];
    public ScriptableExampleHero GetRandomHero() => ExampleHeroes[Random.Range(0, ExampleHeroes.Count)];

    public ScriptableModules GetWFCModule(WFCModuleEnum2D t) => _WFCModulesDict[t];

    /* Attempt 1
    public WFC_2D_Scriptable GetWaveFunctionPart(WFCType t) => _WFCPartsDict[t];
    public WFC_2D_Scriptable GetRandomWaveFunctionPart() => WaveFunctionParts2D[Random.Range(0, WaveFunctionParts2D.Count)];
    public int GetWaveFunctionsPartCount() => WaveFunctionParts2D.Count;
    */
}
