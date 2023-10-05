using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class SpaceshipEmmissions : MonoBehaviour
{
    // Start is called before the first frame update

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
