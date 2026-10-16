using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    //gameboject will need to start with base health and update its current health
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] public float currentHealth;

    [SerializeField] private float healthRegenRate = 0.007f;
    [SerializeField] private float combatHealthRegenRate = 0.0049f;

    PlayerController playerController;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = baseHealth;

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    

    // Update is called once per frame
    void Update()
    {
        // health will regenerate overtime outside combat retiurning to a value EQUAL to baseHealth
        if(currentHealth < 100)
        {
            currentHealth += baseHealth * healthRegenRate * Time.deltaTime;
            //Debug.Log("Health now at: " + currentHealth);

            //keep health form going above max and below 0f
            currentHealth = Mathf.Clamp(currentHealth, 0f, baseHealth);
        }

        if(currentHealth <= 0)
        {
            Die();
        }
        
    }

    public void Damage(float amount)
    {
        if(currentHealth > 0)
        {
            if (gameObject.CompareTag("Player"))
            {
                if(!playerController.isBlocking)
                {
                    currentHealth = currentHealth - amount;
                }
            }
            else
            {
                currentHealth = currentHealth - amount;
            }
        }
    }

    void Die()
    {
        if (!CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (CompareTag("Enemy"))
        {
            playerController.lastAttackedEnemy = null;
        }
    }
}
