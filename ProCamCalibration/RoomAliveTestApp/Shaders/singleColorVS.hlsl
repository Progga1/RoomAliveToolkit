cbuffer constants : register(b0) {
	matrix mvpTransform;
}

struct vertex_in {
	float4 position : POSITION;
};

struct vertex_out {
	float4 position : SV_POSITION;
};

vertex_out main(vertex_in i) {
	vertex_out o;
	o.position = mul(i.position, mvpTransform);
	return o;
}