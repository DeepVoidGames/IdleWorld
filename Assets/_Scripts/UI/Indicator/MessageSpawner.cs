using UnityEngine;

public class MessageSpawner : MonoBehaviour 
{
    [SerializeField] private Vector2 _initialPosition;

    [SerializeField] private GameObject _floatingMessagePrefab;

    public void SpawnMessage(string message, Sprite sprite = null, Color color = default(Color))
    {
        GameObject go = Instantiate(_floatingMessagePrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.transform.localPosition = _initialPosition;
        go.GetComponent<FloatingMessage>().SetText(message);

        if (color != default(Color))
            go.GetComponent<FloatingMessage>().SetColor(color);
        else
            go.GetComponent<FloatingMessage>().SetColor(Color.white); // Set color to white

        if (sprite != null)
        {
            go.GetComponent<FloatingMessage>().SetSprite(sprite);
        }
    }
}