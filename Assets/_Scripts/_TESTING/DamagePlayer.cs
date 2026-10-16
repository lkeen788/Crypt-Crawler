using UnityEngine;
using UnityEngine.InputSystem;

public class DamagePlayer : MonoBehaviour
{
    InputAction attackAction;

    Health playerHealth;

    bool hasAttacked;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get the action for attack form the input system
        attackAction = InputSystem.actions.FindAction("Test_Damage");

        //find the player object and get its helath script
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

        hasAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        //call attack action, check against hasAttacked to chekc if the key has stopped being pressed
        if (attackAction.IsPressed() && hasAttacked == false)
        {
            playerHealth.Damage(5f);
            hasAttacked = true;
        }
        if(attackAction.IsPressed() == false)
        {
            hasAttacked = false;
        }
    }
}
