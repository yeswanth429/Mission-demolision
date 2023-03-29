using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class scoreManager : MonoBehaviour
{
    public float scoreL;
    public UnityEvent<string, float> submitScoreEvent;
    MissionDemolition missionD;
    public TMP_InputField inputName;
    // Start is called before the first frame update
    void Awake()
    {
        missionD = GameObject.Find("_MainCamera").GetComponent<MissionDemolition>();
        scoreL = missionD.finalScore;
    }
    public void SubmitScore()
    {
        submitScoreEvent.Invoke(inputName.text, scoreL);
    }
}
