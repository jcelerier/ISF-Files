/*{
	"DESCRIPTION": "",
	"CREDIT": "VIDVOX",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
		{
			"NAME": "gridCols",
			"LABEL": "Grid Columns",
			"TYPE": "float",
			"DEFAULT": 16.0,
			"MIN": 1.0,
			"MAX": 32.0
		},
		{
			"NAME": "gridRows",
			"LABEL": "Grid Rows",
			"TYPE": "float",
			"DEFAULT": 9.0,
			"MIN": 1.0,
			"MAX": 32.0
		},
		{
			"NAME": "colorShift",
			"LABEL": "Color Shift",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "colorRange",
			"LABEL": "Color Range",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
	
}*/




vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}




void main()	{	
	vec4		outputPixelColor = vec4(1.0);
	vec2		loc = isf_FragNormCoord.xy;
	vec2		gridLoc = loc * vec2(gridCols, gridRows);
		
	//	figure out which grid square we are in, normalized, and use it for the hue
	outputPixelColor.r = (floor(gridLoc.x) + gridCols * floor(mod(gridLoc.y, gridRows))) / (gridCols * gridRows - 1.0);
	
	//	scale to the hue range
	outputPixelColor.r = outputPixelColor.r * colorRange;
	
	//	apply the hue shift
	outputPixelColor.r = mod(outputPixelColor.r + colorShift, 1.0);
	
	//	finally convert to RGB
	outputPixelColor.rgb = hsv2rgb(outputPixelColor.rgb);
	
	gl_FragColor = outputPixelColor;
}
