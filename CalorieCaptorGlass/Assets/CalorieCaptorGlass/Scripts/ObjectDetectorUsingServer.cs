using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using System.IO;
using System;
using Newtonsoft.Json;
using Windows.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace CalorieCaptorGlass
{
    /// <summary>
    /// サーバーに送信
    /// </summary>
    public class ObjectDetectorUsingServer : IObjectDetector<FoodData>
    {
        public bool Calclating { get; } = false;

        private string _serverURL = "";

#if WINDOWS_UWP
        private static HttpClient _client;
#endif

        static ObjectDetectorUsingServer()
        {
#if WINDOWS_UWP
            _client = new HttpClient();
#endif
        }

        public ObjectDetectorUsingServer()
        {
#if WINDOWS_UWP

            Task.Run(async () =>
            {
                using (var stream = await KnownFolders.Objects3D.OpenStreamForReadAsync("DeepCarorieLensURL.txt"))
                {
                    var strRaw = new byte[stream.Length];
                    await stream.ReadAsync(strRaw, 0, strRaw.Length);
                    this._serverURL = System.Text.Encoding.UTF8.GetString(strRaw);
                }
            });
#endif
        }

        public async Task<List<FoodData>> DetectObject(Texture2D cameraPhotoImage)
        {
#if WINDOWS_UWP
            string endpoint = _serverURL;
            var bytes = cameraPhotoImage.EncodeToPNG();
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await _client.PostAsync(endpoint, content);

            string jsonData = await response.Content.ReadAsStringAsync();
            List<FoodData> deserializeData =
                await Task.Run(() => JsonConvert.DeserializeObject<List<FoodData>>(jsonData));

            return deserializeData;
#else
            return null;
#endif
        }

        public void Dispose()
        {
#if WINDOWS_UWP
            _client?.Dispose();
#endif
        }

        public async Task<List<FoodData>> DetectObject(List<byte> cameraPhotoRawImg, int imageHeight, int imageWidth)
        {
#if WINDOWS_UWP
            try
            {
                string endpoint = _serverURL;


                List<FoodData> findingFoodList = await Task.Run(async () =>
                {
                    var content = new ByteArrayContent(cameraPhotoRawImg.ToArray());
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Headers.Add("width", imageWidth.ToString());
                    content.Headers.Add("height", imageHeight.ToString());
                    HttpResponseMessage response = await _client.PostAsync(endpoint, content);

                    string jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<FoodData>>(jsonString);
                });

                return findingFoodList;
            }
            catch (Exception e)
            {
                Debug.LogError("ObjectDetecteでエラー発生" + e.Message);
                return new List<FoodData>();
            }
#else
            return null;
#endif
        }
    }
}

