using UnityEngine;

public class MessageSpawner : MonoBehaviour 
{
    [SerializeField] private Vector2 _initialPosition;

    [SerializeField] private GameObject _floatingMessagePrefab;

    public void SpawnMessage(string message)
    {
        GameObject go = Instantiate(_floatingMessagePrefab, _initialPosition, Quaternion.identity);
        go.GetComponent<FloatingMessage>().SetText(message);
    }
}