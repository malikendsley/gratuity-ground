using UnityEngine;

public class ReticleWorldPosition : MonoBehaviour
{
    public static ReticleWorldPosition Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}