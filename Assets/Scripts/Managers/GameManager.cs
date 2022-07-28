using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    public event Action<DifficultyDataScriptable> DifficultyChanged = delegate { };
    public event Action<int> ScoreChanged = delegate { };

    public DifficultyDataScriptable CurrentDifficulty
    {
        get;
        set;
    }

    public GameStateManager GameStateManager => gameStateManager;
    public ParticleManager ParticleManager => particleManager;
    public Dictionary<EBulletType, ObjectPool<Bullet>> BulletPoolsDictionary => bulletPoolsDictionary;

    [SerializeField]
    private GameStateManager gameStateManager = null;
    [SerializeField]
    private ParticleManager particleManager = null;
    [SerializeField]
    private EnemyManager enemyManager = null;
    [SerializeField]
    private Menu menu = null;
    [SerializeField]
    private InGameUi inGameUi = null;
    [SerializeField]
    private GameOverScreen gameOver = null;
    [SerializeField]
    private Player player = null;
    [SerializeField]
    private List<DifficultyDataScriptable> difficulties = null;          // Order: Easy -> Medium -> Hard
    [SerializeField]
    private List<Bullet> bulletPrefabs = null;
    [SerializeField]
    private RectTransform bulletsParent = null;
    [SerializeField]
    private List<Blocker> blockers = null;
    [SerializeField]
    private GameObject pause = null;

    private int currentDifficultyIndex;
    private int score;
    private Dictionary<EBulletType, ObjectPool<Bullet>> bulletPoolsDictionary = new Dictionary<EBulletType, ObjectPool<Bullet>>();

    private void Awake()
    {
        instance = this;

        gameStateManager.Init();
        gameStateManager.StateChanged += OnGameStateChanged;
        OnGameStateChanged(gameStateManager.CurrentState);

        menu.Init(this);
        inGameUi.Init(this);
        gameOver.Init(this);
        SetDifficulty(1);

        foreach (var bulletPrefab in bulletPrefabs)
        {
            bulletPoolsDictionary[bulletPrefab.BulletType] = new ObjectPool<Bullet>(() =>
            {
                var bullet = Instantiate(bulletPrefab, bulletsParent);
                bullet.Setup(bulletPoolsDictionary[bulletPrefab.BulletType]);
                return bullet;
            });
        }

        player.Init(bulletPoolsDictionary[EBulletType.PlayerBullet]);
        player.PlayerDead += OnPlayerDied;

        enemyManager.Init(this);
    }

    public void ChangeDifficulty(bool right)
    {
        if (right)
        {
            currentDifficultyIndex = currentDifficultyIndex == difficulties.Count - 1 ? 0 : currentDifficultyIndex + 1;
        }
        else
        {
            currentDifficultyIndex = currentDifficultyIndex == 0 ? difficulties.Count - 1 : currentDifficultyIndex - 1;
        }
        SetDifficulty(currentDifficultyIndex);
    }

    public void ChangeScore(int score)
    {
        this.score += score;
        ScoreChanged(this.score);
    }

    private void SetDifficulty(int difficulty)
    {
        currentDifficultyIndex = difficulty;
        CurrentDifficulty = difficulties[currentDifficultyIndex];
        DifficultyChanged(CurrentDifficulty);
    }

    private void OnGameStateChanged(EGameState state)
    {
        switch (state)
        {
            case EGameState.Intro:
                menu.SetShow(true);
                gameOver.SetShow(false);
                score = 0;
                foreach (var blocker in blockers)
                {
                    blocker.Repair();
                }
                player.Setup();
                break;
            case EGameState.Preparation:
                foreach (var blocker in blockers)
                {
                    blocker.SetShow(true);
                }    
                player.SetEnableInput(true);
                player.SetShow(true);
                menu.SetShow(false);
                inGameUi.SetShow(true);
                enemyManager.Setup(CurrentDifficulty);
                ScoreChanged(score);
                break;
            case EGameState.Game:
                Time.timeScale = 1f;
                pause.SetActive(false);
                break;
            case EGameState.Paused:
                pause.SetActive(true);
                Time.timeScale = 0f;
                break;
            case EGameState.End:
                gameOver.Setup(score);
                inGameUi.SetShow(false);
                enemyManager.Clear();
                foreach (var blocker in blockers)
                {
                    blocker.SetShow(false);
                }
                break;
        }
    }

    private void OnPlayerDied()
    {
        gameStateManager.ChangeState(ECommand.End);
    }
}
