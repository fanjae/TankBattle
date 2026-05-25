using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject multiPanel;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_Text messageText;
    private void Start()
    {
        string message = PlayerPrefs.GetString("MenuMessage", "");

        if (string.IsNullOrEmpty(message) == false)
        {
            messageText.text = message;
            PlayerPrefs.DeleteKey("MenuMessage");

            StartCoroutine(ClearMessage(5f)); // 5초뒤 보낸 메시지 삭제
        }
        else
        {
            messageText.text = "";
        }
    }
    private IEnumerator ClearMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = "";
    }

    // 싱글 플레이
    public void OnClickSingle()
    {
        SceneManager.LoadScene("SingleGame");
    }

    // 멀티 플레이 클릭시 IP 입력 패널
    public void OnClickMulti()
    {
        mainPanel.SetActive(false);
        multiPanel.SetActive(true);
    }

    // 멀티 플레이 연결
    public void OnClickConnect()
    {
        string ip = ipInput.text.Trim();

        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogWarning("IP가 비어있음");
            return;
        }

        // 입력한 IP 주소를 저장한다. 이후, 멀티 게임씬에서 꺼내서 사용
        PlayerPrefs.SetString("ServerIP", ip);
        SceneManager.LoadScene("MultiGame");
    }

    // 돌아가기 버튼
    public void OnClickBack()
    {
        multiPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}