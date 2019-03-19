using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using Verse;

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


    [StaticConstructorOnStartup]
    public class BetaTrapsSettings
    {
        public static Func<bool> getFriendlyFireSettingValue;
        public static Func<bool> getAnimalSpringSettingValue;
        public static Func<bool> getUseBodySizeValue;

        static BetaTrapsSettings()
        {
            InitializeFriendlyFireSetting();
            InitializeAnimalSpringSetting();
            InitializeUseBodySizeSetting();
        }

        private static void InitializeFriendlyFireSetting()
        {
            const bool defaultValue = true;
            try
            {
                ((Action)(() => {
                    var settings = HugsLibController.Instance.Settings.GetModSettings("BetaTraps");
                    settings.EntryName = "BetaTraps";
                    object handle = settings.GetHandle("friendlyFire", "Friendly Fire", "If traps should be able to hit friendly pawns", defaultValue);
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
                    object handle = settings.GetHandle("treatAnimalsDifferent", "Treat Animals Different", "If animal spring chance should be calculated differently than for pawns", defaultValue);
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
                    object handle = settings.GetHandle("useBodySize", "Use Body Size", "Use body size in calculation or not", defaultValue);
                    getUseBodySizeValue = () => (SettingHandle<bool>)handle;
                }))();
                return;
            }
            catch (TypeLoadException)
            {
            }
            getUseBodySizeValue = () => defaultValue;
        }
    }


}
