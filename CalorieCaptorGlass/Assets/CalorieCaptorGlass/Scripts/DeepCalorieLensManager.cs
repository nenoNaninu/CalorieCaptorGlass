using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CalorieCaptorGlass
{

    /// <summary>
    /// 今回のアプリケーションの一番でかいManagerクラス。
    /// </summary>
    [RequireComponent(typeof(FoodDataViewManager))]
    public class DeepCalorieLensManager : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// falseだとAir-tapしたタイミングで撮影。
        /// trueだと(いまの実装だと)5秒おきに撮影。
        /// </summary>
        public bool RealTimeDetection = false;

        public int TimeIntervalOfTakePhoto = 5;

        [SerializeField] private GameObject _handPlotObj;

        private ICamera _colorCameraObject;
        private ICamera _depthCameraObject;
        private IObjectDetector<FoodData> _objectDetector;

        private FoodDataViewManager _foodDataViewManager;


        private bool _canTakePhoto = true;

        // Use this for initialization
        void Start()
        {
            _colorCameraObject = InjectColorCamera();
            //_depthCameraObject = InjectDepthCamera();
            _objectDetector = ObjectDetectorInject();
            _foodDataViewManager = gameObject.GetComponent<FoodDataViewManager>();

            if (RealTimeDetection)
            {
                StartCoroutine(TakePhotoCoroutine());
            }
            else
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }

            CalorieCalculater.Initialize();
        }

        private void FindHandPositionOnImage(Matrix4x4 camera2WorkdMatrix, Matrix4x4 projectionMatrix, int height, int width)
        {
            int x = 0, y = 0;
            if (CoordinateTransfer.WorldPos2ImagePos(_handPlotObj.transform.position, projectionMatrix,
                camera2WorkdMatrix, height, width, ref x, ref y))
            {
                Debug.Log($"handPosition on image : {x}, {y}");
            }
        }

        /// <summary>
        /// 5秒ごとに発火される
        /// 5秒ごとにカロリーと食事の位置を認識するので、その情報を表示する。
        /// 表示するとき、前のフレームに残っている連中と比較して、距離が5cm以内なら同じ食事と判断して、食事の表示を書き換える。
        /// 前のフレームに残っている
        /// </summary>
        public void TakePhoto()
        {
            Vector3 cameraPos = CameraCache.Main.transform.position;
            _colorCameraObject?.TakePhoto(async (camera2WorkdMatrix, projectionMatrix, imageRawdata, height, width) =>
            {
                //ここでカメラから実際のオブジェクトの対応を取る。
                List<FoodData> foodDataList = await _objectDetector.DetectObject(imageRawdata, height, width);
                foodDataList = FoodData.CalculateCenterPosition(foodDataList);

                var currentWorldSpaceFoodData = new List<WorldSpaceFoodData>(4);

                //FindHandPositionOnImage(camera2WorkdMatrix, projectionMatrix, height,width);

                //今画面に映っている食事の位置が取得できたやつらを保存。
                foreach (var foodData in foodDataList)
                {
                    //var outはUnityがコンパイルしてくれないので。世知辛い。
                    Vector3 foodCenterPosOnWorldCordinate;

                    //中心に対応があったら端の4点も対応を取って
                    if (CoordinateTransfer.ImagePos2WorldPos(foodData.CenterX, foodData.CenterY, height, width, projectionMatrix,
                        camera2WorkdMatrix, cameraPos, out foodCenterPosOnWorldCordinate))
                    {
                        Vector3 topLeft, topRight, bottomLeft, bottomRight;

                        CoordinateTransfer.ImagePos2WorldPos(foodData.Left, foodData.Top, height, width, projectionMatrix,
                            camera2WorkdMatrix, cameraPos, out topLeft);
                        CoordinateTransfer.ImagePos2WorldPos(foodData.Right, foodData.Top, height, width, projectionMatrix,
                            camera2WorkdMatrix, cameraPos, out topRight);
                        CoordinateTransfer.ImagePos2WorldPos(foodData.Left, foodData.Bottom, height, width, projectionMatrix,
                            camera2WorkdMatrix, cameraPos, out bottomLeft);
                        CoordinateTransfer.ImagePos2WorldPos(foodData.Right, foodData.Bottom, height, width, projectionMatrix,
                            camera2WorkdMatrix, cameraPos, out bottomRight);

                        currentWorldSpaceFoodData.Add(new WorldSpaceFoodData(foodData, foodCenterPosOnWorldCordinate, topLeft, topRight, bottomLeft, bottomRight));
                    }
                }
                _foodDataViewManager.ReDrawFoodData(currentWorldSpaceFoodData);
            });

            //_depthCameraObject?.TakePhoto(async (camera2WorkdMatrix, projectionMatrix, imageRawdata, height, width) =>
            //{
            //    await _objectDetector.DetectObject(imageRawdata, height, width);
            //});
        }

        /// <summary>
        /// デバッグの時と実機の時でカメラのオブジェクトを切り替える。
        /// </summary>
        /// <returns></returns>
        ICamera InjectColorCamera()
        {
#if WINDOWS_UWP
            return new HoloLensColorCamera();
#else
            return null;
#endif
        }

        ICamera InjectDepthCamera()
        {
#if WINDOWS_UWP
            return new HoloLensDepthCamera();
#else
            return null;
#endif
        }

        IObjectDetector<FoodData> ObjectDetectorInject()
        {
#if WINDOWS_UWP
            return new ObjectDetectorUsingServer();
#else
            return null;
#endif
        }

        IEnumerator TakePhotoCoroutine()
        {
            var wait10Sec = new WaitForSeconds(10);
            var wait5Sec = new WaitForSeconds(TimeIntervalOfTakePhoto);
            yield return wait10Sec;

            while (_canTakePhoto)
            {
                TakePhoto();
                yield return wait5Sec;
            }
        }

        void OnApplicationQuit()
        {
            _objectDetector?.Dispose();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            TakePhoto();
        }
    }
}