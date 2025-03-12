using System;
using System.Collections.Generic;
using System.IO;

public class MGAITournament
{
    public enum GameResult
    {
        Win0,
        Win1, 
        Draw
    }

    public int[,] wins;
    public int[,] losses;
    public int[,] draws;
    public int[,] scores;

    public String[] contestants;

    public Dictionary<String, int> contestantToIndex;

    public Dictionary<int, String> contestantByRank;



    public List<String> log;

    public MGAITournament(String[] contestants)
    {
        this.contestants = contestants;
        int size = contestants.Length;
        wins = new int[size, size];
        losses = new int[size, size];
        draws = new int[size, size];
        scores = new int[size, size];

        contestantToIndex = new Dictionary<string, int>();

        log = new List<string>();

        for(int i = 0; i < size; i++)
        {
            contestantToIndex.Add(contestants[i], i);
        }
    }

    public String addOutcome(GameResult result, String player0, String player1)
    {
        return addOutcome(result, player0, player1, contestantToIndex[player0], contestantToIndex[player1]);
    }

    public String addOutcome(GameResult result, String player0, String player1, int pindex0, int pindex1)
    {
        if(result == GameResult.Win0)
        {
            wins[pindex0, pindex1]++;
            losses[pindex1, pindex0]++;
            scores[pindex0, pindex1] += 3;
            log.Add(player0 + " beats " + player1);
        }
        else if (result == GameResult.Win1)
        {
            wins[pindex1, pindex0]++;
            losses[pindex0, pindex1]++;
            scores[pindex1, pindex0] += 3;
            log.Add(player0 + " loses to " + player1);
        }
        else if (result == GameResult.Draw)
        {
            draws[pindex0, pindex1]++;
            draws[pindex1, pindex0]++;
            scores[pindex0, pindex1] += 1;
            scores[pindex1, pindex0] += 1;
            log.Add(player0 + " draws " + player1);
        }

        return log[log.Count - 1];
    }



    const string basepath = "C:/temp3/eval_";

    private void rankContestants()
    {
        List<Tuple<String, int>> scoresPerContestant = new List<Tuple<string, int>>();
        contestantByRank = new Dictionary<int, string>();

        foreach (String contestant in contestants)
        {
            int contestantIndex = contestantToIndex[contestant];
            int total = 0;
            for (int i = 0; i < contestants.Length; i++)
            {
                total += scores[contestantIndex, i];
            }
            scoresPerContestant.Add(new Tuple<string, int>(contestant, total));
            scoresPerContestant.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        }
        int rank = 0;
        foreach (Tuple<String, int> sortedContestant in scoresPerContestant)
        {
            contestantByRank.Add(rank, sortedContestant.Item1);
            rank++;
        }
    }

    public void logResults()
    {
        rankContestants();

        //logArray("wins", wins);
        logRankedArray("wins", wins);
        logRankedArray("losses", losses);
        logRankedArray("draws", draws);
        logRankedArray("scores", scores);
        logList("log", log);
    }

    public void logRankedArray(string name, int[,] array)
    {
        string filePath = basepath + name + ".csv";

        string content = ";";

        //head
        for (int rank = 0; rank < contestantByRank.Count; rank++)
        {
            content += contestantByRank[rank] + ";";
        }

        for (int ranki = 0; ranki < contestants.Length; ranki++)
        {
            int lineIndex = contestantToIndex[contestantByRank[ranki]];

            content += "\n" + contestants[lineIndex] + ";";
            for (int rankj = 0; rankj < contestants.Length; rankj++)
            {
                int colIndex = contestantToIndex[contestantByRank[rankj]];
                content += array[lineIndex, colIndex] + ";";
            }
        }

        File.WriteAllText(filePath, content);

    }

    public void logArray(string name, int[,] array)
    {
        string filePath = basepath + name + ".csv";

        string content = ";";

        //head
        foreach(string contestant in contestants)
        {
            content += contestant + ";";
        }

        for(int i = 0; i < contestants.Length; i++)
        {
            content += "\n" + contestants[i] + ";";
            for (int j = 0; j < contestants.Length; j++)
            {
                content += array[i, j] + ";";
            }
        }
        
        File.WriteAllText(filePath, content);
        
    }

    public void logList(string name, List<string> list)
    {
        string filePath = basepath + name + ".csv";

        string content = "";


        //head
        foreach (string item in list)
        {
            content += item + "\n";
        }

        File.WriteAllText(filePath, content);

    }
}
