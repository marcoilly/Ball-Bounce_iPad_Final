Shader "Unlit/Transparent Color 3" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1) 
}

SubShader {
	Tags {"Queue"="Transparent-1020" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite off
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
