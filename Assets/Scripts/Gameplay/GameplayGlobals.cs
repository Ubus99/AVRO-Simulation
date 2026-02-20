using System;

namespace Gameplay
{
    public static class GameplayGlobals
    {
        public enum Input
        {
            Mouse,
            Touch,
            Speech
        }

        public enum Scenes
        {
            Login,
            Simulation
        }

        public enum Severity
        {
            Easy,
            Hard
        }

        public static Input currentInput = Input.Mouse;
        public static Severity currentSeverity = Severity.Easy;
        public static int currentID = 0;

        // game events
        public static Action restartEvent;
        public static Action<Scenes> switchSceneEvent;
        public static Action<int, Input, Severity> setGameMode;

        public static string logName
        {
            get { return $"{currentID}_{currentInput}_{currentSeverity}"; }
        }
    }
}
