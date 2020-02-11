Shader "Example/Diffuse Texture" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Inclination("倾斜角", Range(0,1)) = 1
		_Azimuth("方位角", Range(0,2)) = 0.5
	}
	SubShader{
		Pass
		{
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM
			 #pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
			};

		
			half _Inclination;
			half _Azimuth;

			float3 Spherical_LightDir(float inclination, float azimuth)
			{
				float3 vector3 = float3(0,0,0);
				azimuth *= -1;
				vector3.z = cos(inclination) * cos(azimuth) * -1;
				vector3.x = cos(inclination) * sin(azimuth);
				vector3.y = sin(inclination);
				return normalize(vector3);			
			}

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float PI = 3.141592654;
				float inclination = PI * _Inclination;
				float azimuth = PI * _Azimuth;

				half3 lightDir = Spherical_LightDir(inclination, azimuth);

				half intensity =  dot(i.normal, lightDir);
				return half4(intensity, intensity, intensity,1);
			}

		  ENDCG
		}
	}
}