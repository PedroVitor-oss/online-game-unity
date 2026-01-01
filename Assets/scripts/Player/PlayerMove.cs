using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
[Header("Configurações de Movimento")]
    public float speed = 5f;
    
    private void Update()
    {
        // Só processa entrada para o jogador local
        
        // Captura entrada
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        // Cria movimento
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement = movement.normalized * speed * Time.deltaTime;
        
        // Aplica movimento
        transform.Translate(movement, Space.World);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("Enviar posição");
            
            // WebSocketClient.Instance.TesteSend();
        }
    }
    
   
}
