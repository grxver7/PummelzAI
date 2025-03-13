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
            // Überprüfe, ob es eine spezifische Einheit gibt und handle sie entsprechend
            MGPumUnit specificUnit = findSpecificUnit();
            if (specificUnit != null)
            {
                MGPumCommand specificUnitCommand = handleSpecificUnit(specificUnit);
                if (specificUnitCommand != null)
                {
                    return specificUnitCommand;
                }
            }

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

        // Hilfsmethode: Finde eine spezifische Einheit
        private MGPumUnit findSpecificUnit()
        {
            return state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID)
                .FirstOrDefault(unit => IsSpecificUnit(unit.name));
        }

        // Hilfsmethode: Überprüfe, ob eine Einheit eine spezifische Einheit ist
        private bool IsSpecificUnit(string unitName)
        {
            switch (unitName)
            {
                case "Czaremir":
                case "Verletzter Chilly":
                case "Buffy":
                case "Spot":
                case "Link":
                case "Haley":
                case "Mampfred":
                case "Fluffy":
                case "Bummz":
                case "Hoppel":
                case "Wolli":
                    return true;
                default:
                    return false;
            }
        }

        // Methode zur Steuerung einer spezifischen Einheit
        private MGPumCommand handleSpecificUnit(MGPumUnit unit)
        {
            switch (unit.name)
            {
                case "Czaremir":
                    return handleCzaremir(unit);
                case "Chilly":
                    return handleChilly(unit);
                case "Buffy":
                    return handleBuffy(unit);
                case "Spot":
                    return handleSpot(unit);
                case "Link":
                    return handleLink(unit);
                case "Haley":
                    return handleHaley(unit);
                case "Mampfred":
                    return handleMampfred(unit);
                case "Fluffy":
                    return handleFluffy(unit);
                case "Bummz":
                    return handleBummz(unit);
                case "Hoppel":
                    return handleHoppel(unit);
                case "Wolli":
                    return handleWolli(unit);
                default:
                    return null;
            }
        }

        // Methode zur Steuerung einer anderen spezifischen Einheit
        private MGPumCommand handleOtherUnit(MGPumUnit otherUnit)
        {
            // Implementiere das spezifische Verhalten für die andere Einheit
            // Beispiel: Bewege die Einheit zu einem bestimmten Ziel
            MGPumMoveCommand moveCommand = findMoveTowardsTarget(otherUnit, new Vector2Int(5, 5));
            if (moveCommand != null)
            {
                return moveCommand;
            }

            // Wenn keine spezifische Aktion gefunden wird, führe eine Standardaktion aus
            return findRandomMoveCommand(otherUnit);
        }


        private MGPumAttackCommand findStrategicAttackCommand()
        {
            List<MGPumUnit> possibleAttackers = state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID)
                .Where(unit => stateOracle.canAttack(unit))
                .ToList();

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

        private MGPumUnit findBestAttackTarget()
        {
            List<MGPumUnit> enemyUnits = state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(this.playerID).playerID);

            return enemyUnits
                .OrderBy(enemy => priorityManager.GetAttackPriority(enemy.name))
                .FirstOrDefault();
        }

        private MGPumUnit findBestAttackerForTarget(MGPumUnit target, List<MGPumUnit> possibleAttackers)
        {
            return possibleAttackers
                .OrderByDescending(attacker => attacker.currentPower)
                .FirstOrDefault(attacker => attacker.currentPower >= target.currentHealth);
        }

        private MGPumAttackCommand findAttackCommand(MGPumUnit attacker, MGPumUnit target)
        {
            MGPumAttackChainMatcher matcher = attacker.getAttackMatcher();

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(attacker.ownerID, matcher);
                chain.add(attacker.field);

                Vector2Int position = attacker.field.coords + direction;

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
            List<MGPumUnit> possibleMovers = state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID)
                .Where(unit => stateOracle.canMove(unit))
                .ToList();

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

                Vector2Int position = unit.field.coords + direction;

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
            List<MGPumUnit> possibleMovers = state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID)
                .Where(unit => stateOracle.canMove(unit))
                .ToList();

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

                Vector2Int position = unit.field.coords + direction;

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

        private void checkDeadlocks()
        {
            LinkedList<MGPumGameEvent> eventsToCheck = state.log.getEventsOfThisTurn();

            bool foundCommand = eventsToCheck.OfType<MGPumCommand>().Any();
            cleanDeadlockTurnNumber = foundCommand ? -1 : state.turnNumber;
            checkDeadlockTurnNumber = state.turnNumber;
        }

        internal IEnumerable<Vector2Int> getRandomizedDirections(MGPumUnit unit)
        {
            List<Vector2Int> shuffledDirections = new List<Vector2Int>
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left + Vector2Int.up,
                Vector2Int.left + Vector2Int.down,
                Vector2Int.right + Vector2Int.up,
                Vector2Int.right + Vector2Int.down
            };

            System.Random rng = new System.Random(unit.id * 77 + state.turnNumber * 990000);

            return shuffledDirections.OrderBy(item => rng.Next());
        }
    }
}
