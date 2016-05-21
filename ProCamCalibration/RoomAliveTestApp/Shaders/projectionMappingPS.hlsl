Texture2D<float4> tex : register(t0);
SamplerState texSampler : register(s0);

cbuffer constants : register(b0) {
	float4 color;
}

struct pixel_in {
	float4 pos : SV_POSITION;
	float4 color : COLOR;
};

float4 main(pixel_in i) : SV_TARGET{
	return color*i.color;
}