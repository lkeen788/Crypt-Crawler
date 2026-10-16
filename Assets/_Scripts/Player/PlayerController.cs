using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    InputSystem_Actions playerInput;
    InputSystem_Actions.PlayerActions input;

    CharacterController controller;
    Animator animator;
    AudioSource audioSource;

    [Header("Controller")]
    [SerializeField] private float baseMovementSpeed = 5f;
    [SerializeField] private float currentMovementSpeed;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.2f;

    [SerializeField] private float playerVelocityX;
    [SerializeField] private float playerVelocityZ;

    Vector3 _PlayerVelocity;
    bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    public float mouseSensitivity;
    public float gamepadSensitivity;
    

    float xRotation = 0f;

    //Looting
    [SerializeField] public bool isLooting;
    [SerializeField] public PlayerInventory playerInventory;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new InputSystem_Actions();
        input = playerInput.Player;
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Movement
        currentMovementSpeed = baseMovementSpeed;

        // Attacking Setup
        isAttacking = false;
        readyToAttack = true;

        //Stamina
        currentStamina = baseStamina;

        //Blocking Setup
        isBlocking = false;
        shieldMeshRenderer = GameObject.Find("Player Shield").GetComponent<MeshRenderer>();
        shieldMeshRenderer.enabled = false;

        // Looting
        playerInventory = GetComponent<PlayerInventory>();
        isLooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        // Repeat Inputs
        if (input.Attack.IsPressed())
        {
            LightAttack();
        }

        // interact
        if (input.Interact.IsPressed() && !isLooting)
        {
            //call the script in player inventory that increasees the amount of gold


            playerInventory.IncreaseTotalGold();
        }

        // Blocking
        if(input.Block.IsPressed() && currentStamina > 0 && !isBlockBroken)
        {
            Blocking();
        }
        else
        {
            isBlocking = false;
            shieldMeshRenderer.enabled = false;
            currentMovementSpeed = baseMovementSpeed;
        }

        if(currentStamina == 0 && !isBlockBroken)
        {
            isBlockBroken = true;
        }
        else if(currentStamina == 100 && isBlockBroken)
        {
            isBlockBroken = false;
        }

        if(currentStamina <  100 && !isBlocking)
        {
            StartCoroutine(regenStamina());
        }

        playerVelocityX = controller.velocity.x;
        playerVelocityX = controller.velocity.z;

        SetAnimations();
    }

    //Physics updates
    void FixedUpdate()
    { 
        MoveInput(input.Move.ReadValue<Vector2>()); 
    }

    void LateUpdate()
    { 
        LookInput(input.Look.ReadValue<Vector2>()); 
    }


    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * currentMovementSpeed * Time.deltaTime);
        _PlayerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && _PlayerVelocity.y < 0)
            _PlayerVelocity.y = -2f;
        controller.Move(_PlayerVelocity * Time.deltaTime);
        if(!isAttacking)
        {
            if(playerVelocityX != 0 || playerVelocityZ != 0)
            {
                ChangeAnimationState(WALK);
            }
            else
            {
                ChangeAnimationState(IDLE);
            }
        }
    }

    void LookInput(Vector2 input)
    {

        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * mouseSensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * mouseSensitivity));
    }

    //Input Actions
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Jump()
    {
        //Adds force to the player rigidbody to jump
        if(isGrounded)
        {
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.started += ctx => LightAttack();
        input.Block.started += ctx => Blocking();    
    }

    //  ---------------  //
    //  Attack Behavior  //
    //  ---------------  //
    [Header("Attack Behavior")]
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private LayerMask attackLayer;

    [Header("Blocking Behavior")]
    [SerializeField] private MeshRenderer shieldMeshRenderer;
    [SerializeField] public bool isBlocking;
    [SerializeField] public bool isBlockBroken;
    [SerializeField] private float blockSpeedReduction = 0.75f;

    [Header("Stamina Behavior")]
    [SerializeField] private float baseStamina = 100;
    [SerializeField] public float currentStamina; 
    [SerializeField] private float staminaRegenRate = 0.05f;
    [SerializeField] private float staminaRegenDelay = 0.2f;

    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip swordSwing;
    [SerializeField] private AudioClip hitSound;

    [SerializeField] private bool isAttacking;
    [SerializeField] private bool readyToAttack;
    [SerializeField] private int attackCount;

    [SerializeField] public GameObject lastAttackedEnemy;

    public void LightAttack()
    {
        if(!readyToAttack || isAttacking) return;

        readyToAttack = false;
        isAttacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        //break up the repetative sound by pitching the sound up and down
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);

        // vary the attack animations kinda like a combo system
        if(attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    public void HeavyAttack()
    {
        //return if not ready to attack or currently attacking

        //set bools to !(current value)


    }

    public void Blocking()
    {
        //while holding the block action input, abilty to take damage will be disabled

        if(isBlocking) return;

        isBlocking = true;

        //testing outcome for shield
        shieldMeshRenderer.enabled = true;


        //save current movement, slow the player movement by .75
        
        currentMovementSpeed *= blockSpeedReduction;
    }

    public void decreaseStamina(float amount)
    {
        if (isBlocking)
        {
            currentStamina -= amount;
        }
    }

    IEnumerator regenStamina()
    {
        //do stuff


        //wait delay amount
        yield return new WaitForSeconds(staminaRegenDelay);

        //reset and end coroutine
        // Stamina
        
        currentStamina += baseStamina * staminaRegenRate * Time.deltaTime;
        //Debug.Log("Health now at: " + currentHealth);

        //keep health form going above max and below 0f
        currentStamina = Mathf.Clamp(currentStamina, 0f, baseStamina);
        
    }

    void AttackRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<Health>().Damage(attackDamage);

                lastAttackedEnemy = hit.collider.gameObject;
            }
            else
            {
                lastAttackedEnemy = null;
            }
        }
        else
        {
            lastAttackedEnemy = null;
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        readyToAttack  = true;
    }

    void HitTarget(Vector3 pos)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(hitSound);

        //GameObject hitObject = Instantiate(hitEffect, pos, Quaternion.identity);
        //Destroy(hitObject, 20);
    }

    //  ---------- //
    //  Animations //
    //  ---------- //
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";
    string currentAnimationState;

    private void SetAnimations()
    {
        //if PLayer is not attacking
        if (!isAttacking)
        {
            if(_PlayerVelocity.x == 0 && _PlayerVelocity.z == 0)
            {
                ChangeAnimationState(IDLE);
            }
            else
            {
                ChangeAnimationState(WALK);                
            }
        }    
    }

    public void ChangeAnimationState(string newState)
    {
        //Stop the same animation from interupting with self
        if(currentAnimationState == newState) return;

        //Play the animation
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }
}
