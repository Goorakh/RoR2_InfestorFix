using BepInEx;
using InfestorFix.Utilities.Extensions;
using RoR2;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace InfestorFix
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class InfestorFixPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "InfestorFix";
        public const string PluginVersion = "1.0.0";

        static InfestorFixPlugin _instance;
        internal static InfestorFixPlugin Instance => _instance;

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            SingletonHelper.Assign(ref _instance, this);

            Log.Init(Logger);

            On.RoR2.LayerIndex.GetAppropriateLayerForTeam += LayerIndex_GetAppropriateLayerForTeam;
            On.RoR2.CharacterMotor.ApplyForceImpulse += CharacterMotor_ApplyForceImpulse;

            Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").CallOnSuccess(voidInfestorBody =>
            {
                TrailRenderer[] trailRenderers = voidInfestorBody.GetComponentsInChildren<TrailRenderer>(true);
                if (trailRenderers.Length > 0)
                {
                    DelayTrailRendererStart delayTrailRendererStart = voidInfestorBody.AddComponent<DelayTrailRendererStart>();
                    delayTrailRendererStart.Delay = 1f / 30f;
                    delayTrailRendererStart.TrailRenderers = trailRenderers;
                }
            });

            stopwatch.Stop();
            Log.Message_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalMilliseconds:F0}ms");
        }

        void OnDestroy()
        {
            SingletonHelper.Unassign(ref _instance, this);

            On.RoR2.LayerIndex.GetAppropriateLayerForTeam -= LayerIndex_GetAppropriateLayerForTeam;
            On.RoR2.CharacterMotor.ApplyForceImpulse -= CharacterMotor_ApplyForceImpulse;
        }

        static int LayerIndex_GetAppropriateLayerForTeam(On.RoR2.LayerIndex.orig_GetAppropriateLayerForTeam orig, TeamIndex teamIndex)
        {
            int layer = orig(teamIndex);

            if (teamIndex == TeamIndex.Void)
            {
                layer = LayerIndex.enemyBody.intVal;
            }

            return layer;
        }

        static void CharacterMotor_ApplyForceImpulse(On.RoR2.CharacterMotor.orig_ApplyForceImpulse orig, CharacterMotor self, ref PhysForceInfo forceInfo)
        {
            if (!self.hasEffectiveAuthority && Util.HasEffectiveAuthority(self.netIdentity))
            {
                Log.Debug($"Fixed authority state for {self}");
                self.UpdateAuthority();
            }

            orig(self, ref forceInfo);
        }
    }
}
