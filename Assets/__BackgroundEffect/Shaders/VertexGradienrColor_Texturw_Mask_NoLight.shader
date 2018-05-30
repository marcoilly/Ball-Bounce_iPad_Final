// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexGradienrColor Selective" {
	Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 
 
   }
 
   SubShader {
    Tags { "Queue" = "Transparent" }
    Pass { 
    	
     Tags{"LightMode" =  "ForwardBase"}
     Blend SrcAlpha one// use alpha blending
 	 Cull Back
 	
     CGPROGRAM
 
     #pragma vertex vert
     #pragma fragment frag
 
     uniform float4 _Color; // define shader property for shaders
     uniform sampler2D _MainTex;
     uniform float4 _MainTex_ST;
     
     uniform float4 _LightColor0;
 
     struct vertexInput {
      float4 vertex   : POSITION;
      float3 normal   : NORMAL;
      float4 texcoord : TEXCOORD0;
      float4 color    : COLOR;

     };
     struct vertexOutput {
      float4 pos : SV_POSITION;
      float4 tex : TEXCOORD2;
     };
 
      vertexOutput vert(vertexInput input)
      {
	      vertexOutput output;
	 
	      output.pos = UnityObjectToClipPos(input.vertex);
	      output.tex = input.texcoord;
	      return output;
     }
 
     float4 frag(vertexOutput input) : COLOR 
     {
 		float4 text = tex2D(_MainTex,input.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);	
	    float newOpacity = text.xyz * _Color.a;	  	
	  	float3 finalOut =  text.xyz;
	    return float4(finalOut ,newOpacity);
     }
     ENDCG
     
    }
    
 
    
    
   }
}
