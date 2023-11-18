cmake_minimum_required(VERSION 3.22)
project(PathFinder)

set(CMAKE_CXX_STANDARD 17)

add_executable(PathFinder main.cpp MovementUtils.cpp MovementUtils.h MathUtils.cpp MathUtils.h DebugUtils.cpp DebugUtils.h tests.cpp FindPath.cpp FindPath.h)
