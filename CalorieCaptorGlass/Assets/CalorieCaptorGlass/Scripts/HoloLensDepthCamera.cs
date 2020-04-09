using System;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Capture.Frames;
using System.Threading.Tasks;
#endif

namespace CalorieCaptorGlass
{
    public class HoloLensDepthCamera : ICamera
    {
        private readonly bool _streaming;
#if WINDOWS_UWP
        private MediaCapture _mediaCapture;
        private MediaFrameReader _depthReader;
#endif

        public HoloLensDepthCamera(bool streaming = false)
        {
            _streaming = streaming;
            Init();
        }

        public async void Init()
        {
#if WINDOWS_UWP
            await CleanupMediaCaptureAsync();
            await InitMediaSourceAsync();
#endif
        }


#if WINDOWS_UWP
        private async Task InitMediaSourceAsync()
        {
            var allGroups = await MediaFrameSourceGroup.FindAllAsync();

            if (allGroups.Count == 0)
            {
                Debug.LogError("cannot found MediaFrameSourceGroup. アプリケーションマニュフェストを確認してください。");
                return;
            }

            MediaFrameSourceGroup sourceGroup =
                allGroups.FirstOrDefault(g => g.SourceInfos.Any(s => s.SourceKind == MediaFrameSourceKind.Depth));

            if (sourceGroup == null)
            {
                Debug.LogError("深度カメラが見つからないようです。");
                return;
            }

            try
            {
                await InitializeMediaCaptureAsync(sourceGroup);
            }
            catch (Exception exception)
            {
                Debug.LogError("InitializeMediaCaptureAsyncに失敗しました" + exception.Message);
                await CleanupMediaCaptureAsync();
                return;
            }

            MediaFrameSource source = _mediaCapture.FrameSources.Values
                .FirstOrDefault(s => s.Info.SourceKind == MediaFrameSourceKind.Depth);

            if (source == null)
            {
                Debug.LogError("sourceが見つかりません。");
            }

            MediaFrameFormat format = source.SupportedFormats.FirstOrDefault(f =>
                String.Equals(f.Subtype, MediaEncodingSubtypes.D16, StringComparison.OrdinalIgnoreCase));

            if (format == null)
            {
                return;
            }

            await source.SetFormatAsync(format);

            _depthReader = await _mediaCapture.CreateFrameReaderAsync(source, format.Subtype);

            MediaFrameReaderStartStatus status = await _depthReader.StartAsync();

            if (status != MediaFrameReaderStartStatus.Success)
            {
                Debug.LogError("_depthReader.StartAsyncに失敗しました");
            }
        }

        private async Task InitializeMediaCaptureAsync(MediaFrameSourceGroup sourceGroup)
        {
            if (_mediaCapture != null)
            {
                return;
            }

            // Initialize mediacapture with the source group.
            _mediaCapture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings
            {
                SourceGroup = sourceGroup,

                // This media capture can share streaming with other apps.
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,

                // Only stream video and don't initialize audio capture devices.
                StreamingCaptureMode = StreamingCaptureMode.Video,

                // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
                // instead of preferring GPU D3DSurface images.
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };

            await _mediaCapture.InitializeAsync(settings);
        }

#endif

        /// <summary>
        /// 深度画像ではこの関数は使えない。
        /// </summary>
        /// <param name="photoAction"></param>
        /// <returns></returns>
        public bool TakePhoto(Action<Matrix4x4, Matrix4x4, Texture2D> photoAction)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoAction"></param>
        /// <returns></returns>
        public bool TakePhoto(Action<Matrix4x4, Matrix4x4, List<byte>, int, int> photoAction)
        {
#if WINDOWS_UWP
            using (MediaFrameReference frame = _depthReader.TryAcquireLatestFrame())
            {
                if (frame == null)
                {
                    return false;
                }

                var videoFrame = frame.VideoMediaFrame;
                using (var bitmap = videoFrame.SoftwareBitmap)
                {
                    if (bitmap == null)
                    {
                        return false;
                    }

                    int width = bitmap.PixelWidth;
                    int height = bitmap.PixelHeight;

                    var bytes = new byte[width * height * 2];

                    bitmap.CopyToBuffer(bytes.AsBuffer());
                    photoAction?.Invoke(Matrix4x4.zero, Matrix4x4.zero, bytes.ToList(), height, width);
                    return true;
                }
            }
#else
            return  false;
#endif
        }
#if WINDOWS_UWP
        /// <summary>
        /// Unregisters FrameArrived event handlers, stops and disposes frame readers
        /// and disposes the MediaCapture object.
        /// </summary>
        private async Task CleanupMediaCaptureAsync()
        {
            if (_mediaCapture != null)
            {
                using (var mediaCapture = _mediaCapture)
                {
                    _mediaCapture = null;

                    if (_depthReader != null)
                    {
                        if (_streaming)
                        {
                            _depthReader.FrameArrived -= FrameReader_FrameArrived;
                        }
                        await _depthReader.StopAsync();
                        _depthReader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// あとで実装するかも。
        /// </summary>
        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            using (var frame = sender.TryAcquireLatestFrame())
            {
                if (frame != null)
                {

                }
            }
        }
#endif

        public async void Dispose()
        {
#if WINDOWS_UWP
            await CleanupMediaCaptureAsync();
#endif
        }
    }
}
