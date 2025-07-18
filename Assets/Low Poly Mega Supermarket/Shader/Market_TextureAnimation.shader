Shader "Custom/Market_TextureAnimation" {
    Properties{
    _Color("Color", Color) = (1, 1, 1, 1)
    _MainTex("Main Texture", 2D) = "white" {}
    _Tiling("Tiling", Vector) = (1, 1, 0, 0)
    _Offset("Offset", Vector) = (0, 0, 0, 0)
    _Speed("Speed", Float) = 1 }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Tiling;
                float4 _Offset;
                float _Speed;
                float4 _Color;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float2 animatedOffset = _Offset.xy + _Speed * _Time.y;
                    o.uv = v.uv * _Tiling.xy + animatedOffset;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }
                ENDCG
            }
    }
}