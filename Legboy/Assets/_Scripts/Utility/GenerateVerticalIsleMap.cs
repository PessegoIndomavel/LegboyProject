using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer))]
public class GenerateVerticalIsleMap : MonoBehaviour
{
    private int maxHeight = 0;
    public void GenerateVertIsleMap()
    {
        Tilemap tilemap = GetComponent<Tilemap>();

        var tiles = GetTiles(tilemap);

        int pixelSize = tilemap.size.x * 16;
        /*string str = "";
        foreach (var tileList in tiles)
        {
            foreach (var tile in tileList)
            {
                str += tile;
            }
            str += "\n";
        }
        print(str);*/

        Texture2D texture = new Texture2D(pixelSize, pixelSize, TextureFormat.RGB24, false);

        var xSize = tiles.Count * 16;
        var ySize = tiles[0].Count * 16;
        
        for (var x = 0; x < xSize; x++)
        {
            for (var y = ySize-1; y >= 0; y--)
            {
                var colorScale = Mathf.InverseLerp(0, maxHeight, tiles[x / 16][y / 16]);
                //if(colorScale != 0) print(colorScale);
                texture.SetPixel(x,ySize-1-y, new Color(colorScale, colorScale, colorScale, 1));
            }
        }
        
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/_Art/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        print("success i guess");
        //SaveTexture(texture);
    }
    
    private List<List<int>> GetTiles(Tilemap tilemap)
    {
        List<List<int>> tiles = new List<List<int>>();
        
        for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
        {
            var tileX = new List<int>();
            for (int y = (tilemap.origin.y + tilemap.size.y); y > tilemap.origin.y; y--)
            {
                int temp = tilemap.HasTile(new Vector3Int(x, y, 0)) ? 1 : 0;
                if(temp != 0) temp += (y < (tilemap.origin.y + tilemap.size.y)) ? tileX[(tilemap.origin.y + tilemap.size.y) - y - 1] : 0;
                //print("x:" + (x-tilemap.origin.x) + " / y:" + ((tilemap.origin.y + tilemap.size.y) - y - 1) + " / Size: " + tileX.Count);
                //if it isn't on the top line -> increment temp with the value on top of it
                
                
                tileX.Add(temp);
                
                if (temp > maxHeight) maxHeight = temp;
            }
            tiles.Add(tileX);
        }
        return tiles;
    }

    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes("Assets/_Art" + "Image" + ".png", bytes);
        print("success i guess");
    }
}
