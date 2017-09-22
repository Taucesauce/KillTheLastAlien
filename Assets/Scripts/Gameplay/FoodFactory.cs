using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FoodCount {
    Small = 25,
    Medium = 32,
    Large = 43
}
enum FoodType {
    GreenMochi,
    OrangeMochi,
    PinkMochi,
    GreenRib,
    OrangeRib,
    PinkRib
}

public class FoodFactory : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer backgroundImage;
    [SerializeField]
    private List<GameObject> prefabList;

    private List<GameObject> foodList = new List<GameObject>();
    public List<GameObject> FoodList { get { return foodList; } }

    //Singleton pattern
    //Private manager instance
    private static FoodFactory foodFactory;

    public static FoodFactory Instance {
        get {
            if (!foodFactory) {
                foodFactory = FindObjectOfType(typeof(FoodFactory)) as FoodFactory;

                if (!foodFactory) {
                    Debug.Log("No Game Manager object found in scene, fix that!");
                }
                else {
                    foodFactory.Init();
                }
            }

            return foodFactory;
        }
    }

    //Accessible position data for Player object
    void Init() {
        
    }

    private void Start() {
        
    }

    public void SpawnFood(int numPlayers) {
        switch (numPlayers) {
            case 2:
                FillList(FoodCount.Small);
                break;
            case 3:
                FillList(FoodCount.Medium);
                break;
            case 4:
                FillList(FoodCount.Large);
                break;
        }
    }

    private void FillList(FoodCount amount) {
        float backgroundWidth = backgroundImage.sprite.bounds.size.x;
        float backgroundHeight = backgroundImage.sprite.bounds.size.y;

        for(int i = 0; i < (int)amount; i++) {
            Vector2 temp = new Vector2(UnityEngine.Random.Range(-backgroundWidth,backgroundWidth), UnityEngine.Random.Range(-backgroundHeight,backgroundHeight));
            foodList.Add(Instantiate(prefabList[UnityEngine.Random.Range(0, 6)],temp,Quaternion.identity)); 
        }
    }

    public Vector2 nearestMochi(string v, Vector3 position) {
        return Vector2.zero;
    }
}
