using System.Collections;
using System.Linq;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace CalorieCaptorGlass
{
    [RequireComponent(typeof(LineRenderer))]
    public class FoodDataPanel : MonoBehaviour
    {
        private WorldSpaceFoodData _worldSpaceFoodData;

        [SerializeField]
        private Text _calorieValue;

        [SerializeField]
        private Text _recipeName;

        [SerializeField]
        private Text _areaValueText;

        [SerializeField]
        private Transform anchor0, anchor1;

        [SerializeField] private Transform[] _vertexes;

        private LineRenderer _lineRenderer;

        private float _animationTimeSpan = 0.5f;

        private bool _startAnimation = false;

        private float _period = 2f;

        private float _frequency;

        private float _startTime;

        private Vector3 _targetPosition;

        /// <summary>
        /// アニメーション後のスケール
        /// </summary>
        public Vector3 TargetScale { get; set; }

        /// <summary>
        /// 接触済みかどうか
        /// </summary>
        public bool Touched { get; set; }

        private int _foodPanelLayer;
        private bool[] _hiddenFlags;
        private BoxCollider _boxCollider;
        /// <summary>
        /// setで情報の書き換えと座標のセットはしているのでご安心を。
        /// </summary>
        public WorldSpaceFoodData WorldSpaceFoodData
        {
            get { return _worldSpaceFoodData; }
            set
            {
                _worldSpaceFoodData = value;

                if (_calorieValue == null || _recipeName == null)
                {
                    Debug.LogError("シリアライズフィールド設定してくれメンス");
                    return;
                }

                _calorieValue.text = $"{value.FoodData.Calorie:F2} kcal";
                _recipeName.text = value.FoodData.RecipeName;
                _areaValueText.text = $"{value.RectAreaValue * value.FoodData.PercentageOfMealArea * 10000f:F2} cm\u00B2";

                transform.position = value.CenterWorldPosition;
                if (_lineRenderer == null)
                {
                    _lineRenderer = gameObject.GetComponent<LineRenderer>();
                }

                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPosition(0, anchor0.position);
                _lineRenderer.SetPosition(1, anchor1.position);
                _lineRenderer.SetPosition(2, _worldSpaceFoodData.CenterWorldPosition);
                _targetPosition = value.CenterWorldPosition + new Vector3(0, 0.08f, 0);

                StartCoroutine(Animation());
            }
        }

        void Awake()
        {
            TargetScale = Vector3.one;
            _foodPanelLayer = 1 << 8;
            _hiddenFlags = new bool[5];
            _boxCollider = gameObject.GetComponent<BoxCollider>();
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        public void Initialize()
        {
            _startAnimation = false;
            TargetScale = new Vector3(0.5f, 0.5f, 0.5f);

            transform.localScale = new Vector3(0f, 1f, 1f);
            _lineRenderer.positionCount = 0;
            _frequency = 1f / _period;
        }

        void Update()
        {
            if (_startAnimation)
            {
                //被っているか判定する。
                var cameraPosition = CameraCache.Main.transform.position;

                var direction0 = new Vector3(cameraPosition.x, _vertexes[0].position.y, cameraPosition.z) - _vertexes[0].position;
                _hiddenFlags[0] = Physics.Raycast(_vertexes[0].position, direction0, 3, _foodPanelLayer);

                var direction1 = new Vector3(cameraPosition.x, _vertexes[1].position.y, cameraPosition.z) - _vertexes[1].position;
                _hiddenFlags[1] = Physics.Raycast(_vertexes[1].position, direction1, 3, _foodPanelLayer);

                var direction2 = new Vector3(cameraPosition.x, _vertexes[2].position.y, cameraPosition.z) - _vertexes[2].position;
                _hiddenFlags[2] = Physics.Raycast(_vertexes[2].position, direction2, 3, _foodPanelLayer);

                var direction0down = direction0 - new Vector3(0, 0.1f, 0);
                _hiddenFlags[3] = Physics.Raycast(_vertexes[0].position, direction0down, 3, _foodPanelLayer);

                var direction1down = direction1 - new Vector3(0, 0.1f, 0);
                _hiddenFlags[4] = Physics.Raycast(_vertexes[1].position, direction1down, 3, _foodPanelLayer);

                Debug.DrawRay(_vertexes[0].position, direction0);
                Debug.DrawRay(_vertexes[1].position, direction1);
                Debug.DrawRay(_vertexes[0].position, direction0down);
                Debug.DrawRay(_vertexes[1].position, direction1down);
                Debug.DrawRay(_vertexes[2].position, direction2);

                if (_hiddenFlags.Count(x => x) >= 2)
                {
                    _targetPosition += new Vector3(0, 0.01f, 0);
                }

                //通常
                var cameraTransform = CameraCache.Main.transform;
                var right = cameraTransform.right - new Vector3(0, cameraTransform.right.y, 0);
                right = right.normalized;

                var diff = 0.01f * Mathf.Sin(2 * Mathf.PI * _frequency * (Time.time - _startTime));
                gameObject.transform.position = _targetPosition + right * 0.11f + new Vector3(0, diff, 0);

                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPosition(0, anchor0.position);
                _lineRenderer.SetPosition(1, anchor1.position);
                _lineRenderer.SetPosition(2, _worldSpaceFoodData.CenterWorldPosition);
            }
        }

        /// <summary>
        /// 3秒で表示、そのあと適当にsinでふわふわ動く感じに。
        /// </summary>
        /// <returns></returns>
        IEnumerator Animation()
        {
            var startTime = Time.time;
            var firstScale = new Vector3(0f, 1f, 1f);

            var cameraTransform = CameraCache.Main.transform;
            for (; Time.time < startTime + _animationTimeSpan;)
            {
                var timeSpan = Time.time - startTime;
                var right = cameraTransform.right - new Vector3(0, cameraTransform.right.y, 0);
                right = right.normalized;

                gameObject.transform.position = Vector3.Lerp(_worldSpaceFoodData.CenterWorldPosition, _targetPosition + right * 0.05f, timeSpan / _animationTimeSpan);
                gameObject.transform.localScale = Vector3.Lerp(firstScale, TargetScale, timeSpan / _animationTimeSpan);

                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPosition(0, anchor0.position);
                _lineRenderer.SetPosition(1, anchor1.position);
                _lineRenderer.SetPosition(2, _worldSpaceFoodData.CenterWorldPosition);
                yield return null;
            }

            this._startTime = Time.time;
            this._startAnimation = true;
        }
    }
}
