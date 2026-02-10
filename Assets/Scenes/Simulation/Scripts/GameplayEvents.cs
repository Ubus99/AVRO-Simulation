using System;

namespace Scenes.Simulation.Scripts
{
    public static class GameplayEvents
    {
        public static Action<MissionSo> changeMissionEvent;
        public static Action<MissionSubState> missionCompletedEvent;
    }
}
