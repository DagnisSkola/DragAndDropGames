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
            GameObject ring = Instantiate(ringPrefabs[i]);
            RingData ringData = ring.GetComponent<RingData>();
            if (ringData == null)
            {
                ringData = ring.AddComponent<RingData>();
            }
            ringData.Initialize(i);
            towers[0].AddRing(ring);
        }

        towers[0].RefreshAllRingPositions();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if we clicked on a tower
            Tower clickedTower = hit.collider.GetComponent<Tower>();

            if (clickedTower != null)
            {
                if (floatingBox.IsEmpty())
                {
                    // Pick up ring from tower
                    PickUpRing(clickedTower);
                }
                else
                {
                    // Place ring on tower
                    PlaceRing(clickedTower);
                }
            }
        }
    }

    void PickUpRing(Tower tower)
    {
        GameObject ring = tower.RemoveRing();

        if (ring != null)
        {
            floatingBox.PlaceRing(ring);
            tower.RefreshAllRingPositions();
            Debug.Log("Picked up ring from tower");
        }
        else
        {
            Debug.Log("Tower is empty!");
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