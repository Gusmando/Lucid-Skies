using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

//This class will function as a controller for the player
//character, reading user inputs and applying apt physics
public class PlayerController : MonoBehaviour
{
    //Values for tuning X and Z movements
    public float normalTopSpeed;
	public float waterTopSpeed;

    public float acceleration;
    public float friction;
    
    //Values for adjusting jump parameters
    public float jumpForce;
    public float fallingAcceleration;
    public float fallingForceMod;

    //Values for adjusting dodge parameters
	public float baseDodgeTime;
	public float baseDodgeCost;
    private float dodgeTime;
    private float dodgeCost;
    public float dodgingTopSpeed;
    public float backwardsTopSpeed;
    public float hurtTime;
    public float shieldCapacity;
    public float hp = 1.0f;
	public float defenseFactor = 1;
    public float stamina = 1.0f;
	public float staminaRegenCooldown = 1f; // seconds
	public float staminaRegenRate = 1.0f/5.0f; // per second
    public float experience = 0.0f;
	public float waterHealthDrainRate = 0.05f;

    public float healthPotionVal;
    public float staminaPotionVal;

    private UIManager uiManager;
    private InventoryManager inventoryManager;
    private GunController gunController;
	public float gunDamage = 0.1f;
    private Rigidbody rigidBody;
    private Collider collider;
	private Camera playerCamera;
	private ProgressionManager progressionManager;

    [SerializeField] private float topSpeed;
	[SerializeField] private Vector2 moveAxis = Vector2.zero;

    //To manage jump state
    [SerializeField] public bool jumping = false;
    [SerializeField] public bool backwardsMovement = false;
    [SerializeField] public bool falling = false;
	[SerializeField] private bool moving = false;
	[SerializeField] private bool braking = false;

    [SerializeField] private bool dodging = false;
    [SerializeField] private bool hurt = false;
    [SerializeField] private bool blocking = false;
    [SerializeField] public bool grounded = false;
	[SerializeField] private bool touchingGround = false; // different from grounded
	[SerializeField] private bool previousTouchingGround = false; // different from grounded
	[SerializeField] private bool inWater = false;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
		progressionManager = GetComponent<ProgressionManager>();
        rigidBody = GetComponent<Rigidbody>();
		playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		uiManager = GameObject.FindGameObjectWithTag("UI Manager").GetComponent<UIManager>();
		inventoryManager = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<InventoryManager>();

        gunController = GetComponent<GunController>();
        collider = GetComponent<Collider>();
        
        topSpeed = normalTopSpeed;

		dodgeCost = baseDodgeCost;
		dodgeTime = baseDodgeTime;

		Cursor.lockState = CursorLockMode.Locked;

        anim = this.gameObject.GetComponent<Animator>();

