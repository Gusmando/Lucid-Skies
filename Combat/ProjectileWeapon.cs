using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The projectile weapon class governs weapons which fire
//projectiles, weapon specs and object assignments are done
//in the inspector
public abstract class ProjectileWeapon : MonoBehaviour
{
    [Header("Assignments")]
    public Transform barrelLocation;
    public GameObject projectileObject;
    public AudioClip fireSfx;

    [Header("Weapon Specs")]
    public bool auto;
    public bool forceBased;
    public float projectileSpeed;
    public float delayTime;
    public float fireVolume;
    public int fullClip;
    public float damage;

    [Header("State Vars")]
    public bool canFire;
    public int clipSize;
    public int clipCount;

    //This abstract function will work to define
    //a unique shooting pattern for each projectile
    //weapon type.
    public abstract void fire(float damage);

    //Coroutine for delaying time in between fired
    //projectiles
    protected IEnumerator fireDelay(float delayLength)
    {
        canFire = false;

        yield return new WaitForSeconds(delayLength);

        canFire = true;

        yield return null;
    }
}
