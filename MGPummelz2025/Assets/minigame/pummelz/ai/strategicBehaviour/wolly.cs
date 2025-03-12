using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Wolli
        private MGPumCommand handleWolli(MGPumUnit wolli)
        {
            if (wolli.ownerID == this.playerID)
            {
                // Eigener Wolli
                return handleOwnWolli(wolli);
            }
            else
            {
                // Gegnerischer Wolli
                return handleEnemyWolli(wolli);
            }
        }

        // Eigener Wolli: Nutze ihn als Tank gegen starke Einheiten
        private MGPumCommand handleOwnWolli(MGPumUnit wolli)
        {
            // Finde starke gegnerische Einheiten
            MGPumUnit strongEnemy = findStrongEnemy();
            if (strongEnemy != null)
            {
                // Positioniere Wolli in der Nähe der starken Einheit
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(wolli, strongEnemy.field.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Gegnerischer Wolli: Greife ihn mit vielen schwächeren Einheiten an
        private MGPumCommand handleEnemyWolli(MGPumUnit wolli)
        {
            // Finde mehrere schwächere Einheiten, um Wolli anzugreifen
            List<MGPumUnit> attackers = findMultipleAttackers(wolli);
            if (attackers.Count > 0)
            {
                foreach (MGPumUnit attacker in attackers)
                {
                    MGPumAttackCommand attackCommand = findAttackCommand(attacker, wolli);
                    if (attackCommand != null)
                    {
                        return attackCommand;
                    }
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde starke gegnerische Einheiten
        private MGPumUnit findStrongEnemy()
        {
            MGPumUnit strongestEnemy = null;
            int highestPower = 0;

            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                if (enemy.currentPower > highestPower)
                {
                    highestPower = enemy.currentPower;
                    strongestEnemy = enemy;
                }
            }

            return strongestEnemy;
        }
    }
}