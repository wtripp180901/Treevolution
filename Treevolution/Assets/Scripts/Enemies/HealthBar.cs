using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    public TMP_Text pointsText;

    private void Start()
    {
        pointsText.isTextObjectScaleStatic = true;
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
