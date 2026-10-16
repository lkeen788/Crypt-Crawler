using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InstantiateSpawnPoint : MonoBehaviour
{
    //will need: spawnpoint object, and position where the SP will instantiate
    public GameObject spawnObject;
    public GameObject deadEndObject;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Vector3 spawnInstancePosition;

    public void CreatePlayerSpawnpoint()
    {

        deadEndObject = GameObject.Find("DeadEnd_0");

        parentTransform = deadEndObject.transform;

        //check the scene to see if there is already a instance of the spawnpoint(SP) if not instantiate the SP
        if (!GameObject.Find("SpawnPoint(Clone)") )
        {
            Instantiate(spawnObject, spawnInstancePosition, quaternion.Euler(0,0,0), parentTransform);
        }
    }
}
