using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : GameStateUi
{
    [SerializeField]
    private GameObject newHighscore = null;
    [SerializeField]
    private TMPro.TMP_Text highscore = null;
    [SerializeField]
    private TMPro.TMP_Text score = null;
    [SerializeField]
    private GameOverInput input = null;

    public void Init(GameManager gameMan)
    {
        input.Init(gameMan);
    }

    public void Setup(int score)
    {
        var highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (score > highscore)
        {
            newHighscore.SetActive(true);
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
        }
        else
        {
            newHighscore.SetActive(false);
        }
        this.highscore.text = "Highscore : " + highscore.ToString();
        this.score.text = "Score : " + score.ToString();
        SetShow(true);
    }
}
