using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Fluffy
        private MGPumCommand handleFluffy(MGPumUnit fluffy)
        {
            if (fluffy.ownerID == this.playerID)
            {
                // Eigener Fluffy
                return handleOwnFluffy(fluffy);
            }
            else
            {
                // Gegnerischer Fluffy
                return handleEnemyFluffy(fluffy);
            }
        }

        // Eigener Fluffy: Flankiere wertvolle Einheiten und nutze Abkürzungen
        private MGPumCommand handleOwnFluffy(MGPumUnit fluffy)
        {
            // Finde wertvolle gegnerische Einheiten
            MGPumUnit valuableTarget = findValuableTarget();
            if (valuableTarget != null)
            {
                // Flankiere die wertvolle Einheit
                MGPumField flankingField = findFlankingField(fluffy, valuableTarget);
                if (flankingField != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(fluffy, flankingField.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Nutze Abkürzungen über das Terrain
            return useTerrainShortcuts(fluffy);
        }

        // Gegnerischer Fluffy: Vermeide, wichtige Einheiten nur durch Terrain zu schützen
        private MGPumCommand handleEnemyFluffy(MGPumUnit fluffy)
        {
            // Überprüfe, ob wichtige Einheiten durch Terrain geschützt sind
            List<MGPumUnit> importantUnits = findImportantUnits();
            foreach (MGPumUnit unit in importantUnits)
            {
                if (isProtectedByTerrain(unit))
                {
                    // Bewege Einheiten, um den Schutz zu umgehen
                    MGPumMoveCommand moveCommand = findMoveToAvoidTerrainProtection(unit);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Überprüfe, ob eine Einheit durch Terrain geschützt ist
        private bool isProtectedByTerrain(MGPumUnit unit)
        {
            // Beispiel: Überprüfe, ob die Einheit von Hindernissen umgeben ist
            foreach (Vector2Int direction in getDirections())
            {
                Vector2Int neighborPosition = unit.field.coords + direction;
                if (state.fields.inBounds(neighborPosition))
                {
                    MGPumField neighborField = state.fields.getField(neighborPosition);
                    if (neighborField.terrain != MGPumField.Terrain.Water && neighborField.terrain != MGPumField.Terrain.Mountain)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Hilfsmethode: Finde eine Bewegung, um den Terrain-Schutz zu umgehen
        private MGPumMoveCommand findMoveToAvoidTerrainProtection(MGPumUnit unit)
        {
            // Beispiel: Bewege eine Einheit, um den Schutz zu umgehen
            foreach (Vector2Int direction in getDirections())
            {
                Vector2Int newPosition = unit.field.coords + direction;
                if (state.fields.inBounds(newPosition))
                {
                    MGPumField flankingField = state.fields.getField(newPosition);
                    if (flankingField.terrain != MGPumField.Terrain.Water && flankingField.terrain != MGPumField.Terrain.Mountain)

                    {
                        MGPumMoveCommand moveCommand = new MGPumMoveCommand(this.playerID, new MGPumFieldChain(this.playerID, unit.getMoveMatcher()), unit);
                        if (stateOracle.checkMoveCommand(moveCommand))
                        {
                            return moveCommand;
                        }
                    }
                }
            }
            return null;
        }
    }
}