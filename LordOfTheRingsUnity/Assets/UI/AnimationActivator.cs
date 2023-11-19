using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationActivator : MonoBehaviour
{
    private bool animate;
    private Animation anim;

    WrapMode wrapMode;
    private void Awake()
    {
        anim = GetComponent<Animation>();
        wrapMode = anim.wrapMode;
        animate = false;
    }
    public void Play()
    {
        if(!animate)
        {
            animate = true;
            anim.wrapMode = wrapMode;
            anim.Play();
        }        
    }

    public void Stop()
    {
        if(animate)
        {
            anim.wrapMode = WrapMode.Clamp;
            //animation.Rewind();
            animate = false;
        }        
    }

}
