using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipEmmissions : MonoBehaviour
{

    private ParticleSystem particleSystem;
    private readonly int maxParticles = 15;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Emit(float chargePercent)
    {
        //at least 1 particle
        int particleAmount = (int)(chargePercent * (maxParticles - 1)) + 1;
        particleSystem.Emit(particleAmount);
    }
}
