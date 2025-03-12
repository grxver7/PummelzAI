using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Chilly
        private MGPumCommand handleChilly(MGPumUnit chilly)
        {
            if (chilly.ownerID == this.playerID)
            {
                // Eigener Chilly
                return handleOwnChilly(chilly);
            }
            else
            {
                // Gegnerischer Chilly
                return handleEnemyChilly(chilly);
            }
        }

        // Eigener Chilly: Versuche, Schaden zu erleiden, aber nicht zu sterben
        private MGPumCommand handleOwnChilly(MGPumUnit chilly)
        {
            // Suche Lava-Felder in der Nähe
            MGPumField lavaField = findLavaFieldNearby(chilly);
            if (lavaField != null && isSafeFromEnemies(chilly, lavaField.coords))
            {
                // Bewege Chilly in die Lava, um Schaden zu erleiden
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(chilly, lavaField.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            // Positioniere Chilly in Bereichen, in denen Schaden durch Gegner erwartet wird
            MGPumField dangerField = findDangerField(chilly);
            if (dangerField != null)
            {
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(chilly, dangerField.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            // Wenn keine spezifische Aktion möglich, bewege Chilly zufällig
            return findRandomMoveCommand(chilly);
        }

        // Gegnerischer Killy: Töte ihn so schnell wie möglich
        // Gegnerischer Chilly: Töte ihn, wenn er Schaden erlitten hat
        private MGPumCommand handleEnemyChilly(MGPumUnit chilly)
        {
            if (chilly.currentHealth < chilly.currentMaxHealth) // Prüft, ob Chilly Schaden erlitten hat
            {
                // Finde eine Einheit, die Chilly angreifen kann
                MGPumUnit attacker = findBestAttacker(chilly);
                if (attacker != null)
                {
                    MGPumAttackCommand attackCommand = findAttackCommand(attacker, chilly);
                    if (attackCommand != null)
                    {
                        return attackCommand;
                    }
                }
            }

            return null; // Keine Aktion, wenn Chilly keinen Schaden erlitten hat
        }

        // Methode zur Steuerung von Killy
        private MGPumCommand handleKilly(MGPumUnit killy)
        {
            if (killy.ownerID == this.playerID)
            {
                // Eigener Killy
                return handleOwnKilly(killy);
            }
            else
            {
                // Gegnerischer Killy
                return handleEnemyKilly(killy);
            }
        }

        // Eigener Killy: Setze ihn aggressiv ein
        private MGPumCommand handleOwnKilly(MGPumUnit killy)
        {
            // Finde wertvolle Ziele (Czaremir, Buffy, Spot, Link)
            MGPumUnit valuableTarget = findValuableTarget();
            if (valuableTarget != null)
            {
                // Greife das wertvolle Ziel an
                MGPumAttackCommand attackCommand = findAttackCommand(killy, valuableTarget);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // Bewege Killy in eine sichere Position, falls kein Angriff möglich
            return findSafeMoveCommand(killy);
        }

        // Gegnerischer Killy: Töte ihn so schnell wie möglich
        private MGPumCommand handleEnemyKilly(MGPumUnit killy)
        {
            // Finde eine starke Einheit, um Killy anzugreifen
            MGPumUnit attacker = findBestAttacker(killy);
            if (attacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(attacker, killy);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // Blockiere Killy mit weniger wertvollen Einheiten
            return blockKillyMovement(killy);
        }

        // Hilfsmethode: Finde ein Lava-Feld in der Nähe
        private MGPumField findLavaFieldNearby(MGPumUnit unit)
        {
            foreach (MGPumField field in state.fields.getFieldsInRange(unit.field.coords, 3))
            {
                if (field.terrain == MGPumField.Terrain.Lava)
                {
                    return field;
                }
            }
            return null;
        }

        // Hilfsmethode: Überprüfe, ob eine Position sicher vor Gegnern ist
        private bool isSafeFromEnemies(MGPumUnit unit, Vector2Int coords)
        {
            foreach (MGPumUnit enemy in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, state.getOpponent(unit.ownerID).playerID))
            {
                int distance = Mathf.Abs(enemy.field.coords.x - coords.x) + Mathf.Abs(enemy.field.coords.y - coords.y);
                if (distance <= enemy.currentRange)
                {
                    return false;
                }
            }
            return true;
        }

        // Hilfsmethode: Finde ein gefährliches Feld (Schaden durch Gegner erwartet)
        private MGPumField findDangerField(MGPumUnit unit)
        {
            // Implementiere die Logik zur Bestimmung eines gefährlichen Feldes
            return null;
        }

        // Hilfsmethode: Blockiere die Bewegung von Killy
        private MGPumCommand blockKillyMovement(MGPumUnit killy)
        {
            // Implementiere die Logik zur Blockierung von Killy
            return null;
        }
    }
}