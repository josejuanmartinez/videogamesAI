using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CityManager
{
    readonly Board board;
    public CityManager(Board b)
    {
        board = b;
    }

    public List<CityUI> GetCitiesOfPlayer(NationsEnum owner)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;
        List<CityUI> res = board.GetTiles().Values.Where(x => x.GetCity() != null && x.GetCity().GetOwner() == owner).Select(x => x.GetCity()).ToList();
        return res;
    }

    public List<string> GetCitiesStringsOfPlayer(NationsEnum owner)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;
        List<CityUI> cities = GetCitiesOfPlayer(owner);
        return cities.Select(x => x.GetCityId()).Union(cities.Select(x => x.GetDetails().regionId)).ToList();
    }

    public List<CityUI> GetCitiesWithCharactersOfPlayer(NationsEnum owner)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;
        return board.GetTiles().Values.Where(x => x.GetCity() != null && x.GetCardsUI().Any(y => y.GetCardClass() == CardClass.Character && y.GetOwner() == owner)).Select(x => x.GetCity()).ToList();
    }

    public CityUI GetHavenOfPlayer(NationsEnum owner)
    {
        if(owner == NationsEnum.ABANDONED)
            return null;
        List<CityUI> cities = GetCitiesOfPlayer(owner);
        return cities.First(x => x.GetDetails().IsHaven());
    }
    public CityUI GetCityOfPlayer(NationsEnum owner, string cityName)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;
        List<CityUI> cities = GetCitiesOfPlayer(owner);
        if (cities.Count < 1)
            return null;
        CityUI exactMatch = cities.DefaultIfEmpty(null).FirstOrDefault(x => x.GetCityId() == cityName);
        if (exactMatch != null)
            return exactMatch;
        System.Random random = new();
        if (exactMatch == null)
        {

            if (cityName == CitiesStringConstants.ANY)
            {
                int index = random.Next(cities.Count);
                return cities[index];
            }

            switch (Nations.alignments[owner])
            {
                case AlignmentsEnum.DARK_SERVANTS:
                    if (cityName == CitiesStringConstants.ANY_DARK)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }                        
                    break;
                case AlignmentsEnum.FREE_PEOPLE:
                    if (cityName == CitiesStringConstants.ANY_FREE)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
                case AlignmentsEnum.NEUTRAL:
                    if (cityName == CitiesStringConstants.ANY_NEUTRAL)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
                case AlignmentsEnum.RENEGADE:
                    if (cityName == CitiesStringConstants.ANY_RENEGADE)
                    {
                        int index = random.Next(cities.Count);
                        return cities[index];
                    }
                    break;
            }
        }
        return null;
    }

    public CityUI GetCityAtHex(Vector2Int hex)
    {
        return board.GetTiles().Values.Where(x => x.HasCity() && x.GetCity().GetHex() == hex).Select(x => x.GetCity()).DefaultIfEmpty(null).FirstOrDefault();
    }

    public Dictionary<NationsEnum, float> GetEnemyNeighbourCities(Vector2Int hex, NationsEnum player)
    {
        Dictionary<NationsEnum, float> distances = new ();

        List<CityUI> tilesWithCities = board.GetTiles().Values.
            Where(x=> x.HasCity() && x.GetCity().GetOwner() != NationsEnum.ABANDONED && Nations.alignments[x.GetCity().GetOwner()] != Nations.alignments[player]).
            Select(x => x.GetCity()).
            ToList();
        tilesWithCities.Sort((x, y) => x.GetDistanceTo(hex).CompareTo(y.GetDistanceTo(hex)));

        int maxCities = 3;
        foreach(CityUI tileWithCity in tilesWithCities)
        {
            if (!distances.ContainsKey(tileWithCity.GetOwner()))
            {
                distances.Add(tileWithCity.GetOwner(), tileWithCity.GetDistanceTo(hex));
                maxCities--;
                if (maxCities == 0)
                    break;
            }
        }
        return distances;
    }

    public NationsEnum GetCityOwner(string cityId)
    {
        return GetCityUI(cityId).GetOwner();
    }

    public CityUI GetCityUI(string cityId)
    {
        return board.GetTiles().Values.Where(x => x.HasCity()).Select(x => x.GetCity()).Where(x => x.GetCityId() == cityId).First();
    }
}
