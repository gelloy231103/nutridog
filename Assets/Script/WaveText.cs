using UnityEngine;
using TMPro;

public class WaveText : MonoBehaviour
{
    public float waveHeight = 10f;   // how high letters move
    public float waveDuration = 0.2f; // time per letter bounce
    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;

    private int currentIndex = 0;
    private float timer = 0f;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        timer += Time.deltaTime;
        if (timer >= waveDuration)
        {
            // Move to next letter
            timer = 0f;
            currentIndex++;

            if (currentIndex >= textMesh.textInfo.characterCount)
            {
                currentIndex = 0; // loop back to first letter
            }
        }

        // Reset all letters to straight alignment
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            var charInfo = textMesh.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;

            Vector3 offset = Vector3.zero;

            // Only animate the current letter
            if (i == currentIndex)
            {
                float y = Mathf.Sin((timer / waveDuration) * Mathf.PI) * waveHeight;
                offset = new Vector3(0, y, 0);
            }

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
}
