using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;
    private UserInput userInput;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        userInput = FindObjectOfType<UserInput>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();

        AssignCardFace();
    }

    void AssignCardFace()
    {
        List<string> deck = Solitaire.GenerateDeck();
        int i = 0;
        foreach (string card in deck)
        {
            if (this.name == card)
            {
                cardFace = solitaire.cardFaces[i];
                Debug.Log("Assigning sprite to card: " + card + " with sprite index " + i);
                break;
            }
            i++;
        }
    }

    void Update()
    {
        if (selectable.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
        if (name == userInput.slot1.name)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
