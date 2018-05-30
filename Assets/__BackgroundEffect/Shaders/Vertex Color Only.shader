Shader "Vertex Color Only"
{
	Subshader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "LightMode"="ForwardBase"}
		
		ZWrite On
		BindChannels
		{
			Bind "vertex", vertex
			Bind "color", color	
		}
		Pass
		{
		
		}
	}
}