using System;
using System.Collections.Generic;

namespace Gameplay.Missions
{
    public static class MissionEvents
    {
        // mission queue
        public static Action<MissionSo> missionQueuedEvent;
        public static Action<IList<MissionSo>> missionQueueUpdateEvent;

        // mission completion
        public static Action<MissionSo> switchMissionEvent;
        public static Action<MissionSubState> missionSubmittedEvent;
        public static Action<MissionSo> missionCompletedEvent;
    }
}
