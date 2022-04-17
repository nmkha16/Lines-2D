
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Savestate
{
    //attribute
    public Dictionary<Vector2, int> _ballPos { get; set; }
    public Dictionary<Vector2, int> _ghostBallPos { get; set; }
    public Dictionary <Vector2,int> _nextQueueBallsPos { get; set; }

    public int _playTime { get; set; }
    public int _playScore { get; set; }

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

    public Savestate (Savestate s)
    {
        _ballPos = s._ballPos;
        _ghostBallPos= s._ghostBallPos;
        _nextQueueBallsPos= s._nextQueueBallsPos;
        _playTime = s._playTime;
        _playScore = s._playScore;
    }


    public Savestate(SavestateFormatter sf)
    {
        _ballPos = new Dictionary<Vector2, int>();
        // iterate ballPos
        foreach(string vectorString in sf._ballPos.Keys)
        {
            Vector2 parseVector2 = stringToVector2(vectorString);
            _ballPos.Add(parseVector2, sf._ballPos[vectorString]);
        }

        _ghostBallPos = new Dictionary<Vector2, int>();
        // iterate ghostPos
        foreach (string vectorString in sf._ghostBallPos.Keys)
        {
            Vector2 parseVector2 = stringToVector2(vectorString);
            _ghostBallPos.Add(parseVector2, sf._ghostBallPos[vectorString]);
        }


        _nextQueueBallsPos = new Dictionary<Vector2, int>();
        //iterate queuePos
        foreach (string vectorString in sf._nextQueueBallsPos.Keys)
        {
            Vector2 parseVector2 = stringToVector2(vectorString);
            _nextQueueBallsPos.Add(parseVector2, sf._nextQueueBallsPos[vectorString]);
        }

        // set playtime and playscore
        _playScore = sf._playScore;
        _playTime = sf._playTime;
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
    

    // format string should be "(x,y)"
    private Vector2 stringToVector2(string input)
    {
        var parts = input.Split(new char[] { '(', ')', ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        var x = float.Parse(parts[0]);
        var y = float.Parse(parts[1]);
        var vector2Int = new Vector2(x, y);
        return vector2Int;
    }
}

public class SavestateFormatter{
    public Dictionary<string, int> _ballPos { get; set; }
    public Dictionary<string, int> _ghostBallPos { get; set; }
    public Dictionary<string, int> _nextQueueBallsPos { get; set; }

    public int _playTime { get; set; }
    public int _playScore { get; set; }
}


