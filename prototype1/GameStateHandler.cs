using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prototype1
{
    public enum GameState { STARTING, RUNNING, ENDING, IDLE };

    public static class GameStateHandler
    {
        private static GameState _currentState;
        public static GameState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
    }
}
