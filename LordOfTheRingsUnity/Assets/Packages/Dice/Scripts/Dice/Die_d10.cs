using UnityEngine;

// Die subclass to expose the D10 side hitVectors
public class Die_d10 : Die {
		
    override protected Vector3 HitVector(int side)
    {
        switch (side)
        {
            case 1: return new Vector3(0.4F, -0.7F, 0.7F);
            case 2: return new Vector3(0.4F, 0.7F, -0.7F);
            case 3: return new Vector3(-0.7F, 0.2F, 0.7F);
            case 4: return new Vector3(0F, -0.7F, -0.7F);
            case 5: return new Vector3(0F, 0.7F, 0.7F);
            case 6: return new Vector3(0.7F, -0.2F, -0.7F);
            case 7: return new Vector3(-0.4F, -0.7F, 0.7F);
            case 8: return new Vector3(-0.4F, 0.7F, -0.7F);
            case 9: return new Vector3(0.7F, 0.2F, 0.7F);
            case 10: return new Vector3(-0.7F, -0.2F, -0.7F);
        }
        return Vector3.zero;
    }
	
}
