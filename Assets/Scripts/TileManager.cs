using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    GameObject definitionParentGO;

    [SerializeField]
    GameObject letterParentGO;

    [SerializeField]
    TextMeshProUGUI downDefTextMeshProUGUI;

    [SerializeField]
    TextMeshProUGUI acrossDefTextMeshProUGUI;

    [SerializeField]
    TextMeshProUGUI letterTextMeshProUGUI;

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

        //mainCanvas.gameObject.SetActive(!(tile.isVoid || tile.isObstacle));
        _renderer.material.color = tile.isObstacle ? colorObstacle : (tile.isVoid ? colorVoid : colorDefault);

        switch(tile){
            case DefinitionTile definitionTile:
                letterParentGO.SetActive(false);
                definitionParentGO.SetActive(true);
                downDefTextMeshProUGUI.gameObject.SetActive(
                    definitionTile.definitionTileLayout == DefinitionTileLayout.Down || 
                    definitionTile.definitionTileLayout == DefinitionTileLayout.DownAndAcross
                );
                acrossDefTextMeshProUGUI.gameObject.SetActive(
                    definitionTile.definitionTileLayout == DefinitionTileLayout.Across || 
                    definitionTile.definitionTileLayout == DefinitionTileLayout.DownAndAcross
                );
                downDefTextMeshProUGUI.text = definitionTile.downWordSearch;
                acrossDefTextMeshProUGUI.text = definitionTile.acrossWordSearch;
                break;

            case LetterTile letterTile:
                letterParentGO.SetActive(true);
                definitionParentGO.SetActive(false);
                letterTextMeshProUGUI.text = letterTile.letter.ToString();
                break;

            case VoidTile voidTile:
                letterParentGO.SetActive(false);
                definitionParentGO.SetActive(false);
                break;

            case ObstacleTile obstacleTile:
                letterParentGO.SetActive(false);
                definitionParentGO.SetActive(false);
                break;

        }

        
    }
}
