cbuffer constants : register(b0) {
	matrix mvpTransform;
}

struct vertex_out {
	float4 pos: SV_POSITION;
	float4 color : COLOR;
};

vertex_out main(float4 position : POSITION,float4 color : COLOR) {
	vertex_out o;
	o.pos = mul(position, mvpTransform);
	o.color = color;
	return o;
}