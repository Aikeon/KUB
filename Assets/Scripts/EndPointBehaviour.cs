using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPointBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.TryGetComponent<PlayerControl>(out var pc))
        {
            //You win !
            Debug.Log("You win !");
            GameManager.Instance.goToNextLevel(SceneManager.GetActiveScene().name == "CustomLevel");
        }
    }
}
