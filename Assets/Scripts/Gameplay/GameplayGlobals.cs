using System;
using System.Collections.Generic;

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

        // mission events
        public static Action<MissionSo> switchMissionEvent;
        public static Action<IList<MissionSo>> missionQueueUpdateEvent;
        public static Action<MissionSubState> missionSubmittedEvent;
        public static Action missionCompletedEvent;

        // game events
        public static Action<Scenes> switchSceneEvent;
        public static Action<int, Input, Severity> setGameMode;

        public static string logName
        {
            get { return $"{currentID}_{currentInput.ToString()}_{currentSeverity.ToString()}"; }
        }
    }
}
