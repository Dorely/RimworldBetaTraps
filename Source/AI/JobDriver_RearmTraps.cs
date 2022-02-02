using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetaTraps
{
    public class JobDriver_RearmTrap : JobDriver
    {
        private int RearmTicks { get => BetaTrapsSettings.getRearmValue(); }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnThingMissingDesignation(TargetIndex.A, BetaTrapDefOf.RearmTrap);
            Toil gotoThing = new Toil();
            gotoThing.initAction = delegate ()
            {
                this.pawn.pather.StartPath(base.TargetThingA, PathEndMode.Touch);
            };
            gotoThing.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            gotoThing.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return gotoThing;
            yield return Toils_General.Wait(RearmTicks).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = this.job.targetA.Thing;
                    Designation designation = base.Map.designationManager.DesignationOn(thing, BetaTrapDefOf.RearmTrap);
                    if (designation != null)
                    {
                        designation.Delete();
                    }
                    Building_TrapRearmable building_TrapRearmable = thing as Building_TrapRearmable;
                    building_TrapRearmable.Rearm();
                    this.pawn.records.Increment(BetaTrapDefOf.TrapsRearmed);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }
        
    }
}
