extends CharacterBody3D
class_name  Player3D

@export var isDead : bool = false
@export var speed = 5.0
@export var head : Node3D
@export var raycast : RayCast3D
@export var inventory : Dictionary[String, int]

@export var forwardAction : String = "ui_up"
@export var backwardAction : String = "ui_down"
@export var leftAction : String = "ui_left"
@export var rightAction : String = "ui_right"
@export var jumpAction : String = ""

var _has_error : bool = false

func _ready() -> void:
	if !head:
		push_error("Player3D: Player Head (Node3D) Is Required But Not Provided!")
		_has_error = true
	
	if _has_error:
		return
	
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED) 
	
func _input(event: InputEvent) -> void:
	if _has_error:
		return
	
	if event.is_action_pressed("ui_cancel"):
		if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

	if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
		return
	
	if event is InputEventMouseMotion:
		rotate_y(-event.relative.x * .0075)
		
		if raycast:
			raycast.rotate_x(-event.relative.y * .0075)
			raycast.rotation.x = clampf(raycast.rotation.x, -deg_to_rad(70), deg_to_rad(70))
		
		head.rotate_x(-event.relative.y * .0075)
		head.rotation.x = clampf($head.rotation.x, -deg_to_rad(70), deg_to_rad(70))

func _physics_process(delta: float) -> void:
	if _has_error:
		return
	
	if not is_on_floor():
		velocity += get_gravity() * delta
	
	if !isDead:
		if raycast:
			raycast.rotation = head.rotation
		var input_dir := Input.get_vector(leftAction, rightAction, forwardAction, backwardAction).normalized()
		var direction := (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
		if direction:
			velocity.x = direction.x * speed
			velocity.z = direction.z * speed
		else:
			velocity.x = move_toward(velocity.x, 0, speed)
			velocity.z = move_toward(velocity.z, 0, speed)
	else:
		velocity = Vector3(0., 0., 0.)
	
	move_and_slide()

func kill() -> void:
	if _has_error:
		return
	
	isDead = true

func add_item_to_inventory(item : String, amount : int) -> void:
	if _has_error:
		return
	
	inventory[item] += amount
