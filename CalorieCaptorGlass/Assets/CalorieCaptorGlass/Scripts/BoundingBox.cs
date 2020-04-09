using System.Collections;
using UnityEngine;

namespace CalorieCaptorGlass
{
    /// <summary>
    /// 画像のバンディングボックスに対応する空間上のバウンディングボックスを
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class BoundingBox : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cornerObject;

        [SerializeField]
        private GameObject _textMesh;

        private Vector3 _topLeft, _topRight, _bottomLeft, _bottomRight;

        private GameObject _topLeftObj, _topRightObj, _bottomLeftObj, _bottomRightObj;
        private TextMesh[] _lengthViewMeshes = new TextMesh[4];

        private LineRenderer _lineRenderer;

        public float animationTimeSpan = 2.0f;

        private void Awake()
        {
            _topLeftObj = Instantiate(_cornerObject, Vector3.zero, Quaternion.identity);
            _topLeftObj.transform.parent = gameObject.transform;

            _topRightObj = Instantiate(_cornerObject, Vector3.zero, Quaternion.identity);
            _topRightObj.transform.parent = gameObject.transform;

            _bottomLeftObj = Instantiate(_cornerObject, Vector3.zero, Quaternion.identity);
            _bottomLeftObj.transform.parent = gameObject.transform;

            _bottomRightObj = Instantiate(_cornerObject, Vector3.zero, Quaternion.identity);
            _bottomRightObj.transform.parent = gameObject.transform;

            for (int i = 0; i < 4; i++)
            {
                _lengthViewMeshes[i] = Instantiate(_textMesh).GetComponent<TextMesh>();
                _lengthViewMeshes[i].transform.parent = gameObject.transform;
            }
        }

        // Use this for initialization
#if UNITY_EDITOR
        void Start()
        {
            Initialize(_topLeft, _topRight, _bottomRight, _bottomLeft);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="centerPosition"></param>
        /// <param name="viewPosition"></param>
        /// <param name="length">普通のUnityの座標系上の長さでOK</param>
        private void SetLengthView(TextMesh target, Vector3 centerPosition, Vector3 viewPosition, float length)
        {
            target.transform.position = viewPosition + new Vector3(0, 0.02f, 0);
            target.transform.rotation = Quaternion.LookRotation(centerPosition - viewPosition);
            target.text = $"{length * 100f} cm";
        }

        public void Initialize(Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomRight = bottomRight;
            _bottomLeft = bottomLeft;

            var centerPosition = (topLeft + topRight + bottomLeft + bottomRight) / 4;
            transform.position = centerPosition;

            _topLeftObj.transform.position = topLeft;

            _topRightObj.transform.position = topRight;

            _bottomLeftObj.transform.position = bottomLeft;

            _bottomRightObj.transform.position = bottomRight;

            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _lineRenderer.loop = true;

            _lineRenderer.positionCount = 4;

            _lineRenderer.SetPosition(0, centerPosition);
            _lineRenderer.SetPosition(1, centerPosition);
            _lineRenderer.SetPosition(2, centerPosition);
            _lineRenderer.SetPosition(3, centerPosition);

            var bottomLength = (bottomLeft - bottomRight).magnitude;
            var topLength = (topLeft - topRight).magnitude;
            var rightLength = (topRight - bottomRight).magnitude;
            var leftLength = (topLeft - bottomLeft).magnitude;

            SetLengthView(_lengthViewMeshes[0], centerPosition, (topLeft + topRight) / 2f, topLength);
            SetLengthView(_lengthViewMeshes[1], centerPosition, (bottomLeft + bottomRight) / 2f, bottomLength);
            SetLengthView(_lengthViewMeshes[2], centerPosition, (topRight + bottomRight) / 2f, rightLength);
            SetLengthView(_lengthViewMeshes[3], centerPosition, (topLeft + bottomLeft) / 2f, leftLength);

            StartCoroutine(BBoxAnimation());
            StartCoroutine(LineAnimation(centerPosition, topLeft, topRight, bottomRight, bottomLeft));
        }

        IEnumerator BBoxAnimation()
        {
            var startTime = Time.time;
            for (; Time.time < startTime + animationTimeSpan;)
            {
                var timeSpan = Time.time - startTime;
                //Debug.Log(timeSpan);
                transform.localScale = (timeSpan / animationTimeSpan) * Vector3.one;
                yield return null;
            }
        }
        /// <summary>
        /// LineRendererはscaleに依存しないので。
        /// </summary>
        /// <param name="center"></param>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomRight"></param>
        /// <param name="bottomLeft"></param>
        /// <returns></returns>
        IEnumerator LineAnimation(Vector3 center, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft)
        {
            var startTime = Time.time;
            for (; Time.time < startTime + animationTimeSpan;)
            {
                var timeSpan = Time.time - startTime;
                var newTopLeft = Vector3.Lerp(center, topLeft, timeSpan / animationTimeSpan);
                var newTopRight = Vector3.Lerp(center, topRight, timeSpan / animationTimeSpan);
                var newBottomRight = Vector3.Lerp(center, bottomRight, timeSpan / animationTimeSpan);
                var newBottomLeft = Vector3.Lerp(center, bottomLeft, timeSpan / animationTimeSpan);

                _lineRenderer.SetPosition(0, newTopLeft);
                _lineRenderer.SetPosition(1, newTopRight);
                _lineRenderer.SetPosition(2, newBottomRight);
                _lineRenderer.SetPosition(3, newBottomLeft);
                yield return null;
            }

            yield return null;
            //最終
            _lineRenderer.SetPosition(0, topLeft);
            _lineRenderer.SetPosition(1, topRight);
            _lineRenderer.SetPosition(2, bottomRight);
            _lineRenderer.SetPosition(3, bottomLeft);
            yield return null;
        }
    }
}
