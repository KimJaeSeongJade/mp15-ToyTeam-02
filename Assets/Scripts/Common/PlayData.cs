[System.Serializable]
public class PlayData
{
    public GameMode GameMode;
    public GameDifficulty GameDifficulty;
    public int TrainDistance;
    public float PlayTime;

    public PlayData(GameMode gameMode, GameDifficulty gameDifficulty, int trainDistance, float playTime)
    {
        GameMode = gameMode;
        GameDifficulty = gameDifficulty;
        TrainDistance = trainDistance;
        PlayTime = playTime;
    }
}