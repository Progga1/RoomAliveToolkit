cbuffer constants : register(b0)
{
	matrix mvpTransform;
}

float4 main(float4 position : POSITION) : SV_POSITION {
	return mul(position,mvpTransform);
}