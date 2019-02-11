using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private string[] keyNames;

    [Header("Items")]
    [SerializeField] private string[] itemNames;

    private GameObject[] keyBoxes;
    private GameObject randomizedItemBox;
    private int randomItemBox;
    private int nOfItems;

    private void Start()
    {
        // shuffle items.
        ShuffleArray(keyNames, 0);
        // find item boxes.
        keyBoxes = GameObject.FindGameObjectsWithTag("KeyBox");

        if(keyBoxes.Length > keyNames.Length)
        {
            nOfItems = keyBoxes.Length - keyNames.Length;
#if UNITY_EDITOR
            // number of items less than item (spawn) boxes
            Debug.Log("Number of items is less than item boxes. Items: " + keyNames.Length + " SpawnBoxes: " + keyBoxes.Length);
#endif
        }
        else
        {
            nOfItems = keyBoxes.Length;
        }

        for(int i = 0; i < nOfItems; i++) {
            Item itemBox = keyBoxes[i].GetComponent<Item>();

            if(keyNames[i].Contains(itemBox.gameObject.transform.name)) {
                while(keyNames[i].Contains(itemBox.gameObject.transform.name)) {                 
                    if(i < nOfItems - 1) {
                        ShuffleArray(keyNames, i);
                    }
                    else {
                        // if last then swap with first itemBox.
                        string swapName = keyNames[i];
                        keyNames[0] = keyNames[i];
                        keyNames[i] = keyBoxes[0].GetComponent<Item>().ItemName;
                        keyBoxes[0].GetComponent<Item>().ItemName = swapName;
                        keyBoxes[i].tag = "Key";
                        break;
                    }
                }
                itemBox.ItemName = keyNames[i];
                keyBoxes[i].tag = "Key";
            }
            else {
                itemBox.ItemName = keyNames[i];
                keyBoxes[i].tag = "Key";
            }
        }
    }

    private void ShuffleArray(string[] texts, int i)
    {
        for(; i < texts.Length; i++)
        {
            string tmp = texts[i];
            int r = UnityEngine.Random.Range(i, texts.Length);
            texts[i] = texts[r];
            texts[r] = tmp;
        }
    }
}
