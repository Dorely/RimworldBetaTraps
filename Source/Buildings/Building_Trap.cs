﻿using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI.Group;
using Verse.Sound;
using System.Diagnostics;
//using Multiplayer.API;

namespace BetaTraps
{
    public abstract class Building_Trap : Building
    {
        protected abstract void SpringSub(Pawn p);
        
        private List<Pawn> touchingPawns = new List<Pawn>();
        
        private const float KnowerSpringChance = 0.004f;
        
        private const ushort KnowerPathFindCost = 800;
        
        private const ushort KnowerPathWalkCost = 30;
        
        private const float AnimalSpringChanceFactor = 0.1f;
        protected float ArmorPenetrationAmount = 0.015f;

        public virtual bool Armed
        {
            get
            {
                return true;
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<Pawn>(ref this.touchingPawns, "testees", LookMode.Reference, new object[0]);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad); 
            
            if (this.def.HasModExtension<BetaTrapDefModExtension>())
            {
                ArmorPenetrationAmount = this.def.GetModExtension<BetaTrapDefModExtension>().TrapArmorPenetration;
            }
        }

        public override void Tick()
        {

            if (this.Spawned)
            {
                if (this.Armed)
                {
                    List<Thing> thingList = base.Position.GetThingList(base.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn pawn = thingList[i] as Pawn;
                        if (pawn != null && !this.touchingPawns.Contains(pawn))
                        {
                            this.touchingPawns.Add(pawn);
                            this.CheckSpring(pawn);
                        }
                    }
                }
                for (int j = 0; j < this.touchingPawns.Count; j++)
                {
                    Pawn pawn2 = this.touchingPawns[j];
                    if (!pawn2.Spawned || pawn2.Position != base.Position)
                    {
                        this.touchingPawns.Remove(pawn2);
                    }
                }
            }
            base.Tick();

        }
        
        protected virtual float SpringChance(Pawn p)
        {
            float num;
            if (this.KnowsOfTrap(p))
            {
                num = 0.004f;
                if (!BetaTrapsSettings.getFriendlyFireSettingValue())
                {
                    num = 0f;
                }
            }
            else
            {
                num = this.GetStatValue(StatDefOf.TrapSpringChance, true);
            }

            if (!BetaTrapsSettings.getUseBodySizeValue())
            {
                num *= GenMath.LerpDouble(0.4f, 0.8f, 0f, 1f, 1f);
            }
            else
            {
                num *= GenMath.LerpDouble(0.4f, 0.8f, 0f, 1f, p.BodySize);
            }

            if (BetaTrapsSettings.getAnimalSpringSettingValue())
            {
                if (p.RaceProps.Animal)
                {
                    num *= 0.1f;
                }
            }
            return Mathf.Clamp01(num);
        }
        
        private void CheckSpring(Pawn p)
        {
            if (Rand.Value < this.SpringChance(p))
            {
                this.Spring(p);
                if (p.Faction == Faction.OfPlayer || p.HostFaction == Faction.OfPlayer)
                {
                    Find.LetterStack.ReceiveLetter("LetterFriendlyTrapSprungLabel".Translate(p.LabelShort, p), "LetterFriendlyTrapSprung".Translate(p.LabelShort, p), LetterDefOf.NegativeEvent,
                        new TargetInfo(base.Position, base.Map, false), null, null);
                }
            }
        }
        
        public bool KnowsOfTrap(Pawn p)
        {
            if (p.Faction != null && !p.Faction.HostileTo(base.Faction))
            {
                return true;
            }
            if (p.Faction == null && p.RaceProps.Animal && !p.InAggroMentalState)
            {
                if (BetaTrapsSettings.getWildAnimalsCanTripValue())
                {
                    return false;
                }
                return true;
            }
            if (p.guest != null && p.guest.Released)
            {
                return true;
            }
            Lord lord = p.GetLord();
            return p.RaceProps.Humanlike && lord != null && lord.LordJob is LordJob_FormAndSendCaravan;
        }
        
        public override ushort PathFindCostFor(Pawn p)
        {
            if (!this.Armed)
            {
                return 0;
            }
            if (this.KnowsOfTrap(p))
            {
                return 800;
            }
            return 0;
        }
        
        public override ushort PathWalkCostFor(Pawn p)
        {
            if (!this.Armed)
            {
                return 0;
            }
            if (this.KnowsOfTrap(p))
            {
                if (BetaTrapsSettings.getTrapsDontSlow())
                {
                    return 0;
                }

                if (BetaTrapsSettings.getSuperSlowTraps())
                {
                    return 300;
                }
                return 30;
            }
            return 0;
        }
        
        public override bool IsDangerousFor(Pawn p)
        {
            return this.Armed && this.KnowsOfTrap(p);
        }
        
        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            if (this.Armed)
            {
                text += "TrapArmed".Translate();
            }
            else
            {
                text += "TrapNotArmed".Translate();
            }
            return text;
        }
        
        //[SyncMethod]
        public void Spring(Pawn p)
        {
            SoundDef.Named("DeadfallSpring").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            this.SpringSub(p);
        }

        [DebuggerHidden]
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            foreach (StatDrawEntry s in base.SpecialDisplayStats())
            {
                yield return s;
            }

            //yield return new StatDrawEntry(StatCategoryDefOf.Building, "TrapArmorPenetration".Translate(), ArmorPenetrationAmount.ToStringPercent(), 0, string.Empty);
            yield return new StatDrawEntry(StatCategoryDefOf.Building, "TrapArmorPenetration".Translate(), ArmorPenetrationAmount.ToStringPercent(), ArmorPenetrationAmount.ToStringPercent(), 0);

        }
    }

}