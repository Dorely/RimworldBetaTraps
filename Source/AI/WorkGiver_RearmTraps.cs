using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetaTraps
{
    public class WorkGiver_RearmTraps : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(BetaTrapDefOf.RearmTrap))
            {
                yield return des.target.Thing;
            }
            yield break;
        }
        
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }
        
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }
        
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.Map.designationManager.DesignationOn(t, BetaTrapDefOf.RearmTrap) == null)
            {
                return false;
            }
            LocalTargetInfo target = t;
            if (!pawn.CanReserve(target, 1, -1, null, forced))
            {
                return false;
            }
            List<Thing> thingList = t.Position.GetThingList(t.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                IntVec3 intVec;
                if (thingList[i] != t && thingList[i].def.category == ThingCategory.Item && (thingList[i].IsForbidden(pawn) || thingList[i].IsInValidStorage() || !HaulAIUtility.CanHaulAside(pawn, thingList[i], out intVec)))
                {
                    return false;
                }
            }
            return true;
        }
        
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            List<Thing> thingList = t.Position.GetThingList(t.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] != t && thingList[i].def.category == ThingCategory.Item)
                {
                    Job job = HaulAIUtility.HaulAsideJobFor(pawn, thingList[i]);
                    if (job != null)
                    {
                        return job;
                    }
                }
            }
            return new Job(BetaTrapDefOf.RearmTrapJob, t);
        }
    }
}
