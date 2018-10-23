/*{
	"CREDIT": "by v002",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Film", "v002"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "leakImage",
			"TYPE": "image"
		},
		{
			"NAME": "amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.5,
			"DEFAULT": 0.5
		},
		{
			"NAME": "length",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "angle",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 360.0,
			"DEFAULT": 0.0
		}
	]
}*/


// rotation matrix
varying mat2 rotmat;



void main (void) 
{

	// normalized point 0 - 1 texcoords
	vec2 point = isf_FragNormCoord;
	// our normal image.
	vec4 input0 = IMG_NORM_PIXEL(inputImage, point);
	// rotate sampling point
	point = ((point - 0.5) * rotmat) + 0.5;
	//point = clamp(point, 0.0, 1.0);

	// this adjusts the length of the leak
	float leakIntensity = pow(point.y, 1.0 + ((1.0 - length) * 19.0));

	// this adjusts the gamma/brightness of the overall effect.
	leakIntensity =  pow(leakIntensity, 1.0 / amount);

	// sample the leak // how do we want to handle edge texcoords during rotation? 
	if (point.x < 0.0)	{
		point.x = abs(point.x);
	}
	if (point.x > 1.0)	{
		point.x = 2.0 - point.x;
	}

	vec4 leak = IMG_NORM_PIXEL(leakImage, vec2(mod(point.x,1.0),0.0));

	leak = pow(leak * leakIntensity, vec4(1.0/(leakIntensity))); // - vec2(0.5, 0.0);
	leak += input0;

	gl_FragColor = mix(input0, leak, amount);
} 