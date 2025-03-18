using UnityEngine.SceneManagement;
using UnityEngine;

public class UI_manager : MonoBehaviour
{
    public GameObject pauseCanvas;

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false); // 반드시 비활성화
        Debug.Log("게임 재개");
    }

    public void ExitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();  // 프로그램 종료 코드 추가

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터에서 게임 종료
#endif
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("게임 재시작");
    }
}
