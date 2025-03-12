using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Spot
        private MGPumCommand handleSpot(MGPumUnit spot)
        {
            if (spot.ownerID == this.playerID)
            {
                // Eigener Spot
                return handleOwnSpot(spot);
            }
            else
            {
                // Gegnerischer Spot
                return handleEnemySpot(spot);
            }
        }

        // Eigener Spot: Bleibe in der N�he von Verb�ndeten und Fernk�mpfern
        private MGPumCommand handleOwnSpot(MGPumUnit spot)
        {
            // Finde Verb�ndete in der N�he
            List<MGPumUnit> nearbyAllies = findNearbyAllies(spot);
            if (nearbyAllies.Count > 0)
            {
                // Bewege Spot in Richtung der Verb�ndeten
                MGPumUnit closestAlly = findClosestUnit(spot, nearbyAllies);
                if (closestAlly != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(spot, closestAlly.field.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Bevorzuge die N�he zu Fernkampfeinheiten
            MGPumUnit rangedAlly = findRangedAlly(spot);
            if (rangedAlly != null)
            {
                MGPumMoveCommand moveCommand = findMoveTowardsTarget(spot, rangedAlly.field.coords);
                if (moveCommand != null)
                {
                    return moveCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }

        // Gegnerischer Spot: T�te ihn zuerst
        private MGPumCommand handleEnemySpot(MGPumUnit spot)
        {
            // Finde den besten Angreifer f�r Spot
            MGPumUnit attacker = findBestAttacker(spot);
            if (attacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(attacker, spot);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            return null; // Keine spezifische Aktion
        }
    }
}