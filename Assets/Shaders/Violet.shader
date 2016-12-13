Shader "Custom/Violet" {
	Properties {
	}
	SubShader {
	    Tags {"Queue" = "Transparent"}

	    Pass {
		CGPROGRAM
		    #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertexInput {
                float4 vertex : POSITION;
            };

            struct fragmentInput {
                float4 pos: SV_POSITION;
                float4 color: COLOR0;
            };

            fragmentInput vert(vertexInput i) {
                fragmentInput o;
                o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
                o.color = half4(14.0 / 255.0, 7.0 / 255.0, 10.0 / 255.0, 0.0);
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
