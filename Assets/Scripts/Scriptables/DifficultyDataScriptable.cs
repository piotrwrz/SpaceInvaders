using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DifficultyData/Data")]
public class DifficultyDataScriptable : ScriptableObject
{
    public EDifficulty Difficulty;
    public string DifficultyName;
    public int MinEnemiesInLine;
    public int MaxEnemiesInLine;
    public int MinEnemyLines;
    public int MaxEnemyLines;
}
