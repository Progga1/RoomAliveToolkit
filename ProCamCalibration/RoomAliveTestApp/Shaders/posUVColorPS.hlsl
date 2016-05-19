cbuffer constants : register(b0) {
	float4 color;
}

struct pixel_in {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 color : COLOR;
};

float4 main(pixel_in i) : SV_TARGET{
	return color*i.color;
}