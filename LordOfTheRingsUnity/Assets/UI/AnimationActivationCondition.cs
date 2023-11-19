using System;
using UnityEngine;

[RequireComponent(typeof(AnimationActivator))]
public class AnimationActivationCondition : MonoBehaviour
{
    AnimationActivator animationActivator;
    Func<bool> condition;

    bool isPlaying;

    private bool isInitialized = false;
    void Awake()
    {
        animationActivator = GetComponent<AnimationActivator>();
        isPlaying = false;
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

        if (condition() && !isPlaying)
        {
            isPlaying = true;
            animationActivator.Play();
        }            
        else if (!condition() && isPlaying)
        {
            animationActivator.Stop();
            isPlaying = false;
        }
            
    }
}
