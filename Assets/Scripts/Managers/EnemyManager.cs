using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Globalization;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Enemy specialEnemy = null;
    [SerializeField]
    private RectTransform specialEnemyRect = null;
    [SerializeField]
    private UnitData specialEnemyData = null;
    [SerializeField]
    private float specialEnemyHorizontalTimeMin = 2f;
    [SerializeField]
    private float specialEnemyHorizontalTimeMax = 5f;
    [SerializeField]
    private List<Enemy> enemyPrefabs = null;
    [SerializeField]
    private RectTransform enemiesParent = null;
    [SerializeField]
    private Vector2 enemiesSpacing = Vector2.zero;
    [SerializeField]
    private float timeToPrepare = 3f;
    [SerializeField]
    private float enemySpeed = 300f;
    [SerializeField]
    private int specialEnemyTime = 10;
    [SerializeField]
    private int specialEnemyChance = 20;

    private GameManager gameManager;

    private int direction;
    private int specialEnemyDirection;
    private Vector2 gridSize;
    private float halfScreenWidth;
    private float halfScreenHeight;
    private float halfEnemiesParentWidth;
    private float prepareTimer;
    private float specialEnemyTimer;
    private float specialEnemyHorizontalTimer;
    private float specialEnemyHorizontalTime;
    private Vector2 enemiesStartPos;
    private Vector2 enemiesEndPos;
    private Vector2 specialEnemyStartPos;
    private Vector2 specialEnemyEndPos;

    private Dictionary<EEnemyType, ObjectPool<Enemy>> enemyPoolsDictionary = new Dictionary<EEnemyType, ObjectPool<Enemy>>();
    private Dictionary<EEnemyType, Enemy> enemyPrefabsDictionary = new Dictionary<EEnemyType, Enemy>();

    private List<Enemy> aliveEnemies = new List<Enemy>();

    private void Update()
    {
        switch (gameManager.GameStateManager.CurrentState)
        {
            case EGameState.Preparation:
                prepareTimer += Time.deltaTime;
                enemiesParent.anchoredPosition = Vector2.Lerp(enemiesStartPos, enemiesEndPos, prepareTimer / timeToPrepare);
                if (prepareTimer >= timeToPrepare)
                {
                    gameManager.GameStateManager.ChangeState(ECommand.Begin);
                }
                break;
            case EGameState.Game:
                foreach (var enemy in aliveEnemies)
                {
                    enemy.MyUpdate();
                }
                enemiesParent.anchoredPosition = new Vector2(Mathf.Clamp(enemiesParent.anchoredPosition.x + enemySpeed * Time.deltaTime * direction, -halfScreenWidth, halfScreenWidth), enemiesParent.anchoredPosition.y);
                if (direction == 1 && enemiesParent.anchoredPosition.x + halfEnemiesParentWidth >= halfScreenWidth)
                {
                    direction = -1;
                }
                else if (direction == -1 && enemiesParent.anchoredPosition.x - halfEnemiesParentWidth <= -halfScreenWidth)
                {
                    direction = 1;
                }
                if (!specialEnemy.gameObject.activeInHierarchy)
                {
                    specialEnemyTimer += Time.deltaTime;
                    if (specialEnemyTimer >= specialEnemyTime)
                    {
                        specialEnemyTimer = 0f;
                        var rand = UnityEngine.Random.Range(0f, 100f);
                        if (rand <= specialEnemyChance)
                        {
                            SpawnSpecialEnemy();
                        }
                    }
                }
                else
                {
                    specialEnemyHorizontalTimer += Time.deltaTime * specialEnemyDirection;
                    float percent = specialEnemyHorizontalTimer / specialEnemyHorizontalTime;
                    specialEnemyRect.anchoredPosition = Vector2.Lerp(specialEnemyStartPos, specialEnemyEndPos, percent);
                    if (percent >= 1)
                    {
                        specialEnemyDirection = -1;
                    }
                    else if (percent <= 0)
                    {
                        specialEnemyDirection = 1;
                    }
                }
                break;
        }
    }

    private void SpawnSpecialEnemy()
    {
        specialEnemyHorizontalTime = UnityEngine.Random.Range(specialEnemyHorizontalTimeMin, specialEnemyHorizontalTimeMax);
        specialEnemy.SetupSpecialEnemy(specialEnemyData, specialEnemyStartPos);
    }

    public void Init(GameManager gameManager)
    {
        halfScreenWidth = Screen.width / 2f;
        halfScreenHeight = Screen.height / 2f;
        enemiesStartPos = new Vector2(0f, halfScreenWidth);
        enemiesEndPos = new Vector2(0f, halfScreenWidth / 5f);
        specialEnemyStartPos = new Vector2(-halfScreenWidth + 50f, halfScreenHeight * 4f / 5f);
        specialEnemyEndPos = new Vector2(halfScreenWidth - 50f, halfScreenHeight * 4f / 5f);
        this.gameManager = gameManager;
        gridSize = enemiesSpacing + enemyPrefabs[0].Size;
        foreach (var enemyPrefab in enemyPrefabs)
        {
            enemyPoolsDictionary[enemyPrefab.EnemyType] = new ObjectPool<Enemy>(() =>
            {
                var enemy = Instantiate(enemyPrefab, enemiesParent);
                enemy.Died += OnEnemyDied;
                return enemy;
            });
            enemyPrefabsDictionary[enemyPrefab.EnemyType] = enemyPrefab;
        }
    }

    public void Setup(DifficultyDataScriptable difficulty)
    {
        aliveEnemies.Clear();
        prepareTimer = 0f;
        direction = 1;
        specialEnemyTimer = 0f;
        foreach (var prefab in enemyPrefabs)
        {
            var input = File.ReadAllLines(prefab.InputFilePath);
            if (input.Length != 12)
            {
                throw new Exception("Invalid enemy input file at path: " + prefab.InputFilePath);
            }
            int difficultyIndex = (int)difficulty.Difficulty;

            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ",";
            var damage = int.Parse(Regex.Match(input[0 + difficultyIndex * 4], @"\d+").Value);
            var shotFrequency = float.Parse(Regex.Match(input[1 + difficultyIndex * 4], @"\d+\,\d+").Value, ci);
            var reloadTime = float.Parse(Regex.Match(input[2 + difficultyIndex * 4], @"\d+\,\d+").Value, ci);
            var score = int.Parse(Regex.Match(input[3 + difficultyIndex * 4], @"\d+").Value);

            prefab.Setup(new UnitData(damage, shotFrequency, reloadTime, score, prefab.BaseHp));
        }

        int enemyLines = UnityEngine.Random.Range(difficulty.MinEnemyLines, difficulty.MaxEnemyLines + 1);
        int enemyInLines = UnityEngine.Random.Range(difficulty.MinEnemiesInLine, difficulty.MaxEnemiesInLine + 1);
        float verticalOffset = enemyLines % 2 == 0 ? 0.5f : 0f;
        float baseHorizontalOffset = enemyInLines % 2 == 0 ? 0.5f : 0f;
        for (int i = 0; i < enemyLines; i++)
        {
            var type = (EEnemyType)UnityEngine.Random.Range(0, (int)EEnemyType.Count);
            float horizontalOffset = baseHorizontalOffset;
            for (int j = 0; j < enemyInLines; j++)
            {
                var enemy = enemyPoolsDictionary[type].Get();
                var position = new Vector2(horizontalOffset * gridSize.x, verticalOffset * gridSize.y);
                enemy.Init(gameManager.BulletPoolsDictionary[enemy.BulletType]);
                enemy.Setup(enemyPrefabsDictionary[type].CurrentUnitData, position, enemyPoolsDictionary[type]);
                aliveEnemies.Add(enemy);
                horizontalOffset = -horizontalOffset;
                if (horizontalOffset >= 0)
                {
                    horizontalOffset++;
                }
            }
            verticalOffset = -verticalOffset;
            if (verticalOffset >= 0)
            {
                verticalOffset++;
            }
        }
        enemiesParent.sizeDelta = new Vector2(enemyInLines * gridSize.x, enemyLines * gridSize.y);
        halfEnemiesParentWidth = enemiesParent.sizeDelta.x / 2f;
    }

    public void Clear()
    {
        foreach (var enemy in aliveEnemies)
        {
            enemy.Return();
        }
        specialEnemy.gameObject.SetActive(false);
    }

    private void OnEnemyDied(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);
        gameManager.ChangeScore(enemy.CurrentUnitData.Score);
        if (aliveEnemies.Count == 0)
        {
            gameManager.GameStateManager.ChangeState(ECommand.Begin);
        }
    }
}
