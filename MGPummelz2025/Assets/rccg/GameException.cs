using System;

namespace RelegatiaCCG.rccg.engine.exceptions
{
    class GameException : Exception
    {
        public GameException(string message) : base(message)
        {
        }
    }
}
