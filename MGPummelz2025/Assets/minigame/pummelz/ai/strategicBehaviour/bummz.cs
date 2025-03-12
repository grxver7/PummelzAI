using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Bummz
        private MGPumCommand handleBummz(MGPumUnit bummz)
        {
            // Überprüfe, ob es sich um einen eigenen oder gegnerischen Bummz handelt
            if (bummz.ownerID == this.playerID)
            {
                // Eigener Bummz: Laufe in Gegner hinein
                return moveBummzTowardsEnemies(bummz);
            }
            else
            {
                // Gegnerischer Bummz: Töte ihn, wenn er in einer Gruppe ist
                return attackBummzIfInGroup(bummz);
            }
        }

        // Eigener Bummz: Laufe in Gegner hinein
        private MGPumCommand moveBummzTowardsEnemies(MGPumUnit bummz)
        {
            // Finde den nächsten Gegner
            MGPumUnit nearestEnemy = findNearestEnemy(bummz);
            if (nearestEnemy != null)
            {
                // Bewege Bummz in Richtung des Gegners
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(bummz, nearestEnemy.field.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            // Wenn kein Gegner in der Nähe ist, bewege Bummz zufällig
            return findRandomMoveCommand(bummz);
        }

        // Gegnerischer Bummz: Töte ihn, wenn er in einer Gruppe ist
        private MGPumCommand attackBummzIfInGroup(MGPumUnit bummz)
        {
            // Zähle die Anzahl der gegnerischen Einheiten in der Nähe von Bummz
            int enemyCount = countEnemiesNearby(bummz.field.coords, 2); // Radius von 2 Feldern
            if (enemyCount >= 2) // Wenn mindestens 2 Gegner in der Nähe sind
            {
                // Finde eine Fernkampfeinheit, um Bummz anzugreifen
                MGPumUnit rangedUnit = findRangedUnit();
                if (rangedUnit != null)
                {
                    MGPumAttackCommand attackCommand = findAttackCommand(rangedUnit, bummz);
                    if (attackCommand != null)
                    {
                        return attackCommand;
                    }
                }
            }

            return null; // Keine Aktion, wenn Bummz nicht in einer Gruppe ist
        }

        // Hilfsmethode: Finde den nächsten Gegner
        private MGPumUnit findNearestEnemy(MGPumUnit unit)
        {
            MGPumUnit nearestEnemy = null;
            int nearestDistance = int.MaxValue;

            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(unit.ownerID).playerID))
            {
                int distance = Mathf.Abs(enemy.field.coords.x - unit.field.coords.x) + Mathf.Abs(enemy.field.coords.y - unit.field.coords.y);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        // Hilfsmethode: Finde eine Bewegung in Richtung eines Ziels
        private MGPumMoveCommand findMoveTowardsTarget(MGPumUnit unit, Vector2Int targetCoords)
        {
            MGPumMoveChainMatcher matcher = unit.getMoveMatcher();

            // Beste Bewegung führt uns näher zum Ziel
            int currentBestDistance = int.MaxValue;
            MGPumMoveCommand currentBestCommand = null;

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, matcher);
                chain.add(unit.field);

                Vector2Int position = unit.field.coords;
                position += direction;

                while (state.fields.inBounds(position))
                {
                    MGPumField fieldAtPosition = state.fields.getField(position);

                    if (chain.canAdd(fieldAtPosition) && (!(fieldAtPosition.terrain == MGPumField.Terrain.Lava) || unit.hasMarker(MGPumMarkerAbility.Type.IgnoreTerrainMoving)))
                    {
                        chain.add(fieldAtPosition);
                    }

                    if (chain.isValidChain() && chain.getLength() > 1)
                    {
                        int distanceToTarget = Mathf.Abs(targetCoords.x - position.x) + Mathf.Abs(targetCoords.y - position.y);
                        if (distanceToTarget < currentBestDistance)
                        {
                            MGPumMoveCommand mc = new MGPumMoveCommand(this.playerID, chain, unit);
                            if (stateOracle.checkMoveCommand(mc))
                            {
                                currentBestDistance = distanceToTarget;
                                currentBestCommand = mc;
                            }
                        }
                    }

                    position += direction;
                }
            }

            return currentBestCommand;
        }

        // Hilfsmethode: Zähle Gegner in der Nähe
        private int countEnemiesNearby(Vector2Int coords, int radius)
        {
            int count = 0;
            for (int x = coords.x - radius; x <= coords.x + radius; x++)
            {
                for (int y = coords.y - radius; y <= coords.y + radius; y++)
                {
                    if (state.fields.inBounds(new Vector2Int(x, y)))
                    {
                        MGPumField field = state.fields.getField(new Vector2Int(x, y));
                        if (field.unit != null && field.unit.ownerID == state.getOpponent(this.playerID).playerID)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        // Hilfsmethode: Finde eine Fernkampfeinheit
        private MGPumUnit findRangedUnit()
        {
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.range > 1) // Einheit mit Fernkampf
                {
                    return unit;
                }
            }
            return null;
        }

        // Hilfsmethode: Finde einen zufälligen Bewegungsbefehl
        private MGPumMoveCommand findRandomMoveCommand(MGPumUnit unit)
        {
            foreach (Vector2Int direction in getRandomizedDirections(unit))
            {
                MGPumMoveCommand mc = findMoveCommand(unit);
                if (mc != null)
                {
                    return mc;
                }
            }
            return null;
        }
    }
}