using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Mampfred
        private MGPumCommand handleMampfred(MGPumUnit mampfred)
        {
            if (mampfred.ownerID == this.playerID)
            {
                // Eigener Mampfred
                return handleOwnMampfred(mampfred);
            }
            else
            {
                // Gegnerischer Mampfred
                return handleEnemyMampfred(mampfred);
            }
        }

        // Eigener Mampfred: Suche Gras-Terrain, wenn Schaden erlitten wurde
        private MGPumCommand handleOwnMampfred(MGPumUnit mampfred)
        {
            // Überprüfe, ob Mampfred Schaden erlitten hat
            if (mampfred.health < mampfred.maxHealth)
            {
                // Suche Gras-Terrain in der Nähe
                MGPumField grassField = findGrassFieldNearby(mampfred);
                if (grassField != null)
                {
                    // Bewege Mampfred in Richtung des Gras-Terrains
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(mampfred, grassField.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Bleibe in der Nähe von Gras-Terrain
            return stayNearGrassTerrain(mampfred);
        }

        // Gegnerischer Mampfred: Versuche, ihn zu töten oder Gras-Terrain zu blockieren
        private MGPumCommand handleEnemyMampfred(MGPumUnit mampfred)
        {
            // Versuche, Mampfred direkt zu töten
            MGPumUnit attacker = findBestAttacker(mampfred);
            if (attacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(attacker, mampfred);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // Stiehl Gras-Terrain mit eigenem Mampfred
            MGPumField grassField = findGrassFieldNearby(mampfred);
            if (grassField != null)
            {
                MGPumUnit ownMampfred = findOwnMampfred();
                if (ownMampfred != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(ownMampfred, grassField.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Belagere Gras-Terrain
            return siegeGrassTerrain(mampfred);
        }

        // Hilfsmethode: Finde Gras-Terrain in der Nähe
        private MGPumField findGrassFieldNearby(MGPumUnit unit)
        {
            foreach (MGPumField field in state.fields.getFieldsInRange(unit.field.coords, 3)) // Beispiel: Suche innerhalb von 3 Feldern
            {
                if (field.terrain == MGPumField.Terrain.Grass)
                {
                    return field;
                }
            }
            return null;
        }

        // Hilfsmethode: Bleibe in der Nähe von Gras-Terrain
        private MGPumCommand stayNearGrassTerrain(MGPumUnit mampfred)
        {
            MGPumField grassField = findGrassFieldNearby(mampfred);
            if (grassField != null)
            {
                // Bewege Mampfred in die Nähe des Gras-Terrains
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(mampfred, grassField.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde eigenen Mampfred
        private MGPumUnit findOwnMampfred()
        {
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.type == "Mampfred")
                {
                    return unit;
                }
            }
            return null;
        }

        // Hilfsmethode: Belagere Gras-Terrain
        private MGPumCommand siegeGrassTerrain(MGPumUnit mampfred)
        {
            MGPumField grassField = findGrassFieldNearby(mampfred);
            if (grassField != null)
            {
                // Bewege Einheiten in die Nähe des Gras-Terrains
                List<MGPumUnit> units = findUnitsToSiege(grassField);
                foreach (MGPumUnit unit in units)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(unit, grassField.coords);
                    if (moveCommand != null)
                    {
                        stateOracle.executeCommand(moveCommand);
                    }
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Hilfsmethode: Finde Einheiten, um Gras-Terrain zu belagern
        private List<MGPumUnit> findUnitsToSiege(MGPumField grassField)
        {
            List<MGPumUnit> units = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.type != "Mampfred") // Vermeide, Mampfred zu bewegen
                {
                    int distance = Mathf.Abs(unit.field.coords.x - grassField.coords.x) + Mathf.Abs(unit.field.coords.y - grassField.coords.y);
                    if (distance <= 5) // Beispiel: Einheiten innerhalb von 5 Feldern
                    {
                        units.Add(unit);
                    }
                }
            }
            return units;
        }
    }
}