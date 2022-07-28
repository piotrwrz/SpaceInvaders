using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUi : GameStateUi
{
    [SerializeField]
    private Player player = null;
    [SerializeField]
    private TMPro.TMP_Text score = null;
    [SerializeField]
    private TMPro.TMP_Text playerHealth = null;

    public void Init(GameManager gameMan)
    {
        player.Hit += OnPlayerHit;
        gameMan.ScoreChanged += ScoreChanged;
    }

    private void OnEnable()
    {
        playerHealth.text = "Health : " + player.CurrentUnitData.Hp.ToString();
    }

    private void OnPlayerHit(int hpLeft)
    {
        playerHealth.text = "Health : " + hpLeft;
    }

    private void ScoreChanged(int score)
    {
        this.score.text = "Score : " + score;
    }
}
