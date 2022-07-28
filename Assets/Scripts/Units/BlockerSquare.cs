using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerSquare : MonoBehaviour, IDamagable
{
    public void MakeDamage(int _)
    {
        SetShow(false);
    }

    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
