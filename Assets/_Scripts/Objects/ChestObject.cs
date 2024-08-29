using UnityEngine;

public class ChestObject : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;

    private MessageSpawner _messageSpawner;

    public void OpenChest()
    {
        if(IdleSystem.Instance.IdleChestTime <= IdleSystem.Instance.MinIdleTime)
            return;
        animator.SetTrigger("Open0");
        double r = DifficultySystem.Instance.GetIdleReward();
        GoldSystem.Instance.AddGold(r);
        _messageSpawner.SpawnMessage("+" + UISystem.Instance.NumberFormat(r));
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.2f);
        IdleSystem.Instance.RestartIdleChestTime();
    }

    private void OnMouseDown() 
    {
        OpenChest();
    }

    private void Start() 
    {
        _messageSpawner = GetComponent<MessageSpawner>();
        animator = GetComponent<Animator>();
    }

    
}
