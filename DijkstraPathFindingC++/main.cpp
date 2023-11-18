#include <vector>
#include "FindPath.h"

/**
 * Entry point of the exercise. Will call FindPath class, ::ShortestPath method.
 * @param Start
 * @param Target
 * @param Map
 * @param MapDimensions
 * @param OutPath
 * @return
 */
bool FindPath(std::pair<int, int> Start,
              std::pair<int, int> Target,
              const std::vector<int>& Map,
              std::pair<int, int> MapDimensions,
              std::vector<int>& OutPath) {

    return FindPath::ShortestPath(Start, Target, Map, MapDimensions, OutPath);
}
