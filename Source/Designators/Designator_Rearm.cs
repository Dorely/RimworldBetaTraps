using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace beta_traps
{
    public class Designator_Rearm : Designator
    {
        public Designator_Rearm()
        {
            this.defaultLabel = "DesignatorRearmTrap".Translate();
            this.defaultDesc = "DesignatorRearmTrapDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/RearmTrap", true);
            this.soundDragSustain = SoundDefOf.Designate_DragStandard;
            this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            this.useMouseIcon = true;
            this.soundSucceeded = SoundDefOf.Designate_Claim;
            //this.hotKey = KeyBindingDefOf.Misc7;
        }
        
        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }
        
        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map))
            {
                return false;
            }
            if (!this.RearmablesInCell(c).Any<Thing>())
            {
                return false;
            }
            return true;
        }
        
        public override void DesignateSingleCell(IntVec3 c)
        {
            foreach (Thing t in this.RearmablesInCell(c))
            {
                this.DesignateThing(t);
            }
        }
        
        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            Building_TrapRearmable building_TrapRearmable = t as Building_TrapRearmable;
            return building_TrapRearmable != null && !building_TrapRearmable.Armed && base.Map.designationManager.DesignationOn(building_TrapRearmable, BetaTrapDefOf.RearmTrap) == null;
        }
        
        public override void DesignateThing(Thing t)
        {
            base.Map.designationManager.AddDesignation(new Designation(t, BetaTrapDefOf.RearmTrap));
        }
        
        private IEnumerable<Thing> RearmablesInCell(IntVec3 c)
        {
            if (c.Fogged(base.Map))
            {
                yield break;
            }
            List<Thing> thingList = c.GetThingList(base.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (this.CanDesignateThing(thingList[i]).Accepted)
                {
                    yield return thingList[i];
                }
            }
            yield break;
        }
    }
}
