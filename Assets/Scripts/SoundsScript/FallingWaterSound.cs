using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWaterSound : MonoBehaviour
{
    private ParticleSystem particle_system;
    private AudioManager audioManager;
    [SerializeField] private Transform playerTransform;


    private int currentNumberOfParticles = 0;
    public float hearingDist;
    public Vector3 offsetDistance;

    private void Start()
    {
        particle_system = GetComponent<ParticleSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
       
        float distToPlayer = Vector3.Distance(offsetDistance, playerTransform.position);
        if(distToPlayer <= hearingDist)
        {
            if(particle_system.particleCount < currentNumberOfParticles)
            {
                audioManager.Play("Water Drop", 0.8f * (1f - distToPlayer / hearingDist));
            }
        }

        currentNumberOfParticles = particle_system.particleCount;
    }

}
