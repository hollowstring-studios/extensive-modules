extends Marker3D
class_name AxisCamera

@export var radius : float = 5.0
@export var sensv : float = 2.5
@export var target : Car

func _ready() -> void:
	%Camera3D.position.z += radius

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		rotation.y -= deg_to_rad(event.relative.x * sensv)
		rotation.x -= deg_to_rad(-event.relative.y * sensv)
		rotation.x = clampf(rotation.x, deg_to_rad(-44), deg_to_rad(0))

func _process(_delta: float) -> void:
	if !target:
		return
	
	global_position = target.global_position
