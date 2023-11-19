using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject popup;

    private CameraController cameraController;
    private bool isInitialized = false;
    void Awake()
    {
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        isInitialized = true;
    }

    public virtual void ShowPopup()
    {   
        if (!isInitialized)
            Awake();
        cameraController.SetPopupOpen();
        popup.SetActive(true);
    }

    public virtual void HidePopup()
    {
        if (!isInitialized)
            Awake();
        cameraController.RemovePopupOpen();
        popup.SetActive(false);
    }

    public bool IsShown()
    {
        return popup.activeSelf;
    }
}
