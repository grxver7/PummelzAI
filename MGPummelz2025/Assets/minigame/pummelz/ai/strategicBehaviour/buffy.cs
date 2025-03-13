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

        // Eigener Buffy: Bleibe in der N�he von Verb�ndeten und Fernk�mpfern
        private MGPumCommand handleOwnBuffy(MGPumUnit buffy)
        {
            // Finde Verb�ndete in der N�he
            List<MGPumUnit> nearbyAllies = findNearbyAllies(buffy);
            if (nearbyAllies.Count > 0)
            {
                // Bewege Buffy in Richtung der Verb�ndeten
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

            // Bevorzuge die N�he zu Fernkampfeinheiten
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

        // Gegnerischer Buffy: T�te ihn fr�h und mit einer sicheren Fernkampfeinheit
        private MGPumCommand handleEnemyBuffy(MGPumUnit buffy)
        {
            // Finde eine sichere Fernkampfeinheit, um Buffy zu t�ten
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

        // Hilfsmethode: Finde eine sichere Fernkampfeinheit, um Buffy zu t�ten
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

        // Hilfsmethode: Finde Verb�ndete in der N�he
        private List<MGPumUnit> findNearbyAllies(MGPumUnit unit)
        {
            List<MGPumUnit> nearbyAllies = new List<MGPumUnit>();
            foreach (MGPumUnit ally in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                int distance = Mathf.Abs(unit.field.coords.x - ally.field.coords.x) + Mathf.Abs(unit.field.coords.y - ally.field.coords.y);
                if (distance <= 2) // Beispiel: Einheiten innerhalb von 2 Feldern Entfernung
                {
                    nearbyAllies.Add(ally);
                }
            }
            return nearbyAllies;
        }

        // Hilfsmethode: Finde eine Fernkampfeinheit in der N�he
        private MGPumUnit findRangedAlly(MGPumUnit unit)
        {
            foreach (MGPumUnit ally in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (ally.currentRange > 1) // Fernkampfeinheit
                {
                    return ally;
                }
            }
            return null;
        }

        // Hilfsmethode: Finde die n�chste Einheit
        private MGPumUnit findClosestUnit(MGPumUnit unit, List<MGPumUnit> units)
        {
            MGPumUnit closestUnit = null;
            int closestDistance = int.MaxValue;

            foreach (MGPumUnit u in units)
            {
                int distance = Mathf.Abs(unit.field.coords.x - u.field.coords.x) + Mathf.Abs(unit.field.coords.y - u.field.coords.y);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestUnit = u;
                }
            }

            return closestUnit;
        }
    }
}
