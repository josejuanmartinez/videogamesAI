//
// Created by jjmca on 15/04/2022.
//

#include <iostream>
#include "DebugUtils.h"
#include "MathUtils.h"

/**
 * Prints a sentence with the cell/position
 * @param Position
 * @param name
 * @param indent
 */
void DebugUtils::PrintPosition(const int& Position, const std::string& name, const int& indent, const std::pair<int, int>& MapDimensions) {
    std::pair<int,int> pPosition = MathUtils::Untranspose(Position, MapDimensions);
    std::cout << std::string(indent, '-') << name << ": (" << pPosition.first << "," << pPosition.second << ")" << "\n";
}

/**
 * Prints just the position
 * @param Position
 */
void DebugUtils::PrintPositionSlim(const std::pair<int, int>& Position) {
    std::cout << "(" << Position.first << "," << Position.second << ")";
}

/**
 * Prints a distance from coordinates
 * @param i
 * @param j
 * @param distance
 */
void DebugUtils::PrintDistance(const int& i, const int& j, const int& distance) {
    std::cout << "(" << i << "," << j << ")=" << distance <<" ";
}

/**
 * Prints the distance matrix
 * @param distances
 * @param MapDimensions
 */
void DebugUtils::PrintDistances(const std::vector<int>& distances, const std::pair<int, int>& MapDimensions) {
    for (int j=MapDimensions.second-1;j>=0;j--) {
        for (int i=0;i<MapDimensions.first;i++) {
            int distance = distances[MathUtils::Transpose({i, j}, MapDimensions)];
            DebugUtils::PrintDistance(i, j , distance);
        }
        std::cout << "\n";
    }
}

/**
 * Prints the closest-nodes matrix
 * @param closests
 * @param MapDimensions
 */
void DebugUtils::PrintClosest(const std::vector<int>& closests, const std::pair<int, int>& MapDimensions) {
    for (int j=MapDimensions.second-1;j>=0;j--) {
        for (int i=0;i<MapDimensions.first;i++) {
            int iClosest = closests[MathUtils::Transpose({i, j}, MapDimensions)];
            std::pair<int, int> closest = MathUtils::Untranspose(iClosest, MapDimensions);
            DebugUtils::PrintPositionSlim({i,j});
            std::cout << "=";
            DebugUtils::PrintPositionSlim(closest);
            std::cout << " ";
        }
        std::cout << "\n";
    }
}

void DebugUtils::PrintDistancesAndClosest(const std::vector<int>& distances, const std::vector<int>& closest, const std::pair<int, int>& MapDimensions) {
    DebugUtils::PrintDistances(distances, MapDimensions);
    std::cout << "\n";
    DebugUtils::PrintClosest(closest, MapDimensions);
    std::cout << "\n";
}

