cbuffer constants : register(b0) {
	float4 color;
}

struct pixel_in {
	float4 position : SV_POSITION;
};

float4 main(pixel_in i) : SV_TARGET{
	return color;
}