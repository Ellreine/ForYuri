using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    public GameObject deckButton;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] top;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> deck;
    public List<string> discardPile = new List<string>();
    private int deckLocation;
    private int trips;
    private int tripsRemainder;

    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        PlayCards();
    }

    void Update()
    {

    }

    public void PlayCards()
    {
        // Сброс всех списков и значений
        foreach (List<string> bottom in bottoms)
        {
            bottom.Clear();
        }
        deck.Clear();
        discardPile.Clear();
        tripsOnDisplay.Clear();
        deckTrips.Clear();
        deckLocation = 0; // Сбрасываем deckLocation
        trips = 0;
        tripsRemainder = 0;

        // Генерация и перемешивание новой колоды
        deck = GenerateDeck();
        Shuffle(deck);

        // Раскладка карт
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }
        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.03f;

            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(0.03f);
                GameObject newCard = Instantiate(cardPrefab,
                    new Vector3(bottomPos[i].transform.position.x,
                                bottomPos[i].transform.position.y - yOffset,
                                bottomPos[i].transform.position.z - zOffset),
                    Quaternion.identity, bottomPos[i].transform);

                newCard.name = card;
                newCard.tag = "Card";
                newCard.layer = LayerMask.NameToLayer("Default");
                newCard.GetComponent<Selectable>().row = i;
                newCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // Установим масштаб карты

                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }

                yOffset += 0.3f; // Увеличение отступа для следующей карты
                zOffset += 0.03f; // Увеличение отступа по Z для правильного наложения
                discardPile.Add(card);
            }
        }

        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }

    void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                bottoms[i].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        trips = deck.Count / 3;
        tripsRemainder = deck.Count % 3;
        deckTrips.Clear();

        int modifier = 0;
        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(deck[j + modifier]);
            }
            deckTrips.Add(myTrips);
            modifier = modifier + 3;
        }
        if (tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k < tripsRemainder; k++)
            {
                myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
                modifier++;
            }
            deckTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }

    public void DealFromDeck()
    {
        // Если больше нет карт в колоде, не делаем ничего
        if (deckLocation >= deck.Count)
        {
            return;
        }

        // Начальные позиции для смещения
        float xOffset = 2.5f;
        float yOffset = 0f;
        float zOffset = -0.3f;

        // Удалить предыдущие карты из отображения
        foreach (Transform child in deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                // Переворачиваем предыдущую карту рубашкой вверх
                Selectable selectable = child.GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.faceUp = false;
                    child.transform.position = new Vector3(child.transform.position.x, child.transform.position.y, child.transform.position.z + 0.1f); // Смещаем назад по оси Z
                }
            }
        }

        // Добавляем одну карту на стол
        string card = deck[deckLocation];
        GameObject newTopCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y + yOffset, deckButton.transform.position.z + zOffset), Quaternion.identity, deckButton.transform);
        newTopCard.name = card;
        newTopCard.tag = "Card"; // Убедитесь, что тег установлен
        newTopCard.layer = LayerMask.NameToLayer("Default"); // Убедитесь, что слой установлен

        // Убедитесь, что компонент Selectable настроен правильно
        Selectable selectableNew = newTopCard.GetComponent<Selectable>();
        if (selectableNew != null)
        {
            selectableNew.faceUp = true;
            selectableNew.inDeckPile = true;
        }

        tripsOnDisplay.Add(card);
        deckLocation++;
    }


    public void RestackTopDeck()
    {
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        SortDeckIntoTrips();
        deckLocation = 0; // Сбрасываем deckLocation, чтобы начать снова
    }
}
