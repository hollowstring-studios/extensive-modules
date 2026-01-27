extends RigidBody3D
class_name Car

@export var max_speed : float = 500.0
@export var power : float = 150.0
@export var handling : float = 0.5

@export var anti_roll : bool = false
@export var roll_ratio: float = 0.5

@export var wheels : Array[Wheel]
@export var accel_curve : Curve

var velocity : float = 0.0

func _physics_process(_delta: float) -> void:
	DebugDraw3D.draw_arrow_ray(global_position, linear_velocity, .25, Color(1, 1, 0, 1), 0.01)
	
	for wheel in wheels:
		if anti_roll:
			_process_anti_roll(wheel)
		
		if Input.is_action_pressed("handbrake"):
			wheel.process_handbrake(self, true)
		else:
			wheel.process_handbrake(self, false)
		
		if wheel.is_slipping:
			wheel.emit_skid_particle(self)
		else:
			wheel.skid_particle.emitting = false
		
		wheel.force_raycast_update()
		wheel.process_suspension_logic(self)
		wheel.turn(self, Input.get_axis("steer_right", "steer_left") * handling)
		wheel.accelerate(self, Input.get_axis("accelerate", "brake") * power)
		wheel.process_traction(self)

func _process_anti_roll(wheel : Wheel) -> void:
	if wheel.is_grounded():
		center_of_mass = Vector3.ZERO
	else:
		center_of_mass_mode = RigidBody3D.CENTER_OF_MASS_MODE_CUSTOM
		center_of_mass = Vector3.DOWN * roll_ratio
