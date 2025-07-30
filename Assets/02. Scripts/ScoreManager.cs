using UnityEngine;
using TMPro;

public class ScoreManger : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private GameManager2 gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager2>();
    }

    void Update()
    {
        scoreText.text = "Score: " + gameManager.score;
    }
}

