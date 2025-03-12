using UnityEngine;

using rccg.frontend;
using UnityEngine.UI;

public class MGTileMap : MonoBehaviour
{

    internal int[,] tileIndizes;

    Sprite[] tileset;

    internal int tilesize = 8;

    internal int resolutionX = 160;
    internal int resolutionY = 160;

    internal int tilesXLength = 20;
    internal int tilesYLength = 20;

    string tilesetName;

    Texture2D tilemapTexture;

    public RawImage tilemapImage;

    public void initialize()
    {
        MGTileSelector selector = new MGTileSelector();

        tilemapTexture = new Texture2D((int)resolutionX, (int)resolutionY);
        tilemapTexture.filterMode = FilterMode.Point;
        tilesXLength = resolutionX / tilesize;
        tilesYLength = resolutionY / tilesize;
        if(tileIndizes == null)
        {
            return;
            //tileIndizes = new int[resolutionX / tilesize, resolutionY / tilesize];
        } 
        
        loadTileset();

        UnityEngine.Object prefab = GUIResourceLoader.getResourceLoaderInstance().loadMinigamePrefab("MGTile");

        for (int x = 0; x < tilesXLength; x++)
        {
            for (int y = 0; y < tilesYLength; y++)
            {
             
                Sprite sprite = tileset[tileIndizes[x, y]];

           
                selector.copyToTexture(x, y, tileIndizes, tileset, tilemapTexture, tilesize);
             
            }
        }
        tilemapTexture.Apply();
        tilemapImage.texture = tilemapTexture;
    }

    public void rerender(int[,] tileIndizes)
    {
        this.tileIndizes = tileIndizes;
        rerender();
    }

    public void rerender()
    {
        Debug.LogError("rerender");
        initialize();
      

    }

    public void loadTileset()
    {
        if(this.tileset == null)
        {
            this.tileset = GUIResourceLoader.getResourceLoaderInstance().loadMinigameTileset("tileset_puzzle_background");
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
