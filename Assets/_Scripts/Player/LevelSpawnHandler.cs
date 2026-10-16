using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class LevelSpawnHandler : MonoBehaviour
{
    // Find the scenes spawn point object and instatiate the player there
    [SerializeField] public GameObject spawnPoint;
    [SerializeField] public InstantiateSpawnPoint instantiateSpawnPoint;
    [SerializeField] public GameObject player;




    void Start()
    {
        spawnPoint = instantiateSpawnPoint.spawnObject;

        Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 1, spawnPoint.transform.position.z);

        //create the Player
        if (!GameObject.Find("SpawnPoint(Clone)"))
        {
            Instantiate(player, spawnPos, quaternion.Euler(0,0,0));
        }

        spawnPoint = gameObject;

        //player = GameObject.FindGameObjectWithTag("Player");

        //NOTE: characterController wants to force player position at start
        if(player != null)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = spawnPoint.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }

    }
}
