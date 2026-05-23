using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private Image hpFillImage;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp; 
        UpdateHpBar(); // 체력바 초기 상태 반영
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // 체력 조정.

        UpdateHpBar(); // 변경된 체력 반영

        if (currentHp <= 0) Destroy(gameObject); // 탱크 제거
    }

    private void UpdateHpBar()
    {
        float ratio = (float)currentHp / maxHp; // 비율 계산

        hpFillImage.fillAmount = ratio; // 체력바 길이 갱신

        // 체력에 따른 색상 변경 처리
        if (ratio > 0.5f) hpFillImage.color = Color.green;
        else if (ratio > 0.25f) hpFillImage.color = Color.yellow;
        else hpFillImage.color = Color.red;
    }
}