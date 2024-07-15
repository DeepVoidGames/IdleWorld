
using UnityEngine;

public class MiningExpShow : MonoBehaviour
{
    private MessageSpawner _messageSpawner;

    private void Start()
    {
        _messageSpawner = GetComponent<MessageSpawner>();
        MiningSystem.Instance.OnExperienceAdded += HandleExperienceAdded;
    }

    private void HandleExperienceAdded(double experienceAdded)
    {
        ShowMiningExp(experienceAdded);
    }

    private void OnApplicationQuit()
    {
        MiningSystem.Instance.OnExperienceAdded -= HandleExperienceAdded;
    }

    private void ShowMiningExp(double experienceAdded)
    {
        _messageSpawner.SpawnMessage("+" + UISystem.Instance.NumberFormat(experienceAdded) + " Mining Exp");
    }
}
