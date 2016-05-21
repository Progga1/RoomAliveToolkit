cbuffer constants : register(b0) {
	matrix m;
	matrix vp;
	matrix userVP;
}

struct vertex_in
{
	float4 pos : POSITION;
};

struct vertex_out
{
	float4 pos : SV_POSITION;
	float4 posWorld : TEXCOORD;
	float4 color: COLOR;
};

vertex_out main(vertex_in i) {
	vertex_out o = (vertex_out)0;
	o.pos = mul(i.pos, mul(vp,m));
	o.posWorld = mul(i.pos, mul(userVP, m));
	o.color = float4(1.0,1.0,1.0,1.0);
	return o;
}