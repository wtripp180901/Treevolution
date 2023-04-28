using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    private void Update()
    {
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
    }
}
