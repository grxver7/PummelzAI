using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Haley
        private MGPumCommand handleHaley(MGPumUnit haley)
        {
            if (haley.ownerID == this.playerID)
            {
                // Eigene Haley
                return handleOwnHaley(haley);
            }
            else
            {
                // Gegnerische Haley
                return handleEnemyHaley(haley);
            }
        }

        // Eigene Haley: Bleibe in der Nähe von Verbündeten und Tanks
        private MGPumCommand handleOwnHaley(MGPumUnit haley)
        {
            // Finde Verbündete in der Nähe
            List<MGPumUnit> nearbyAllies = findNearbyAllies(haley);
            if (nearbyAllies.Count > 0)
            {
                // Bewege Haley in Richtung der Verbündeten
                MGPumUnit closestAlly = findClosestUnit(haley, nearbyAllies);
                if (closestAlly != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(haley, closestAlly.field.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Bevorzuge die Nähe zu Tank-Einheiten
            MGPumUnit tankAlly = findTankAlly(haley);
            if (tankAlly != null)
            {
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(haley, tankAlly.field.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Gegnerische Haley: Töte sie früh, wenn keine verwundbaren Gegner in der Nähe sind
        private MGPumCommand handleEnemyHaley(MGPumUnit haley)
        {
            // Überprüfe, ob verwundbare Gegner in der Nähe sind
            if (!areVulnerableEnemiesNearby(haley))
            {
                // Finde den besten Angreifer für Haley
                MGPumUnit attacker = findBestAttacker(haley);
                if (attacker != null)
                {
                    MGPumAttackCommand attackCommand = findAttackCommand(attacker, haley);
                    if (attackCommand != null)
                    {
                        return attackCommand;
                    }
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde eine Tank-Einheit in der Nähe
        private MGPumUnit findTankAlly(MGPumUnit unit)
        {
            foreach (MGPumUnit ally in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (ally.currentHealth >= 5) // Beispiel: Einheiten mit mindestens 5 Lebenspunkten
                {
                    int distance = Mathf.Abs(ally.field.coords.x - unit.field.coords.x) + Mathf.Abs(ally.field.coords.y - unit.field.coords.y);
                    if (distance <= 5) // Beispiel: Tank-Einheit innerhalb von 5 Feldern
                    {
                        return ally;
                    }
                }
            }
            return null;
        }

        // Hilfsmethode: Überprüfe, ob verwundbare Gegner in der Nähe sind
        private bool areVulnerableEnemiesNearby(MGPumUnit haley)
        {
            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                int distance = Mathf.Abs(enemy.field.coords.x - haley.field.coords.x) + Mathf.Abs(enemy.field.coords.y - haley.field.coords.y);
                if (distance <= 3 && enemy.currentHealth <= 3) // Beispiel: Verwundbare Gegner innerhalb von 3 Feldern
                {
                    return true;
                }
            }
            return false;
        }
    }
}