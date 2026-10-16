using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // This will handle the constant calculation of the players
    // gold reserve throughout the game.

    [SerializeField] public int goldCollected;
    //[SerializeField] private int currentGoldCollected;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // NOTE: Eventually this will need to be changed to a saved value.
        goldCollected = 0;
    }


    public void IncreaseTotalGold()
    {
        goldCollected += Random.Range(5, 101);
    }
}
