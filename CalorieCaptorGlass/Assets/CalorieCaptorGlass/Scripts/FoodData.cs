using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalorieCaptorGlass
{
    /// <summary>
    /// 食事の画像から取得するレシピ、カロリー、画像上のX,画像上のYのデータ。
    /// 画像上の値は左上原点、右向きにx,下向きにyでとる
    /// こいつのListをjsonにする。
    /// </summary>
    public class FoodData
    {
        [JsonProperty("class_name")]
        public string RecipeName { get; set; }

        [JsonIgnore]
        public float Calorie { get; set; }

        [JsonProperty("class")]
        public int Class { get; set; }

        [JsonProperty("left")]
        public float Left { get; set; }

        [JsonProperty("right")]
        public float Right { get; set; }

        [JsonProperty("top")]
        public float Top { get; set; }

        [JsonProperty("bottom")]
        public float Bottom { get; set; }

        [JsonProperty("percentage_of_meal_area")]
        public float PercentageOfMealArea { get; set; }

        /// <summary>
        /// 画像上のX
        /// </summary>
        [JsonIgnore]
        public float CenterX { get; set; }

        /// <summary>
        /// 画像上のY
        /// </summary>
        [JsonIgnore]
        public float CenterY { get; set; }

        public static List<FoodData> FromJson(string json) => JsonConvert.DeserializeObject<List<FoodData>>(json);

        public static List<FoodData> CalculateCenterPosition(List<FoodData> foodDatas)
        {
            foreach (var foodData in foodDatas)
            {
                foodData.CenterX = (foodData.Left + foodData.Right) / 2f;
                foodData.CenterY = (foodData.Top + foodData.Bottom) / 2f;
            }

            return foodDatas;
        }
    }
}