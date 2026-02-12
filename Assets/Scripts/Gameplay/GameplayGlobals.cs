using System;
using System.Collections.Generic;
using Scenes.Simulation.Scripts;

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

        public static Action<MissionSo> missionSelectedEvent;
        public static Action<IEnumerable<MissionSo>> missionQueueUpdateEvent;
        public static Action<MissionSubState> missionSubmittedEvent;

        public static Action<Scenes> switchSceneEvent;
        public static Action<int, Input, Severity> setGameMode;
    }
}
