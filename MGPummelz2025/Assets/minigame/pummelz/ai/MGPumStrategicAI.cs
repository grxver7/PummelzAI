using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        public const string type = "strategic";

        private int checkDeadlockTurnNumber = -1;
        private int cleanDeadlockTurnNumber = -1;

        // Priority Manager für strategische Entscheidungen
        private MGPumPriorityManager priorityManager;

        public MGPumStrategicAIController(int playerID) : base(playerID)
        {
            priorityManager = new MGPumPriorityManager();
        }

        // Implementierung der abstrakten Methode getTeamMatrikels
        protected override int[] getTeamMatrikels()
        {
            // Hier müssen die Matrikelnummern deines Teams zurückgegeben werden.
            // Beispiel: Ein Array von Matrikelnummern
            return new int[] { 123456, 654321 }; // Ersetze dies mit den echten Matrikelnummern
        }

        internal override MGPumCommand calculateCommand()
        {
            // Priorisiere Angriffe auf wichtige Ziele
            MGPumAttackCommand ac = findStrategicAttackCommand();
            if (ac != null)
            {
                return ac;
            }

            // Bewege Einheiten strategisch
            MGPumMoveCommand mc = findStrategicMoveCommand();
            if (mc != null)
            {
                return mc;
            }

            // Überprüfe Deadlocks nur einmal pro Runde
            if (checkDeadlockTurnNumber != state.turnNumber)
            {
                checkDeadlocks();
            }

            if (cleanDeadlockTurnNumber == state.turnNumber)
            {
                mc = findDeadlockMoveCommand();
            }
            if (mc != null)
            {
                return mc;
            }

            return new MGPumEndTurnCommand(this.playerID);
        }

        private MGPumAttackCommand findStrategicAttackCommand()
        {
            List<MGPumUnit> possibleAttackers = new List<MGPumUnit>();

            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (stateOracle.canAttack(unit))
                {
                    possibleAttackers.Add(unit);
                }
            }

            // Finde das beste Angriffsziel basierend auf den Prioritäten
            MGPumUnit bestTarget = findBestAttackTarget();
            if (bestTarget != null)
            {
                // Finde den besten Angreifer für das Ziel
                MGPumUnit bestAttacker = findBestAttackerForTarget(bestTarget, possibleAttackers);
                if (bestAttacker != null)
                {
                    MGPumAttackCommand ac = findAttackCommand(bestAttacker, bestTarget);
                    if (ac != null)
                    {
                        return ac;
                    }
                }
            }

            return null;
        }

        // Hilfsmethode: Finde Verbündete in der Nähe
        private List<MGPumUnit> findNearbyAllies(MGPumUnit unit)
        {
            List<MGPumUnit> nearbyAllies = new List<MGPumUnit>();
            foreach (MGPumUnit ally in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (ally != unit)
                {
                    int distance = Mathf.Abs(ally.field.coords.x - unit.field.coords.x) + Mathf.Abs(ally.field.coords.y - unit.field.coords.y);
                    if (distance <= 3) // Beispiel: Verbündete innerhalb von 3 Feldern
                    {
                        nearbyAllies.Add(ally);
                    }
                }
            }
            return nearbyAllies;
        }

        private MGPumUnit findBestAttackTarget()
        {
            List<MGPumUnit> enemyUnits = state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID);

            MGPumUnit bestTarget = null;
            int highestPriority = int.MaxValue;

            foreach (MGPumUnit enemy in enemyUnits)
            {
                int priority = priorityManager.GetAttackPriority(enemy.name);
                if (priority < highestPriority)
                {
                    highestPriority = priority;
                    bestTarget = enemy;
                }
            }

            return bestTarget;
        }

        private MGPumUnit findBestAttackerForTarget(MGPumUnit target, List<MGPumUnit> possibleAttackers)
        {
            MGPumUnit bestAttacker = null;
            int highestDamage = 0;

            foreach (MGPumUnit attacker in possibleAttackers)
            {
                if (attacker.currentPower >= target.currentHealth) // Einheit kann das Ziel töten
                {
                    return attacker;
                }
                else if (attacker.currentPower > highestDamage)
                {
                    highestDamage = attacker.currentPower;
                    bestAttacker = attacker;
                }
            }

            return bestAttacker;
        }

        private MGPumAttackCommand findAttackCommand(MGPumUnit attacker, MGPumUnit target)
        {
            MGPumAttackChainMatcher matcher = attacker.getAttackMatcher();

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(attacker.ownerID, matcher);
                chain.add(attacker.field);

                Vector2Int position = attacker.field.coords;
                position += direction;

                while (state.fields.inBounds(position))
                {
                    MGPumField fieldAtPosition = state.fields.getField(position);

                    if (chain.canAdd(fieldAtPosition))
                    {
                        chain.add(fieldAtPosition);
                    }

                    if (chain.isValidChain())
                    {
                        break;
                    }

                    position += direction;
                }

                if (chain.isValidChain())
                {
                    MGPumAttackCommand ac = new MGPumAttackCommand(this.playerID, chain, attacker);
                    if (stateOracle.checkAttackCommand(ac))
                    {
                        return ac;
                    }
                }
            }
            return null;
        }

        private MGPumMoveCommand findStrategicMoveCommand()
        {
            List<MGPumUnit> possibleMovers = new List<MGPumUnit>();

            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (stateOracle.canMove(unit))
                {
                    possibleMovers.Add(unit);
                }
            }

            // Bewege Einheiten strategisch
            foreach (MGPumUnit unit in possibleMovers)
            {
                MGPumMoveCommand mc = findMoveCommand(unit);
                if (mc != null)
                {
                    return mc;
                }
            }
            return null;
        }

        private MGPumMoveCommand findMoveCommand(MGPumUnit unit)
        {
            MGPumMoveChainMatcher matcher = unit.getMoveMatcher();

            // Beste Bewegung führt uns näher zu einem wichtigen Ziel
            int currentBestDistance = int.MaxValue;
            MGPumMoveCommand currentBestCommand = null;

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, matcher);
                chain.add(unit.field);

                bool chainFinished = false;
                int fieldsSearchedForTarget = 0;

                Vector2Int position = unit.field.coords;
                position += direction;

                while (state.fields.inBounds(position))
                {
                    MGPumField fieldAtPosition = state.fields.getField(position);

                    if (!chainFinished)
                    {
                        if (chain.canAdd(fieldAtPosition) && (!(fieldAtPosition.terrain == MGPumField.Terrain.Lava) || unit.hasMarker(MGPumMarkerAbility.Type.IgnoreTerrainMoving)))
                        {
                            chain.add(fieldAtPosition);
                        }
                        else
                        {
                            chainFinished = true;
                        }
                    }

                    if (chainFinished)
                    {
                        fieldsSearchedForTarget++;
                        if (fieldAtPosition.unit != null && fieldAtPosition.unit.ownerID == state.getOpponent(this.playerID).playerID)
                        {
                            int priority = priorityManager.GetAttackPriority(fieldAtPosition.unit.name);
                            if (priority < currentBestDistance)
                            {
                                if (chain.isValidChain() && chain.getLength() > 1)
                                {
                                    MGPumMoveCommand mc = new MGPumMoveCommand(this.playerID, chain, unit);
                                    if (stateOracle.checkMoveCommand(mc))
                                    {
                                        currentBestDistance = priority;
                                        currentBestCommand = mc;
                                    }
                                }
                            }
                        }
                    }

                    position += direction;
                }
            }
            return currentBestCommand;
        }

        private MGPumMoveCommand findDeadlockMoveCommand()
        {
            List<MGPumUnit> possibleMovers = new List<MGPumUnit>();

            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (stateOracle.canMove(unit))
                {
                    possibleMovers.Add(unit);
                }
            }

            foreach (MGPumUnit unit in possibleMovers)
            {
                MGPumMoveCommand mc = findDeadlockMoveCommand(unit);
                if (mc != null)
                {
                    return mc;
                }
            }
            return null;
        }

        private MGPumMoveCommand findDeadlockMoveCommand(MGPumUnit unit)
        {
            MGPumMoveChainMatcher matcher = unit.getMoveMatcher();

            foreach (Vector2Int direction in getRandomizedDirections(unit))
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, matcher);
                chain.add(unit.field);

                bool chainFinished = false;

                Vector2Int position = unit.field.coords;
                position += direction;

                while (state.fields.inBounds(position))
                {
                    MGPumField fieldAtPosition = state.fields.getField(position);

                    if (!chainFinished)
                    {
                        if (chain.canAdd(fieldAtPosition))
                        {
                            chain.add(fieldAtPosition);
                        }
                        else
                        {
                            chainFinished = true;
                        }
                    }

                    if (chainFinished && chain.isValidChain() && chain.getLength() > 1)
                    {
                        MGPumMoveCommand mc = new MGPumMoveCommand(this.playerID, chain, unit);
                        if (stateOracle.checkMoveCommand(mc))
                        {
                            return mc;
                        }
                    }
                    position += direction;
                }
            }
            return null;
        }

        // Hilfsmethode: Finde die nächste Einheit
        private MGPumUnit findClosestUnit(MGPumUnit source, List<MGPumUnit> targets)
        {
            MGPumUnit closestUnit = null;
            int minDistance = int.MaxValue;

            foreach (MGPumUnit target in targets)
            {
                int distance = Mathf.Abs(target.field.coords.x - source.field.coords.x) + Mathf.Abs(target.field.coords.y - source.field.coords.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestUnit = target;
                }
            }

            return closestUnit;
        }

        // Hilfsmethode: Finde eine Fernkampfeinheit in der Nähe
        private MGPumUnit findRangedAlly(MGPumUnit unit)
        {
            foreach (MGPumUnit ally in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (ally.currentRange > 1) // Fernkampfeinheit
                {
                    int distance = Mathf.Abs(ally.field.coords.x - unit.field.coords.x) + Mathf.Abs(ally.field.coords.y - unit.field.coords.y);
                    if (distance <= 5) // Beispiel: Fernkampfeinheit innerhalb von 5 Feldern
                    {
                        return ally;
                    }
                }
            }
            return null;
        }

        // Hilfsmethode: Finde wertvolle gegnerische Einheiten
        private MGPumUnit findValuableTarget()
        {
            // Beispiel: Priorisiere Einheiten wie Czaremir, Buffy, Spot, Link
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                if (unit.name == "Czaremir" || unit.name == "Buffy" || unit.name == "Spot" || unit.name == "Link")
                {
                    return unit;
                }
            }
            return null;
        }

        // Hilfsmethode: Finde wichtige Einheiten
        private List<MGPumUnit> findImportantUnits()
        {
            List<MGPumUnit> importantUnits = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.name == "Czaremir" || unit.name == "Buffy" || unit.name == "Spot" || unit.name == "Link")
                {
                    importantUnits.Add(unit);
                }
            }
            return importantUnits;
        }

        // Hilfsmethode: Finde den besten Angreifer für ein Ziel
        private MGPumUnit findBestAttacker(MGPumUnit target)
        {
            MGPumUnit bestAttacker = null;
            int highestDamage = 0;

            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.currentPower >= target.currentHealth) // Einheit kann das Ziel töten
                {
                    return unit;
                }
                else if (unit.currentPower > highestDamage)
                {
                    highestDamage = unit.currentPower;
                    bestAttacker = unit;
                }
            }

            return bestAttacker;
        }

        // Hilfsmethode: Finde mehrere schwächere Einheiten
        private List<MGPumUnit> findMultipleAttackers(MGPumUnit target)
        {
            List<MGPumUnit> attackers = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.currentPower < target.currentPower) // Schwächere Einheiten
                {
                    attackers.Add(unit);
                }
            }
            return attackers;
        }

        // Hilfsmethode: Finde ein Flankenfeld für die wertvolle Einheit
        private MGPumField findFlankingField(MGPumUnit fluffy, MGPumUnit target)
        {
            // Beispiel: Finde ein Feld, das die wertvolle Einheit von der Seite angreift
            foreach (Vector2Int direction in getDirections())
            {
                Vector2Int flankingPosition = target.field.coords + direction;
                if (state.fields.inBounds(flankingPosition))
                {
                    MGPumField flankingField = state.fields.getField(flankingPosition);
                    if (flankingField.unit == null && (flankingField.terrain != MGPumField.Terrain.Water && flankingField.terrain != MGPumField.Terrain.Mountain))
                    {
                        return flankingField;
                    }
                }
            }
            return null;
        }

        // Hilfsmethode: Nutze Abkürzungen über das Terrain
        private MGPumCommand useTerrainShortcuts(MGPumUnit fluffy)
        {
            // Beispiel: Bewege Fluffy über Hindernisse hinweg
            foreach (Vector2Int direction in getDirections())
            {
                Vector2Int newPosition = fluffy.field.coords + direction;
                if (state.fields.inBounds(newPosition))
                {
                    MGPumField field = state.fields.getField(newPosition);
                    if ((field.terrain != MGPumField.Terrain.Water && field.terrain != MGPumField.Terrain.Mountain) && fluffy.hasMarker(MGPumMarkerAbility.Type.IgnoreTerrainMoving))
                    {
                        MGPumMoveCommand moveCommand = new MGPumMoveCommand(this.playerID, new MGPumFieldChain(this.playerID, fluffy.getMoveMatcher()), fluffy);
                        if (stateOracle.checkMoveCommand(moveCommand))
                        {
                            return moveCommand;
                        }
                    }
                }
            }
            return null;
        }

        private MGPumCommand findSafeMoveCommand(MGPumUnit unit)
        {
            MGPumField safestField = null;
            int maxDistance = 0;

            foreach (MGPumField field in state.fields.getAllFields())
            {
                if (state.isMovePossible(unit, field.coords))
                {
                    int minEnemyDistance = getMinEnemyAttackDistance(field.coords);
                    if (minEnemyDistance > maxDistance)
                    {
                        maxDistance = minEnemyDistance;
                        safestField = field;
                    }
                }
            }

            if (safestField != null)
            {
                return findMoveTowardsTarget(unit, safestField.coords);
            }

            return findRandomMoveCommand(unit);
        }

        private MGPumField findSafestField(MGPumUnit unit)
        {
            MGPumField safestField = null;
            int maxDistance = int.MinValue;

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, unit.getMoveMatcher());
                chain.add(unit.field);

                Vector2Int position = unit.field.coords + direction;
                while (state.fields.inBounds(position))
                {
                    MGPumField fieldAtPosition = state.fields.getField(position);

                    if (chain.canAdd(fieldAtPosition))
                    {
                        chain.add(fieldAtPosition);
                    }
                    else
                    {
                        break;
                    }

                    int distanceToEnemy = calculateDistanceToNearestEnemy(fieldAtPosition);
                    if (distanceToEnemy > maxDistance)
                    {
                        maxDistance = distanceToEnemy;
                        safestField = fieldAtPosition;
                    }
                    position += direction;
                }
            }
            return safestField;
        }

        private int calculateDistanceToNearestEnemy(MGPumField field)
        {
            int minDistance = int.MaxValue;
            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID))
            {
                int distance = Mathf.Abs(enemy.field.coords.x - field.coords.x) + Mathf.Abs(enemy.field.coords.y - field.coords.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            return minDistance;
        }

        private void checkDeadlocks()
        {
            LinkedList<MGPumGameEvent> eventsToCheck = state.log.getEventsOfThisTurn();

            bool foundCommand = false;
            foreach (MGPumGameEvent e in eventsToCheck)
            {
                if (e is MGPumCommand)
                {
                    foundCommand = true;
                    break;
                }
            }
            if (!foundCommand)
            {
                cleanDeadlockTurnNumber = state.turnNumber;
            }
            else
            {
                cleanDeadlockTurnNumber = -1;
            }
            checkDeadlockTurnNumber = state.turnNumber;
        }

        internal IEnumerable<Vector2Int> getRandomizedDirections(MGPumUnit unit)
        {
            List<Vector2Int> shuffledDirections = new List<Vector2Int>();

            shuffledDirections.Add(Vector2Int.left);
            shuffledDirections.Add(Vector2Int.right);
            shuffledDirections.Add(Vector2Int.up);
            shuffledDirections.Add(Vector2Int.down);
            shuffledDirections.Add(Vector2Int.left + Vector2Int.up);
            shuffledDirections.Add(Vector2Int.left + Vector2Int.down);
            shuffledDirections.Add(Vector2Int.right + Vector2Int.up);
            shuffledDirections.Add(Vector2Int.right + Vector2Int.down);

            System.Random rng = new System.Random(unit.id * 77 + state.turnNumber * 990000);

            return shuffledDirections.OrderBy(item => rng.Next());
        }
    }
}