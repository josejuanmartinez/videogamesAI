//
// Created by jjmca on 15/04/2022.
//

#ifndef PATHFINDER_DEBUGUTILS_H
#define PATHFINDER_DEBUGUTILS_H

#include <vector>

class DebugUtils {
public:
    static void PrintPosition(const int&, const std::string&, const int&, const std::pair<int, int>&);
    static void PrintDistance(const int&, const int&, const int&);
    static void PrintDistances(const std::vector<int>&, const std::pair<int, int>&);
    static void PrintClosest(const std::vector<int>&, const std::pair<int, int>&);
    static void PrintPositionSlim(const std::pair<int, int>&);
    static void PrintDistancesAndClosest(const std::vector<int>& distances, const std::vector<int>& closest,
                                         const std::pair<int, int>& MapDimensions);
};


#endif //PATHFINDER_DEBUGUTILS_H
