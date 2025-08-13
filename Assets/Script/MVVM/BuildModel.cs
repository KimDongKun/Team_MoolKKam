using UnityEngine;

public class BuildModel
{
    public PlayerModel PlayerModel;

    public GameObject BuildModelPrefab;
    public Vector3 InstallPos;
    public BuildType buildType;
    public enum BuildType
    {
        None,
        Barricade,
        Tower
    }

    public BuildModel(GameObject buildPrefab, Vector3 installPos, BuildType type) 
    {
        BuildModelPrefab = buildPrefab;
        InstallPos = installPos;
        buildType = type;
    }
}
