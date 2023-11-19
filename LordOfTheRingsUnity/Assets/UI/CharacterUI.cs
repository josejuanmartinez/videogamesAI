using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public Image cardImage;
    public Image alignment;
    public void Initialize(CardUI card)
    {
        if (card.GetDetails() == null)
        {
            Debug.LogError("Unable to get CardDetails from " + card.name);
            return;
        }
        CardDetails cardDetails = card.GetDetails();
        cardImage.sprite = cardDetails.cardSprite;
    }

}
