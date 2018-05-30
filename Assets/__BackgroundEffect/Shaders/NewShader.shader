﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewShader" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _CutTex ("Cutout (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
   }
 
   SubShader {
    Tags { "Queue" = "Transparent" }
    Pass {
     ZWrite Off // don't write to depth buffer 
     Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 	 Cull Back
 	
     CGPROGRAM
 
     #pragma vertex vert
     #pragma fragment frag
 
     uniform float4 _Color; // define shader property for shaders
     uniform sampler2D _MainTex;
     uniform sampler2D _CutTex;
     uniform float _Cutoff;
 
     struct vertexInput {
      float4 vertex : POSITION;
      float4 texcoord : TEXCOORD0;
      float4 color : COLOR;

     };
     struct vertexOutput {
      float4 pos : SV_POSITION;
      float4 tex : TEXCOORD0;
      float4 color : COLOR;

     };
 
     vertexOutput vert(vertexInput input) {
      vertexOutput output;
 
      output.tex = input.texcoord;
      output.pos = UnityObjectToClipPos(input.vertex);
      output.color=input.color;
      return output;
     }
 
     float4 frag(vertexOutput input) : COLOR {
     float2 tp = float2(input.tex.x, input.tex.y); //get textures coordinate
     float4 col = tex2D(_MainTex, tp) * _Color; //load main texture
     float newOpacity = input.color.r;
      
      return float4(col.r, col.g, col.b, newOpacity);
     }
     ENDCG
    }
   }
}