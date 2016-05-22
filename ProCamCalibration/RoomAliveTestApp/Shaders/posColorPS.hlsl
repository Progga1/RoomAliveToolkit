cbuffer constants : register(b0) {
	float4 colorFactor;
}

float4 main(float4 position : SV_POSITION,float4 color : COLOR) : SV_TARGET{
	return color*colorFactor;
}