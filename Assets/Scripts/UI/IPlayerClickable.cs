using UnityEngine;

namespace UI
{
    public interface IPlayerClickable
    {
        public void ClickOn(object source, Vector2 position);
    }
}
