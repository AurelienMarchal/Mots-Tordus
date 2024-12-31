using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    

    [SerializeField]
    GameObject tilePrefab;

    private Grid grid_;

    public Grid grid{
        get { return grid_; }
        set { grid_ = value; UpdateAccordingToGrid();}
    }

    private List<TileManager> tileManagers;
    
    
    // Start is called before the first frame update
    void Start(){
        tileManagers = new List<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateAccordingToGrid(){

    }
}
