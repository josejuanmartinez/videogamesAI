//
// Created by jjmca on 15/04/2022.
//

#include "MathUtils.h"
#include <vector>


/*
 * (0,h-1) ...   ...  ... (w-1, h-1)
 * ...                       ...
 * ...                       ...
 * ...                       ...
 * ...                       ...
 * ...                       ...
 * (0,0)   (1,0) (2,0) ... (w-1, 0)
 */


/*
 * Transpose: max_w * h + w
 * Example: (3,5) means row=3 col=5, what means 5(h) times full width and 3(w) positions
 */

/**
 * Transforms 2D into 1D position
 * @param Position Current position in map
 * @param MapDimensions Max width and height
 * @return int
 */
int MathUtils::Transpose(const std::pair<int,int>& Position, const std::pair<int, int>& MapDimensions) {
    return MapDimensions.first * Position.second + Position.first;
}

/**
 * Transforms 1D into 2D position
 * @param Node Current position in a 1D arrow
 * @param MapDimensions Max width and height
 * @return int
 */
std::pair<int, int> MathUtils::Untranspose(const int& Node, const std::pair<int, int>& MapDimensions) {
    return { Node % MapDimensions.first, static_cast<int>(Node / MapDimensions.first) };
}

/**
 * Calculates distance in a grid between two points. Used to have a greedy estimator to the best node to visit
 * @param Node
 * @param Target
 * @return int (Manhattan Distance)
 */
int MathUtils::ManhattanDistance(std::pair<int,int> Node, std::pair<int, int> Target) {
    return abs(Target.first - Node.first) + abs(Target.second - Node.second);
}

/**
 * Some unit tests...
 */

void MathUtils::Test() {
    assert(MathUtils::Transpose(std::pair(1,2),std::pair(10,10)) == 21);
    assert(MathUtils::Transpose(std::pair(3,5),std::pair(10,10)) == 53);
    assert(MathUtils::Transpose(std::pair(0,0),std::pair(10,10)) == 0);
    assert(MathUtils::Transpose(std::pair(9,9),std::pair(10,10)) == 99);

    std::pair<int,int> a = MathUtils::Untranspose(4,std::pair(4,4));
    assert(a.first == 0 && a.second == 1);

    a = MathUtils::Untranspose(9,std::pair(4,4));
    assert(a.first == 1 && a.second == 2);

    a = MathUtils::Untranspose(10,std::pair(4,4));
    assert(a.first == 2 && a.second == 2);

    a = MathUtils::Untranspose(0,std::pair(4,4));
    assert(a.first == 0 && a.second == 0);
}