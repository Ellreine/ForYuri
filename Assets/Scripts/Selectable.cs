using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool top = false;
    public string suit;
    public int value;
    public int row;
    public bool faceUp = false;
    public bool inDeckPile = false;

    private string valueString;

    void Start()
    {
        if (CompareTag("Card"))
        {
            suit = transform.name[0].ToString();
            valueString = transform.name.Substring(1);

            switch (valueString)
            {
                case "A":
                    value = 1; // Òóç = 1
                    break;
                case "2":
                    value = 2;
                    break;
                case "3":
                    value = 3;
                    break;
                case "4":
                    value = 4;
                    break;
                case "5":
                    value = 5;
                    break;
                case "6":
                    value = 6;
                    break;
                case "7":
                    value = 7;
                    break;
                case "8":
                    value = 8;
                    break;
                case "9":
                    value = 9;
                    break;
                case "10":
                    value = 10;
                    break;
                case "J":
                    value = 11;
                    break;
                case "Q":
                    value = 12;
                    break;
                case "K":
                    value = 13;
                    break;
                default:
                    Debug.LogError("Invalid card value: " + valueString);
                    break;
            }
            Debug.Log("Card created: " + transform.name + " with value " + value);
        }
    }

    void Update()
    {

    }
}
