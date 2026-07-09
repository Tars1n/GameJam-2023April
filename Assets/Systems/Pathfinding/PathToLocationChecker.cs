using UnityEngine;

namespace GameJam
{
    public class PathToLocationChecker : MonoBehaviour
    {
        //TileNodeManager - contaions the tile nodes relative to the map coordinates.

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
        @Param: entityLocation - the tile location the entity is currently in in vector3int.
        @Parm: destinationLocation - the tile location this is checking he path to.
        @Return: boolean - true if the entity can move to the destination.
        */
        

        /**
        public IsDestinationInRange
        check if the destination location is in range of the the player entitie's movement range.
        @Parm: entityLocation: the tile location the entity is currently in in vector3int.
        @Parm: destinationLocation - the tile location the entity wants to move to.
        @Parm: maxMovement - the maximum movement the entity can move.
        @Return: boolen - true if the entities destination is within range.
        */

        /**
        private GetTilesAlongPath
        gets an array of tiles along the path the entity wants to move along.
        Dees this by finding the total distane of the path, and the x distance and y distance,
        then divides the distance by x and y to find the ratio of movement for one step. then
        iterates each step a number of times of the distance, and each time finds the next tile 
        by using the move ratio and adds that next tile to the array to return.
        @Parm: entityLocation: the tile location the entity is currently in in vector3int.
        @Parm: destinationLocation: the tile location the entity wants to move to.
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
