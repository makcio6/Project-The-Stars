using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private GameObject heartPrefab;

    private List<Image> heartImages = new List<Image>();

    public void Init(int max)
    {
        for (int i = 0; i < max; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image img = heart.GetComponent<Image>();
            heartImages.Add(img);
        }
    }

    public void SetHealth(int current)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].sprite = (i < current) ? fullHeartSprite : emptyHeartSprite;
        }
    }

    public void AddContainer()
    {
        GameObject heart = Instantiate(heartPrefab, transform);
        Image img = heart.GetComponent<Image>();
        heartImages.Add(img);
    }

    public void RemoveContainer()
    {
        if (heartImages.Count == 0) return;
        Destroy(heartImages[^1].gameObject);
        heartImages.RemoveAt(heartImages.Count - 1);
    }
}
