/*{
	"DESCRIPTION": "",
	"CREDIT": "by VIDVOX",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Film"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "uBias",
			"LABEL": "Bias",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 0.0,
			"DEFAULT": -0.67
		},
		{
			"NAME": "uScale",
			"LABEL": "Scale",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 10.0,
			"DEFAULT": 1.25
		},
		{
			"NAME": "uSource",
			"LABEL": "Source Gain",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "uChromatic",
			"LABEL": "Chromatic Distortion",
			"TYPE": "color",
			"DEFAULT": [
				0.75,
				0.875,
				1.0,
				0.75
			]
		},
		{
			"NAME": "uGhosts",
			"LABEL": "Ghosts",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 5.0,
			"DEFAULT": 3.0
		},
		{
			"NAME": "uGhostDispersal",
			"LABEL": "Ghost Dispersal",
			"TYPE": "float",
			"MIN": 0.125,
			"MAX": 0.75,
			"DEFAULT": 0.25
		},
		{
			"NAME": "uLensColor",
			"LABEL": "Lens Color",
			"TYPE": "color",
			"DEFAULT": [
				0.67,
				0.5,
				0.75,
				1.0
			]
		},
		{
			"NAME": "uHaloWidth",
			"LABEL": "Halo Width",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.33
		},
		{
			"NAME": "uNoise",
			"LABEL": "Dirt",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.25
		},
		{
			"NAME": "uCenter",
			"LABEL": "Center",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	],
	"PASSES": [
		{
			"TARGET": "downsampleAndThresholdImage",
			"WIDTH": "floor($WIDTH/2.0)",
			"HEIGHT": "floor($HEIGHT/2.0)",
			"DESCRIPTION": "Downsample and threshold"
		},
		{
			"TARGET": "featureGenerationImage",
			"WIDTH": "floor($WIDTH/2.0)",
			"HEIGHT": "floor($HEIGHT/2.0)",
			"DESCRIPTION": "Feature generation"
		},
		{
			"TARGET": "blurredImage",
			"WIDTH": "floor($WIDTH/4.0)",
			"HEIGHT": "floor($HEIGHT/4.0)",
			"DESCRIPTION": "Blur"
		},
		{
			
		}
	]
	
}*/


//	as a guide, http://john-chapman-graphics.blogspot.co.uk/2013/02/pseudo-lens-flare.html
//	1 downsample and threshold
//	2 feature generation: ghosts, halos and chromatic distortion
//	3 blur
//	4 upscale / blend


varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;


const float seed = 0.87342;



float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}



void main()
{

	if (PASSINDEX == 0)	{
		vec2 loc = isf_FragNormCoord;
		gl_FragColor = max(vec4(0.0), IMG_NORM_PIXEL(inputImage,loc) + uBias) * uScale;
	}
	else if (PASSINDEX == 1)	{
		//	flip the coordinates on this pass
		vec2 centerVec = uCenter/RENDERSIZE - vec2(1.0);
		vec2 texcoord = vec2(1.0) - isf_FragNormCoord;
		vec2 texelSize = 1.0 / RENDERSIZE;
		vec2 ghostVec = (vec2(0.5) - texcoord + centerVec) * uGhostDispersal;
		vec4 result = vec4(0.0);
		for (int i = 0; i < 5; ++i) { 
			if (float(i)>uGhosts)
				break;
			vec2 offset = fract(texcoord + ghostVec * float(i));

			//result += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset);

			result.r += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset * uChromatic.r).r * uChromatic.a;
			result.g += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset * uChromatic.g).g * uChromatic.a;
			result.b += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset * uChromatic.b).b * uChromatic.a;
			
		}
		
		//vec4 result = vec4(0.0);
		for (int i = 0; i < 5; ++i) {
			if (float(i)>uGhosts)
				break;
			vec2 offset = fract(texcoord + ghostVec * float(i));

			float weight = length(vec2(0.5) - offset) / length(vec2(0.5));
			weight = pow(1.0 - weight, 10.0);

			result += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset) * weight;
		}
		
		vec2 haloVec = normalize(ghostVec) * uHaloWidth;
		float weight = length(vec2(0.5) - fract(texcoord + haloVec)) / length(vec2(0.5));
		weight = pow(1.0 - weight, 5.0);
		result += IMG_NORM_PIXEL(downsampleAndThresholdImage, texcoord + haloVec) * weight;
		
		result *= uLensColor;
		
		gl_FragColor = result;
	}
	else if (PASSINDEX == 2)	{
		vec4 color = IMG_THIS_NORM_PIXEL(featureGenerationImage);
		vec4 colorL = IMG_NORM_PIXEL(featureGenerationImage, left_coord);
		vec4 colorR = IMG_NORM_PIXEL(featureGenerationImage, right_coord);
		vec4 colorA = IMG_NORM_PIXEL(featureGenerationImage, above_coord);
		vec4 colorB = IMG_NORM_PIXEL(featureGenerationImage, below_coord);

		vec4 colorLA = IMG_NORM_PIXEL(featureGenerationImage, lefta_coord);
		vec4 colorRA = IMG_NORM_PIXEL(featureGenerationImage, righta_coord);
		vec4 colorLB = IMG_NORM_PIXEL(featureGenerationImage, leftb_coord);
		vec4 colorRB = IMG_NORM_PIXEL(featureGenerationImage, rightb_coord);

		vec4 avg = (color + colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB) / 9.0;
		//avg = mix(color, (avg + depth*blur)/(1.0+depth), softness);
		gl_FragColor = avg;
	}
	else	{
		vec2	loc = isf_FragNormCoord;
		//	SHOULD REPLACE THIS WITH A BETTER / ADJUSTABLE FILM GRAIN OR DIRT GENERATOR
		vec4	noise = vec4(0.0);
		noise = (1.0 - uNoise) + uNoise * vec4(rand(loc*vec2(uNoise*seed,uChromatic.x)),rand(loc*vec2(uNoise*seed,uChromatic.y)),rand(loc*vec2(uNoise*seed,uChromatic.z)),1.0);

		vec4	srcColor = IMG_NORM_PIXEL(inputImage,loc);
		vec4	lensFlareColor = IMG_NORM_PIXEL(blurredImage,loc) * noise;
		gl_FragColor = uSource * srcColor + lensFlareColor * lensFlareColor.a;
	}
}