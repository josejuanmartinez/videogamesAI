using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 1f; // The speed at which the camera zooms
    [SerializeField] private float panSpeed = 10f; // The speed at which the camera pans
    [SerializeField] private float dragSpeed = 2f; // The speed at which the camera drags    
    [SerializeField] private float minZoom = 1f; // The minimum zoom level
    [SerializeField] private float maxZoom = 10f; // The maximum zoom level
    [SerializeField] private float minX = -10f; // The minimum X position of the camera
    [SerializeField] private float maxX = 10f; // The maximum X position of the camera
    [SerializeField] private float minY = -10f; // The minimum Y position of the camera
    [SerializeField] private float maxY = 10f; // The maximum Y position of the camera
    [SerializeField] private float moveSpeed = 10f; // The maximum Y position of the camera
    [SerializeField] private GameObject minimapFrame;
    [SerializeField] Vector2 zoomDecay;

    private bool preventDrag = false;
    private bool isPopupOpen = false;

    private Vector3 NONE = Vector3.one * int.MinValue;

    private Camera cam; // The camera component
    private Mouse mouse;
    private Tilemap tilemap;
    private DiceManager diceManager;
    private Board board;
    private Game game;

    private float zoomLevel = 5f; // The current zoom level
    private Vector3 dragOrigin; // The starting point of a drag gesture
    private bool isDragging;
    private Vector3 lookToPosition = Vector3.one * int.MinValue;

    private void Awake()
    {
        isDragging = false;

        cam = GetComponent<Camera>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        tilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    private void Update()
    {
        if (!game.FinishedLoading())
            return;

        if (diceManager.IsDicing() || isPopupOpen || preventDrag)
            return;

        // Zoom in and out with the scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomLevel -= scroll * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
        minimapFrame.transform.localScale = new Vector3(
            zoomLevel/ zoomDecay.x,
            zoomLevel/ zoomDecay.y,
            1);


        // Pan with the arrow keys or WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(panSpeed * Time.deltaTime * new Vector3(horizontal, vertical, 0f));

        IsMouseAtBorder();

        if (preventDrag || (isDragging && (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))))
            isDragging = false;

        if(Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            mouse.RemoveCursor();
            isDragging = false;
            preventDrag = false;
        }    

        // Drag with the mouse
        
        if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
        {
            if (!preventDrag)
            {
                isDragging = true;
                dragOrigin = Input.mousePosition;
                mouse.ChangeCursor("drag");
                //Debug.Log("Start dragging");
            }
            //else
            //{
            //   Debug.Log("Drag start prevented!");
            //}
        }
        else if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            if (!preventDrag)
            {
                isDragging = true;
                Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                Vector3 move = new(difference.x * dragSpeed, difference.y * dragSpeed, 0);
                transform.Translate(move, Space.World);
                dragOrigin = Input.mousePosition;
                mouse.ChangeCursor("drag");
                //Debug.Log("Dragging");
            }
            //else
            //{
            //    Debug.Log("Drag prevented!");
            //}
        }
        

        // Clamp the camera position to the bounds
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        // Set the camera's orthographic size based on the zoom level
        cam.orthographicSize = zoomLevel;

        if (!preventDrag && isDragging)
            isDragging = false;

        if (isDragging)
            lookToPosition = NONE;

        //LOOK TO
        if (lookToPosition != NONE && lookToPosition != transform.position)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, lookToPosition, Time.deltaTime * moveSpeed);
            transform.position = newPosition;
            if (transform.position == lookToPosition)
                lookToPosition = NONE;
        }
        
    }

    public bool IsPreventedDrag()
    {
        return preventDrag;
    }

    public void PreventDrag()
    {
        preventDrag = true;
    }

    public void RemovePreventDrag()
    {
        preventDrag = false;
    }
    public void SetPopupOpen()
    {
        isPopupOpen = true;
    }

    public void RemovePopupOpen()
    {
        isPopupOpen = false;
    }

    public bool IsDragging()
    {
        return isDragging;
    }
    public bool IsPopupOpen()
    {
        return isPopupOpen;
    }

    public void LookToCard(CardUI card)
    {
        if (card == null) 
            return;

        Vector2Int hex = new (int.MinValue, int.MinValue);
        if ((card as CharacterCardUIBoard) != null)
            hex = (card as CharacterCardUIBoard).GetHex();
        else if ((card as HazardCreatureCardUIBoard) != null)
            hex = (card as HazardCreatureCardUIBoard).GetHex();

        if (hex.x == int.MinValue || hex.y == int.MinValue)
            return;

        Vector3Int v3 = new (hex.x, hex.y, 0);
        Vector3 v3World = tilemap.CellToWorld(v3);
        v3World = new Vector3(v3World.x, v3World.y, transform.position.z);
        LookTo(v3World);
    }

    public void LookToCard(CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;

        CardUI card = board.GetCardManager().GetCardUI(cardDetails);
        if (card == null)
            return;

        Vector2Int hex = cardDetails.cardClass == CardClass.Character ? (card as CharacterCardUIBoard).GetHex() : (card as HazardCreatureCardUIBoard).GetHex();
        Vector3Int v3 = new(hex.x, hex.y, 0);
        Vector3 v3World = tilemap.CellToWorld(v3);
        v3World = new Vector3(v3World.x, v3World.y, transform.position.z);
        LookTo(v3World);
    }

    public void LookToCity(CityUI city)
    {
        if (city == null)
            return;

        Vector3Int v3 = new (city.GetHex().x, city.GetHex().y, 0);
        Vector3 v3World = tilemap.CellToWorld(v3);
        v3World = new Vector3(v3World.x, v3World.y, transform.position.z);
        LookTo(v3World);
    }

    public void LookTo(Vector3 position)
    {
        // Calculate the new position and rotation of the camera
        lookToPosition = position;
    }
    public void LookToImmediate(Vector3 newPosition)
    {
        // Calculate the new position and rotation of the camera
        transform.position = newPosition;
    }

    public void IsMouseAtBorder()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 translatePosition = new (0f, 0f, 0f);

        if (mousePosition.x >=0 && mousePosition.x <= 5)
            translatePosition.x = -5;
        
        if (mousePosition.x >= (Screen.width - 5) && mousePosition.x <=  (Screen.width))
           translatePosition.x = 5;

        if (mousePosition.y >= (Screen.height - 5) && mousePosition.y <= (Screen.height))
            translatePosition.y = 5;

        if (mousePosition.y >= 0 && mousePosition.y <= 5)
            translatePosition.y = -5;

        transform.Translate(panSpeed * Time.deltaTime * translatePosition);
    }
}