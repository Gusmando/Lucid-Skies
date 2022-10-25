using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used to define a projectile
public class Projectile : MonoBehaviour
{   
    [Header("Assignments")]
    public bool playerOwned;
    public float lifeTime;
    public float damage;
    public float parryDistanceThreshold;
    public float knockbackForce;

    //For non-player bullets
    private Transform playerPos;
    private bool parryable;
	public int pierce = 1;

    void Start()
    {
        //Will be used for parryable projectiles
        if(!playerOwned)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        }

        //Begin counting the life of the projectile down
        StartCoroutine(lifeCountdown(lifeTime));
    }

    void Update()
    {
        //Calculate current player distance 
        if(!playerOwned)
        {
            //Projectile is parryable if within distance
            float currentDistanceFromPlayer = Vector3.Distance(playerPos.position,this.transform.position);
            parryable = currentDistanceFromPlayer < parryDistanceThreshold ? true : false; 
        }

		if(pierce <= 0)
        {
			Destroy(gameObject);
		}
    }

    //Function fires when there is a collision
    private void OnTriggerEnter(Collider other) 
    {
        //Current direction of the projectile
        Vector3 direction = Vector3.Normalize(GetComponent<Rigidbody>().velocity);

        //Collision of projectile with the player
        if(other.gameObject.tag == "Player" && !playerprojectile)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.EnemyCollision(knockbackForce * direction, damage);
            Destroy(this.gameObject);
        }

        if(other.gameObject.tag == "Enemy" && playerprojectile)
        {
            Enemy enemyController = other.gameObject.GetComponent<Enemy>();
			if(enemyController != null){
				Debug.Log(damage);
            	enemyController.attacked(damage);
            	pierce -= 1;
			}
        }

        if(other.gameObject.tag == "Terrain")
        {
            Destroy(this.gameObject);
        }

        
    }

    //Coroutine counts down the projectile's life
    private IEnumerator lifeCountdown(float delayLength)
    {
        yield return new WaitForSeconds(delayLength);
        Destroy(this.gameObject);
    }


}
