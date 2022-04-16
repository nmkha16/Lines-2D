
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Savestate
{
    //attribute
    public Dictionary<Vector2, int> _ballPos;
    public Dictionary<Vector2, int> _ghostBallPos;
    public Dictionary <Vector2,int >_nextQueueBallsPos;

    public int _playTime;
    public int _playScore;

    // constructor for lazy-easy savestate
    public Savestate(Dictionary<Vector2, Ball> ballPos, Dictionary<Vector2, Ball> ghostBallPos, Dictionary<Vector2,int> nextQueueBalls,
        int playTime, int playScore)
    {
        // we dont need to save <Vector2,Ball>, just need <Vector2,int>
        // int here means we store the ID color
        _ballPos = formatDictionaryToInt(ballPos);
        _ghostBallPos = formatDictionaryToInt(ghostBallPos);
        //
        _nextQueueBallsPos = nextQueueBalls;
        _playTime = playTime;
        _playScore = playScore;
    }

    private Dictionary<Vector2,int> formatDictionaryToInt(Dictionary<Vector2,Ball> input)
    {
        Dictionary<Vector2,int> result = new Dictionary<Vector2,int>();

        foreach (var key in input.Keys)
        {
            result.Add(key, input[key]._colorID);
        }

        return result;
    }
}


