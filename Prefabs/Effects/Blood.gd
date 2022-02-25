extends KinematicBody2D

var vel = Vector2()
enum {active, inactive}
var state

var rng = RandomNumberGenerator.new()
func _ready():
	var rand_num = rng.randi_range(1, 5)
	$AnimationPlayer.play(str(rand_num))
	

func _process(delta):
	pass

func _physics_process(delta):
	pass
