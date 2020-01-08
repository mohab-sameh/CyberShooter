using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public float delay = 3f;
    public float blastRadius = 20f;
    public float grenadeDamage = 400f;
    public GameObject explosionEffect;

    float countdown;
    bool hasExploded = false;
    LayerMask layers = -1;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;

        }
    }

    void Explode()
    {
        GameObject spawnedParticle = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(spawnedParticle, 1);
        /*Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {

            }
        }*/
        Dictionary<Health, Damageable> uniqueDamagedHealths = new Dictionary<Health, Damageable>();

        // Create a collection of unique health components that would be damaged in the area of effect (in order to avoid damaging a same entity multiple times)
        Collider[] affectedColliders = Physics.OverlapSphere(transform.position, blastRadius, layers, QueryTriggerInteraction.Collide);
        foreach (var coll in affectedColliders)
        {
            Damageable damageable = coll.GetComponent<Damageable>();
            if (damageable)
            {
                Health health = damageable.GetComponentInParent<Health>();
                if (health && !uniqueDamagedHealths.ContainsKey(health))
                {
                    uniqueDamagedHealths.Add(health, damageable);
                }
            }
        }
        foreach (Damageable uniqueDamageable in uniqueDamagedHealths.Values)
        {
            float distance = Vector3.Distance(uniqueDamageable.transform.position, transform.position);
            uniqueDamageable.InflictDamage(grenadeDamage, true, gameObject);
        }
        DestroyImmediate(gameObject, true);
    }
}
