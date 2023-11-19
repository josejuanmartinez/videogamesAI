using System;
using System.Collections.Generic;

public enum ResourceType
{
    FOOD,
    GOLD,
    CLOTHES,
    WOOD,
    METAL,
    MOUNTS,
    GEMS,
    LEATHER
}

public struct Resources
{
    public Dictionary<ResourceType, int> resources;

    public Resources(int food, int gold, int cloth, int wood, int metal, int horses, int gems, int leather)
    {
        resources = new()
        {
            { ResourceType.FOOD, food },
            { ResourceType.GOLD, gold },
            { ResourceType.CLOTHES, cloth },
            { ResourceType.WOOD, wood },
            { ResourceType.METAL, metal},
            { ResourceType.MOUNTS, horses },
            { ResourceType.GEMS, gems},
            { ResourceType.LEATHER, leather }
        };
    }
    public Resources(Resources a)
    {
        resources = new(a.resources);
    }
    public static Resources operator +(Resources a, Resources b)
       => new(
           (a.resources[ResourceType.FOOD] + b.resources[ResourceType.FOOD]),
           (a.resources[ResourceType.GOLD] + b.resources[ResourceType.GOLD]),
           (a.resources[ResourceType.CLOTHES] + b.resources[ResourceType.CLOTHES]),
           (a.resources[ResourceType.WOOD] + b.resources[ResourceType.WOOD]),
           (a.resources[ResourceType.METAL] + b.resources[ResourceType.METAL]),
           (a.resources[ResourceType.MOUNTS] + b.resources[ResourceType.MOUNTS]),
           (a.resources[ResourceType.GEMS] + b.resources[ResourceType.GEMS]),
           (a.resources[ResourceType.LEATHER] + b.resources[ResourceType.LEATHER]));

    public static Resources operator *(Resources a, short b)
       => new(
           (a.resources[ResourceType.FOOD] * b),
           (a.resources[ResourceType.GOLD] * b),
           (a.resources[ResourceType.CLOTHES] * b),
           (a.resources[ResourceType.WOOD] * b),
           (a.resources[ResourceType.METAL] * b),
           (a.resources[ResourceType.MOUNTS] * b),
           (a.resources[ResourceType.GEMS] * b),
           (a.resources[ResourceType.LEATHER] * b));
    public static bool operator ==(Resources a, short b)
       => a.resources[ResourceType.FOOD] == b && a.resources[ResourceType.GOLD] == b && a.resources[ResourceType.CLOTHES] == b && a.resources[ResourceType.WOOD] == b && a.resources[ResourceType.METAL] == b && a.resources[ResourceType.MOUNTS] == b && a.resources[ResourceType.GEMS] == b && a.resources[ResourceType.LEATHER] == b;

    public static bool operator !=(Resources a, short b)
       => a.resources[ResourceType.FOOD] != b && a.resources[ResourceType.GOLD] != b && a.resources[ResourceType.CLOTHES] != b && a.resources[ResourceType.WOOD] != b && a.resources[ResourceType.METAL] != b && a.resources[ResourceType.MOUNTS] != b && a.resources[ResourceType.GEMS] != b && a.resources[ResourceType.LEATHER] != b;

    public readonly int Sum()
    {
        return resources[ResourceType.FOOD] + resources[ResourceType.GOLD] + resources[ResourceType.CLOTHES] + resources[ResourceType.WOOD] + resources[ResourceType.CLOTHES] + resources[ResourceType.METAL] + resources[ResourceType.MOUNTS] + resources[ResourceType.GEMS] + resources[ResourceType.LEATHER];
    }

    public override readonly bool Equals(object obj)
    {
        return obj is Resources resources &&
               EqualityComparer<Dictionary<ResourceType, int>>.Default.Equals(this.resources, resources.resources);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(resources);
    }
}
