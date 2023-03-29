using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;


public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;
    [SerializeField]
    private List<TextMeshProUGUI> Scores;

    private float scoreL;
    MissionDemolition missionD;
    public TMP_InputField inputName;
    void Awake()
    {
        GetLeaderboard();
    }

    public string publicLeaderboardKey = "228845ed04594a37785f32fb0b9f47fea4359ce44ef5b1c0b929ce012583cd45";


    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            for (int i = 0; i < names.Count; ++i)
            {
                names[i].text = msg[i].Username;
                Scores[i].text = msg[i].Score.ToString();
            }
        }));

    }
    public void SubmitScore()
    {
        missionD = GameObject.Find("_MainCamera").GetComponent<MissionDemolition>();
        scoreL = missionD.finalScore;
        SetLeaderboardEntry(inputName.text, scoreL);
    }
    public void SetLeaderboardEntry(string username, float score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, (int)score, ((msg) =>
        {
            GetLeaderboard();
        }));
    }
}
