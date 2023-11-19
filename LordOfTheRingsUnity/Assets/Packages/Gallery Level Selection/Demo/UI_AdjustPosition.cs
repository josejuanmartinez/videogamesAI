#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

public enum HorizontalAnchor { left, center, right, ignore }
public enum VerticalAnchor { top, center, bottom, ignore }

[ExecuteInEditMode]
public class UI_AdjustPosition : MonoBehaviour
{
    public HorizontalAnchor horizontalAnchor = HorizontalAnchor.ignore;
    public VerticalAnchor verticalAnchor = VerticalAnchor.ignore;
    
    void Start()
    {
        AdjustPosition();
    }

    private void Update()
    {
#if UNITY_EDITOR
        AdjustPosition();
#endif
    }
    
    private void AdjustPosition()
    {

        Vector3 dest = Vector3.zero;
        if (horizontalAnchor == HorizontalAnchor.center) dest.x = 0.5f;
        else if (horizontalAnchor == HorizontalAnchor.right) dest.x = 1.0f;
        if (verticalAnchor == VerticalAnchor.center) dest.y = 0.5f;
        else if (verticalAnchor == VerticalAnchor.top) dest.y = 1.0f;

        Vector3 pos = Camera.main.ViewportToWorldPoint(dest);
        Vector3 finalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (horizontalAnchor != HorizontalAnchor.ignore) finalPos.x = pos.x;
        if (verticalAnchor != VerticalAnchor.ignore) finalPos.y = pos.y;
        transform.position = finalPos;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif

    }
}
