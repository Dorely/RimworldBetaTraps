//using Multiplayer.API;
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

        private static readonly FloatRange DamageRandomFactorRange = new FloatRange(0.8f, 1.2f);

        public const float DamageCount = 5f;


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
            float num = this.GetStatValue(StatDefOf.TrapMeleeDamage) * DamageRandomFactorRange.RandomInRange;
            float armorPenetration = num * ArmorPenetrationAmount;
            Log.Message($"[BetaTraps] Trap at {this.Position} damaging pawn {p.LabelShort} (ID: {p.ThingID}) for {num:F2} damage, armorPen: {armorPenetration:F2}, armed: {this.armedInt}");

            for (int i = 0; (float)i < DamageCount; i++)
            {
                //DamageInfo dinfo = new DamageInfo(DamageDefOf.Stab, num, armorPenetration, -1f, this);
                //DamageWorker.DamageResult damageResult = p.TakeDamage(dinfo);
                Log.Message($"[BetaTraps] Damage instance {i + 1}/{DamageCount} applied to {p.LabelShort} (ID: {p.ThingID}).");

                if (i == 0)
                {
                    BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike);
                    Find.BattleLog.Add(battleLogEntry_DamageTaken);
                    //damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
                }
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