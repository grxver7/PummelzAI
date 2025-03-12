using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Czaremir
        private MGPumCommand handleCzaremir(MGPumUnit czaremir)
        {
            if (czaremir.ownerID == this.playerID)
            {
                // Eigener Czaremir
                return handleOwnCzaremir(czaremir);
            }
            else
            {
                // Gegnerischer Czaremir
                return handleEnemyCzaremir(czaremir);
            }
        }

        // Eigener Czaremir: Halte ihn sicher und besch�tze ihn
        private MGPumCommand handleOwnCzaremir(MGPumUnit czaremir)
        {
            // Halte Czaremir weit von Gegnern fern
            MGPumField safeField = findSafeField(czaremir);
            if (safeField != null && safeField != czaremir.field)
            {
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(czaremir, safeField.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            // Besch�tze Czaremir mit Tanks und Fernk�mpfern
            protectCzaremir(czaremir);

            // Halte Fluchtwege offen
            maintainEscapeRoutes(czaremir);

            return null; // Keine spezifische Aktion
        }

        // Gegnerischer Czaremir: Versuche, ihn zu t�ten
        private MGPumCommand handleEnemyCzaremir(MGPumUnit czaremir)
        {
            // Durchbreche die Verteidigung mit Nahkampfeinheiten
            MGPumUnit meleeAttacker = findMeleeAttacker(czaremir);
            if (meleeAttacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(meleeAttacker, czaremir);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // T�te die sch�tzenden Fernk�mpfer
            MGPumUnit rangedProtector = findRangedProtector(czaremir);
            if (rangedProtector != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(findBestAttacker(rangedProtector), rangedProtector);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // Blockiere Fluchtwege
            blockEscapeRoutes(czaremir);

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde ein sicheres Feld f�r Czaremir
        private MGPumField findSafeField(MGPumUnit czaremir)
        {
            // Finde das am weitesten von Gegnern entfernte Feld
            MGPumField safestField = null;
            int maxDistance = -1;

            foreach (MGPumField field in state.fields.getFieldsInRange(czaremir.field.coords, czaremir.speed))
            {
                int distance = calculateDistanceToNearestEnemy(field.coords);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    safestField = field;
                }
            }

            return safestField;
        }

        // Hilfsmethode: Berechne die Entfernung zum n�chsten Gegner
        private int calculateDistanceToNearestEnemy(Vector2Int coords)
        {
            int minDistance = int.MaxValue;

            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                int distance = Mathf.Abs(enemy.field.coords.x - coords.x) + Mathf.Abs(enemy.field.coords.y - coords.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance;
        }

        // Hilfsmethode: Besch�tze Czaremir mit Tanks und Fernk�mpfern
        private void protectCzaremir(MGPumUnit czaremir)
        {
            // Finde Tanks (Einheiten mit hohen Lebenspunkten)
            List<MGPumUnit> tanks = findTanks();
            foreach (MGPumUnit tank in tanks)
            {
                // Bewege Tanks in die N�he von Czaremir
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(tank, czaremir.field.coords);
                if (moveCommand != null)
                {
                    stateOracle.executeCommand(moveCommand);
                }
            }

            // Finde Fernk�mpfer
            List<MGPumUnit> rangedUnits = findRangedUnits();
            foreach (MGPumUnit rangedUnit in rangedUnits)
            {
                // Positioniere Fernk�mpfer in der N�he von Czaremir
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(rangedUnit, czaremir.field.coords);
                if (moveCommand != null)
                {
                    stateOracle.executeCommand(moveCommand);
                }
            }
        }

        // Hilfsmethode: Halte Fluchtwege offen
        private void maintainEscapeRoutes(MGPumUnit czaremir)
        {
            // Implementiere die Logik zur Offenhaltung von Fluchtwegen
        }

        // Hilfsmethode: Finde Nahkampfangreifer
        private MGPumUnit findMeleeAttacker(MGPumUnit target)
        {
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.range == 1 && unit.power >= target.health) // Nahkampfeinheit mit ausreichender St�rke
                {
                    return unit;
                }
            }
            return null;
        }

        // Hilfsmethode: Finde sch�tzende Fernk�mpfer
        private MGPumUnit findRangedProtector(MGPumUnit czaremir)
        {
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                if (unit.range > 1 && unit.field.coords == czaremir.field.coords) // Fernk�mpfer in der N�he von Czaremir
                {
                    return unit;
                }
            }
            return null;
        }

        // Hilfsmethode: Blockiere Fluchtwege
        private void blockEscapeRoutes(MGPumUnit czaremir)
        {
            // Implementiere die Logik zur Blockierung von Fluchtwegen
        }

        // Hilfsmethode: Finde Tanks (Einheiten mit hohen Lebenspunkten)
        private List<MGPumUnit> findTanks()
        {
            List<MGPumUnit> tanks = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.health >= 5) // Beispiel: Einheiten mit mindestens 5 Lebenspunkten
                {
                    tanks.Add(unit);
                }
            }
            return tanks;
        }

        // Hilfsmethode: Finde Fernkampfeinheiten
        private List<MGPumUnit> findRangedUnits()
        {
            List<MGPumUnit> rangedUnits = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.range > 1) // Fernkampfeinheiten
                {
                    rangedUnits.Add(unit);
                }
            }
            return rangedUnits;
        }
    }
}