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
        try
        {
            await UnityServices.InitializeAsync();
            GiveConsent(); // Get user consent according to various legislations
        }
        catch (ConsentCheckException e)
        {
            Debug.Log(e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            CustomEvent();
        }
        catch (ConsentCheckException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void CustomEvent()
    {
        // Define Custom Parameters
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "goldAmount", GoldSystem.Instance.Gold },
            { "miningLevel", MiningSystem.Instance.MiningLevel },
            { "slayerLevel", LevelSystem.Instance.Level }
        };

        #pragma warning disable CS0618 // Type or member is obsolete
        AnalyticsService.Instance.CustomData("gameData", parameters);
        #pragma warning restore CS0618 // Type or member is obsolete

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