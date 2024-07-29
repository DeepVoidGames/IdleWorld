using UnityEngine;

public class MessageSpawner : MonoBehaviour 
{
    [SerializeField] private Vector2 _initialPosition;

    [SerializeField] private GameObject _floatingMessagePrefab;

    public void SpawnMessage(string message)
    {
        GameObject go = Instantiate(_floatingMessagePrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.GetComponent<FloatingMessage>().SetText(message);
    }
}