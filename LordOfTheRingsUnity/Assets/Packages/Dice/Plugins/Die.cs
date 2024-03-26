/**
 * Copyright (c) 2010-2015, WyrmTale Games and Game Components
 * All rights reserved.
 * http://www.wyrmtale.com
 *
 * THIS SOFTWARE IS PROVIDED BY WYRMTALE GAMES AND GAME COMPONENTS 'AS IS' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL WYRMTALE GAMES AND GAME COMPONENTS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Die base class to determine if a die is rolling and to calculate it's current value
/// </summary>
public class Die : MonoBehaviour {

	//------------------------------------------------------------------------------------------------------------------------------
	// public attributes
	//------------------------------------------------------------------------------------------------------------------------------
	
	// current value, 0 is undetermined (die is rolling) or invalid.
	public int value = 0;

	//------------------------------------------------------------------------------------------------------------------------------
	// protected and private attributes
	//------------------------------------------------------------------------------------------------------------------------------	
	
	// normalized (hit)vector from die center to upper side in local space is used to determine what side of a specific die is up/down = value
    protected Vector3 localHitNormalized;
	// hitVector check margin
    protected float validMargin = 0.45F;

    public Vector3 initialPosition;
    public Vector3 currentPosition;
    public Quaternion initialRotation;
    public Quaternion currentRotation;

    public float rotationDuration = 2f;
    public float rotatedTime = 0f;
    public float rotationSpeed;
    public Vector3 rotationAxis = Vector3.up;

    public bool rotateFirst = true;

    public float velocity;
    public float angularVelocity;
    
    public float forceStrength;

    public bool velocityRolling = true;
    public bool angularVelocityRolling = true;

    public bool isRolling = false;

    // true is die is still rolling
    public bool rolling
    {
    get
        {
            velocity = GetComponent<Rigidbody>().linearVelocity.sqrMagnitude;
            angularVelocity = GetComponent<Rigidbody>().angularVelocity.sqrMagnitude;
            if (velocity < .01F)
                velocityRolling = false;
            else
                velocityRolling = true;

            if (angularVelocity < .01F)
                angularVelocityRolling = false;
            else
                angularVelocityRolling = true;

            isRolling = velocityRolling || angularVelocityRolling;
            return isRolling;
        }
    }

	// calculate the normalized hit vector and should always return true
    protected bool localHit
    {
        get
        {
			// create a Ray from straight above this Die , moving downwards
            Ray ray = new (transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
            // cast the ray and validate it against this die's collider
            if (GetComponent<Collider>().Raycast(ray, out RaycastHit hit, 3 * transform.localScale.magnitude))
            {
                // we got a hit so we determine the local normalized vector from the die center to the face that was hit.
                // because we are using local space, each die side will have its own local hit vector coordinates that will always be the same.
                localHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
                return true;
            }
            // in theory we should not get at this position!
            return false;
        }
    }

	// calculate this die's value
    void GetValue()
    {
		// value = 0 -> undetermined or invalid
        value = -1;
        float delta = 1;
		// start with side 1 going up.
        int side = 1;
        Vector3 testHitVector;
		// check all sides of this die, the side that has a valid hitVector and smallest x,y,z delta (if more sides are valid) will be the closest and this die's value
        do
        {
			// get testHitVector from current side, HitVector is a overriden method in the dieType specific Die subclass
			// eacht dieType subclass will expose all hitVectors for its sides,
            testHitVector = HitVector(side);
            if (testHitVector != Vector3.zero)
            {
				// this side has a hitVector so validate the x,y and z value against the local normalized hitVector using the margin.
                if (valid(localHitNormalized.x, testHitVector.x) &&
                    valid(localHitNormalized.y, testHitVector.y) &&
                    valid(localHitNormalized.z, testHitVector.z))
                {
					// this side is valid within the margin, check the x,y, and z delta to see if we can set this side as this die's value
					// if more than one side is within the margin (especially with d10, d12, d20 ) we have to use the closest as the right side
                    float nDelta = Mathf.Abs(localHitNormalized.x - testHitVector.x) + Mathf.Abs(localHitNormalized.y - testHitVector.y) + Mathf.Abs(localHitNormalized.z - testHitVector.z);
                    if (nDelta < delta)
                    {
                        value = side;
                        delta = nDelta;
                        GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
			// increment side
            side++;
			// if we got a Vector.zero as the testHitVector we have checked all sides of this die
        } while (testHitVector != Vector3.zero && value < 0);
        
    }

    void ApplyDownwardForce()
    {
        // Apply the downward force to the object's Rigidbody
        Vector3 randomVector = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 0f),
            Random.Range(-1f, 1f)
        );

        GetComponent<Rigidbody>().AddForce(randomVector * forceStrength, ForceMode.Impulse);
    }

    private void Awake()
    {
        initialPosition = transform.position;
        transform.rotation = Random.rotation;
        initialRotation = transform.rotation;

        ApplyDownwardForce();
    }

    void Update()
    {

        currentPosition = transform.position;
        currentRotation = transform.rotation;

        if(rotateFirst)
        {

            float rotationAngle = rotationSpeed * Time.deltaTime; // Calculate the rotation angle for this frame

            transform.Rotate(rotationAxis, rotationAngle); // Rotate the object around the specified axis

            rotatedTime += Time.deltaTime; // Decrease the remaining duration

            if (rotationDuration - rotatedTime <= 0f)
                rotateFirst = false;
        }

        // determine the value is the die is not rolling
        if (!rolling && localHit && value == 0)
            GetValue();
    }

	// validate a test value against a value within a specific margin.
    protected bool valid(float t, float v)
    {
        if (t > (v - validMargin) && t < (v + validMargin))
            return true;
        else
            return false;
    }

	// virtual  method that to get a die side hitVector.
	// this has to be overridden in the dieType specific subclass
    protected virtual Vector3 HitVector(int side)
    {
        return Vector3.zero;
    }
	
}
