using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : GameStateUi
{
    [SerializeField]
    private MenuInput menuInput = null;
    [SerializeField]
    private Button difficultyLeft = null;
    [SerializeField]
    private Button difficultyRight = null;
    [SerializeField]
    private TMPro.TMP_Text difficultyText = null;

    private GameManager gameManager;

    public void Init(GameManager gameMan)
    {
        gameManager = gameMan;
        menuInput.Init(gameManager);
        difficultyLeft.onClick.AddListener(() => gameManager.ChangeDifficulty(false));
        difficultyRight.onClick.AddListener(() => gameManager.ChangeDifficulty(true));
        gameManager.DifficultyChanged += DifficultyChanged;
    }

    public void DifficultyChanged(DifficultyDataScriptable difficultyData)
    {
        difficultyText.text = difficultyData.DifficultyName;
    }
}
