[System.Serializable]
public class PlayData
{
    public float TrainSpeed;
    public float PlayTime;

    public PlayData(float trainSpeed, float playTime)
    {
        TrainSpeed = trainSpeed;
        PlayTime = playTime;
    }
}