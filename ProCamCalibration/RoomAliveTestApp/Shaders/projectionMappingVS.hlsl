cbuffer constants : register(b0) {
	matrix mvpProjector;
	matrix mvpUser;
}

struct vertex_in
{
	float4 pos : POSITION;
};

struct vertex_out
{
	float4 pos : SV_POSITION;
	float2 texCoords : TEXCOORD;
	float4 color: COLOR;
};

vertex_out main(vertex_in i) {
	vertex_out o = (vertex_out)0;
	o.pos = mul(i.pos, mvpProjector);
	float4 userProj = mul(i.pos, mvpUser);
	o.texCoords = userProj.xy / userProj.w;
	o.color = float4(1.0,1.0,1.0,1.0);
	return o;
}