Shader "Unlit/Transparent Color 2" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1) 
}

SubShader {
	Tags {"Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite off
//	ZWrite On
//	ZTest  LEqual
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Lighting Off
		SetTexture [_MainTex] {

            constantColor [_Color]

            Combine texture * constant, texture * constant 

         } 
	}
}
}
