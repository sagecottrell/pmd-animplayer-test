shader_type canvas_item;
uniform vec3 team_color;
uniform vec3 replace_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	if (COLOR.xyz == replace_color) {
		COLOR.xyz = team_color;
	}

}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
