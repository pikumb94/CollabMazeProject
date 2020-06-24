using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ParameterManager allows to exchange information between different scenes.
/// </summary>
public class ParameterManager:MonoBehaviour /*: Singleton<ParameterManager> */{

    public enum BuildVersion { COMPLETE, GAME_ONLY, EXPERIMENT_CONTROL, EXPERIMENT_ONLY };

    // Map data.
    [HideInInspector] public ITypeGrid GridType { get; set; }
    [HideInInspector] public TileObject[,] MapToPlay { get; set; }
    [HideInInspector] public TileObject[,] MapToPlayWTrapBorder { get; set; }
    [HideInInspector] public Vector2Int StartCell { get; set; }
    [HideInInspector] public Vector2Int EndCell { get; set; }
    [HideInInspector] public bool IsTrapsOnMapBorder { get; set; }
    [HideInInspector] public Dictionary<int, StructuredAlias>.ValueCollection AliasMaps { get; set; }

    [HideInInspector] public int rndSeed { get; set; }
    // Gameplay data.
    [HideInInspector] public int countdownSecondsParam { get; set; }
    [HideInInspector] public int penaltySecondsParam { get; set; }
    [HideInInspector] public bool areObstacleTraversableParam { get; set; }

    // Alias Generation data.
    [HideInInspector] public int aliasNum { get; set; }
    [HideInInspector] public int minStepsSolution { get; set; }
    [HideInInspector] public int maxStepsSolution { get; set; }
    [HideInInspector] public bool allowAutosolverForAlias { get; set; }
    [HideInInspector] public bool considerSimilar { get; set; }
    [HideInInspector] public bool considerNovelty { get; set; }
    [HideInInspector] public bool isBestPathOnlyExplorative { get; set; }

    // Optimization data.
    [HideInInspector] public bool isOptimizerOn { get; set; }
    [HideInInspector] public int optimizerType { get; set; }
    [HideInInspector] public int hillClimberNumBatch { get; set; }
    [HideInInspector] public int timeCap { get; set; }

    // Export data.
    [HideInInspector] public bool Export { get; set; }
    [HideInInspector] public string ExportPath { get; set; }

    // Error data.
    [HideInInspector] public int ErrorCode { get; set; }
    [HideInInspector] public string ErrorMessage { get; set; }

    // Experiment data.
    [HideInInspector] public bool LogOnline { get; set; }
    [HideInInspector] public bool LogOffline { get; set; }
    [HideInInspector] public bool LogSetted { get; set; }
    [HideInInspector] public string ExperimentControlScene { get; set; }

    /*// Mouse sensibility data.
    [HideInInspector] public float MinSensibility { get; internal set; }
    [HideInInspector] public float MaxSensibility { get; internal set; }
    [HideInInspector] public float DefaultSensibility { get; internal set; }
    [HideInInspector] public float WebSensibilityDownscale { get; internal set; }*/
    
    // Other data.
    [HideInInspector] public Quaternion BackgroundRotation { get; set; }
    [HideInInspector] public BuildVersion Version { get; set; }
    [HideInInspector] public string InitialScene { get; set; }

    public static ParameterManager Instance = null;

    protected ParameterManager() { }


    void Awake() {
        ErrorCode = 0;

        LogOnline = true;
        LogOffline = true;
        LogSetted = false;
        /*
        MinSensibility = 0.75f;
        MaxSensibility = 2.75f;
        DefaultSensibility = 1.75f;
        WebSensibilityDownscale = 3f;*/


        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }

}