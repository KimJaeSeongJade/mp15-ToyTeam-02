[System.Serializable]
public class PlayData
{
    public GameMode GameMode;
    public float TrainSpeed;
    public int TrainDistance;
    public float PlayTime;

    public PlayData(GameMode gameMode, float trainSpeed, int trainDistance, float playTime)
    {
        GameMode = gameMode;
        TrainSpeed = trainSpeed;
        TrainDistance = trainDistance;
        PlayTime = playTime;
    }
}