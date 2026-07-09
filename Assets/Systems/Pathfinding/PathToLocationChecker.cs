using UnityEngine;
using GameJam.Map

namespace GameJam
{
    /**
    PathToLocationChecker.cs
    @Author: Luke Johnson
    Checks if the path to from one tile to another is free for movement. Can also be used to get the 
    distance between tiles, and find the tiles between two tiles.
    Current design is using the coordinate with offsets the game origionally was built with.
    */
    public class PathToLocationChecker : MonoBehaviour
    {
        //TileNodeManager - contaions the tile nodes relative to the map coordinates.

        private TileNodeManager tileNodeManager;

        private void Awake() {
            this.tileNodeManager = new GetComponent<TileNodeManager>();
        }

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /**
        public CanMoveToDestination:
        check if the diret path to the location is clear. Will be used by the player entity.
        Check if the destination tile is empty, and the direct tiles along the path can be moved along.
        does this by getting an array of all tiles along the path, and checks them as well as the
        destionation tile to see if they do not contaion an obstacle blocking movement.
        @Param: vector3Int: entityLocation - the tile location the entity is currently in in vector3int.
        @Parm: vector3Int, destinationLocation - the tile location this is checking he path to.
        @Return: boolean - true if the entity can move to the destination.
        */
        

        /**
        public IsDestinationInRange
        check if the destination location is in range of the the player entitie's movement range.
        Does this by getting the x and y distance, then if the y distance is greater than the x distance, 
        uses the following forumla:
        dist = ( x + y ) / 2
        @Parm: vector3Int, entityLocation: the tile location the entity is currently in in vector3int.
        @Parm: vector3Int, destinationLocation - the tile location the entity wants to move to.
        @Parm: maxMovement - the maximum movement the entity can move.
        @Return: boolen - true if the entities destination is within range.
        */

        /**
        public GetDistance
        returns the distance from one tile to another.
        if using the 2d offset coordinate system, 
        Does this by getting the x and y distance, then if the y distance is greater than the x distance, 
        uses the following forumla:
        dist = ( x + y ) / 2
        else dist = x
        @Parm: vector3Int: locationFrom - the coordinate location of the source.
        @Parm: vector3Int: locationTo - the coordinate location of the destination.
        @Return: int - the distance
        */

        /**
        private GetTilesAlongPath
        gets an array of tiles along the path the entity wants to move along.
        Dees this by finding the total distane of the path, and the x distance and y distance,
        then divides the distance by x and y to find the ratio of movement for one step. then
        iterates each step a number of times of the distance, and each time finds the next tile 
        by using the move ratio and adds that next tile to the array to return.
        @Parm: vector3Int, entityLocation: the tile location the entity is currently in in vector3int.
        @Parm: vector3Int, destinationLocation: the tile location the entity wants to move to.
        @Return: vector3Int[] - array of tile locations along path.
        */

        /**
        private checkIfArrayOfCoordsIsMovable
        this should probably go in TileNodeManager and be made public.
        checks if each coorinate in the array is movable, if they all are, returns true.
        It does this by checking the TileNode for each coord, and seeing if it is movable.
        if they are all movable, returns true, else false.
        @Parm: vector3Int[] arrayOfCoords - the array of vector3 ints to check if are all movable.
        @Return: boolean - returns true if all coords can be moved along.

        */
    }
}
