using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        public const string type = "strategic";

        private int checkDeadlockTurnNumber = -1;
        private int cleanDeadlockTurnNumber = -1;

        public MGPumStrategicAIController(int playerID) : base(playerID)
        {
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

            // Priorisiere Angriffe auf wichtige Ziele
            foreach (MGPumUnit unit in possibleAttackers)
            {
                MGPumAttackCommand ac = findAttackCommand(unit);
                if (ac != null)
                {
                    return ac;
                }
            }
            return null;
        }

        private MGPumAttackCommand findAttackCommand(MGPumUnit unit)
        {
            MGPumAttackChainMatcher matcher = unit.getAttackMatcher();

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, matcher);
                chain.add(unit.field);

                Vector2Int position = unit.field.coords;
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
                    MGPumAttackCommand ac = new MGPumAttackCommand(this.playerID, chain, unit);
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

        private MGPumMoveCommand findMoveCommand(MGPumUnit unit)
        {
            MGPumMoveChainMatcher matcher = unit.getMoveMatcher();

            // Beste Bewegung führt uns näher zu einem Gegner
            int currentBestDistance = int.MaxValue;
            MGPumMoveCommand currentBestCommand = null;

            foreach (Vector2Int direction in getDirections())
            {
                MGPumFieldChain chain = new MGPumFieldChain(unit.ownerID, matcher);
                chain.add(unit.field);

                bool chainFinished = false;
                int fieldsSearchedForEnemy = 0;

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
                        fieldsSearchedForEnemy++;
                        if (fieldAtPosition.unit != null && fieldAtPosition.unit.ownerID == state.getOpponent(this.playerID).playerID)
                        {
                            if (fieldsSearchedForEnemy <= currentBestDistance)
                            {
                                if (chain.isValidChain() && chain.getLength() > 1)
                                {
                                    MGPumMoveCommand mc = new MGPumMoveCommand(this.playerID, chain, unit);
                                    if (stateOracle.checkMoveCommand(mc))
                                    {
                                        currentBestDistance = fieldsSearchedForEnemy;
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