using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public TextMeshProUGUI P1ScoreText;
    public TextMeshProUGUI P2ScoreText;

    protected int P1Score = 0;
    protected int P2Score = 0;

    public void ScorePoints (Player.Players player, int points)
    {
        switch (player)
        {
            case Player.Players.P1:
                P1Score += points;
                P1ScoreText.text = P1Score.ToString("00000");
                break;

            case Player.Players.P2:
                P2Score += points;
                P2ScoreText.text = P2Score.ToString("00000");
                break;
        }
    }

}
