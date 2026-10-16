using System.Collections;
using System.ComponentModel;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float stoppingDistance = 2.5f;
    [SerializeField] private float lookRotationDampening = 2;

    // ------ //
    // Attack //
    // ------ //
    [SerializeField] private bool canAttack;
    [SerializeField] private float attackDelay = 5;
    [SerializeField] private float attackDamage = 5;
    [SerializeField] private MeshRenderer hand;
    [SerializeField] private float staminaDamage = 25f;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = FindFirstObjectByType<PlayerController>().gameObject;
        playerTransform = playerObject.transform;

        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        // Move the transform at a constant speed tiem.deltatiem toward the position of the player
        // Pause a specified distance away from the player

        //check if current distance is greater than stopping distance
        if (playerTransform)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if(distanceFromPlayer > stoppingDistance)
            {
                hand.enabled = false;
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, movementSpeed * Time.deltaTime);
            }

            Vector3 lookPosition = transform.position - playerTransform.position;
            lookPosition.y = 0;
            quaternion lookRotation = Quaternion.LookRotation(lookPosition);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookRotationDampening * Time.deltaTime);


            // Attacking
            // When player is in range, attempt attack
            if(distanceFromPlayer <= stoppingDistance && canAttack)
            {
                StartCoroutine(Attack());
                hand.enabled = true;
            }
        }
        
    }

    IEnumerator Attack()
    {
        //do stuff
        canAttack = false;
        playerObject.GetComponent<Health>().Damage(attackDamage);
        playerObject.GetComponent<PlayerController>().decreaseStamina(staminaDamage);

        //wait for delay
        yield return new WaitForSeconds(attackDelay);

        //reset
        canAttack = true;
    }
}
