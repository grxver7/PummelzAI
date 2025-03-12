using rccg.frontend;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace mg.pummelz
{

    public class MGPumFieldRender : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        public Image tile;

        internal MGPummelz controller;

        private MGPumField _field;

        internal MGPumField field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                applyTerrain();
            }

        }

        public void init(MGPummelz controller, MGPumField field)
        {
            this.controller = controller;
            this.field = field;
        }

        public void applyTerrain()
        {
            MGPumField.Terrain terrain = this.field.terrain;
            Sprite[] tiles = GUIResourceLoader.getResourceLoaderInstance().loadMinigameSprites("pummelz/tiles");
            System.Random rng = new System.Random();
            if (terrain == MGPumField.Terrain.Earth)
            {
                    tile.sprite = tiles[0];
            }
            else if (terrain == MGPumField.Terrain.Sand)
            {
                int[] tileset = new int[] { 1, 1, 1, 9 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
            }
            else if(terrain == MGPumField.Terrain.Grass)
            {
                int[] tileset = new int[] { 2, 7, 7, 8, 8, 8, 8 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
            }
            else if (terrain == MGPumField.Terrain.Mountain)
            {
                int[] tileset = new int[] { 13 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
            }
            else if (terrain == MGPumField.Terrain.Ice)
            {
                int[] tileset = new int[] { 6 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
                
            }
            else if (terrain == MGPumField.Terrain.Water)
            {
                int[] tileset = new int[] {  12 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
            }
            else if (terrain == MGPumField.Terrain.Lava)
            {
                int[] tileset = new int[] { 5 };
                tile.sprite = tiles[tileset[rng.Next(tileset.Length)]];
            }
            else
            {
                Debug.LogError("Can't render unknown terrain: " + terrain);
            }


        }


        internal Vector2Int coords { get { return field != null ? field.coords : new Vector2Int(-1, -1); } }
        internal int x { get { return field != null ? field.coords.x : -1; } }
        internal int y { get { return field != null ? field.coords.y : -1; } }

        //private MGCheckers controller;

        internal MGPumUnitRender unitRender;


        void Awake()
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            controller.inputManager.fieldPointerUp(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            controller.inputManager.fieldPointerDown(this, eventData.button);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            controller.inputManager.fieldPointerExit(this);
        }
    }

}