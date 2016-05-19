cbuffer constants : register(b0) {
	matrix mvpTransform;
}

struct vertex_in
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	float4 color : COLOR;
};

struct vertex_out
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 color: COLOR;
};

vertex_out main(vertex_in i) {
	vertex_out o = (vertex_out)0;
	o.pos = mul(i.pos, mvpTransform);
	o.uv = i.uv;
	o.color = i.color;
	return o;
}