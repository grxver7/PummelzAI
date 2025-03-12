using System;

public interface MGAIEvaluable
{
    void registerResultCallback(Action<MGAITournament.GameResult> callback);

}
