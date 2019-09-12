Shader "Unlit/UnlitWaterfall"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		_FoamC("Foam", Color) = (1, 1, 1, .5)
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Extra Wave Noise", 2D) = "white" {}
		_Speed("Speed", Range(-10,10)) = 0.0
		_TextureDistort("Texture Wobble", range(0,1)) = 0.1
		_Amount("Wave Amount", Range(0,1)) = 0.6
		_Foam("Foamline Thickness", Range(0,10)) = 8
		_Height("Wave Height", Range(0,1)) = 0.1
		_Scale("Scale", Range(0,1)) = 0.5
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD2;//
				float4 worldPos : TEXCOORD4;//
            };
							float _TextureDistort;
            sampler2D _MainTex, _NoiseTex;
            float4 _MainTex_ST;
			sampler2D _CameraDepthTexture; //Depth Texture
			fixed4 _Color;
			fixed _Speed;
			float _Foam, _Amount, _Height, _Scale;
			float4 _FoamC;
			

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				float4 tex = tex2Dlod(_NoiseTex, float4(v.uv.xy, 0, 0));//extra noise tex
				v.vertex.z += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height * .5f;//movement
				o.scrPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv + fixed2(0, _Time.y * _Speed))  * _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				//col.Alpha = _MainTex.a;

				fixed distortx = tex2D(_NoiseTex, (i.worldPos.xz * _Scale) + (_Time.x * 2)).r;// distortion

				//col *= (distortx * _TextureDistort);// texture times tint;  

				//Add a Foamline
				half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))); // depth
				half4 foamLine = 1 - saturate(_Foam* (depth - i.scrPos.w));// foam line by comparing depth and screenposition
				col += (step(0.4 * distortx, foamLine) * _FoamC); // add the foam line and tint to the texture
                return col;
            }
            ENDCG
        }
    }
}
