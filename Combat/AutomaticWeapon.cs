using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The auto weapon class inherits from the abstract projectile
//weapon class, it will allow the player to fire in a constant and
//automatic stream
public class AutomaticWeapon : ProjectileWeapon
{
    
    private void Start() 
    {
        //Setting initial conditions
        canFire = true;
        fullClip = clipSize;
    }
    

    //Resolving the fire function
    public override void fire(float damage)
    {
        //The automatic weapon will only spawn one projectile at a time based
        //on an inspector-set barrel location transform
        GameObject arrow = Instantiate(projectileObject,barrelLocation.position,barrelLocation.rotation);

		arrow.GetComponent<Projectile>().damage = damage;

        //Using force on the rigid body to cause the projectile prefab to
        //launch forward
        if(forceBased)
        {
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.AddForce(barrelLocation.forward * projectileSpeed,ForceMode.Impulse);
        }

        //Timer begins for the next projectile, allowing for a change in fire rate
        StartCoroutine(firingDelay(delayTime));

        //Play firing sfx
        AudioSource.PlayClipAtPoint(fireSfx,this.transform.position,fireVolume);

        clipSize--;
    }

}