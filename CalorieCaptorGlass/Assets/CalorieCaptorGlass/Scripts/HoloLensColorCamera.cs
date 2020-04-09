using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;

namespace CalorieCaptorGlass
{
    public class HoloLensColorCamera : ICamera
    {

        private PhotoCapture _photoCaptureObj;
        private CameraParameters _cameraParameters;
        private Action<Matrix4x4, Matrix4x4, Texture2D> _takePhotoActionCopyToTexture2D;
        private Action<Matrix4x4, Matrix4x4, List<byte>, int, int> _takePhotoActionCopyToBytes;

        public bool CanTakePhoto { get; private set; } = false;
        private bool _initialized = false;

        public HoloLensColorCamera()
        {
            Init();
            _initialized = true;
        }

        public void Init()
        {
            if (!_initialized)
            {
                //1280 * 720
                Resolution selectedResolution = PhotoCapture.SupportedResolutions.First();

                _cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
                {
                    cameraResolutionWidth = selectedResolution.width,
                    cameraResolutionHeight = selectedResolution.height,
                    hologramOpacity = 0.0f,
                    pixelFormat = CapturePixelFormat.BGRA32
                };

                PhotoCapture.CreateAsync(false, OnCreatedPhotoCaptureObject);
            }
        }

        void OnCreatedPhotoCaptureObject(PhotoCapture captureObject)
        {
            _photoCaptureObj = captureObject;
            _photoCaptureObj.StartPhotoModeAsync(_cameraParameters, _ => { });
        }

        void OnPhotoCapturedCopyToTexture(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            Matrix4x4 cameraToWorldMatrix;
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);

            Matrix4x4 projectionMatrix;
            photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            var texture = new Texture2D(_cameraParameters.cameraResolutionWidth, _cameraParameters.cameraResolutionHeight, TextureFormat.RGBA32, false);
            photoCaptureFrame.UploadImageDataToTexture(texture);
            texture.wrapMode = TextureWrapMode.Clamp;

            photoCaptureFrame.Dispose();

            _takePhotoActionCopyToTexture2D?.Invoke(cameraToWorldMatrix, projectionMatrix, texture);
            CanTakePhoto = false;
            _takePhotoActionCopyToTexture2D = null;
        }

        void OnPhotoCapturedCopyToBytes(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            Matrix4x4 cameraToWorldMatrix;
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);

            Matrix4x4 projectionMatrix;
            photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            List<byte> capturedImg = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(capturedImg);

            photoCaptureFrame.Dispose();

            _takePhotoActionCopyToBytes?.Invoke(cameraToWorldMatrix, projectionMatrix, capturedImg, _cameraParameters.cameraResolutionHeight, _cameraParameters.cameraResolutionWidth);
            CanTakePhoto = false;
            _takePhotoActionCopyToBytes = null;
        }

        public bool TakePhoto(Action<Matrix4x4, Matrix4x4, Texture2D> photoAction)
        {
            if (CanTakePhoto)
            {
                return false;
            }
            CanTakePhoto = true;
            _takePhotoActionCopyToTexture2D = photoAction;
            _photoCaptureObj.TakePhotoAsync(OnPhotoCapturedCopyToTexture);
            return true;
        }

        /// <summary>
        /// Actionには
        /// cameraToWorldMatrix,projectionMatrix,imgRawData,imgHeight,imgWidth
        /// の順でいろいろ振ってくる。
        /// </summary>
        /// <param name="photoAction"></param>
        /// <returns></returns>
        public bool TakePhoto(Action<Matrix4x4, Matrix4x4, List<byte>, int, int> photoAction)
        {
            if (CanTakePhoto)
            {
                return false;
            }
            CanTakePhoto = true;
            _takePhotoActionCopyToBytes = photoAction;
            _photoCaptureObj.TakePhotoAsync(OnPhotoCapturedCopyToBytes);
            return true;
        }

        public void Dispose()
        {
            _photoCaptureObj.Dispose();
        }
    }
}

