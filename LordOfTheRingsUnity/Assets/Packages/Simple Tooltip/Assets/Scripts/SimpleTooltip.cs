using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SimpleTooltip : MonoBehaviour
{
    public SimpleTooltipStyle simpleTooltipStyle;
    [SerializeField, TextArea] private string infoLeft = "";
    [SerializeField, TextArea] private string infoRight = "";
    private STController tooltipController;
    private EventSystem eventSystem;
    private bool isUIObject = false;
    private bool showing = false;

    private void Awake()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
        if (!eventSystem)
        {
            Debug.LogWarning("Could not find the EventSystem prefab");
        }
        tooltipController = FindFirstObjectByType<STController>();

        // Add a new tooltip prefab if one does not exist yet
        if (!tooltipController)
        {
            tooltipController = AddTooltipPrefabToScene();
        }
        if (!tooltipController)
        {
            Debug.LogWarning("Could not find the Tooltip prefab");
            Debug.LogWarning("Make sure you don't have any other prefabs named `SimpleTooltip`");
        }

        if (GetComponent<RectTransform>())
            isUIObject = true;

        // Always make sure there's a style loaded
        if (!simpleTooltipStyle)
            simpleTooltipStyle = UnityEngine.Resources.Load<SimpleTooltipStyle>("STDefault");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            HideTooltip();
    }

    public static STController AddTooltipPrefabToScene()
    {
        return Instantiate(UnityEngine.Resources.Load<GameObject>("SimpleTooltip")).GetComponentInChildren<STController>();
    }


    private void OnMouseExit()
    {
        if (isUIObject)
            return;
        HideTooltip();
    }

    public void ShowTooltip(string leftText, string rightText)
    {
        if (leftText == infoLeft && rightText == infoRight && showing)
            return;

        showing = true;

        infoLeft = leftText;
        infoRight = rightText;

        // Update the text for both layers
        tooltipController.SetCustomStyledText(infoLeft, simpleTooltipStyle, STController.TextAlign.Left);
        tooltipController.SetCustomStyledText(infoRight, simpleTooltipStyle, STController.TextAlign.Right);

        // Then tell the controller to show it
        tooltipController.ShowTooltip();

    }

    public void HideTooltip()
    {
        if (!showing)
            return;
        showing = false;
        tooltipController.HideTooltip();
    }

    private void Reset()
    {
        // Load the default style if none is specified
        if (!simpleTooltipStyle)
            simpleTooltipStyle = UnityEngine.Resources.Load<SimpleTooltipStyle>("STDefault");

        // If UI, nothing else needs to be done
        if (GetComponent<RectTransform>())
            return;

        // If has a collider, nothing else needs to be done
        if (GetComponent<Collider>())
            return;

        // There were no colliders found when the component is added so we'll add a box collider by default
        // If you are making a 2D game you can change this to a BoxCollider2D for convenience
        // You can obviously still swap it manually in the editor but this should speed up development
        gameObject.AddComponent<BoxCollider>();
    }
}
