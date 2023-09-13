using UnityEngine;

// Used to test out movement
public class MoveHiyori : MonoBehaviour
{
    [SerializeField] private Transform hiyori;
    [SerializeField] private float moveSpeed = 1f;

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0f || v != 0f)
            hiyori.Translate(new(h * moveSpeed * Time.deltaTime, v * moveSpeed * Time.deltaTime, 0f));
    }
}
