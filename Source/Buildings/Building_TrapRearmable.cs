﻿//using Multiplayer.API;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BetaTraps
{
    public class Building_TrapRearmable : Building_Trap
    {
        //[SyncField]
        private bool autoRearm = true;

        //[SyncField]
        private bool armedInt = true;

        private Graphic graphicUnarmedInt;

        private static readonly FloatRange TrapDamageFactor = new FloatRange(0.7f, 1.3f);

        private static readonly IntRange DamageCount = new IntRange(1, 2);


        public override bool Armed
        {
            get
            {
                return this.armedInt;
            }
        }
        
        public override Graphic Graphic
        {
            get
            {
                if (this.armedInt)
                {
                    return base.Graphic;
                }
                if (this.graphicUnarmedInt == null)
                {
                    this.graphicUnarmedInt = this.def.building.trapUnarmedGraphicData.GraphicColoredFor(this);
                }
                return this.graphicUnarmedInt;
            }
        } 
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.armedInt, "armed", false, false);
            Scribe_Values.Look<bool>(ref this.autoRearm, "autoRearm", false, false);
        }

        //[SyncMethod]
        protected override void SpringSub(Pawn p)
        {
            SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            this.armedInt = false;
            if (p != null)
            {
                this.DamagePawn(p);
            }
            if (this.autoRearm)
            {
                if (canBeDesignatedRearm())
                {
                    base.Map.designationManager.AddDesignation(new Designation(this, BetaTrapDefOf.RearmTrap));
                }
            }
        }

        private bool canBeDesignatedRearm()
        {
            return !armedInt && Map.designationManager.AllDesignationsOn(this).Where(i => i.def == BetaTrapDefOf.RearmTrap).FirstOrDefault() == null;
        }
        
        //[SyncMethod]
        public void Rearm()
        {
            this.armedInt = true;
        }
        
        private void DamagePawn(Pawn p)
        {
            BodyPartHeight height = (Rand.Value >= 0.666f) ? BodyPartHeight.Middle : BodyPartHeight.Top;
            float num = this.GetStatValue(StatDefOf.TrapMeleeDamage, true) * Building_TrapRearmable.TrapDamageFactor.RandomInRange;
            float num2 = num / Building_TrapRearmable.DamageCount.RandomInRange;

            

            float armorPenetration = num2 * ArmorPenetrationAmount;
            int num3 = 0;
            while ((float)num3 < Building_TrapRearmable.DamageCount.RandomInRange)
            {
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Stab, num2, armorPenetration, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                DamageWorker.DamageResult damageResult = p.TakeDamage(dinfo);
                if (num3 == 0)
                {
                    BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike, null);
                    Find.BattleLog.Add(battleLogEntry_DamageTaken);
                    damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
                }
                num3++;
            }
        }
        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            yield return new Command_Toggle
            {
                defaultLabel = "CommandAutoRearm".Translate(),
                defaultDesc = "CommandAutoRearmDesc".Translate(),
                hotKey = KeyBindingDefOf.Misc3,
                icon = TexCommand.RearmTrap,
                isActive = (() => this.autoRearm),
                toggleAction = ToggleAutoRearm
            };
            if (canBeDesignatedRearm())
            {
                yield return new Command_Action
                {
                    defaultLabel = "CommandRearm".Translate(),
                    defaultDesc = "CommandRearmDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc4,
                    icon = TexCommand.RearmTrap,
                    action = AddRearmDesignation
                };
            }
            yield break;
        }

        //[SyncMethod]
        public void ToggleAutoRearm()
        {
            this.autoRearm = !this.autoRearm;
        }

        //[SyncMethod]
        public void AddRearmDesignation()
        {
            base.Map.designationManager.AddDesignation(new Designation(this, BetaTrapDefOf.RearmTrap));
        }
        
    }

}