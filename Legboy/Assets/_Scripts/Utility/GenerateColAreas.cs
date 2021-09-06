using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateColAreas : MonoBehaviour
{
    private TilemapCollider2D myCol;

    private void Awake()
    {
        myCol = GetComponent<TilemapCollider2D>();
        myCol.composite.enabled = false;
        myCol.enabled = false;
    }

    public void GenerateColliders()
    {
        myCol = GetComponent<TilemapCollider2D>();
        CreateCollidersObjects();
    }
    
    //creates a game object with collider for each path (polygon) on composite collider 
    private void CreateCollidersObjects()
    {
        DeleteCollidersObjects();
        
        var compositeCol = myCol.composite;
            
        for (int i = 0; i < compositeCol.pathCount; i++)
        {
            Vector2[] pathVerts = new Vector2[compositeCol.GetPathPointCount(i)];
                
            compositeCol.GetPath(i, pathVerts);

            GameObject newObj = new GameObject("LevelCollision_"+i);
            newObj.transform.SetParent(this.transform);
            newObj.layer = 11;
            
            PolygonCollider2D col = newObj.AddComponent<PolygonCollider2D>();
            
            col.SetPath(0, pathVerts);
            col.isTrigger = true;
            
            newObj.transform.localPosition = Vector2.zero;
        }
    }

    private void DeleteCollidersObjects()
    {
        int children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

}
