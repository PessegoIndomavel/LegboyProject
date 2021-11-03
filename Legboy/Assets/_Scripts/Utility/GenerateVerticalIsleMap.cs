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
    
    private int maxHeight = 0;
    public void GenerateVertIsleMap()
    {
        var tilemap = GetComponent<Tilemap>();

        var tiles = GetTiles(tilemap);

        var pixelSize = tilemap.size.x * 16;

        var texture = new Texture2D(pixelSize, pixelSize, TextureFormat.RGB24, false);

        var xSize = tiles.Count * 16;
        var ySize = tiles[0].Count * 16;
        var y = ySize - 1;
        var x = 0;
        var upperCellColor = 0f;
        
        for (; x < xSize; x++)
        {
            for (y = ySize-1; y >= 0; y--)
            {
                if((y/16)-1 >= 0) upperCellColor = Mathf.InverseLerp(0, maxHeight, tiles[x / 16][(y/ 16)-1]);

                var currCellColor = Mathf.InverseLerp(0, maxHeight, tiles[x / 16][y / 16]);
                
                float pixelColor = currCellColor > 0 ? Mathf.Lerp(currCellColor, upperCellColor, ((ySize-1-y)%16)/16f) : pixelColor = 0;
                
                texture.SetPixel(x,ySize-1-y, new Color(pixelColor, pixelColor, pixelColor, 1));
            }
        }

        SaveTexture(texture);
        SetMatMask("VerticalIsleMap_" + gameObject.scene.handle + ".png");
    }
    
    private List<List<int>> GetTiles(Tilemap tilemap)
    {
        var tiles = new List<List<int>>();
        
        for (var x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
        {
            var tileX = new List<int>();
            for (var y = (tilemap.origin.y + tilemap.size.y); y > tilemap.origin.y; y--)
            {
                var temp = tilemap.HasTile(new Vector3Int(x, y, 0)) ? 1 : 0;
                if(temp != 0) temp += (y < (tilemap.origin.y + tilemap.size.y)) ? tileX[(tilemap.origin.y + tilemap.size.y) - y - 1] : 0;

                tileX.Add(temp);
                
                if (temp > maxHeight) maxHeight = temp;
            }
            tiles.Add(tileX);
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
        File.WriteAllBytes(dirPath + "VerticalIsleMap_" + gameObject.scene.handle + ".png", bytes);
        print("Vertical isle map generated.");
    }

    private void SetMatMask(string fileName)
    {
        var mat = GetComponent<Renderer>().sharedMaterial;
        var tex = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/_Art/Sprites/Effects/" + fileName, typeof(Texture2D));
        if (tex == null)
        {
            print("Error when assigning mask on material. File not found.");
            return;
        }
        mat.SetTexture("_MaskTex", tex);
    }
}
