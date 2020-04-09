using UnityEngine;

namespace CalorieCaptorGlass
{
    public class WorldSpaceFoodData
    {
        public Vector3 CenterWorldPosition { get; set; }
        public FoodData FoodData { get; set; }

        public Vector3 TopLeftWorldPosition { get; set; }
        public Vector3 TopRightWorldPosition { get; set; }
        public Vector3 BottomLeftWorldPosition { get; set; }
        public Vector3 BottomRightWorldPosition { get; set; }

        public float RectAreaValue => AreaCalculator.Calculate(TopLeftWorldPosition, TopRightWorldPosition, BottomLeftWorldPosition) +
                                      AreaCalculator.Calculate(BottomRightWorldPosition, TopRightWorldPosition, BottomLeftWorldPosition);

        public float Calorie => FoodData.Calorie;

        public WorldSpaceFoodData(FoodData foodData, Vector3 centerWorldPos,
            Vector3 topLeftWorldPosition,
            Vector3 topRightWorldPosition,
            Vector3 bottomLeftWorldPosition,
            Vector3 bottomRightWorldPosition)
        {
            this.CenterWorldPosition = centerWorldPos;
            this.FoodData = foodData;
            this.TopLeftWorldPosition = topLeftWorldPosition;
            this.TopRightWorldPosition = topRightWorldPosition;
            this.BottomLeftWorldPosition = bottomLeftWorldPosition;
            this.BottomRightWorldPosition = bottomRightWorldPosition;
            this.FoodData.Calorie =
                CalorieCalculater.CalculateCalorie(foodData.Class, RectAreaValue, foodData.PercentageOfMealArea);
        }
    }
}