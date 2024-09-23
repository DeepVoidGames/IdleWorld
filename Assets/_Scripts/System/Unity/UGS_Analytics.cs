using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;

public class UGS_Analytics : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();
        GiveConsent(); // Get user consent according to various legislations
    }

    void OnApplicationQuit()
    {
        CustomEvent();
    }

    private void CustomEvent()
    {
        GameDataEvent gameDataEvent = new GameDataEvent
        {
            GoldAmount = (float)GoldSystem.Instance.Gold,
            MiningLevel = (int)MiningSystem.Instance.MiningLevel,
            SlayerLevel = LevelSystem.Instance.Level
        };
        AnalyticsService.Instance.RecordEvent(gameDataEvent);
        // You can call Events.Flush() to send the event immediately
        // AnalyticsService.Instance.Flush();
    }

    public void GiveConsent()
    {
	    // Call if consent has been given by the user
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log($"Consent has been provided. The SDK is now collecting data!");
    }
}

public class GameDataEvent : Unity.Services.Analytics.Event
{
    public GameDataEvent() : base("gameDataEvent")
    {
    }

    public float GoldAmount { set { SetParameter("goldAmount", value); } }
    public int MiningLevel { set { SetParameter("miningLevel", value); } }
    public int SlayerLevel { set { SetParameter("slayerLevel", value); } }

}