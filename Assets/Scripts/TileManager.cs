using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    
    private Tile tile_;

    public Tile tile{
        get { return tile_; }
        set { tile_ = value; UpdateAccordingToTile();}
    }

    [SerializeField]
    Highlight highlight;

    public bool isHighlighted{
        set{
            highlight.ToggleHighlight(value);
        }
        
    }

    [SerializeField]
    Canvas mainCanvas;

    [SerializeField]
    Renderer _renderer;

    [SerializeField]
    Color colorDefault;

    [SerializeField]
    Color colorVoid;

    [SerializeField]
    Color colorObstacle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        
    }


    private void OnMouseOver() {
        FindFirstObjectByType<GridManager>().tileManagerHovered = this;
    }


    void UpdateAccordingToTile(){
        if(tile == null){
            return;
        }

        mainCanvas.gameObject.SetActive(!(tile.isVoid || tile.isObstacle));

        _renderer.material.color = tile.isObstacle ? colorObstacle : (tile.isVoid ? colorVoid : colorDefault);
    }
}
