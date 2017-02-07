using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.


        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        Ray shootRayBack = new Ray();
        Ray shootRayRight = new Ray();
        Ray shootRayLeft = new Ray();
        Ray shootRay45 = new Ray();
        Ray shootRayneg45 = new Ray();
        Ray shootRay135 = new Ray();
        Ray shootRayneg135 = new Ray();
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
		public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        SphereCollider player;
        bool bulletExplosion = false;



        void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            player = GetComponent<SphereCollider> ();
            gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
			//faceLight = GetComponentInChildren<Light> ();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("powerUp"))
            {
                bulletExplosion = true;
                Destroy(other.gameObject);
            }
        }




        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;
            
#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot ();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
        {
            if (!bulletExplosion)
            {

                // Reset the timer.
                timer = 0f;

                // Play the gun shot audioclip.
                gunAudio.Play();

                // Enable the lights.
                gunLight.enabled = true;
                faceLight.enabled = true;

                // Stop the particles from playing if they were, then start the particles.
                gunParticles.Stop();
                gunParticles.Play();

                // Enable the line renderer and set it's first position to be the end of the gun.
                gunLine.enabled = true;
                gunLine.SetPosition(0, transform.position);

                // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
                shootRay.origin = transform.position;
                shootRay.direction = transform.forward;


                // Perform the raycast against gameobjects on the shootable layer and if it hits something...
                if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
                {
                    // Try and find an EnemyHealth script on the gameobject hit.
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    // If the EnemyHealth component exist...
                    if (enemyHealth != null)
                    {
                        // ... the enemy should take damage.
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    // Set the second position of the line renderer to the point the raycast hit.
                    gunLine.SetPosition(1, shootHit.point);
                }
                // If the raycast didn't hit anything on the shootable layer...
                else
                {
                    // ... set the second position of the line renderer to the fullest extent of the gun's range.
                    gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
                }
            }
            else
            {

                // Reset the timer.
                timer = 0f;

                // Play the gun shot audioclip.
                gunAudio.Play();

                // Enable the lights.
                gunLight.enabled = true;
                faceLight.enabled = true;

                // Stop the particles from playing if they were, then start the particles.
                gunParticles.Stop();
                gunParticles.Play();

                // Enable the line renderer and set it's first position to be the end of the gun.
                gunLine.enabled = true;
                gunLine.SetPosition(0, transform.position);

                // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
                shootRay.origin = transform.position;
                shootRay.direction = transform.forward;
                shootRayBack.origin = transform.position;
                shootRayBack.direction = transform.forward * -1;
                shootRayLeft.origin = transform.position;
                shootRayLeft.direction = transform.right * -1;
                shootRayRight.origin = transform.position;
                shootRayRight.direction = transform.right;
                shootRay45.origin = transform.position;
                shootRay45.direction = (transform.forward + transform.right).normalized;
                shootRayneg45.origin = transform.position;
                shootRayneg45.direction = (transform.forward - transform.right).normalized;
                shootRay135.origin = transform.position;
                shootRay135.direction = -(transform.forward + transform.right).normalized;
                shootRayneg135.origin = transform.position;
                shootRayneg135.direction = -(transform.forward - transform.right).normalized;

                if (Physics.Raycast(shootRayneg45, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRayneg45.origin + shootRayneg45.direction * range);
                }
                if (Physics.Raycast(shootRayneg135, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRayneg135.origin + shootRayneg135.direction * range);
                }
                if (Physics.Raycast(shootRay135, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRay135.origin + shootRay135.direction * range);
                }

                if (Physics.Raycast(shootRayBack, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRayBack.origin + shootRayBack.direction * range);
                }
                if (Physics.Raycast(shootRayLeft, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRayLeft.origin + shootRayLeft.direction * range);
                }
                if (Physics.Raycast(shootRayRight, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRayRight.origin + shootRayRight.direction * range);
                }
                if (Physics.Raycast(shootRay45, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRay45.origin + shootRay45.direction * range);
                }
                if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
                {
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }

                    gunLine.SetPosition(1, shootHit.point);
                }
                else
                {
                    gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
                }
            }

        }
    }
}