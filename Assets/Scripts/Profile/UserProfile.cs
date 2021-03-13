using System;
using UnityEngine;

[Serializable]
public class UserProfile
{
    public const string ANONIMOUS = "Anonimous";

    public string name;
    public int whiteWins;
    public int whiteLose;
    public int whiteDraw;
    public int whitePlayed;
    public int blackWins;
    public int blackLose;
    public int blackDraw;
    public int blackPlayed;
    public int sesionNumber;

    public UserProfile(bool exist)
    {
        if (!exist)
            name = ANONIMOUS;
    }

    public UserProfile(string _name)
    {
        name = _name;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        UserProfile profileTemp = JsonUtility.FromJson<UserProfile>(json);

        this.name = profileTemp.name;
        this.whiteWins = profileTemp.whiteWins;
        this.whiteLose = profileTemp.whiteLose;
        this.whitePlayed = profileTemp.whitePlayed;
        this.blackWins = profileTemp.blackWins;
        this.blackLose = profileTemp.blackLose;
        this.blackPlayed = profileTemp.blackPlayed;
    }
}
