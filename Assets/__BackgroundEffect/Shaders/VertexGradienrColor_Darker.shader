// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/VertexGradienrColor Darker" {
	Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
//    _SpecColor ("Specular Color",Color) = (1,1,1,1)
//    _Shininess("Shininess",float) = 10
//    _RimColor("Rim Color",Color) = (1,1,1,1)
//    _RimPower("RimPower",Range(-1,5)) = 1
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 
 
   }
 
   SubShader {
    Tags { "Queue" = "Transparent" }
    Pass { 
    	
     Tags{"LightMode" =  "ForwardBase"}
//     ZWrite Off // don't write to depth buffer 
//	 ZWrite On
//	 ZTest  LEqual
     Blend SrcAlpha OneMinusSrcAlpha// use alpha blending
 	 Cull Back
 	
     CGPROGRAM
 
     #pragma vertex vert
     #pragma fragment frag
 
     uniform float4 _Color; // define shader property for shaders
     uniform float4 _SpecColor;
     uniform float _Shininess;
     uniform float4 _RimColor;
     uniform float _RimPower;
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
      float4 posWorld : TEXCOORD0;
      float3 normalDir : TEXCOORD1;
      float4 tex : TEXCOORD2;
      float4 color : COLOR;
     };
 
      vertexOutput vert(vertexInput input)
      {
	      vertexOutput output;
	 
	      output.pos = UnityObjectToClipPos(input.vertex);
	      output.posWorld = mul(unity_ObjectToWorld,input.vertex);
	      output.normalDir = normalize(mul(float4(input.normal,0.0f),unity_WorldToObject).xyz);
	      output.tex = input.texcoord;
	      output.color=input.color;
	      return output;
     }
 
     float4 frag(vertexOutput input) : COLOR 
     {
//	     float2 tp = float2(input.tex.x, input.tex.y); //get textures coordinate
//	     float4 col = texturesx2D(_MainTex, tp) * _Color; //load main texture
//	   	 return float4(col.r, col.g, col.b, newOpacity);
 		
 		float3 normalDirection = input.normalDir;
 		float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
 		float3 lightDirection;
 		float atten;
 		
 		if(_WorldSpaceLightPos0.w == 0) /// DirectionLight
 		{
 			atten =1.0;
 		 	lightDirection= normalize (_WorldSpaceLightPos0.xyz);
 		}
 		else
 		{
 			float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
 			float distance= length(fragmentToLightSource);
 			atten = 1/distance;
 			lightDirection= normalize (fragmentToLightSource);

 		}		
 		////lighting
 		float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection,lightDirection));
 		float3 spacularReflection = diffuseReflection * pow(saturate(dot(reflect(-lightDirection,normalDirection),viewDirection)),_Shininess);
 		
// 		////Rim Lighting
// 		float rim = 1- saturate(dot(viewDirection,normalDirection));
// 		float3 rimLighting = diffuseReflection * _RimColor.xyz * pow(rim ,_RimPower);
// 		
 		float4 text = tex2D(_MainTex,input.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);

 		float3 lightFinal =  diffuseReflection  + UNITY_LIGHTMODEL_AMBIENT.rgb;
 		
	    float newOpacity = input.color.r * _Color.a;
	  
	  	float3 finalColor = _Color.xyz;
	  	
	  	float3 finalOut =  finalColor * text.xyz;
	
	    return float4(finalOut *  lightFinal.xyz ,newOpacity); 

	  
//	    return float4(text.xyz * lightFinal.xyz * _Color.xyz,newOpacity); 
	
	    
     }
     ENDCG
    }
    
   
    Pass {
    
     Tags{"LightMode" =  "ForwardAdd"}
//     ZWrite Off // don't write to depth buffer 
//   	 ZWrite On
//	 ZTest  LEqual
   	 Blend SrcAlpha One // use alpha blending
 	 Cull Back
 	
     CGPROGRAM
 
     #pragma vertex vert
     #pragma fragment frag
 
     uniform float4 _Color; // define shader property for shaders
     uniform float4 _SpecColor;
     uniform float _Shininess;
     uniform float4 _RimColor;
     uniform float _RimPower;
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
      float4 posWorld : TEXCOORD0;
      float3 normalDir : TEXCOORD1;
      float4 tex : TEXCOORD2;
      float4 color : COLOR;
     };
 
      vertexOutput vert(vertexInput input)
      {
	      vertexOutput output;
	 
	      output.pos = UnityObjectToClipPos(input.vertex);
	      output.posWorld = mul(unity_ObjectToWorld,input.vertex);
	      output.normalDir = normalize(mul(float4(input.normal,0.0f),unity_WorldToObject).xyz);
	      output.tex = input.texcoord;
	      output.color=input.color;
	      return output;
     }
 
     float4 frag(vertexOutput input) : COLOR 
     {
//	     float2 tp = float2(input.tex.x, input.tex.y); //get textures coordinate
//	     float4 col = texturesx2D(_MainTex, tp) * _Color; //load main texture
//	   	 return float4(col.r, col.g, col.b, newOpacity);
 		
 		float3 normalDirection = input.normalDir;
 		float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
 		float3 lightDirection;
 		float atten;
 		
 		if(_WorldSpaceLightPos0.w == 0) /// DirectionLight
 		{
 			atten =1.0;
 		 	lightDirection= normalize (_WorldSpaceLightPos0.xyz);
 		}
 		else
 		{
 			float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
 			float distance= length(fragmentToLightSource);
 			atten = 1/distance;
 			lightDirection= normalize (fragmentToLightSource);

 		}		
 		////lighting
 		float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection,lightDirection));
 		float3 spacularReflection = diffuseReflection * pow(saturate(dot(reflect(-lightDirection,normalDirection),viewDirection)),_Shininess);
 		
// 		////Rim Lighting
// 		float rim = 1- saturate(dot(viewDirection,normalDirection));
// 		float3 rimLighting = diffuseReflection * _RimColor.xyz * pow(rim ,_RimPower);
 		
 		float4 text = tex2D(_MainTex,input.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
 		
 		float3 lightFinal =  diffuseReflection ;
 		
	    float newOpacity = input.color.r * _Color.a;
	 	
	 	float3 finalColor =  _Color.xyz;
	  	
	  	float3 finalOut =  finalColor * text.xyz;
	
	    return float4(finalOut * lightFinal.xyz,newOpacity); 

	  
//	    return float4(text.xyz * lightFinal.xyz * _Color.xyz,newOpacity); 
     }
     ENDCG
    } 
    
    
    
   }
}
