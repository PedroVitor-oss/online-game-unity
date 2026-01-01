using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string id;
    private Vector3 lastPosition;

    void Update()
    {
        if (transform.position != lastPosition)
        {
            lastPosition = transform.position;
            //enviar nova posição para o servidor
            var positionData = new
            {
                position = new float[] { transform.position.x, transform.position.y, transform.position.z },
                id = id
            };
            WebSocketClient.Instance.SendJsonDataAsync("newPosition", positionData);
        }
    }

    public void setPostion(Vector3 newPosition)
    {
        transform.position = newPosition;
        lastPosition = newPosition;
    }


}