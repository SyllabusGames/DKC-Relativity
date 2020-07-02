// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "RelativisticBackground" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		//_xpos("xpos", Float) = 1.0
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			uniform sampler2D _MainTex;
			uniform float _xpos;
		
			struct vertexInput{
				float4 vertex: POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};
		
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = v.texcoord;
				return o;
			}
		
			float4 frag(vertexOutput i) : COLOR{
				return tex2D(_MainTex, i.tex.xy)+_xpos;
		}
		ENDCG
	}
}
	FallBack "Diffuse"

}