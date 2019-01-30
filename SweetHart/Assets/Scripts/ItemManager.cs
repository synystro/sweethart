using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private string[] itemNames;

    [Header("Spawned?")]
    [SerializeField] private bool backDoorKeySpawned;

    private GameObject[] itemBoxes;
    private GameObject randomizedItemBox;
    private int randomItemBox;
    private int nOfItems;

    private void Start()
    {
        // shuffle items.
        ShuffleArray(itemNames);
        // find item boxes.
        itemBoxes = GameObject.FindGameObjectsWithTag("ItemBox");

        if(itemBoxes.Length > itemNames.Length)
        {
            nOfItems = itemBoxes.Length - itemNames.Length;
#if UNITY_EDITOR
            // number of items less than item (spawn) boxes
            Debug.Log("Number of items is less than item boxes. Items: " + itemNames.Length + " SpawnBoxes: " + itemBoxes.Length);
#endif
        }
        else
        {
            nOfItems = itemBoxes.Length;
        }

        for (int i = 0; i < nOfItems; i++)
        {
            Item itemBox = itemBoxes[i].GetComponent<Item>();
            itemBox.ItemName = itemNames[i];
            itemBoxes[i].tag = "Item";
        }
    }

    private void ShuffleArray(string[] texts)
    {
        for(int i = 0; i < texts.Length; i++)
        {
            string tmp = texts[i];
            int r = UnityEngine.Random.Range(i, texts.Length);
            texts[i] = texts[r];
            texts[r] = tmp;
        }
    }
}
