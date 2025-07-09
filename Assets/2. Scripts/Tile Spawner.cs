using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform  tilePannel;

    private readonly int    width = 10, height = 15;
    private readonly int    spacing = 15;
    private readonly int    yOffset = 120;

    private void Awake()
    {
        SpawnTile();
    }
    public void SpawnTile()
    {
        Vector2 size = tilePrefab.GetComponent<RectTransform>().sizeDelta;
        size += new Vector2(spacing, spacing);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject clone = Instantiate(tilePrefab, tilePannel);
                RectTransform rect = clone.GetComponent<RectTransform>();

                float px = (-width * 0.5f + 0.5f + x) * size.x;
                float py = (height * 0.5f - 0.5f - y) * size.y - yOffset;
                rect.anchoredPosition = new Vector2(px, py);
            }
        }
    }
}
