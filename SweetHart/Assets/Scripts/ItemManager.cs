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

    void Start()
    {
        // shuffle items.
        ShuffleArray(itemNames);
        // find item boxes.
        itemBoxes = GameObject.FindGameObjectsWithTag("ItemBox");

        for (int i = 0; i < itemBoxes.Length; i++)
        {
            Item itemBox = itemBoxes[i].GetComponent<Item>();
            itemBox.ItemName = itemNames[i];
            itemBoxes[i].tag = "Item";
        }
    }

    void ShuffleArray(string[] texts)
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
