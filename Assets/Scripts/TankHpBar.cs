using UnityEngine;
using UnityEngine.UI;

public class TankHpBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private int maxHp = 100;

    public void SetHp(int hp)
    {
        Debug.Log($"SetHp called: {hp}");

        float ratio = Mathf.Clamp01((float)hp / maxHp); // 0~1 사이로 조정

        fillImage.fillAmount = ratio;

        if (ratio >= 0.7f)
        {
            fillImage.color = Color.green;
        }
        else if (ratio >= 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}