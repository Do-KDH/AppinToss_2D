using System.Collections; // 코루틴 사용을 위해 추가
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour
{
    public static List<int> list = new();
    public static List<ButtonImage> buttonObjList = new();
    public static List<Vector2Int> matList = new();

    public static bool successObj;
    public int score;
    public float timeLeft = 120f;
    public bool isGameOver = false;
    public GameObject gameOverPanel;

    public RandomCreate randomCreateRef;
    public GameObject randomCreatePrefab;
    private GameObject randomCreateInstance;
        
    public Animator resetAnimator;
    public AnimationClip resetAnimation;
    private bool isResetting = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;

        if (randomCreateRef == null)
        {
            randomCreateRef = FindAnyObjectByType<RandomCreate>();

            if (randomCreateRef == null && randomCreatePrefab != null)
            {
                randomCreateInstance = Instantiate(randomCreatePrefab);
                randomCreateRef = randomCreateInstance.GetComponent<RandomCreate>();
            }
        }
    }

    void Update()
    {
        if (isGameOver) return;
        UpdateTimer();

        CheckAnswer();

        if (CanCheckRemainingSequences() && !isResetting)
        {
            randomCreateRef.InitializeCurrentActiveGrid();

            if (randomCreateRef.HasNoRemainingSequences())
                StartCoroutine(PlayResetAnimationAndResetGrid());
        }
    }

    private IEnumerator PlayResetAnimationAndResetGrid()
    {
        isResetting = true;
        Debug.Log("리셋 애니메이션 시작");

        if (resetAnimator != null)
        {
            resetAnimator.SetTrigger("StartReset");
        }
        float animationLength = (resetAnimation != null) ? resetAnimation.length : 1.0f;
        yield return new WaitForSeconds(animationLength);

        Debug.Log("리셋 로직 실행");
        if (randomCreateRef != null)
        {
            randomCreateRef.ResetGrid();
        }

        isResetting = false;
    }
    void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("GameOver");
    }

    private bool CanCheckRemainingSequences()
    {
        return !PointDown.isDragging && !successObj && randomCreateRef != null && list.Count == 0;
    }

    void CheckAnswer()
    {
        if (list.Count > 2 && list[^2] + 1 != list[^1])
        {
            list.Clear();
            buttonObjList.Clear();
        }
        else if (list.Count > 2 && !PointDown.isDragging)
        {
            int point = list.Count;
            score += point switch
            {
                3 => 50,
                4 => 80,
                5 => 130,
                6 => 200,
                7 => 300,
                _ => 0
            };

            Debug.Log($"{point}개 성공! 현재 점수: {score}");
            Success();
        }
        else if (list.Count <= 2 && !PointDown.isDragging)
        {
            list.Clear();
            buttonObjList.Clear();
        }
    }

    void Success()
    {
        successObj = true;
        matList.Clear();
    }
}
