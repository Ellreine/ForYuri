using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public GameObject highScorePanel;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }
    public void ResetScene()
    {
        // ”ничтожить все объекты карт
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        // ќчистить списки значений и карт
        ClearTopValues();
        ClearBottomValues();

        // —бросить колоду и раздать карты заново
        FindObjectOfType<Solitaire>().PlayCards();
    }

    void ClearTopValues()
    {
        Selectable[] selectables = FindObjectsOfType<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            if (selectable.CompareTag("Top"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
    }

    void ClearBottomValues()
    {
        Selectable[] selectables = FindObjectsOfType<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            if (selectable.CompareTag("Bottom"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
    }
}
