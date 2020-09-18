Shader "putaoshader/shader_transparenttoyblendtex"
{
	Properties
	{	 
		_Color("Color",Color) = (1,1,1,1)
	    _Alpha("Alpha",range(.4,.6)) = .4
	    _MainTex ("Main Tex", 2D) = "white" {}
        [HideInInspector]
		_SpecularPower("SpecularPower",range(0,11)) = 11
		[HideInInspector]
		_MaskPower("MaskPower",range(0,1)) = 1
		[HideInInspector]
		_RimPower("RimPower",range(0,1)) = 0
		[HideInInspector]
		_RimPower2("RimPower2",range(0,2)) = .95
		[HideInInspector]
		_DiffPower("DiffPower",range(1,1.2)) = 1.2
		[HideInInspector]
		 _colorfulPower("colorfulPower",range(10,1000)) = 224
		 
		 _intensity("Intensity", range(1,100)) = 100
	}
    SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 100

        Pass
		{
		    Tags{ "LightMode" = "ForwardBase" "Queue" = "Transparent-500"}
			blend srcalpha oneminussrcalpha
			cull front
			zwrite off
			ztest lequal
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
			#pragma target 3.0 

		 
            float4 _Color;
            float _Alpha;
            float _SpecularPower;
            float _MaskPower;
            float _RimPower;
            float _DiffPower;
            float _RimPower2;
             
            float _colorfulPower;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct data
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                fixed4 tangent : TANGENT;
            };
    
            struct v2f
            {
                float4 vertex0 : SV_POSITION;
                fixed4 uv0 : TEXCOORD0;
                fixed3 normal0 : TEXCOORD1;
                fixed3 worldpos0 : TEXCOORD2;
    
                fixed3 tangent0 : TEXCOORD3;
                fixed3 binormal0 : TEXCOORD4;
                float2 uv : TEXCOORD5;
    
            };
    
            v2f vert(data v)
            {
                v2f o;
                o.uv0.rg = v.texcoord.xy;
                o.normal0 = UnityObjectToWorldNormal(v.normal);
                fixed3 wpos = mul(unity_ObjectToWorld, v.vertex);
                o.worldpos0 = wpos;
                o.vertex0 = UnityObjectToClipPos(v.vertex);
    
                o.tangent0 = UnityObjectToWorldDir(v.tangent.xyz);
                o.binormal0 = cross(v.normal , o.tangent0.xyz) * v.tangent.w;
                
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
    
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = (fixed4)1; 
                fixed4 maskColor = (fixed4)1; 
                fixed3 L = _WorldSpaceLightPos0.xyz;
                fixed3 N = normalize(i.normal0); 
                fixed3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldpos0);
                fixed3 H = normalize(L + V);
                float NL = max(dot(N, L), 0);
                float NV = max(dot(N, V), 0);
                float NH = max(dot(N, H), 0);
                fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz * texColor.rgb;
    
                fixed3 diff = texColor.rgb * _DiffPower * NL * _LightColor0.rgb;  
                fixed3 spec = _LightColor0.rgb * texColor.rgb * pow(NH, _colorfulPower) * _SpecularPower * saturate(maskColor.r * _MaskPower);
                fixed3 rim = (texColor.rgb * saturate(1 - NV * _RimPower))  *_Color.rgb; 
                fixed3 Lo = ambi + diff + spec   + rim +     dot(N, V)* _RimPower2;
                Lo *= _Color.rgb;
                
                fixed4 texcolor = tex2D(_MainTex,i.uv);	
                
                
                return float4(Lo , _Alpha)*step(texcolor.a - 0.01,0) + texcolor*step(0.01 - texcolor.a,0);
            }
            ENDCG
	        }

			Pass
			{
			    Tags{ "LightMode" = "ForwardBase" "Queue" = "Transparent+1000"}
				blend srcalpha oneminussrcalpha
				cull back
				zwrite on
				ztest lequal
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
				#pragma target 3.0 

				 
				float4 _Color;
				float _Alpha;
				float _SpecularPower;
				float _MaskPower;
				float _RimPower;
				float _DiffPower;
				float _RimPower2;
				 
				float _colorfulPower;
				float _intensity;
				struct data
				{
					float4 vertex : POSITION;
					fixed3 normal : NORMAL;
					float2 texcoord : TEXCOORD0;
					 fixed4 tangent : TANGENT;
				};

				struct v2f
				{
					float4 vertex0 : SV_POSITION;
					fixed4 uv0 : TEXCOORD0;
					fixed3 normal0 : TEXCOORD1;
					fixed3 worldpos0 : TEXCOORD2;

					fixed3 tangent0 : TEXCOORD3;
					fixed3 binormal0 : TEXCOORD4;

				};

			 

				v2f vert(data v)
				{
					v2f o;
				 

					o.uv0.rg = v.texcoord.xy;
					o.normal0 = UnityObjectToWorldNormal(v.normal);
					fixed3 wpos = mul(unity_ObjectToWorld, v.vertex);
					o.worldpos0 = wpos;
					o.vertex0 = UnityObjectToClipPos(v.vertex);

					o.tangent0 = UnityObjectToWorldDir(v.tangent.xyz);
					o.binormal0 = cross(v.normal , o.tangent0.xyz) * v.tangent.w;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texColor = (fixed4)1; 
					fixed4 maskColor = (fixed4)1; 
					fixed3 L = _WorldSpaceLightPos0.xyz;
					fixed3 N = normalize(i.normal0); 
					fixed3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldpos0);
					fixed3 H = normalize(L + V);
					float NL = max(dot(N, L), 0);

					float NH = max(dot(N, H), 0);
					fixed3 ambi = UNITY_LIGHTMODEL_AMBIENT.xyz * texColor.rgb;

					fixed3 diff = texColor.rgb * _DiffPower * NL * _LightColor0.rgb; 
					fixed3 spec = _LightColor0.rgb * texColor.rgb * pow(NH, _colorfulPower) * _SpecularPower * saturate(maskColor.r * _MaskPower);
					fixed3 rim = (texColor.rgb * saturate(1 - NH * _RimPower)) * _Color.rgb; 
					fixed3 Lo = ambi + diff + spec * _intensity +rim; 
					Lo *= _Color.rgb;
					return float4(Lo , _Alpha);
				}
				ENDCG
			}

	}
    FallBack "Diffuse"
}

