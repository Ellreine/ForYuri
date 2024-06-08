using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public Selectable[] topStacks;

    void Start()
    {
        
    }

    void Update()
    {
        if (HasWon())
        {
            Win();
        }
    }

    public bool HasWon()
    {
        int i = 0;
        foreach (Selectable topstack in topStacks)
        {
            i += topstack.value;
        }
        if (i >= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Win()
    {
        print("You have won");
    }
}
