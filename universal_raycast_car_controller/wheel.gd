extends RayCast3D
class_name Wheel

@export var is_steering : bool = false
@export var is_engine : bool = false

@export var wheelMesh : MeshInstance3D
@export var spring_strength : float =  100.
@export var spring_damp : float = 2.
@export var spring_rest : float = .5
@export var radius : float = .4
@export var max_angle : float = 45.
@export var grip : float = 1.5
@export var drift_grip : float = 0.1
@export var min_slip : float = .2
@export var grip_curve : Curve
@export var drag : float = .1
@export var brake_strength = .1
@export var stickyness : float = 0.0

@export var skid_particle: GPUParticles3D

var is_slipping : bool = false
var is_handbrake_enabled : bool = false
var current_grip : float = grip
var current_drag : float = drag

func is_grounded() -> bool:
	return is_colliding()

func _get_relative_vel(car : Car, dir : Vector3, point : Vector3) -> float:
	return dir.dot(car.linear_velocity + car.angular_velocity.cross(point - car.global_position))

func _get_world_vel(car : Car, point : Vector3) -> Vector3:
	return car.linear_velocity + car.angular_velocity.cross(point - car.global_position)

func turn(car : Car, input : float) -> void:
	if is_steering:
		if input:
			rotation.y = clampf(rotation.y + input * get_process_delta_time(), deg_to_rad(-max_angle), deg_to_rad(max_angle))
		else:
			rotation.y = move_toward(rotation.y, 0.0, get_process_delta_time() * car.handling)
	
func accelerate(car : Car, power : float) -> void:
	var forward_dir : Vector3 = -global_basis.z
	var vel : float = forward_dir.dot(car.linear_velocity)
	var speed_ratio : float = vel / car.max_speed
	var accel : float = car.accel_curve.sample_baked(speed_ratio)
	
	if !is_handbrake_enabled or power:
		wheelMesh.rotate_x((-vel * get_process_delta_time()) / radius)
	
	if is_colliding():
		var collision_point : Vector3 = global_position
		if is_engine:
			var forcev : Vector3 = forward_dir * power * accel
			var projected_forcev : Vector3 = (forcev - get_collision_normal() * forcev.dot(get_collision_normal()))
			var offset : Vector3 = collision_point - car.global_position
			car.apply_force(projected_forcev, offset)
			DebugDraw3D.draw_arrow_ray(collision_point, forcev/car.mass, .25, Color(0, 0, 1, 1), 0.01)

func process_traction(car : Car) -> void:
	if is_colliding():
		var side_dir : Vector3 = global_basis.x
		var side_vel : float = _get_relative_vel(car, side_dir, wheelMesh.global_position)
		
		if !is_handbrake_enabled:
			current_grip = lerpf(current_grip, grip_curve.sample_baked(absf(side_vel/_get_world_vel(car, wheelMesh.global_position).length())) * grip, grip * get_process_delta_time())
		
		var forcev : Vector3 = -side_dir * side_vel * current_grip * ((car.mass * ProjectSettings.get_setting("physics/3d/default_gravity"))/car.wheels.size())
		var offset : Vector3 = wheelMesh.global_position - car.global_position
		
		var forward_dir : Vector3 = -global_basis.z
		var forward_vel : float = _get_relative_vel(car, forward_dir, wheelMesh.global_position)
		var drag_forcev : Vector3 = -forward_dir * forward_vel * current_drag * ((car.mass * ProjectSettings.get_setting("physics/3d/default_gravity"))/car.wheels.size())
		
		if current_grip < min_slip:
			is_slipping = true
		else:
			is_slipping = false
		
		car.apply_force(forcev, offset)
		car.apply_force(drag_forcev, offset)
		
		DebugDraw3D.draw_arrow_ray(wheelMesh.global_position, forcev/car.mass, .25, Color(1, 0, 1, 1), 0.01)
		DebugDraw3D.draw_arrow_ray(wheelMesh.global_position, drag_forcev/car.mass, .25, Color(0, 1, 0, 1), 0.01)

func process_handbrake(car : Car, input : bool) -> void:
	if input:
		is_handbrake_enabled = true
		current_grip = drift_grip
		if abs(_get_world_vel(car, car.global_position)) > Vector3.ZERO:
			current_drag = brake_strength
	else:
		is_handbrake_enabled = false
		current_drag = drag

func emit_skid_particle(car : Car) -> void:
	skid_particle.global_position = get_collision_point() + Vector3.UP * 0.01
	skid_particle.look_at(skid_particle.global_position + car.global_basis.z)
	if !skid_particle.emitting:
		skid_particle.emitting = true

func process_suspension_logic(car : Car) -> void:
	if is_colliding():
		target_position.y = -(spring_rest + radius + stickyness)
		var collision_point : Vector3 = get_collision_point()
		var up_dir : Vector3 = global_transform.basis.y
		var length : float = global_position.distance_to(collision_point) - radius
		var offset : float = spring_rest - length
		
		wheelMesh.position.y = -length
		
		var damping_force : float = spring_damp * _get_relative_vel(car, up_dir, collision_point)
		
		var force : float = spring_strength * offset
		var forcev : Vector3 = (force - damping_force) * get_collision_normal()
		collision_point = global_position
		var force_offset : Vector3 = collision_point - car.global_position
		
		car.apply_force(forcev, force_offset)
		DebugDraw3D.draw_arrow_ray(collision_point, forcev/car.mass, .25, Color(1, 0, 0, 1), 0.01)
