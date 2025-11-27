using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class SerializedDictionarySampleThree : MonoBehaviour
    {
        [SerializeField]
        SerializedDictionary<ScriptableObject, string> _nameOverrides;
    }
}
