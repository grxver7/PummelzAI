using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Buffy
        private MGPumCommand handleBuffy(MGPumUnit buffy)
        {
            if (buffy.ownerID == this.playerID)
            {
                // Eigener Buffy
                return handleOwnBuffy(buffy);
            }
            else
            {
                // Gegnerischer Buffy
                return handleEnemyBuffy(buffy);
            }
        }

        // Eigener Buffy: Bleibe in der Nähe von Verbündeten und Fernkämpfern
        private MGPumCommand handleOwnBuffy(MGPumUnit buffy)
        {
            // Finde Verbündete in der Nähe
            List<MGPumUnit> nearbyAllies = findNearbyAllies(buffy);
            if (nearbyAllies.Count > 0)
            {
                // Bewege Buffy in Richtung der Verbündeten
                MGPumUnit closestAlly = findClosestUnit(buffy, nearbyAllies);
                if (closestAlly != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(buffy, closestAlly.field.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Bevorzuge die Nähe zu Fernkampfeinheiten
            MGPumUnit rangedAlly = findRangedAlly(buffy);
            if (rangedAlly != null)
            {
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(buffy, rangedAlly.field.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Gegnerischer Buffy: Töte ihn früh und mit einer sicheren Fernkampfeinheit
        private MGPumCommand handleEnemyBuffy(MGPumUnit buffy)
        {
            // Finde eine sichere Fernkampfeinheit, um Buffy zu töten
            MGPumUnit safeRangedAttacker = findSafeRangedAttacker(buffy);
            if (safeRangedAttacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(safeRangedAttacker, buffy);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde eine sichere Fernkampfeinheit, um Buffy zu töten
        private MGPumUnit findSafeRangedAttacker(MGPumUnit buffy)
        {
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.currentRange > 1) // Sichere Fernkampfeinheit
                {
                    int distance = Mathf.Abs(unit.field.coords.x - buffy.field.coords.x) + Mathf.Abs(unit.field.coords.y - buffy.field.coords.y);
                    if (distance <= unit.currentRange) // Einheit kann Buffy angreifen
                    {
                        return unit;
                    }
                }
            }
            return null;
        }
    }
}