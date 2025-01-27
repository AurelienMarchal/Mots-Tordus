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
    TextMeshProUGUI firstDefTextMeshProUGUI;

    [SerializeField]
    TextMeshProUGUI secondDefTextMeshProUGUI;

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
        _renderer.sharedMaterial.color = tile.isObstacle ? colorObstacle : (tile.isVoid ? colorVoid : colorDefault);

        switch(tile){
            case DefinitionTile definitionTile:
                letterParentGO.SetActive(false);
                definitionParentGO.SetActive(true);
                firstDefTextMeshProUGUI.gameObject.SetActive(
                    definitionTile.definitionTileLayout == DefinitionTileLayout.FirstWordOnly || 
                    definitionTile.definitionTileLayout == DefinitionTileLayout.FirstAndSecondWord
                );
                secondDefTextMeshProUGUI.gameObject.SetActive(
                    definitionTile.definitionTileLayout == DefinitionTileLayout.SecondWordOnly || 
                    definitionTile.definitionTileLayout == DefinitionTileLayout.FirstAndSecondWord
                );
                if(definitionTile.possibleFirstWordEntries != null){
                    firstDefTextMeshProUGUI.text = definitionTile.possibleFirstWordEntries.Count.ToString();
                }
                else{
                    firstDefTextMeshProUGUI.text = definitionTile.firstWordSearch;
                }
                if(definitionTile.possibleSecondWordEntries != null){
                    secondDefTextMeshProUGUI.text = definitionTile.possibleSecondWordEntries.Count.ToString();
                }
                else{
                    secondDefTextMeshProUGUI.text = definitionTile.secondWordSearch;
                }
                //firstDefTextMeshProUGUI.text = definitionTile.firstWordSearch;
                //secondDefTextMeshProUGUI.text = definitionTile.secondWordSearch;
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
