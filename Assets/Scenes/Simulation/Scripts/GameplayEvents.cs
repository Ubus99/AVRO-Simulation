using System;
using System.Collections.Generic;

namespace Scenes.Simulation.Scripts
{
    public static class GameplayEvents
    {
        public static Action<MissionSo> missionSelectedEvent;
        public static Action<IEnumerable<MissionSo>> missionQueueUpdateEvent;
        public static Action<MissionSubState> missionSubmittedEvent;
    }
}
