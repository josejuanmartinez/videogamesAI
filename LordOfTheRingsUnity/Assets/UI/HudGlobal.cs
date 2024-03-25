using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(AnimationActivator))]
public class HudGlobal : MonoBehaviour
{
    [SerializeField]
    private Image preImage;
    [SerializeField]
    private TextMeshProUGUI tmPro;
    [SerializeField]
    private Image postImage;
    [SerializeField]
    private Sprite defaultSprite;

    private SpritesRepo spritesRepo;
    private CanvasGroup hudGlobalCanvasGroup;
    private AnimationActivator hudGlobalAnimationActivator;

    public void Awake()
    {
        hudGlobalAnimationActivator = GetComponent<AnimationActivator>();
        hudGlobalCanvasGroup = GetComponent<CanvasGroup>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize(string message, Sprite pre, Sprite post)
    {
        tmPro.text = message;
        preImage.sprite = pre;
        postImage.sprite = post;
        Animate();
    }

    public void Initialize(string message, string preSpriteString, string postSpriteString)
    {
        tmPro.text = message;
        preImage.sprite = spritesRepo.GetSprite(preSpriteString);
        postImage.sprite = spritesRepo.GetSprite(postSpriteString);
        Animate();
    }

    public void Initialize(string message, Sprite pre)
    {
        tmPro.text = message;
        preImage.sprite = pre;
        postImage.sprite = pre;
        Animate();
    }

    public void Initialize(string message, string preSpriteString)
    {
        tmPro.text = message;
        preImage.sprite = spritesRepo.GetSprite(preSpriteString); ;
        postImage.sprite = spritesRepo.GetSprite(preSpriteString); ;
        Animate();
    }
    public void Initialize(string message)
    {
        tmPro.text = message;
        preImage.sprite = defaultSprite;
        postImage.sprite = defaultSprite;
        Animate();
    }

    public void Animate()
    {
        preImage.enabled = preImage.sprite != null;
        postImage.enabled = postImage.sprite != null;
        hudGlobalAnimationActivator.Play(WrapMode.Once);
        hudGlobalCanvasGroup.alpha = 1;
    }

    public void Update()
    {
        if (!hudGlobalAnimationActivator.IsPlaying())
        {
            hudGlobalAnimationActivator.Stop(true);
            hudGlobalCanvasGroup.alpha = 0;
        }
    }
}
