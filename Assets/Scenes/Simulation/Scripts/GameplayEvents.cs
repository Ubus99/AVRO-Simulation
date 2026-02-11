using System;
using System.Collections.Generic;

namespace Scenes.Simulation.Scripts
{
    public static class GameplayEvents
    {
        public enum Input
        {
            Mouse,
            Touch,
            Speech
        }

        public enum Severity
        {
            Easy,
            Hard
        }

        public static Action<MissionSo> missionSelectedEvent;
        public static Action<IEnumerable<MissionSo>> missionQueueUpdateEvent;
        public static Action<MissionSubState> missionSubmittedEvent;

        public static Action startLoginEvent;
        public static Action<int, Input, Severity> startSimulationEvent;
    }
}
