using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CalorieCaptorGlass
{
    /// <summary>
    /// 物体検出のインターフェース。
    /// 最終的にはデバッグ用の実装とhttpの実装,ONNXの実装を分ける。
    /// </summary>
    public interface IObjectDetector<T> : IDisposable
    {
        /// <summary>
        /// 物体検出中かどうか
        /// </summary>
        bool Calclating { get; }

        Task<List<T>> DetectObject(Texture2D cameraPhotoImage);
        Task<List<T>> DetectObject(List<byte> cameraPhotoRawImg, int imageHeight, int imageWidth);
    }
}