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

    public void Stop(bool rewind=false)
    {
        if(animate)
        {
            if (rewind)
                GetComponent<Animation>().Rewind();
            animate = false;
            anim.wrapMode = WrapMode.Clamp;
        }        
    }

    public bool IsPlaying()
    {
        return anim.isPlaying;
    }

    public void Update()
    {
        if (animate)
        {
            Play(wrapMode);
            animate = false;
        }
    }

}
