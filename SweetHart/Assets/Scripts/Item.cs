using UnityEngine;
using System.Collections;

public class Item : ItemManager
{
    [SerializeField] private string itemName;
    
    public string ItemName { get { return itemName; } set { itemName = value; } }
}
