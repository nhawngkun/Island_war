// BuildingButton.cs

using UnityEngine;
using Sirenix.OdinInspector;

public class BuildingButton : MonoBehaviour
{
    [Title("Building Info")]
    [SerializeField] private int buildingId;
    [SerializeField] private BuildingLayer buildingLayer;

    public void OnClick()
    {
        
        if (GameController.IsInstanceValid())
        {
            GameController.Instance.BuidingManager.SelectBuilding(buildingId, buildingLayer);
        }
        else
        {
            Debug.LogError("GameController is not found in the scene!");
        }
    }
}