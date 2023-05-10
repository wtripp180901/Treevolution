using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls UI for enemy healthbars
/// </summary>
public class HealthBar : MonoBehaviour
{

    public Slider slider;

    public TMP_Text pointsText;

    private void Start()
    {
        if(pointsText != null) pointsText.isTextObjectScaleStatic = true;
    }

    public void SetupForTest(Slider slider,TMP_Text points)
    {
        this.slider = slider;

        GameObject rect = new GameObject();
        rect.AddComponent<RectTransform>();

        slider.fillRect = rect.GetComponent<RectTransform>();
        slider.fillRect.gameObject.AddComponent<Image>();
        pointsText = points;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        if (slider.value < 5)
        {
            slider.fillRect.gameObject.GetComponent<Image>().color = Color.red;
        }
        if(slider.value <= 0)
        {
            ShowPoints();
        }
    }


    private void ShowPoints()
    {
        slider.gameObject.SetActive(false);
        pointsText.text = "+" + GetComponent<EnemyScript>().points.ToString();
        pointsText.transform.SetParent(pointsText.transform.parent.parent);
        pointsText.isTextObjectScaleStatic = true;
    }
}
