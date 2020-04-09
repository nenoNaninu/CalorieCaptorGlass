using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace CalorieCaptorGlass
{
    public class HandPositionPloter : MonoBehaviour, ISourcePositionHandler, ISourceStateHandler, IInputClickHandler, IHoldHandler
    {
        private Transform _children;

        private Renderer _childMaterial;

        [SerializeField]
        private Material _holdStateMaterial;

        [SerializeField]
        private Material _normalStateMaterial;

        public void OnInputClicked(InputClickedEventData eventData)
        {
        }

        public void OnPositionChanged(SourcePositionEventData eventData)
        {
            gameObject.transform.position = eventData.GripPosition;
            gameObject.transform.LookAt(CameraCache.Main.transform.position);
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            _children.gameObject.SetActive(true);
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            _children.gameObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
            _children = gameObject.transform.GetChild(0);
            _childMaterial = _children.GetComponentInChildren<Renderer>();
            _children.gameObject.SetActive(false);
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            _childMaterial.material = _holdStateMaterial;
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            _childMaterial.material = _normalStateMaterial;
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
        }
    }
}