using UnityEngine.SceneManagement;
using UnityEngine;

public class UI_manager : MonoBehaviour
{
    public GameObject pauseCanvas;

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false); // �ݵ�� ��Ȱ��ȭ
        Debug.Log("���� �簳");
    }

    public void ExitGame()
    {
        Debug.Log("���� ����");
        Application.Quit();  // ���α׷� ���� �ڵ� �߰�

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // �����Ϳ��� ���� ����
#endif
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("���� �����");
    }
}
