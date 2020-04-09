using System.Collections.Generic;
using CalorieCaptorGlass;
using Newtonsoft.Json;
using UnityEngine;
/// <summary>
/// テストユニット
/// </summary>
public class DebugUnit : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField]
    private GameObject _propertyView;

    [SerializeField] private FoodDataViewManager _manager;
    // Use this for initialization
    void Start()
    {
        JsonSerializeTest();
        //PropertyViewDebug();
        //PropertyViewDebugVer2();
        ManagerTest();
    }


    void JsonSerializeTest()
    {
        var food1 = new FoodData();
        food1.CenterX = 100;
        food1.CenterY = 200;
        food1.RecipeName = "carry";
        food1.Calorie = 1820f;

        var food2 = new FoodData();
        food2.CenterX = 150;
        food2.CenterY = 444;
        food2.RecipeName = "gokiburi";

        var list = new List<FoodData>();
        list.Add(food1);
        list.Add(food2);

        string json = JsonConvert.SerializeObject(list, Formatting.Indented);
        Debug.Log(json);
    }


    void PropertyViewDebug()
    {
        var obj = Instantiate(_propertyView);
        List<FoodData> jsonObj = FoodData.FromJson(@"[{""class"": 41, ""class_name"": ""jiaozi"", ""left"": 310, ""right"": 474, ""top"": 300, ""bottom"": 148, ""calorie"": 66.14926114040404}]");
        jsonObj = FoodData.CalculateCenterPosition(jsonObj);

        var testFoodData = jsonObj[0];
        var topLeft = new Vector3(0, 0, 0.2f);
        var topRight = new Vector3(0.2f, 0, 0.2f);
        var bottomLeft = new Vector3(0, 0, 0);
        var bottomRight = new Vector3(0.2f, 0, 0);
        var center = new Vector3(0.1f, 0, 0.1f);
        var worldSpaceFoodData = new WorldSpaceFoodData(testFoodData, center, topLeft, topRight, bottomLeft, bottomRight);

        var foodDataPanel = obj.GetComponent<FoodDataPanel>();
        foodDataPanel.WorldSpaceFoodData = worldSpaceFoodData;
    }

    void PropertyViewDebugVer2()
    {
        var obj = Instantiate(_propertyView);
        List<FoodData> jsonObj = FoodData.FromJson(@"[{""class"": 41, ""class_name"": ""jiaozi"", ""left"": 310, ""right"": 474, ""top"": 300, ""bottom"": 148, ""calorie"": 66.14926114040404}]");
        jsonObj = FoodData.CalculateCenterPosition(jsonObj);

        var testFoodData = jsonObj[0];
        var topLeft = new Vector3(0, 0, 0.4f);
        var topRight = new Vector3(0.4f, 0, 0.4f);
        var bottomLeft = new Vector3(0, 0, 0);
        var bottomRight = new Vector3(0.4f, 0, 0);
        var center = new Vector3(0.2f, 0, 0.2f);
        var worldSpaceFoodData = new WorldSpaceFoodData(testFoodData, center, topLeft, topRight, bottomLeft, bottomRight);

        var foodDataPanel = obj.GetComponent<FoodDataPanel>();
        foodDataPanel.WorldSpaceFoodData = worldSpaceFoodData;
    }

    void ManagerTest()
    {
        var obj = Instantiate(_propertyView);
        List<FoodData> jsonObj = FoodData.FromJson(@"[{""class"": 41, ""class_name"": ""jiaozi"", ""left"": 310, ""right"": 474, ""top"": 300, ""bottom"": 148, ""calorie"": 66.14926114040404}]");
        jsonObj = FoodData.CalculateCenterPosition(jsonObj);

        var testFoodData = jsonObj[0];
        var topLeft = new Vector3(0, 0, 0.4f);
        var topRight = new Vector3(0.4f, 0, 0.4f);
        var bottomLeft = new Vector3(0, 0, 0);
        var bottomRight = new Vector3(0.4f, 0, 0);
        var center = new Vector3(0.2f, 0, 0.2f);
        var worldSpaceFoodData = new WorldSpaceFoodData(testFoodData, center, topLeft, topRight, bottomLeft, bottomRight);

        _manager.ReDrawFoodData(new List<WorldSpaceFoodData>{worldSpaceFoodData});
    }
#endif
}
