﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloody.Core;
using Bloody.Core.API;
using HarmonyLib;
using System.Linq;
using Unity.Entities;

namespace BloodyCoreTest;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;

    public static Bloody.Core.Helper.Logger Logger;
    public static SystemsCore SystemsCore;

    public override void Load()
    {
        Logger = new(Log);
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        EventsHandlerSystem.OnInitialize += GameDataOnInitialize;

    }

    private static void GameDataOnInitialize(World world)
    {
        Logger.LogInfo("GameDataOnInitialize");
        SystemsCore = Core.SystemsCore;

        Logger.LogWarning("All Users:");
        foreach (var userModel in Core.Users.All)
        {
            Logger.LogMessage($"{userModel.CharacterName} Connected: {userModel.IsConnected}");
            foreach (var inventoryItem in userModel.Inventory.Items)
            {
                Logger.LogMessage($"\tSlot: {inventoryItem.Slot} Item: {inventoryItem.Item.Name} ({inventoryItem.Stacks})");
            }
        }

        var weapons = Core.Items.Weapons.Take(10);
        Logger.LogWarning("Some Weapons:");
        foreach (var itemModel in weapons)
        {
            Logger.LogMessage($"{itemModel.ItemType} {itemModel.ItemCategory}");
        }

    }

    public override bool Unload()
    {
        EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
        _harmony?.UnpatchSelf();
        return true;
    }

}