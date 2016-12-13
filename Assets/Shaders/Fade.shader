Shader "Custom/Fade" {
	Properties {
	}
	SubShader {
	    Tags {"Queue" = "Transparent"}
	    ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha

	    Pass {
		CGPROGRAM
		    #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord: TEXCOORD0;
            };

            struct fragmentInput {
                float4 pos: SV_POSITION;
                float4 color: COLOR0;
            };

            fragmentInput vert(vertexInput i) {
                fragmentInput o;
                o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
                o.color = half4(14.0 / 255.0, 7.0 / 255.0, 10.0 / 255.0, i.texcoord.y);
                return o;
            }

            half4 frag(fragmentInput i): COLOR {
                return i.color;
            }
		ENDCG
		}
	}
	FallBack "Diffuse"
}
