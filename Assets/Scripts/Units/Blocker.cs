using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField]
    private List<BlockerSquare> squares = null;

    public void Repair()
    {
        foreach (var s in squares)
        {
            s.SetShow(true);
        }
    }

    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
