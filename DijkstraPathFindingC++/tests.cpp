//
// Created by jjmca on 16/04/2022.
//
/** TESTING SCENARIOS **/
#include <vector>
#include <algorithm>
#include "MathUtils.h"
#include "MovementUtils.h"
#include "DebugUtils.h"
#include "FindPath.h"
#include <sys/time.h>

// Paradox example 1
void test_path_1() {
    std::vector<int> Map = {1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {1, 2}, Map, {4, 3}, OutPath));
    std::vector<int> Expected = {1, 5, 9};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// Paradox example 2
void test_path_2() {
    std::vector<int> Map = {0, 0, 1, 0, 1, 1, 1, 0, 1};
    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({2, 0}, {0, 2}, Map, {3, 3}, OutPath));
    assert(OutPath.empty());
}

// 4x4
void test_path_3() {
    std::vector<int> Map = {1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1 ,1 ,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {3, 3}, Map, {4, 4}, OutPath));
    std::vector<int> Expected = {4, 8, 12, 13, 14, 15};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 4x4 blocked col 1
void test_path_4() {
    std::vector<int> Map = {1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0 ,1 ,1};
    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({0, 0}, {3, 3}, Map, {4, 4}, OutPath));
    assert(OutPath.empty());
}

// 5x5 starts from 0,0
void test_path_5() {
    std::vector<int> Map = {1, 0, 1, 1, 1,
                             1, 0, 0, 0, 1,
                             1, 1, 1, 1, 1,
                             1, 1, 0, 0, 1,
                             1, 1, 0, 1, 1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {3, 4}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 5, 10, 11, 12, 13, 14, 19, 24, 23};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 5x5 starts reverse way
void test_path_5r() {
    std::vector<int> Map = {1, 0, 1, 1, 1,
                             1, 0, 0, 0, 1,
                             1, 1, 1, 1, 1,
                             1, 1, 0, 0, 1,
                             1, 1, 0, 1, 1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({3, 4}, {0, 0}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 24, 19, 14, 13, 12, 11, 10, 5, 0};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 5x5 starts from 0,2
void test_path_6() {
    std::vector<int> Map = {1, 0, 1, 1, 1,
                             1, 0, 0, 0, 1,
                             1, 1, 1, 1, 1,
                             1, 1, 0, 0, 1,
                             1, 1, 0, 1, 1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 2}, {3, 4}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 11, 12, 13, 14, 19, 24, 23};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 5x5 blocked row 1
void test_path_7() {
    std::vector<int> Map = {1, 0, 1, 1, 1,
                             0, 0, 0, 0, 0,
                             1, 1, 1, 1, 1,
                             1, 1, 0, 0, 1,
                             1, 1, 0, 1, 1};
    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({0, 0}, {3, 4}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// Same location
void test_path_8() {
    std::vector<int> Map = {1, 0, 1, 1, 1,
                             0, 0, 0, 0, 0,
                             1, 1, 1, 1, 1,
                             1, 1, 0, 0, 1,
                             1, 1, 0, 1, 1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {0, 0}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// Same location
void test_path_9() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({4, 0}, {0, 4}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 9, 14, 19, 24, 23, 22, 17, 12, 7, 2, 1, 0, 5, 10, 15, 20};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// Locket around start only diagonal
void test_path_10() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 0,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({4, 0}, {0, 4}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}


// From a cell to itself in the middle
void test_path_11() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 0,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({2, 3}, {2, 3}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// From a cell to itself in the middle
void test_path_12() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 0,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({4, 0}, {4, 0}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// Start is impassable
void test_path_13() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({3, 0}, {4, 0}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}


// Target is impassable
void test_path_14() {
    std::vector<int> Map = {1, 1, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 0, 1,
                             1, 0, 1, 1, 1};

    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({0, 0}, {3, 0}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// 1x1
void test_path_15() {
    std::vector<int> Map = {1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {0, 0}, Map, {1, 1}, OutPath));
    assert(OutPath.empty());
}

// 1x5 reverse path
void test_path_16() {
    std::vector<int> Map = {1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({4, 0}, {0, 0}, Map, {5, 1}, OutPath));
    std::vector<int> Expected = { 3, 2, 1, 0};
    for(int i=0;i<OutPath.size();i++) {
        // std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// center block and edge
void test_path_17() {
    std::vector<int> Map = {1,1,1,1,0,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {4, 1}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 5,10,15,20,21,22,23,24,19,14,9};
    for(int i=0;i<OutPath.size();i++) {
        // std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// center block
void test_path_18() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {4, 1}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 1,2,3,4,9};
    for(int i=0;i<OutPath.size();i++) {
        // std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// center block with enter
void test_path_19() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,0,0,0,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {4, 1}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 1,2,3,4,9};
    for(int i=0;i<OutPath.size();i++) {
        // std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 1 step
void test_path_20() {
    std::vector<int> Map = {1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {1, 0}, Map, {1, 2}, OutPath));
    std::vector<int> Expected = { 1 };
    for(int i=0;i<OutPath.size();i++) {
        // std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// 3,7 two similar ways, one shorter
void test_path_21() {
    std::vector<int> Map = {1,1,1,
                             1,1,1,
                             1,0,0,
                             1,0,1,
                             1,0,1,
                             1,1,1,
                             1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({1, 1}, {2, 3}, Map, {3, 7}, OutPath));
    std::vector<int> Expected = { 3, 6, 9, 12, 15, 16, 17, 14, 11};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// Trap
void test_path_22() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,0,0,1,0,
                             1,1,1,0,1,
                             1,1,1,1,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {4, 2}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 5, 10, 15, 16, 17, 18, 19, 14};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// Trap from 4,0
void test_path_23() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,0,0,1,0,
                             1,1,1,0,1,
                             1,1,1,1,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({4, 0}, {4, 2}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 3,2,1,0,5, 10, 15, 16, 17, 18, 19, 14};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// Unreachable corner
void test_path_24() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,1,1,1,1,
                             1,1,1,1,1,
                             1,1,1,0,0,
                             1,1,1,0,1};
    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({1, 3}, {4, 4}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// Unreachable corner now reachable
void test_path_25() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,1,1,1,1,
                             1,1,1,1,1,
                             1,1,1,0,0,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({1, 2}, {4, 4}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = { 16, 21, 22, 23, 24};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// All 0 except start
void test_path_26() {
    std::vector<int> Map = {1,0,0,0,0,
                             0,0,0,0,0,
                             0,0,0,0,0,
                             0,0,0,0,0,
                             0,0,0,0,0};
    std::vector<int> OutPath;
    assert(!FindPath::ShortestPath({0, 0}, {4, 4}, Map, {5, 5}, OutPath));
    assert(OutPath.empty());
}

// From top right to bottom left
void test_path_27() {
    std::vector<int> Map = {0,0,0,0,1,
                             1,0,1,1,1,
                             1,1,1,0,1,
                             1,1,1,0,1,
                             1,1,1,0,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({4, 3}, {0, 1}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = {14, 9, 8,7,12,11,10,5};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}


// From top right to bottom left
void test_path_28() {
    std::vector<int> Map = {1,1,1,1,1,
                             1,1,1,1,1,
                             1,1,1,1,1,
                             1,0,0,0,1,
                             1,1,1,1,1};
    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({1, 2}, {2, 4}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = {10, 15, 20, 21, 22};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}


// From top right to bottom left
void test_path_29() {
    std::vector<int> Map = {1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,0,0,0,0,1,0,0,1,
                             1,1,1,1,1,0,1,0,0,1,
                             1,1,1,1,1,0,1,1,1,1,
                             1,1,1,1,1,0,1,1,1,1,
                             1,1,1,1,1,0,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({1, 2}, {8, 5}, Map, {10, 10}, OutPath));
    std::vector<int> Expected = {22,23,24,25,26,36,46,56,57,58};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}



// From top right to bottom left
void test_path_30() {
    std::vector<int> Map = {1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,0,0,0,0,1,0,0,1,
                             1,1,1,1,1,0,1,0,0,1,
                             1,1,1,1,1,0,1,0,1,1,
                             1,1,1,1,1,0,1,0,1,1,
                             1,1,1,1,1,0,1,0,1,1,
                             1,1,1,1,1,1,1,1,1,1,
                             1,1,1,1,1,1,1,1,1,1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({1, 2}, {8, 5}, Map, {10, 10}, OutPath));
    std::vector<int> Expected = {22,23,24,25,26,27,28,29,39,49,59,58};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}

// From top right to bottom left
void test_path_31() {
    std::vector<int> Map = {1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1,
                            1,1,1,1,1,1,1,1,1,1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 0}, {5, 5}, Map, {10, 10}, OutPath));
    std::vector<int> Expected = {1,2,3,4,5, 15, 25, 35, 45, 55};
    for(int i=0;i<OutPath.size();i++) {
        //std::cerr << OutPath[i] << " " << Expected[i] << "\n";
        assert(OutPath[i] == Expected[i]);
    }
}


// One path, checking greedy
void test_path_32() {
    std::vector<int> Map = {1,1,1,0,1,
                            1,0,1,0,1,
                            1,0,1,0,1,
                            1,0,1,0,1,
                            1,0,1,1,1};

    std::vector<int> OutPath;
    assert(FindPath::ShortestPath({0, 4}, {4, 0}, Map, {5, 5}, OutPath));
    std::vector<int> Expected = {15, 10, 5, 0,1, 2, 7, 12, 17, 22, 23, 24, 19, 14, 9, 4};
    for(int i=0;i<OutPath.size();i++) {
       std::cerr << OutPath[i] << " " << Expected[i] << "\n";
       assert(OutPath[i] == Expected[i]);
    }
}
/*
int main() {
    struct timeval start{}, end{};
    // start timer.
    gettimeofday(&start, nullptr);
    // sync the I/O of C and C++.
    std::ios_base::sync_with_stdio(false);

    MathUtils::Test();
    MovementUtils::Test();
    test_path_1();
    test_path_2();
    test_path_3();
    test_path_4();
    test_path_5();
    test_path_5r();
    test_path_6();
    test_path_7();
    test_path_8();
    test_path_9();
    test_path_10();
    test_path_11();
    test_path_12();
    test_path_13();
    test_path_14();
    test_path_15();
    test_path_16();
    test_path_17();
    test_path_18();
    test_path_19();
    test_path_20();
    test_path_21();
    test_path_24();
    test_path_23();
    test_path_22();
    test_path_28();
    test_path_27();
    test_path_26();
    test_path_25();
    test_path_30();
    test_path_29();
    test_path_31();
    test_path_32();

    gettimeofday(&end, nullptr);
    double time_taken;

    time_taken = (end.tv_sec - start.tv_sec) * 1e6;
    time_taken = (time_taken + (end.tv_usec -
                                start.tv_usec)) * 1e-6;
    std::cerr << "Test finished " << time_taken;
}*/