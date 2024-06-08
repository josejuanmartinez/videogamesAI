Shader "HoneyFramework/Terrain2.0LT" 
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _HeightTex ("Height Texture", 2D) = "gray" {}
		_MixerTex ("Mixer Texture", 2D) = "gray" {}
		_WorldData ("World Data Texture", 2D) = "gray" {}

		//Atlas density (one direction texture count), World data texture resolution (eg 128).
		_AdditionalData ("(Atlas,WD,,)", float) = (2,64,1,1)

        _NormalMap ("Normalmap", 2D) = "bump" {}
        _Displacement ("Displacement", Range(0, 3.0)) = 1.5
        _Color ("Color", color) = (1,1,1,0)
        _SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 300
            
        CGPROGRAM
        #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:vert nolightmap
        #pragma target 3.0
        #pragma glsl

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;					
        };
        
        float _Displacement;

		//------- UTILS --------
		sampler2D _MainTex;
		sampler2D _HeightTex;
		sampler2D _MixerTex;
		sampler2D _WorldData;
        
		fixed4 _AdditionalData;
        fixed4 _Color;
		

        struct Vector3i 
        {
            int x;
            int y;
            int z;

            //this value tells us uv of the hex. which is 0,0 in one corner and 1,1 in the oposite. 
            //Perfect to draw texture for teh hex. Note that only single hex will have influence in each pixel! no blenddrawings here!
            float2 uv;
        };

        //converts integer hex coordinates into flat world position used for uv and other 2d scapce calculations
        float2 ConvertToPosition(Vector3i v)
        {
			float cos30 = sqrt(3.0) * 0.5;
			float sin30 = 0.5;

            float2 X = float2(1, 0);
            float2 Y = float2(-0.5, cos30);
            float2 Z = float2(-0.5, -cos30);
            return X * v.x + Y * v.y + Z * v.z;                
        }

		//for defined hex it would build int-vector data and add UV value
		Vector3i GetHexCoordFixed(int x, int y, int z, float2 pos)
		{
			Vector3i v;
            v.x = x;
            v.y = y;
            v.z = z;

			//recover delta between testpoint and hex center as UV
            float2 center = ConvertToPosition(v);
            float2 offset = pos - center;						
            v.uv = offset*0.5 + float2(0.5, 0.5);
						
            return v;
		}

        //this code expects hex radius to be 1 for simplification. 
        Vector3i GetHexCoord(float2 pos)
        {
            //Convert world flat coordinates into hex FLOAT position
            float TWO_THIRD = 2.0 / 3.0;
            float ONE_THIRD = 1.0 / 3.0;
            float COMPONENT = ONE_THIRD * sqrt(3.0);

            float x = TWO_THIRD * pos.x;
            float y = (COMPONENT * pos.y - ONE_THIRD * pos.x);
            float z = -x - y;

            //we cant use floating hex position, so before return we need to convert it to integer.
            //also its important to understand that floating point position contains some artifacts if converted separately into integers
            //we have to do some post-calculation cleanup to be able to recover from them. 
            
            int vx = round(x);
            int vy = round(y);
            int vz = round(z);

            //find delta between rounded and original value
            float dx = abs(vx - x);
            float dy = abs(vy - y);
            float dz = abs(vz - z);

            //value which after rounding get most offset contains biggest artifacts, we want to discard it and recover form {a + b + c = 0} equation
            if (dz > dy && dz > dx) 
            { 
                vz = -vx - vy; 
            }
            else if (dy > dx) 
            { 
                vy = -vx - vz; 
            }
            else 
            { 
                vx = -vy - vz; 
            }
            
            return GetHexCoordFixed(vx, vy, vz, pos);
        }
		
		float2 GetTextureUV(int type, float2 localUV)
        {					
			int atlasSize = round(_AdditionalData.x);
			int row = floor((float)type / (float)atlasSize);
			int column =  type % atlasSize; 
						 			
			return float2((column + localUV.x) / _AdditionalData.x, ((row + localUV.y) / _AdditionalData.x) );
		}		

		//------- MASTER PROCESSES --------		

		float4 GetPixelStageData(float2 coords)
		{			
			float2 dataUV = coords / _AdditionalData.y;
            return tex2D (_WorldData, dataUV); 
		}

		float4 GetVertexStageData(float2 coords)
		{			
			float2 dataUV = coords / _AdditionalData.y;
            return tex2Dlod(_WorldData, float4(dataUV,0,0));			
		}
		

		float2 GetUV(float4 data, float2 offset)
		{			
			int type = round(data.r * _AdditionalData.x * _AdditionalData.x);						
			return GetTextureUV(type , offset );								
		}

		float2 RotateUV(float2 uv, float angle)
		{
			//do rotation around center
			float2 center = uv - float2(0.5, 0.5);

			angle *= 360;

			float2 r = float2 ( center.x * cos(angle) - center.y * sin(angle) ,
								center.x * sin(angle) + center.y * cos(angle) );
			r = r + float2(0.5, 0.5);
			return r;

		}


		float4 ProduceHeight(float2 position)
		{

			//hex positions which we take into consideration for this position
			Vector3i v = GetHexCoord(position);				

            float4 d = GetVertexStageData(float2(v.x, v.y));
			v.uv = RotateUV(v.uv, d.y);
			float2 uv= GetUV(d, v.uv);
			float4 c = tex2Dlod(_HeightTex, float4(uv,0,0));

			
			return c;
		}


		float4 ProduceColor(float2 position)
		{

			//hex positions which we take into consideration for this position
			Vector3i v1 = GetHexCoord(position);	
			

            float4 d1 = GetPixelStageData(float2(v1.x, v1.y));
			v1.uv = RotateUV(v1.uv, d1.y);
			float2 uv1= GetUV(d1, v1.uv);			
			float4 c = tex2D (_MainTex, uv1);

			return c;
		}


		//------- BODY --------

		struct Input {            			
             float3 worldPos;
			 float color;			 
        };

        void vert (inout appdata vData, out Input o)
        {			
			UNITY_INITIALIZE_OUTPUT(Input,o);

			float4 c = ProduceHeight(float2(vData.vertex.x, vData.vertex.z));
			float2 sunDir = float2(-0.3, 0.1);
			float4 c2 = ProduceHeight(float2(vData.vertex.x, vData.vertex.z) + sunDir);

            float d = (c.r - 0.5) * _Displacement;

            if (d<0)
            {
                d *= 0.2;				
            }
			
			vData.vertex.xyz += vData.normal * d; 						
			o.worldPos = vData.vertex.xyz;

			if(d > -0.1)
			{
				o.color = (c2.r - c.r) * 2 * (d+0.1);
			}
			else
			{
				o.color = 0;
			}		
        }                				

        void surf (Input IN, inout SurfaceOutput o) 
		{			                              
            float2 pos = float2(IN.worldPos.x, IN.worldPos.z);           
			float4 c = ProduceColor(pos) * (1 + clamp(IN.color * 10, -0.4, 0.3) );	
								

            o.Albedo = c.rgb;
            o.Specular = 0.2;
            o.Gloss = 1.0;
      //      o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
        }
        ENDCG
    }
    FallBack "Diffuse"
}