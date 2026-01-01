using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
public class PlayersManage : MonoBehaviour
{
    public static PlayersManage Instance { get; private set; }
    public GameObject otherPlayerPrefab;
    public PlayerData mainPlayer;
    public string idPlayer;
    public List<PlayerData> playersList = new List<PlayerData>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void SetStartData(string mes)
    {
        JObject data = JObject.Parse(mes);
        idPlayer = (string)data["id"];
        JArray players = (JArray)data["players"];
        foreach (var p in players)
        {
            string pid = (string)p["id"];
            JArray pos = (JArray)p["position"];
            Vector3 position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
            if (pid == idPlayer)
            {
                mainPlayer.id = pid;
                mainPlayer.setPostion(position);
            }
            else
            {
                GameObject op = Instantiate(otherPlayerPrefab, position, Quaternion.identity);
                PlayerData opd = op.GetComponent<PlayerData>();
                opd.id = pid;
                playersList.Add(opd);
            }
        }

    }

    public void UpdatePlayerPosition(string mes)
    {
        JObject data = JObject.Parse(mes);
        string pid = (string)data["id"];
        JArray pos = (JArray)data["position"];
        Vector3 position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);

        foreach (var p in playersList)
        {
            if (p.id == pid)
            {
                p.setPostion(position);
                break;
            }
        }
    }
    // Update is called once per frame
    public void PlayerDisconnect(string mes)
    {
        JObject data = JObject.Parse(mes);
        string pid = (string)data["id"];
        for (int i = 0; i < playersList.Count; i++)
        {
            if (playersList[i].id == pid)
            {
                Destroy(playersList[i].gameObject);
                playersList.RemoveAt(i);
                break;
            }
        }
    }

    public void AddNewPlayer(string mes)
    {
        Debug.Log("Nova conecção de jogador");
        Debug.Log(mes);
        JObject data = JObject.Parse(mes);
        JObject player = (JObject)data["player"];
        string pid = (string)player["id"];
        JArray pos = (JArray)player["position"];
        Vector3 position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
        if (pid != idPlayer)
        {
            GameObject op = Instantiate(otherPlayerPrefab, position, Quaternion.identity);
            PlayerData opd = op.GetComponent<PlayerData>();
            opd.id = pid;
            playersList.Add(opd);
        }
    }
    void Update()
    {

    }
}
