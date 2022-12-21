using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ScrunkMachine
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            var Harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            Harmony.PatchAll();
        }
    }
    [HarmonyPatch(typeof(SwordsMachine), "Start")]
    static class ScrunkyPatchStart
    {
        [HarmonyPostfix]
        static void Postfix(SwordsMachine __instance)
        {
            __instance.gameObject.transform.localScale = new UnityEngine.Vector3(__instance.gameObject.transform.localScale.x / 2, __instance.gameObject.transform.localScale.y / 2, __instance.gameObject.transform.localScale.z / 2);
            __instance.gameObject.GetComponent<Machine>().health = 300;
            __instance.target = null;
            __instance.active = false;
            __instance.player = null;
        }
    }
    [HarmonyPatch(typeof(SwordsMachine), "Update")]
    static class ScrunkyPatchUpdate
    {
        [HarmonyPostfix]
        static void Postfix(SwordsMachine __instance)
        {
            EnemyIdentifier[] Enemies = EnemyTracker.Instance.GetCurrentEnemies().ToArray();
            if (Enemies.Length == 0)
            {
                __instance.target = null;
                __instance.active = false;
                __instance.player = null;
            }
            else
            {
                SortedDictionary<float, EnemyIdentifier> ClosestEnemies = new SortedDictionary<float, EnemyIdentifier>();
                for (int i = 0; i < Enemies.Length; i++)
                {
                    if (Enemies[i].enemyType != EnemyType.Swordsmachine)
                    {
                        ClosestEnemies.Add(UnityEngine.Vector3.Distance(__instance.gameObject.transform.position, Enemies[i].gameObject.transform.position), Enemies[i]);
                    }
                }
                if (ClosestEnemies.Count == 0)
                {
                    __instance.target = null;
                    __instance.active = false;
                    __instance.player = null;
                }
                else
                {
                    __instance.target = ClosestEnemies.First().Value.transform;
                    __instance.active = true;
                    __instance.player = ClosestEnemies.First().Value.gameObject;
                }
            }
        }
    }
}