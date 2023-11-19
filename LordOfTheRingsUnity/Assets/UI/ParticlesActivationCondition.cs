using System;
using UnityEngine;

[RequireComponent(typeof(ParticlesActivator))]
public class ParticlesActivationCondition : MonoBehaviour
{
    ParticlesActivator particlesActivator;
    Func<bool> condition;

    private bool isInitialized = false;
    void Awake()
    {
        particlesActivator = GetComponent<ParticlesActivator>();
    }

    public void Initialize(Func<bool> condition)
    {
        this.condition = condition;
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
            return;

        if(condition())
            particlesActivator.Play();
        else
            particlesActivator.Stop();
    }
}
