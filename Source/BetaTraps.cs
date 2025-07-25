﻿using HarmonyLib;
using HugsLib;
using HugsLib.Settings;
//using Multiplayer.API;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetaTraps
{
    [DefOf]
    public static class BetaTrapDefOf
    {
        public static WorkTypeDef Rearm;
        public static DesignationDef RearmTrap;
        public static JobDef RearmTrapJob;
        public static RecordDef TrapsRearmed;
    }

    public class BetaTrapDefModExtension : DefModExtension
    {
        public float TrapArmorPenetration = 0.015f;
    }


    [HarmonyPatch(typeof(HaulAIUtility), "HaulablePlaceValidator")]
    static class HaulAIUtility_HaulablePlaceValidator_Patches
    {
        static void Postfix(Thing haulable, Pawn worker, IntVec3 c, ref bool __result)
        {
            if (__result == true)
            {
                Building edifice = c.GetEdifice(worker.Map);
                if (edifice != null && edifice is BetaTraps.Building_Trap)
                {
                    __result = false;
                }
            }
        }
    }


    [StaticConstructorOnStartup]
    public class BetaTrapsSettings : ModBase
    {
        public static BetaTrapsSettings Instance { get; private set; }

        private BetaTrapsSettings()
        {
            Instance = this;
        }

        public static Func<bool> getFriendlyFireSettingValue;
        public static Func<bool> getAnimalSpringSettingValue;
        public static Func<bool> getUseBodySizeValue;
        public static Func<bool> getWildAnimalsCanTripValue;
        public static Func<bool> getSuperSlowTraps;
        public static Func<bool> getTrapsDontSlow;
        public static Func<bool> getTrapsHonorImmunity;
        public static Func<bool> getImmunesKnowOfTraps;
        public static Func<int> getRearmValue;


        public override string ModIdentifier
        {
            get { return "BetaTraps"; }
        }

        public override void StaticInitialize()
        {
            InitializeFriendlyFireSetting();
            InitializeAnimalSpringSetting();
            InitializeUseBodySizeSetting();
            InitializeWildAnimalsCanTripSetting();
            InitializeSuperSlowTraps();
            InitializeTrapsDontSlow();
            InitializeTrapsHonorImmunity();
            InitializeImmunesKnowOfTraps();
            InitializeRearmValue();
        }

        private static void InitializeFriendlyFireSetting()
        {
            const bool defaultValue = true;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = Instance.Settings.GetHandle("friendlyFire", "FriendlyFireLabel".Translate(), "FriendlyFireDesc".Translate(), defaultValue);
                    getFriendlyFireSettingValue = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getFriendlyFireSettingValue = () => defaultValue;
        }

        private static void InitializeAnimalSpringSetting()
        {
            const bool defaultValue = true;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("treatAnimalsDifferent", "TreatAnimalsDifferentLabel".Translate(), "TreatAnimalsDifferentDesc".Translate(), defaultValue);
                    getAnimalSpringSettingValue = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getAnimalSpringSettingValue = () => defaultValue;
        }

        private static void InitializeUseBodySizeSetting()
        {
            const bool defaultValue = true;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("useBodySize", "UseBodySizeLabel".Translate(), "UseBodySizeDesc".Translate(), defaultValue);
                    getUseBodySizeValue = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getUseBodySizeValue = () => defaultValue;
        }

        private static void InitializeWildAnimalsCanTripSetting()
        {
            const bool defaultValue = false;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("wildAnimalsCanTrip", "WildAnimalsCanTripLabel".Translate(), "WildAnimalsCanTripDesc".Translate(), defaultValue);
                    getWildAnimalsCanTripValue = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getWildAnimalsCanTripValue = () => defaultValue;
        }

        private static void InitializeSuperSlowTraps()
        {
            const bool defaultValue = false;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("superSlowTraps", "SuperSlowTrapsLabel".Translate(), "SuperSlowTrapsDesc".Translate(), defaultValue);
                    getSuperSlowTraps = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getSuperSlowTraps = () => defaultValue;
        }

        private static void InitializeTrapsDontSlow()
        {
            const bool defaultValue = false;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("trapsDontSlow", "TrapsDontSlowLabel".Translate(), "TrapsDontSlowDesc".Translate(), defaultValue);
                    getTrapsDontSlow = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getTrapsDontSlow = () => defaultValue;
        }

        private static void InitializeTrapsHonorImmunity()
        {
            const bool defaultValue = true;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("trapsHonorImmunity", "TrapsHonorImmunityLabel".Translate(), "TrapsHonorImmunityDesc".Translate(), defaultValue);
                    getTrapsHonorImmunity = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getTrapsHonorImmunity = () => defaultValue;
        }

        private static void InitializeImmunesKnowOfTraps()
        {
            const bool defaultValue = false;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("immunesKnowOfTraps", "ImmunesKnowOfTrapsLabel".Translate(), "ImmunesKnowOfTrapsDesc".Translate(), defaultValue);
                    getImmunesKnowOfTraps = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getTrapsHonorImmunity = () => defaultValue;
        }

        private static void InitializeRearmValue()
        {
            const int defaultValue = 1125;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    var handle = settings.GetHandle("RearmTicks", "RearmTicksLabel".Translate(), "RearmTicksDesc".Translate(), defaultValue, Validators.IntRangeValidator(1, 9999));
                    handle.SpinnerIncrement = 25;
                    getRearmValue = () => handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getRearmValue = () => defaultValue;
        }

    }


}
