using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections;

public class SimpleServerChecker : MonoBehaviour
{
    public bool useLocal = false;
    public string urlGame = "wss://server-game-unity.onrender.com/ws";
    public string urlLocal = "ws://localhost:8080/ws";
    private string serverURL;
    [SerializeField] private UnityEvent onServerOnline;
    [SerializeField] private UnityEvent onServerOffline;
    
    void Start()
    {
        StartCoroutine(CheckServer());
    }
    
    IEnumerator CheckServer()
    {
        Debug.Log("start Checkserver");
        if(useLocal)
        {
            serverURL = urlLocal;
        }
        else
        {
            serverURL = urlGame;
        }
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(serverURL))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success || 
                    (request.responseCode >= 200 && request.responseCode < 300))
                {
                    onServerOnline?.Invoke();
                    Debug.Log("Server is online");
                }
                else
                {
                    Debug.Log("Server no is online");
                    onServerOffline?.Invoke();
                }
            }
            
            yield return new WaitForSeconds(10f); // Verifica a cada 10 segundos
        }
    }

    public void IrParaGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game");
    }
}