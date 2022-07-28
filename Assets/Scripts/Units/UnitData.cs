using System;

[Serializable]
public class UnitData
{
    public int Damage;
    public float ShotFrequency;
    public float ReloadTime;
    public int Score;
    public int Hp;

    public UnitData (int damage, float shotFrequency, float reloadTime, int score, int hp)
    {
        Damage = damage;
        ShotFrequency = shotFrequency;
        ReloadTime = reloadTime;
        Score = score;
        Hp = hp;
    }

    public UnitData Duplicate()
    {
        return new UnitData(Damage, ShotFrequency, ReloadTime, Score, Hp);
    }
}