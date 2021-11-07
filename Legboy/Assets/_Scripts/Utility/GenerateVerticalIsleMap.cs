using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[RequireComponent(typeof(TilemapRenderer))]
public class GenerateVerticalIsleMap : MonoBehaviour
{
    [SerializeField] private string path = "/_Art/Sprites/Effects/";
    [SerializeField] private float gradientVariation = 0.5f;
    
    public void GenerateVertIsleMap()
    {
        var tilemap = GetComponent<Tilemap>();

        var tiles = GetTilesGradient(tilemap);
        //gets a squares gradient mask without any lerp between tiles

        var xPixelSize = tilemap.size.x * 16;
        var yPixelSize = tilemap.size.y * 16;
        //16 is the amount of pixels per unit

        var texture = new Texture2D(xPixelSize, yPixelSize, TextureFormat.RGB24, false);
        //creates a new texture

        var xSize = tiles.Count * 16;
        var ySize = tiles[0].Count * 16;
        
        var upperCellColor = 0f;
        
        for (var x = 0; x < xSize; x++)
        {
            //y starts at the top because the GetTilesGradient function returns a matrix inverted on the y axis
            for (var y = ySize-1; y >= 0; y--)
            {
                if ((y / 16) - 1 >= 0) upperCellColor = tiles[x / 16][(y / 16) - 1];
                //gets the upper square's color, except when the current one is the highest
                
                var currCellColor = tiles[x / 16][y / 16];
                //gets current cell color
                
                float pixelColor = currCellColor > (gradientVariation*0.9f) ? Mathf.Lerp(currCellColor, upperCellColor, ((ySize-1-y)%16)/16f) : pixelColor = 0f;
                //lerps between the upper square's color and the current one to create a smooth gradient
                
                texture.SetPixel(x,ySize-1-y, new Color(pixelColor, pixelColor, pixelColor, 1));
            }
        }

        SaveTexture(texture);
        
        SetMatValues("VerticalIsleMap_" + gameObject.scene.name + ".png", new Vector2(tilemap.size.x, tilemap.size.y));
    }
    
    private List<List<float>> GetTilesGradient(Tilemap tilemap)
    {
        var tiles = new List<List<float>>();
        
        //x goes from the first tile's location to the last one's location
        for (var x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
        {
            var tileColumn = new List<float>();//initializes a column list at current x position's tile
            
            //y goes from the last tile's location to the first one's location
            for (var y = (tilemap.origin.y + tilemap.size.y); y > tilemap.origin.y; y--)
            {
                //if current tile is filled, sets a positive value other than 0f
                var tileColor = tilemap.HasTile(new Vector3Int(x, y, 0)) ? gradientVariation : 0f;
                //if current tile is filled and not in the highest line adds the upper tile's value to the current one's value
                if(tileColor != 0) tileColor += (y < (tilemap.origin.y + tilemap.size.y)) ? tileColumn[(tilemap.origin.y + tilemap.size.y) - y - 1] : 0f;

                tileColumn.Add(tileColor);
            }
            tiles.Add(tileColumn);
        }
        return tiles;
    }

    private void SaveTexture(Texture2D texture)
    {
        var bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + path;
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "VerticalIsleMap_" + gameObject.scene.name + ".png", bytes);
        print("Vertical isle map generated.");
    }

    //updates the renderer's material's values like mask texture and mask scale
    private void SetMatValues(string fileName, Vector2 maskScale)
    {
        var mat = GetComponent<Renderer>().sharedMaterial;
        
        mat.SetVector("_MaskScale", maskScale);
        
        var tex = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets" + path + fileName, typeof(Texture2D));
        if (tex == null)
        {
            print("Error when assigning mask on material. File " + fileName + " not found at Assets" + path + ".");
            return;
        }
        mat.SetTexture("_MaskTex", tex);
    }
}
