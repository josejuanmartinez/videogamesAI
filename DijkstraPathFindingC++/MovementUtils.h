//
// Created by jjmca on 15/04/2022.
//

#ifndef PATHFINDER_MOVEMENTUTILS_H
#define PATHFINDER_MOVEMENTUTILS_H

#include <iostream>
#include <vector>
#include <cassert>

class MovementUtils {
public:
    static std::vector<int> GetNeighbours(const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static bool IsOutsideMap(const std::pair<int,int>&, const std::pair<int, int>&);
    static bool CanMoveTo (const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static int CanGoRight(const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static int CanGoUp(const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static int CanGoLeft(const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static int CanGoDown(const std::pair<int,int>&, const std::vector<int>&, const std::pair<int, int>&);
    static int GetClosestCandidate(const std::vector<std::pair<int, int>>& toVisit, const std::pair<int, int>& MapDimensions);
    static void Test();
};


#endif //PATHFINDER_MOVEMENTUTILS_H
