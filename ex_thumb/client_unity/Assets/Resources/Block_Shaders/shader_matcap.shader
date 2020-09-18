// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/**
 * @author dizgid Kenji Inokuchi / http://www.dizgid.com/
 *
 * MatCap Shader
 * Ported from spherical-environment-mapping
 * http://www.clicktorelease.com/blog/creating-spherical-environment-mapping-shader
 *
 */

Shader "putaoshader/shader_matcap"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_MatCap ("MatCap (RGB)", 2D) = "white" {}
	}
	
	Subshader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend  SrcAlpha OneMinusSrcAlpha
		Pass
		{
			//Tags { "LightMode" = "Always" }
			
			// http://developer.download.nvidia.com/GPU_Programming_Guide/GPU_Programming_Guide_Japanese.pdf
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				//#pragma fragmentoption ARB_precision_hint_nicest
				#include "UnityCG.cginc"				
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 cap	: TEXCOORD1;
				};
				
				uniform float4 _Color;
				uniform sampler2D _MainTex;
				float4 _MainTex_ST;
				uniform sampler2D _MatCap;
				
				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					
//					half2 capCoord;
//					// Inverse transpose of model*view matrix
//					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
//					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
//					o.cap = capCoord * 0.5 + 0.5;

					//float4 p = float4( v.vertex );

					float3 e = normalize( mul( UNITY_MATRIX_MV , v.vertex) );
					float3 n = normalize( mul( UNITY_MATRIX_MV , float4(v.normal, 0.)) );

					float3 r = reflect( e, n );
						float m = 2. * sqrt( 
						pow( r.x, 2. ) + 
						pow( r.y, 2. ) + 
						pow( r.z + 1., 2. ) 
					);
					half2 capCoord;
					capCoord = r.xy / m + 0.5;
					o.cap = capCoord;
					return o;
				}
				
				float4 frag (v2f i) : COLOR
				{
					float4 base = tex2D(_MainTex, i.cap);
					float3 color = base.rgb * step(0.5, base.a) + tex2D(_MatCap, i.cap).rgb + _Color.rgb;
					
					return float4(color, _Color.a);
				}
			ENDCG
		}
	}
}