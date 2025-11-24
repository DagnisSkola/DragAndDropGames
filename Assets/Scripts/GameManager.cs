using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tower[] towers; // Assign 3 towers in inspector
    public FloatingBox floatingBox; // Assign floating box in inspector
    public GameObject[] ringPrefabs; // Assign ring prefabs (smallest to largest)
    public int numberOfRings = 3; // How many rings to start with

    private int moveCount = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeGame();
    }

    void InitializeGame()
    {
        // Create rings on the first tower (smallest on top)
        for (int i = numberOfRings - 1; i >= 0; i--)
        {
            GameObject ring = Instantiate(ringPrefabs[i], towers[0].transform);
            RingData ringData = ring.GetComponent<RingData>();
            if (ringData == null)
            {
                ringData = ring.AddComponent<RingData>();
            }
            ringData.Initialize(i);

            // Disable ring collider so it doesn't block tower clicks
            Collider2D ringCollider = ring.GetComponent<Collider2D>();
            if (ringCollider != null)
            {
                ringCollider.enabled = false;
            }

            towers[0].AddRing(ring);
        }

        towers[0].RefreshAllRingPositions();
    }

    void Update()
    {
        // Check for mouse click (desktop) or touch (mobile)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 inputPosition;

        // Get position from touch or mouse
        if (Input.touchCount > 0)
        {
            Vector3 touchPos = Input.GetTouch(0).position;
            touchPos.z = 10f; // Set Z distance from camera
            inputPosition = mainCamera.ScreenToWorldPoint(touchPos);
            Debug.Log($"[TOUCH] Touch detected at screen position: {Input.GetTouch(0).position}");
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Set Z distance from camera
            inputPosition = mainCamera.ScreenToWorldPoint(mousePos);
            Debug.Log($"[MOUSE] Click detected at screen position: {Input.mousePosition}");
        }

        Debug.Log($"[INPUT] World position: {inputPosition}");

        RaycastHit2D hit = Physics2D.Raycast(inputPosition, Vector2.zero);

        // Also try this debug to see what's at that position
        Collider2D[] colliders = Physics2D.OverlapPointAll(inputPosition);
        Debug.Log($"[OVERLAP] Found {colliders.Length} colliders at click position");
        foreach (var col in colliders)
        {
            Debug.Log($"[OVERLAP] - {col.gameObject.name}");
        }

        if (hit.collider != null)
        {
            Debug.Log($"[HIT] Hit object: {hit.collider.gameObject.name}");
            Debug.Log($"[HIT] Has Tower component: {hit.collider.GetComponent<Tower>() != null}");

            Tower clickedTower = hit.collider.GetComponent<Tower>();

            if (clickedTower != null)
            {
                Debug.Log($"[TOWER] Clicked on tower, FloatingBox empty: {floatingBox.IsEmpty()}");

                if (floatingBox.IsEmpty())
                {
                    PickUpRing(clickedTower);
                }
                else
                {
                    PlaceRing(clickedTower);
                }
            }
            else
            {
                Debug.Log("[ERROR] Hit object doesn't have Tower component!");
            }
        }
        else
        {
            Debug.Log("[RAYCAST] Raycast hit nothing!");
        }
    }

    void PickUpRing(Tower tower)
    {
        Debug.Log($"[PICKUP] Attempting to pick up ring from tower. Ring count: {tower.GetRingCount()}");

        GameObject ring = tower.RemoveRing();

        if (ring != null)
        {
            Debug.Log($"[PICKUP] Successfully removed ring: {ring.name}");
            floatingBox.PlaceRing(ring);
            tower.RefreshAllRingPositions();
            Debug.Log("[PICKUP] Ring placed in floating box");
        }
        else
        {
            Debug.Log("[PICKUP] Tower is empty!");
        }
    }

    void PlaceRing(Tower tower)
    {
        int heldRingSize = floatingBox.GetHeldRingSize();
        int topRingSize = tower.GetTopRingSize();

        // Check if move is valid (smaller ring on larger ring, or empty tower)
        if (heldRingSize < topRingSize)
        {
            GameObject ring = floatingBox.RemoveRing();
            tower.AddRing(ring);
            tower.RefreshAllRingPositions();

            moveCount++;
            Debug.Log($"Placed ring on tower. Moves: {moveCount}");

            CheckWinCondition();
        }
        else
        {
            Debug.Log("Invalid move! Cannot place larger ring on smaller ring.");
            // Optional: Add shake animation or sound effect here
        }
    }

    void CheckWinCondition()
    {
        // Win if all rings are on tower 2 or 3
        if (towers[1].GetRingCount() == numberOfRings ||
            towers[2].GetRingCount() == numberOfRings)
        {
            Debug.Log($"You Win! Completed in {moveCount} moves!");
            // Optional: Add win screen or restart button
        }
    }
}