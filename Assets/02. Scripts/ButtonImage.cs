using UnityEngine;

public class ButtonImage : MonoBehaviour
{
    public int colorValue;
    public Vector2Int mat;

    private bool hasClicked;
    private bool sideCheck = true;

    private RectTransform rectTransform;

    void Awake() => rectTransform = GetComponent<RectTransform>();

    void Update()
    {
        bool isMouseOver = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);

        if (isMouseOver && PointDown.isDragging)
        {
            if (!hasClicked)
                TryAddColor();
        }
        else
        {
            hasClicked = false;
        }

        if (isMouseOver && GameManager2.successObj && !PointDown.isDragging)
        {
            foreach (var btn in GameManager2.buttonObjList)
                btn.gameObject.SetActive(false);

            GameManager2.successObj = false;
            GameManager2.list.Clear();
            GameManager2.buttonObjList.Clear();
            GameManager2.matList.Clear();
        }
    }

    private void TryAddColor()
    {
        GameManager2.list.Add(colorValue);
        GameManager2.buttonObjList.Add(this);
        GameManager2.matList.Add(mat);
        hasClicked = true;
        CheckSide();
        if (!sideCheck)
        {
            GameManager2.list.Clear();
            GameManager2.buttonObjList.Clear();
            GameManager2.matList.Clear();
        }
    }

    private void CheckSide()
    {
        sideCheck = true;
        var matList = GameManager2.matList;

        for (int i = 1; i < matList.Count; i++)
        {
            Vector2Int a = matList[i - 1];
            Vector2Int b = matList[i];

            if (!(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1))
            {
                sideCheck = false;
                break;
            }
        }
    }
}
