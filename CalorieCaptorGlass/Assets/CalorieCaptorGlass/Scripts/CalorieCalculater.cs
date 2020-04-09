using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace CalorieCaptorGlass
{

    public static class CalorieCalculater
    {
        private static readonly float[][] _calorieDb;

        /// <summary>
        /// staticコンストラクタを事前に呼ぶためだけのダミー
        /// </summary>
        public static void Initialize()
        {
        }

        static CalorieCalculater()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, "CalorieDB.json");
            var jsonContent = File.ReadAllText(filePath);
            _calorieDb = JsonConvert.DeserializeObject<float[][]>(jsonContent);
        }

        public static float CalculateCalorie(int classLabel, float rectArea, float foodAreaPercentage)
        {
            float[] row = _calorieDb[classLabel - 1];
            //面積はm^2なので、カロリーのがcm^2基準だと10000倍しないといけない
            float foodArea = foodAreaPercentage * rectArea * 10000;
            return row[0] * foodArea * foodArea + row[1] * foodArea + row[2];
        }
    }
}
