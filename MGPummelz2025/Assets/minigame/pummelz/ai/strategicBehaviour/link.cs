using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mg.pummelz
{
    public partial class MGPumStrategicAIController : MGPumStudentAIPlayerController
    {
        // Methode zur Steuerung von Link
        private MGPumCommand handleLink(MGPumUnit link)
        {
            if (link.ownerID == this.playerID)
            {
                // Eigener Link
                return handleOwnLink(link);
            }
            else
            {
                // Gegnerischer Link
                return handleEnemyLink(link);
            }
        }

        // Eigener Link: Bleibe in der Nähe von anderen Links
        private MGPumCommand handleOwnLink(MGPumUnit link)
        {
            // Finde andere Links in der Nähe
            List<MGPumUnit> nearbyLinks = findNearbyLinksForOwn(link);
            if (nearbyLinks.Count > 0)
            {
                // Bewege Link in Richtung des nächsten Links
                MGPumUnit closestLink = findClosestUnit(link, nearbyLinks);
                if (closestLink != null)
                {
                    MGPumMoveCommand moveCommand = findMoveTowardsTarget(link, closestLink.field.coords);
                    if (moveCommand != null)
                    {
                        return moveCommand;
                    }
                }
            }

            // Wenn keine anderen Links in der Nähe sind, bewege Link gestaffelt
            return findStaggeredMoveCommand(link);
        }

        // Gegnerischer Link: Versuche, Links zu trennen und zu töten
        private MGPumCommand handleEnemyLink(MGPumUnit link)
        {
            // Trenne Links voneinander
            separateLinks(link);

            // Töte Link in der Nähe anderer Links
            MGPumUnit attacker = findBestAttacker(link);
            if (attacker != null)
            {
                MGPumAttackCommand attackCommand = findAttackCommand(attacker, link);
                if (attackCommand != null)
                {
                    return attackCommand;
                }
            }

            // Nähere dich mit vielen Einheiten
            return approachWithMultipleUnits(link);
        }

        // Hilfsmethode: Finde andere Links in der Nähe für eigenen Link
        private List<MGPumUnit> findNearbyLinksForOwn(MGPumUnit link)
        {
            List<MGPumUnit> nearbyLinks = new List<MGPumUnit>();
            foreach (MGPumUnit unit in state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, this.playerID))
            {
                if (unit.name == "Link" && unit != link)
                {
                    int distance = Mathf.Abs(unit.field.coords.x - link.field.coords.x) + Mathf.Abs(unit.field.coords.y - link.field.coords.y);
                    if (distance <= 3) // Beispiel: Links innerhalb von 3 Feldern
                    {
                        nearbyLinks.Add(unit);
                    }
                }
            }
            return nearbyLinks;
        }

        // Hilfsmethode: Bewege Link gestaffelt
        private MGPumCommand findStaggeredMoveCommand(MGPumUnit link)
        {
            // Implementiere die Logik für gestaffelte Bewegungen
            return null;
        }

        // Hilfsmethode: Trenne Links voneinander
        private void separateLinks(MGPumUnit link)
        {
            // Implementiere die Logik zur Trennung von Links
        }

        // Hilfsmethode: Nähere dich mit vielen Einheiten
        private MGPumCommand approachWithMultipleUnits(MGPumUnit link)
        {
            // Finde mehrere Einheiten, um Link anzugreifen
            List<MGPumUnit> attackers = findMultipleAttackers(link);
            if (attackers.Count > 0)
            {
                foreach (MGPumUnit attacker in attackers)
                {
                    MGPumAttackCommand attackCommand = findAttackCommand(attacker, link);
                    if (attackCommand != null)
                    {
                        return attackCommand;
                    }
                }
            }

            return null;
        }

        private List<MGPumUnit> findMultipleAttackers(MGPumUnit link)
        {
            // Beispielimplementierung: Finde mehrere Einheiten, um Link anzugreifen
            return state.players
                .Where(p => p.playerID != this.playerID)
                .SelectMany(p => state.getAllUnitsInZone(MGPumZoneType.Battlegrounds, p.playerID)) // Verwende getAllUnitsInZone
                .Where(u => u.currentPower > 3) // Beispielkriterium
                .ToList();
        }
    }
}
