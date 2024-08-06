using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using System.Collections.Generic;

public class RemoteConfig : MonoBehaviour
{
    public struct userAttributes {}
    public struct appAttributes {}

    async Task InitializeRemoteConfigAsync()
    {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
    }

    async Task Start()
    {
        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        var config = RemoteConfigService.Instance.appConfig.config;

        if (config == null)
            return;

        // Biome
        BiomeDataWrapper biomeDataWrapper = JsonUtility.FromJson<BiomeDataWrapper>(config["Biomes"].ToString());
        List<Biomes> biomes = biomeDataWrapper.biomes;
        BiomeSystem.Instance.SetBiomesCollection(biomes);

        // Crafting
        CraftingRecipesWrapper craftingRecipesWrapper = JsonUtility.FromJson<CraftingRecipesWrapper>(config["CraftingRecipes"].ToString());
        List<CraftingRecipe> craftingRecipes = craftingRecipesWrapper.craftingRecipes;
        CraftingSystem.Instance.CraftingRecipes = craftingRecipes;

        // Cave 
        CaveDataWrapper caveDataWrapper = JsonUtility.FromJson<CaveDataWrapper>(config["Caves"].ToString());
        List<Cave> caves = caveDataWrapper.caves;
        CaveSystem.Instance.SetCavesCollection(caves);

        // Items
        ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(config["Items"].ToString());
        List<Items> items = itemDataWrapper.items;
        ItemSystem.Instance.SetItemsCollection(items);
        Debug.Log("Remote Config settings have been applied");
    }
}