        anim.SetBool("IsMoving", false);
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsFalling", false);
        anim.SetBool("isGrounded", true);
    }

    // Update is called once per frame
    void Update()
    {
		//Player's orientation matches camera when moving or shooting
		if(moving || gunController.shooting)
        {
			float camXPos = playerCamera.transform.position.x;
			float camZPos = playerCamera.transform.position.z;
		    transform.LookAt(new Vector3(camXPos, this.transform.position.y, camZPos)); // look at the opposite direction of the camera but only laterally
			transform.Rotate(Vector3.up, 180);
        }

        //Dodging raises the maximum velocity of the player
        //resulting in small speed boost
        if(dodging && moving)
        {
            topSpeed = dodgingTopSpeed;
        }

        if(moving && moveAxis.y < 0 && !dodging)
        {
            backwardsMovement = true;
            topSpeed = backwardsTopSpeed;
        }
        else
        {
            backwardsMovement = false;
        }
    
		if(inWater){
			topSpeed = waterTopSpeed;
			hp -= waterHealthDrainRate * Time.deltaTime;
		}
		else if(!dodging && !backwardsMovement){
			topSpeed = normalTopSpeed;
		}

		
        //Setting state variables within animation controller
        anim.SetBool("IsJumping", jumping);
        anim.SetBool("IsFalling", falling);
		anim.SetBool("isGrounded", grounded);
        anim.SetFloat("InputY", moveAxis.y);
        anim.SetFloat("InputX", moveAxis.x);
		anim.SetBool("IsAttacking", gunController.shooting);
        anim.SetBool("IsHit", hurt);

    }

    void FixedUpdate()
    {
        //Fires the projectile controller
        if(gunController.shooting)
        {
			Debug.Log("damage: " + gunDamage);
            gunController.Shoot(gunDamage);
        }

		// handle ground collison and raycast
		
		RaycastHit hit;
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, hitInfo: out hit, collider.bounds.extents.y + 0.00001f);

		// do not update grounded bool until the the touching ground raycast goes from true to false
		if(touchingGround && !previousTouchingGround){
			grounded = true;
			jumping = false;
			falling = false;
		}
		if(!touchingGround && previousTouchingGround){
			grounded = false;
		}

		if(touchingGround){
			inWater = hit.collider.gameObject.tag == "Water"; // if touching water, update inWater boolean
		}

        if(!touchingGround && jumping && rigidBody.velocity.y < 0)
        {
            jumping = false;
        }

        if(!touchingGround && rigidBody.velocity.y < 0)
        {
            falling = true;
        }

		
		//First half of jump applies extra gravity
        if(jumping && !falling)
        {
            rigidBody.AddForce(Vector3.down * fallingAcceleration, ForceMode.Acceleration);
        }

		//Predict movementVector using lean values
		Vector3 moveVector = moveAxis.y * transform.forward  +  moveAxis.x * transform.right;

		//Decompose movementVector into x and z components
		float xVel = Vector3.Dot(moveVector, Vector3.right);
		float zVel = Vector3.Dot(moveVector, Vector3.forward);

		//Set velocity using x and z velocities
		rigidBody.velocity = new Vector3(xVel * topSpeed, rigidBody.velocity.y, zVel * topSpeed);

		previousTouchingGround = touchingGround;
    }

	// Handles the "Jump" Input Action
	public void Jump(InputAction.CallbackContext context)
    {
		if(!jumping && !falling && grounded && context.performed)
        {
			jumping = context.performed;
            grounded = false;
			rigidBody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);

        }

    }

	// Handles the "Move" Input Action
	public void Move(InputAction.CallbackContext context)
    {
		moving = context.performed;
		moveAxis = context.ReadValue<Vector2>();

        anim.SetBool("isMoving", moving);
	}

    //Handles "Shoot" input action
    public void Shoot(InputAction.CallbackContext context)
    {
		if(progressionManager.canPerform("shoot"))
        	gunController.shooting = context.performed;
    }

	//Handles the "BrakeBlock" Input action
	public void BrakeBlock(InputAction.CallbackContext context)
    {
		braking = context.performed;
        
        if(shieldCapacity <= 0)
        {
            blocking = context.performed;
        }
	}

    public void togglePause(InputAction.CallbackContext context)
    {
		if(context.performed)
        {
            uiManager.togglePause();
        }
	}

    public void toggleInventory(InputAction.CallbackContext context)
    {
		if(context.performed)
        {
            uiManager.toggleInventory();
        }
	}

    public void toggleSkill(InputAction.CallbackContext context)
    {
		if(context.performed)
        {
            uiManager.toggleSkill();
        }
	}

    //Handles the "Dodge" input action
    public void Dodge(InputAction.CallbackContext context)
    {
        if(!dodging && !inWater && context.performed && stamina > dodgeCost && progressionManager.canPerform("dodge"))
        {
            addStamina(-dodgeCost);
			
            StartCoroutine(DodgingCoroutine(dodgeTime));
        }
    }

    public void EnemyCollision(Vector3 force, float damage)
    {
		force = new Vector3(force.x, 0, force.z);
        if(!hurt)
        {
            if(!blocking)
            {
                hp -= damage / defenseFactor;
                StartCoroutine(HurtCoroutine(hurtTime));
                rigidBody.AddForce(force, ForceMode.Impulse);
            }
            else
            {
                shieldCapacity -= damage;
            }
        }
    }

    public void useHealthPotion(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(inventoryManager.useHealth())
            {
                addHealth(healthPotionVal);
            }
            
        }
    }

    public void useStaminaPotion(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(inventoryManager.useStamina())
            {
                addStamina(staminaPotionVal);
            }
        }
    }

    public void addHealth(float healthIncrease)
    {
        hp += healthIncrease;
        hp = Mathf.Clamp(hp, 0.0f,1.0f);
    }

    public void addStamina(float staminaIncrease)
    {
		if(staminaIncrease < 0.0f){
			// reset cooldown for stamina regen
			StopCoroutine("StaminaRegenCoroutine");
			StartCoroutine("StaminaRegenCoroutine");
		}
        stamina += staminaIncrease;
        stamina = Mathf.Clamp(stamina, 0.0f,1.0f);
    }

    public void addPuff()
    {
        inventoryManager.addPuff();
    }

    public void addHealthPotion()
    {
        inventoryManager.addHealthPotion();
    }

    public void addStaminaPotion()
    {
        inventoryManager.addStaminaPotion();
    }

    public void addCosmetic(string cosmName)
    {
        uiManager.addItemToCollection(cosmName);
    }

	// update dodge parameters
	public void updateDodge(int level){
		switch(level){
			case 1:
				// increase dodge time
				dodgeTime = baseDodgeTime + 0.1f;
				break;
			case 2:
				// increase dodge time
				dodgeTime = baseDodgeTime + 0.2f;
				// decrease stamina cost
				dodgeCost = baseDodgeCost - 0.05f;
				break;
			case 3:
				// increase dodge time
				dodgeTime = baseDodgeTime + 0.3f;
				// decrease stamina cost
				dodgeCost = baseDodgeCost - 0.05f;
				break;
		}
	}

	public void upgradePotionVal(){
		healthPotionVal *= 2;
		staminaPotionVal *= 2;
	}

	public void updateAtkDef(int level){
		switch(level){
			case 1:
				// increase attack
				gunDamage *= 2;
				break;
			case 2:
				// increase defense
				defenseFactor = 2;
				break;
			case 3:
				// increase attack
				gunDamage *= 2;
				// increase defense
				break;
		}
	}

    //Coroutine for handling length of positive dodging boolean
    IEnumerator DodgingCoroutine(float seconds)
    {
        dodging = true;
        topSpeed = dodgingTopSpeed;

        yield return new WaitForSeconds (seconds);
        
        topSpeed = normalTopSpeed;
        dodging = false;

    }

    IEnumerator HurtCoroutine(float seconds)
    {
        hurt = true;

        yield return new WaitForSeconds (seconds);
        
        hurt = false;

    }


	IEnumerator StaminaRegenCoroutine(){
		// wait for cooldown before regenning
		yield return new WaitForSeconds(staminaRegenCooldown);

		// regen staminaRegenRate per frame
		while(stamina < 1.0f){
			stamina += moving ? staminaRegenRate * Time.deltaTime : staminaRegenRate * 2.0f * Time.deltaTime; //  * Time.deltaTime to maintain consistency, * 2 when not moving
			Mathf.Clamp(stamina, 0.0f, 1.0f);
			yield return null;
		}

	}

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "ShadowWater")
        {
            hp -= 0.00055f / defenseFactor;
        }
    }

}
