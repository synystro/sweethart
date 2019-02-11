using UnityEngine;
using System.Collections;

public class Item : ItemManager
{
    [SerializeField] private string itemName;
    private bool hasSpawned;
    
    public string ItemName { get { return itemName; } set { itemName = value; } }

    private void Start()
    {

    }
    private void Update()
    {

    }
}
