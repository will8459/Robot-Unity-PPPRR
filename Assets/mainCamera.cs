using UnityEngine;

public class mainCamera : MonoBehaviour
{
    public Transform cible ;
    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(cible);
    }
}
