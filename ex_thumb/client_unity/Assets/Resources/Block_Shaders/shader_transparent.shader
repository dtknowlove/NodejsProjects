Shader "putaoshader/shader_transparent" {
    Properties {
		_Color ("Color", Color) = (0.67,0.623,0.176,1)
		//_SColor ("Secondary Color", Color) = (1,0,0,1)
		[HideInInspector]
		_ColorAlpha("Color Alpha",Range(0,1)) = 0.2
		[HideInInspector]
		_SubColorAlpha("SubColor Alpha",Range(0,1)) = 0.85
		[HideInInspector]
		_Rim ("Rim", Float) = 1.0
		[HideInInspector]
		_Thr ("Threshold", Range(0.0, 1.0)) = 0.3
	}
	
	SubShader {
		Tags { "Queue" = "Transparent" }
		
		Pass {
			ZWrite Off 
			Blend SrcAlpha OneMinusSrcAlpha 
			Cull Back
			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc" 

			uniform float4 _Color;
			uniform float _Rim;
			//uniform float4 _SColor;
			uniform float _Thr;
			uniform float _ColorAlpha;
			uniform float _SubColorAlpha;

			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
			};

			struct vertexOutput{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
			};

			vertexOutput vert(vertexInput input){ 
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.normal = normalize(mul(float4(input.normal,0.0), unity_WorldToObject).xyz);
				output.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, input.vertex).xyz);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR { 

				float3 _SColor = float3(_Color.r,_Color.g,_Color.b);

				float3 viewDir = normalize(input.viewDir);
				float3 norm = normalize(input.normal);
				float newOpacity = min(1.0, _ColorAlpha / abs((pow(dot(viewDir, norm), _Rim)) - (0.3 - _ColorAlpha )));
				if (newOpacity > _Thr){ 
					float3 variationColor = _Color.xyz + newOpacity * _SColor.xyz;
					return float4(variationColor, newOpacity);

				}else{ 
					float3 variationColor = _Color.xyz;
					return float4(variationColor, newOpacity);
				}
			}
						
			ENDCG
		}
		
		Pass {
			ZWrite Off 
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc" 

			uniform float4 _Color;
			uniform float _Rim;
			uniform float4 _SColor;
			uniform float _Thr;

			uniform float _ColorAlpha;
			uniform float _SubColorAlpha;

			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
			};

			struct vertexOutput{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
			};

			vertexOutput vert(vertexInput input){ 
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.normal = normalize(mul(float4(input.normal,0.0), unity_WorldToObject).xyz);
				output.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, input.vertex).xyz);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR{

				float3 _SColor = float3(_Color.r,_Color.g,_Color.b);

				float3 viewDir = normalize(input.viewDir);
				float3 norm = normalize(input.normal);
				return float4(_SColor.rgb, _SubColorAlpha - abs(dot(viewDir, norm)));
			}
	
			ENDCG
		}
	}
}
