/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "level",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : -0.5,
      "MIN" : -1
    },
    {
      "NAME" : "radius",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "MIN" : 0
    },
    {
      "NAME" : "center",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.5,
        0.5
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "bgColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        0,
        0
      ]
    },
    {
      "NAME" : "lineColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.94360309839248657,
        0.56016194820404053,
        0,
        1
      ]
    },
    {
      "LABELS" : [
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16"
      ],
      "NAME" : "majorDivisions",
      "TYPE" : "long",
      "DEFAULT" : 6,
      "VALUES" : [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16
      ]
    },
    {
      "LABELS" : [
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8"
      ],
      "NAME" : "minorHDivisions",
      "TYPE" : "long",
      "DEFAULT" : 2,
      "VALUES" : [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8
      ]
    },
    {
      "LABELS" : [
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8"
      ],
      "NAME" : "minorVDivisions",
      "TYPE" : "long",
      "DEFAULT" : 1,
      "VALUES" : [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8
      ]
    },
    {
      "NAME" : "majorDivisionLineWidth",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 3,
      "MIN" : 1
    },
    {
      "NAME" : "square",
      "TYPE" : "bool",
      "DEFAULT" : true
    }
  ],
  "CREDIT" : "by VIDVOX"
}
*/

const float minorDivisionLineWidth = 1.0;
const float pi = 3.14159265359;

#ifndef GL_ES
float distance (vec2 center, vec2 pt)
{
	float tmp = pow(center.x-pt.x,2.0)+pow(center.y-pt.y,2.0);
	return pow(tmp,0.5);
}
#endif

void main() {
	vec2 uv = vec2(isf_FragNormCoord[0],isf_FragNormCoord[1]);
	vec2 texSize = RENDERSIZE;
	vec2 tc = uv * texSize;
	vec2 modifiedCenter = center * RENDERSIZE;
	float r = distance(modifiedCenter, tc);
	float a = atan ((tc.y-modifiedCenter.y),(tc.x-modifiedCenter.x));
	float radius_sized = radius * length(RENDERSIZE);
	vec4 inputPixelColor = bgColor;
	
	tc -= modifiedCenter;

	if (r < radius_sized) 	{
		float percent = 1.0-(radius_sized - r) / radius_sized;
		if (level>=0.0)	{
			percent = percent * percent;
			tc.x = r*pow(percent,level) * cos(a);
			tc.y = r*pow(percent,level) * sin(a);
		}
		else	{
			float adjustedLevel = level/2.0;
			tc.x = r*pow(percent,adjustedLevel) * cos(a);
			tc.y = r*pow(percent,adjustedLevel) * sin(a);		
		}
	}
	tc += modifiedCenter;
	
	vec2 loc = tc;
	vec2 divisionSize = (square) ? vec2(max(RENDERSIZE.x,RENDERSIZE.y)) : RENDERSIZE;
	divisionSize = (divisionSize - majorDivisionLineWidth) / (1.0 + float(majorDivisions));
	vec2 modLoc = mod(loc,divisionSize);
	if ((modLoc.x < majorDivisionLineWidth)||(modLoc.y < majorDivisionLineWidth))	{
		inputPixelColor = lineColor;
	}
	if (minorHDivisions > 0)	{
		vec2	locDivisionSize = (divisionSize) / (1.0+float(minorHDivisions));
		modLoc = mod(loc,locDivisionSize);
		if (modLoc.x < minorDivisionLineWidth)	{
			inputPixelColor = lineColor;
		}
	}
	if (minorVDivisions > 0)	{
		vec2	locDivisionSize = (divisionSize) / (1.0+float(minorVDivisions));
		modLoc = mod(loc,locDivisionSize);
		if (modLoc.y < minorDivisionLineWidth)	{
			inputPixelColor = lineColor;
		}
	}
	
	gl_FragColor = inputPixelColor;
}
