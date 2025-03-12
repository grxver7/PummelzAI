using System.Collections.Generic;

namespace mg.pummelz
{
    public class MGPumPriorityManager
    {
        // Angriffsprioritäten (Reihenfolge der Ziele für Angriffe)
        private List<string> attackPriorities;

        // Schutzprioritäten (Reihenfolge der eigenen Einheiten, die geschützt werden sollten)
        private List<string> defensePriorities;

        public MGPumPriorityManager()
        {
            // Initialisiere die Angriffsprioritäten
            attackPriorities = new List<string>
            {
                "Czaremir",   // Höchste Priorität
                "Killy",
                "Buffy",
                "Spot",
                "Link",
                "Haley",
                "Mampfred",
                "Fluffy",
                "Bummz",
                "Chilly",
                "Hoppel",
                "Wolli"       // Niedrigste Priorität
            };

            // Initialisiere die Schutzprioritäten
            defensePriorities = new List<string>
            {
                "Czaremir",   // Höchste Priorität
                "Chilly",     // Verletzter Chilly
                "Buffy",
                "Spot",
                "Link",
                "Haley",
                "Mampfred",
                "Fluffy",
                "Bummz",
                "Hoppel",
                "Wolli"       // Niedrigste Priorität
            };
        }

        // Gibt die Angriffsprioritäten zurück
        public List<string> GetAttackPriorities()
        {
            return attackPriorities;
        }

        // Gibt die Schutzprioritäten zurück
        public List<string> GetDefensePriorities()
        {
            return defensePriorities;
        }

        // Gibt die Priorität einer Einheit für Angriffe zurück
        public int GetAttackPriority(string unitType)
        {
            return attackPriorities.IndexOf(unitType);
        }

        // Gibt die Priorität einer Einheit für den Schutz zurück
        public int GetDefensePriority(string unitType)
        {
            return defensePriorities.IndexOf(unitType);
        }

        // Überprüft, ob eine Einheit eine hohe Angriffspriorität hat
        public bool IsHighPriorityTarget(string unitType)
        {
            // Beispiel: Einheiten in den Top 5 der Angriffsprioritäten
            return GetAttackPriority(unitType) <= 4;
        }

        // Überprüft, ob eine Einheit eine hohe Schutzpriorität hat
        public bool IsHighPriorityDefense(string unitType)
        {
            // Beispiel: Einheiten in den Top 5 der Schutzprioritäten
            return GetDefensePriority(unitType) <= 4;
        }
    }
}