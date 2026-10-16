using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GUITest : MonoBehaviour
{
    // Player Testing
    GameObject player;
    [SerializeField] PlayerController playerController;
    [SerializeField] Health playerHealthScript;
    [SerializeField] float playerCurrentHealth;
    [SerializeField] float playerStamina;
    [SerializeField] PlayerInventory playerInv;
    [SerializeField] int playerGold;


    // Enemy Testing
    [SerializeField] GameObject currentEnemyObj;
    [SerializeField] Health enemyHealthScript;
    [SerializeField] float currentEnemyHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealthScript = player.GetComponent<Health>();
        playerController = player.GetComponent<PlayerController>();
        playerInv = player.GetComponent<PlayerInventory>();
        
    }

    void Update()
    {
        playerCurrentHealth = playerHealthScript.currentHealth;
        playerStamina = playerController.currentStamina;

        playerGold = playerInv.goldCollected;

        currentEnemyObj = playerController.lastAttackedEnemy;

        if(currentEnemyObj != null)
        {
            enemyHealthScript = currentEnemyObj.GetComponent<Health>();
            currentEnemyHealth = enemyHealthScript.currentHealth;
        }
        else
        {
            
            enemyHealthScript = null;
            currentEnemyObj = null;
            currentEnemyHealth = 0;
        }
        
    }


    void OnGUI()
    {
        int playerHealthInt = (int)playerCurrentHealth;
        string playerHealthStr = playerHealthInt.ToString();
        GUI.Box 
        (
            new Rect (10,10,150,250), 
            "Player Health:\n" + playerHealthStr + 
            "\n\n" + 
            "Current Enemy Health: \n" + currentEnemyHealth + 
            "\n\n" + 
            "Player Stamina: \n" + playerStamina + 
            "\n\n" + 
            //"Blocking State: \n" + playerController.isBlocking + 
            //"\n\n" +
            //"Defense Broke State: \n" + playerController.isBlockBroken + 
            //"\n\n" +
            "Gold: \n" + playerGold
        );
        
    }


}
