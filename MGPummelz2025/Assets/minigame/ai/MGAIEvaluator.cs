using mg.pummelz;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MGAIEvaluator : MonoBehaviour
{
    public MGMinigame game;

    public int gamesPerPairing;

    public Text progressText;

    MGLevelConfigurable lvlcfgable;
    public String[] levels;

    private MGAIConfigurable cfgable;
    private MGAIEvaluable evable;
    private MGAITournament tournament;
    private Queue<Tuple<string, string, string>> pairings;
    private Tuple<string, string, string> currentPairing;

    public void runEvaluation()
    {
        cfgable = (MGAIConfigurable)game;
        lvlcfgable = (MGLevelConfigurable)game;
        evable = (MGAIEvaluable)game;
        evable.registerResultCallback(this.pairingDone);

        List<string> contestants = new List<string>();
        foreach(string aiType in cfgable.getAITypes())
        {
            if("HUMAN".Equals(aiType.ToUpper()))
            {
                continue;
            }
            contestants.Add(aiType);
            Debug.Log("contestant: " + aiType);
        }

        tournament = new MGAITournament(contestants.ToArray());

        pairings = new Queue<Tuple<string, string, string>>();

        foreach(string contestant0 in contestants)
        {
            foreach (string contestant1 in contestants)
            {
                if (contestant0 != contestant1)
                {
                    for (int i = 0; i < gamesPerPairing; i++)
                    {
                        foreach (String level in levels)
                        {
                            //if (contestant0.Equals(MGPumSimpleAIPlayerController.type) || contestant1.Equals(MGPumSimpleAIPlayerController.type))
                            {
                                pairings.Enqueue(new Tuple<string, string, string>(contestant0, contestant1, level));
                            }
                        }

                            
                    }
                }
            }
        }

    }

    private void pairingDone(MGAITournament.GameResult result)
    {
        String outcome = tournament.addOutcome(result, currentPairing.Item1, currentPairing.Item2);
        //Debug.Log(outcome);
        progressText.text = pairings.Count + " remain";
        currentPairing = null;
    }

    private void nextPairing()
    {
        
        MGAIConfigurable cfgable = (MGAIConfigurable)game;

        if (pairings.Count > 0)
        {
            currentPairing = pairings.Dequeue();
            Debug.Log(currentPairing);
            cfgable.setAIType(0, currentPairing.Item1);
            cfgable.setAIType(1, currentPairing.Item2);
            lvlcfgable.setLevel(currentPairing.Item3);
            //reset already starts the game
            game.reset();
            //game.startGame();
        }
        else
        {
            Debug.Log("logging results");
            tournament.logResults();
            pairings = null;
        }
        
    }

    private void Update()
    {
        //wait for a new frame to give UI time to update and to avoid call stack overflow
        if (pairings != null && currentPairing == null)
        {
            nextPairing();
        }
            
    }

}
