using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationActivator : MonoBehaviour
{
    [SerializeField]
    private bool animate;
    [SerializeField]
    WrapMode wrapMode;

    private Animation anim;
    
    private void Awake()
    {
        anim = GetComponent<Animation>();
        wrapMode = anim.wrapMode;
        animate = false;
    }
    public void Play(WrapMode wrapMode)
    {
        this.wrapMode = wrapMode;
        if(!animate || !anim.isPlaying || anim.wrapMode != wrapMode)
        {
            anim.wrapMode = wrapMode;
            animate = true;
            anim.Play();
        }        
    }

    public void Stop()
    {
        if(animate)
        {
            //animation.Rewind();
            animate = false;
            anim.wrapMode = WrapMode.Clamp;
        }        
    }

}
