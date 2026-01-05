@tool
extends EditorImportPlugin

func _get_importer_name():
	return "pmd.zip"

func _get_visible_name() -> String:
	return "PMD Sprite Zip"

func _get_recognized_extensions() -> PackedStringArray:
	return ["zip"]

func _get_resource_type():
	return "AnimationLibrary"

func _get_import_options(path: String, preset_index: int) -> Array[Dictionary]:
	return [
		{
		"name": "FPS",
		"default_value": 30
		},
		{
			"name": "loops",
			"default_value": "Walk,Idle,Sleep,Charge"
		}
	]

func _get_save_extension() -> String:
	return "res"
	
func _get_option_visibility(path, option_name, options):
	return true

const DIRECTIONS: Array[String] = ['down', 'downright', 'right', 'upright', 'up', 'upleft', 'left', 'downleft']

func _import(source_file: String, save_path: String, options: Dictionary, platform_variants: Array[String], gen_files: Array[String]) -> Error:
	var zip = ZIPReader.new()
	if zip.open(source_file) != OK:
		return ERR_CANT_OPEN
		
	var DELTA = 1.0 / float(options.FPS)
	var loops: PackedStringArray = options.loops.split(",")
	
	var anim_data = parse_anim_data_xml(zip.read_file("AnimData.xml"))
	
	# ============================================================================
	# ============================================================================
	# load images
	var images: Dictionary[String, SpriteSheetInfo] = {}
	for anim_name in anim_data.Anims:
		var anim: Anim = anim_data.Anims[anim_name]
		var png = zip.read_file("%s-Anim.png" % anim.Name)
		var image = Image.new()
		image.load_png_from_buffer(png)
		images[anim.Name] = SpriteSheetInfo.new(image.get_width() / anim.FrameWidth, image.get_height() / anim.FrameHeight, ImageTexture.create_from_image(image))
	
	# ============================================================================
	# ============================================================================
	# set up animation library
	var lib = AnimationLibrary.new()
	
	for anim_name in anim_data.Anims:
		var anim: Anim = anim_data.Anims[anim_name]
		for dir_idx in range(DIRECTIONS.size()):
			var direction = DIRECTIONS[dir_idx]
			var aname = "%s-%s" % [anim_name, direction]
			
			var atlas = AtlasTexture.new()
			var img_info = images[anim_name]
			atlas.atlas = img_info.image
			var y = min(anim.FrameHeight * dir_idx, img_info.image.get_height() - anim.FrameHeight)
			atlas.region = Rect2(0, y, img_info.image.get_width(), anim.FrameHeight)
			
			var animation = Animation.new()
			animation.step = DELTA
			animation.resource_name = aname
			lib.add_animation(aname, animation)
			
			var aidx = animation.add_track(Animation.TYPE_VALUE)
			animation.track_set_path(aidx, "Sprite2D:frame")
			animation.value_track_set_update_mode(aidx, Animation.UPDATE_DISCRETE)
			animation.track_set_interpolation_loop_wrap(aidx, false)
			
			var texidx = animation.add_track(Animation.TYPE_VALUE)
			animation.track_set_path(texidx, "Sprite2D:texture")
			animation.track_insert_key(texidx, 0, atlas)
			
			var cbidx = animation.add_track(Animation.TYPE_METHOD)
			animation.track_set_path(cbidx, ".")
			
			var hfidx = animation.add_track(Animation.TYPE_VALUE)
			animation.track_set_path(hfidx, "Sprite2D:hframes")
			animation.track_insert_key(hfidx, 0, img_info.hFrames)
			
			var cur_time = 0
			for idx in range(anim.Durations.size()):
				animation.track_insert_key(aidx, cur_time, idx)
				cur_time += anim.Durations[idx] * DELTA
				
				if anim.HitFrame == idx:
					animation.track_insert_key(cbidx, cur_time, {"method": "on_hit_frame", "args": []})
				if anim.RushFrame == idx:
					animation.track_insert_key(cbidx, cur_time, {"method": "on_rush_frame", "args": []})
				if anim.ReturnFrame == idx:
					animation.track_insert_key(cbidx, cur_time, {"method": "on_return_frame", "args": []})
	
			animation.length = cur_time
			if anim_name in loops:
				animation.loop_mode = Animation.LOOP_LINEAR
			else:
				animation.loop_mode = Animation.LOOP_NONE
			
	return ResourceSaver.save(lib, "%s.%s" % [save_path, _get_save_extension()])

func parse_anim_data_xml(content: PackedByteArray) -> AnimData:
	var xml = XMLParser.new()
	xml.open_buffer(content)
	var data = AnimData.new()
	var current_anim: Anim
	
	while xml.read() != ERR_FILE_EOF:
		match xml.get_node_type():
			XMLParser.NODE_ELEMENT:
				var node_name = xml.get_node_name()
				match node_name:
					"AnimData":
						pass
					"ShadowSize":
						xml.read()
						data.ShadowSize = int(xml.get_node_data())
					"Anims":
						pass
					"Anim":
						current_anim = Anim.new()
					"Name":
						xml.read()
						data.Anims[xml.get_node_data()] = current_anim
						current_anim.Name = xml.get_node_data()
					"Index":
						xml.read()
						current_anim.Index = int(xml.get_node_data())
					"FrameWidth":
						xml.read()
						current_anim.FrameWidth = int(xml.get_node_data())
					"FrameHeight":
						xml.read()
						current_anim.FrameHeight = int(xml.get_node_data())
					"Durations":
						current_anim.Durations = []
					"Duration":
						xml.read()
						current_anim.Durations.append(int(xml.get_node_data()))
					"RushFrame":
						xml.read()
						current_anim.RushFrame = int(xml.get_node_data())
					"HitFrame":
						xml.read()
						current_anim.HitFrame = int(xml.get_node_data())
					"ReturnFrame":
						xml.read()
						current_anim.ReturnFrame = int(xml.get_node_data())
					"CopyOf":
						data.copies[current_anim.Name] = current_anim
						data.Anims.erase(current_anim.Name)
						xml.read()
						current_anim.CopyOf = xml.get_node_data()
	
	return data


class AnimData:
	var ShadowSize: int = 1
	var Anims: Dictionary[String, Anim]
	var copies: Dictionary[String, Anim]
	
class Anim:
	var Name: String
	var Index: int
	var FrameWidth: int
	var FrameHeight: int
	var Durations: Array[int]
	var RushFrame: int
	var ReturnFrame: int
	var HitFrame: int
	var CopyOf: String

class SpriteSheetInfo:
	var hFrames: int
	var vFrames: int
	var image: ImageTexture
	func _init(h: int, v: int, i: ImageTexture) -> void:
		hFrames = h
		vFrames = v
		image = i
