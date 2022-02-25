extends KinematicBody2D

var vel = Vector2()
var velFactor = 100
enum {active, inactive}
var state = active
var gravity = 40
var terminal_vel = 480
var rng = RandomNumberGenerator.new()

func _ready():
	rng.randomize()
	var rand_num = rng.randi_range(1, 5)
	$AnimationPlayer.play(str(rand_num))
	var rand_x = rng.randf_range(-6, 6)
	var rand_y = rng.randf_range(-8, 0)
	vel.x = rand_x * velFactor
	vel.y = rand_y * velFactor
	

func _physics_process(delta):
	if get_slide_count():
		state = inactive
	if state == inactive:
		rng.randomize()
		yield(get_tree().create_timer(rng.randf_range(1, 5)), "timeout")
		queue_free()
	else:
		vel.x = lerp(vel.x, 0, 0.05)
		vel.y += gravity
		vel.y = min(vel.y, terminal_vel)
		vel = move_and_slide(vel)
