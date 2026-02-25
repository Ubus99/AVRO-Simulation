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

        public static GameSettings currentSettings = new()
        {
            ID = 0,
            PracticeMode = false,
            Input = Input.Mouse,
            Severity = Severity.Easy
        };

        // game events
        public static Action restartEvent;
        public static Action<Scenes> switchSceneEvent;
        public static Action gameModeUpdatedEvent;

        public static string ParticipantString
        {
            get { return $"P{currentSettings.ID}"; }
        }

        public static string GameModeString
        {
            get { return $"{currentSettings.Input.ToString()[0]}_{currentSettings.Severity.ToString()[0]}"; }
        }

        public struct GameSettings
        {
            public bool PracticeMode;
            public Input Input;
            public Severity Severity;
            public int ID;
        }
    }
}
