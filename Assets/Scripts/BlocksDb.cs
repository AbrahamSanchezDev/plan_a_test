using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a database of block icons, providing access to their sprites and related functionality.
/// </summary>
/// <remarks>This class is a ScriptableObject that stores a collection of block icons as sprites. It allows
/// retrieval of individual icons by index and provides the total count of available icons.</remarks>
[CreateAssetMenu(fileName = "BlocksDb", menuName = "DataBases/BlocksDb", order = 1)]
public class BlocksDb : ScriptableObject
{
    public List<Sprite> blockIcons;

    public Sprite GetIcon(int index)
    {
        if (blockIcons == null || blockIcons.Count == 0)
        {
            Debug.LogWarning("Block icons list is empty!");
            return null;
        }
        if (index < 0 || index >= blockIcons.Count)
        {
            Debug.LogWarning("Index out of range!");
            return null;
        }
        return blockIcons[index];
    }

    public int GetIconsCount()
    {
        return blockIcons != null ? blockIcons.Count : 0;
    }
}
