using UnityEngine;
using UnityEngine.UI;

public class FloatingMessage : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private Text _text;

    public float InintialYVelocity = 7f;
    public float InitialXVelocityRange = 3f;
    public float LifeTime = .8f;

    public void SetText(string text)
    {
        _text.text = text;
    }

    private void Start() {
        _rb.velocity = new Vector2(Random.Range(-InitialXVelocityRange, InitialXVelocityRange), InintialYVelocity);
        Destroy(gameObject, LifeTime);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _text = GetComponentInChildren<Text>();
    }

    
}
