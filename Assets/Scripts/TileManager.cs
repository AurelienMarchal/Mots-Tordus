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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        
    }


    void UpdateAccordingToTile(){

    }
}
