extends Line2D


export var length = 50

var point

func _ready():
	set_as_toplevel(true)

func _process(delta):
	point = get_parent().global_position
	add_point(point)

	if points.size() > length:
		remove_point(0)
