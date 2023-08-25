using UnityEngine;

[ExecuteInEditMode]
public class EditorOnly : MonoBehaviour
{
    private void Start()
    {
        if (Application.isPlaying)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}