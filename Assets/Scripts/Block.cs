using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a block within the grid, which can be interacted with via pointer input.
/// </summary>
/// <remarks>A block is associated with a specific position in the grid, an icon, and a grid manager.  It handles
/// pointer input events and notifies the grid manager when clicked. The block's  visual representation is updated based
/// on its assigned icon.</remarks>
public class Block : MonoBehaviour, IPointerDownHandler
{
    public int IconIndex;
    public Sprite IconForTheBlock;
    public int X;
    public int Y;

    private GridManager gridManager;

    [SerializeField]
    private Image theRenderer;

    public void Initialize(int x, int y, int iconIndex, Sprite icon, GridManager manager)
    {
        X = x;
        Y = y;
        IconIndex = iconIndex;
        IconForTheBlock = icon;
        gridManager = manager;
        // Set visual based on color type
        UpdateVisual();
    }

    void UpdateVisual()
    {
        // This would be replaced with actual sprite assignments from the assets
        theRenderer.sprite = IconForTheBlock;
    }

    // This will work for both mouse and touch input
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log($"Block clicked at ({x}, {y})");

        if (GameManager.Instance.CanMakeMove())
        {
            gridManager.OnBlockClicked(this);
        }
    }
}