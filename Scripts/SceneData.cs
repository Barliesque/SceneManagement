using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData
{
    public string id;
    public string subtitle;
    public string title;
    public string titleColor;
    public int numObjectives;
    public int numObjectivesCompleted;
    public float seconds;
    public string infoText;

    public SceneData(
        string _id,
        string _subtitle, 
        string _title, 
        string _titleColor, 
        int _numObjectives,
        int _seconds,
        string _infoText
        )
    {
        id = _id;
        subtitle = _subtitle;
        title = _title;
        titleColor = _titleColor;
        numObjectives = _numObjectives;
        seconds = _seconds;
        infoText = _infoText;
    }
}
