//
// Created by jjmca on 15/04/2022.
//

#ifndef PATHFINDER_MATHUTILS_H
#define PATHFINDER_MATHUTILS_H

#include <iostream>
#include <vector>
#include <cassert>


class MathUtils {
public:
    static int Transpose(const std::pair<int,int>&, const std::pair<int, int>&);
    static std::pair<int,int> Untranspose(const int&, const std::pair<int, int>&);
    static int ManhattanDistance(std::pair<int,int>, std::pair<int, int>);
    static void Test();
};


#endif //PATHFINDER_MATHUTILS_H
