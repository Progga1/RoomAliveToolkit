Texture2D<float4> tex : register(t0);
SamplerState texSampler : register(s0);

cbuffer constants : register(b0) {
	float4 color;
}

struct pixel_in {
	float4 pos : SV_POSITION;
	float4 posWorld : TEXCOORD;
	float4 color : COLOR;
};

float4 main(pixel_in i) : SV_TARGET{
	float fac = 1.0 / i.posWorld.w;
	float2 texCoord = float2(
		i.posWorld.x*fac*0.5+0.5,
		1-(i.posWorld.y*fac*0.5+0.5)
		);
	return color*i.color*tex.Sample(texSampler,texCoord);
}