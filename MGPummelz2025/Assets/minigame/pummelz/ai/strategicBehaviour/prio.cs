using System.Collections.Generic;

namespace mg.pummelz
{
    public class MGPumPriorityManager
    {
        // Angriffspriorit�ten (Reihenfolge der Ziele f�r Angriffe)
        private List<string> attackPriorities;

        // Schutzpriorit�ten (Reihenfolge der eigenen Einheiten, die gesch�tzt werden sollten)
        private List<string> defensePriorities;

        public MGPumPriorityManager()
        {
            // Initialisiere die Angriffspriorit�ten
            attackPriorities = new List<string>
            {
                "Czaremir",   // H�chste Priorit�t
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
                "Wolli"       // Niedrigste Priorit�t
            };

            // Initialisiere die Schutzpriorit�ten
            defensePriorities = new List<string>
            {
                "Czaremir",   // H�chste Priorit�t
                "Chilly",     // Verletzter Chilly
                "Buffy",
                "Spot",
                "Link",
                "Haley",
                "Mampfred",
                "Fluffy",
                "Bummz",
                "Hoppel",
                "Wolli"       // Niedrigste Priorit�t
            };
        }

        // Gibt die Angriffspriorit�ten zur�ck
        public List<string> GetAttackPriorities()
        {
            return attackPriorities;
        }

        // Gibt die Schutzpriorit�ten zur�ck
        public List<string> GetDefensePriorities()
        {
            return defensePriorities;
        }

        // Gibt die Priorit�t einer Einheit f�r Angriffe zur�ck
        public int GetAttackPriority(string unitType)
        {
            return attackPriorities.IndexOf(unitType);
        }

        // Gibt die Priorit�t einer Einheit f�r den Schutz zur�ck
        public int GetDefensePriority(string unitType)
        {
            return defensePriorities.IndexOf(unitType);
        }

        // �berpr�ft, ob eine Einheit eine hohe Angriffspriorit�t hat
        public bool IsHighPriorityTarget(string unitType)
        {
            // Beispiel: Einheiten in den Top 5 der Angriffspriorit�ten
            return GetAttackPriority(unitType) <= 4;
        }

        // �berpr�ft, ob eine Einheit eine hohe Schutzpriorit�t hat
        public bool IsHighPriorityDefense(string unitType)
        {
            // Beispiel: Einheiten in den Top 5 der Schutzpriorit�ten
            return GetDefensePriority(unitType) <= 4;
        }
    }
}