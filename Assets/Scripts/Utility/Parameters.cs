using UnityEngine;

namespace Deenote
{
    public sealed class Parameters : MonoBehaviour
    {
        public static Parameters Instance { get; private set; }
        [SerializeField] private UIParameters paramObject;
        public static UIParameters Params => Instance.paramObject;

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of Parameters");
            }
#else
            Instance = this;
#endif
        }
    }
}
