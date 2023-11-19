using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent (typeof(ParticleSystem))]
public class ParticlesActivator : MonoBehaviour
{
    ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    public void Play()
    {
        particle.Play();
        EmissionModule emission = particle.emission;
        emission.enabled = true;
    }

    public void Stop()
    {
        particle.Stop();
        EmissionModule emission = particle.emission;
        emission.enabled = false;
    }
}
