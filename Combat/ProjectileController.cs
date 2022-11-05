using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class can be utilized as a controller for the 
//projectile weapon class, it contins functions for 
//automatic reload and firing of the weapon
public class ProjectileController : MonoBehaviour
{
    [Header("Current Weapon")]
    public ProjectileWeapon currentWeapon;
    public float reloadTime;
    public Transform barrel;

    [Header("State Variables")]
    public bool reloading;
    public bool shooting;

    public bool player;

    private GameObject playerCamera;

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera"); 
    }

    void Update() 
    {
        //Auto Reload
        if(currentWeapon.clipSize == 0 && !reloading) 
        {
            Reload();
        }   

        //Ensure camera angle is used for aiming of projectile weapon
        if(player)
        {
            barrel.eulerAngles = playerCamera.transform.eulerAngles;
        }        
    }

    //Fire function fires the current weapon with the passed in 
    //damage value
	public void fire(float damage)
    {
        if(currentWeapon.canFire && currentWeapon.clipSize != 0)
        {
            currentWeapon.fire(damage);
        }
    }

    //Resets weapon clip size and removes the clip from the 
    //current total
    public void Reload()
    {
        StartCoroutine(reloadDelay(reloadTime));
        currentWeapon.clipSize = currentWeapon.fullClip;
        currentWeapon.clipCount--;
    }

    //Coroutine used for delaying the reload boolean
    private IEnumerator reloadDelay(float delayLength)
    {
        reloading = true;
        yield return new WaitForSeconds(delayLength);
        reloading = false;
        yield return null;
    }
}
