# Reduce la espada esculpida en Nomad para VR: menos de 5,000 vertices (contados como Unity),
# UVs nuevas y una textura de 1024 horneada desde los colores por vertice.
import bpy, sys, os
from mathutils import Vector, Matrix

entrada, salida = sys.argv[-2], sys.argv[-1]
os.makedirs(salida, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=entrada)

# Triangulos objetivo por pieza (total ~3,050)
objetivo = {"Caja": 600, "Tubo 3": 360, "Tubo 4": 360, "Tubo.003": 450,
            "Esfera de quads 2": 300, "Tubo.001": 300}
nombres = {"Caja": "Hoja", "Tubo": "Hoja_Centro", "Tubo 1": "Hoja_Centro2", "Tubo 2": "Hoja_Centro3",
           "Tubo.001": "Guarda", "Tubo 3": "Guarda_Izq", "Tubo 4": "Guarda_Der",
           "Tubo.003": "Mango", "Esfera de quads 2": "Pomo"}

piezas = [o for o in bpy.context.scene.objects if o.type == 'MESH']
for o in piezas:
    # Quitar padres conservando la posicion
    mw = o.matrix_world.copy(); o.parent = None; o.matrix_world = mw
    bpy.context.view_layer.objects.active = o
    o.select_set(True)
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    o.select_set(False)
    tris = sum(len(p.vertices) - 2 for p in o.data.polygons)
    if o.name in objetivo and tris > objetivo[o.name]:
        m = o.modifiers.new("Decimate", 'DECIMATE')
        m.ratio = objetivo[o.name] / tris
        m.use_collapse_triangulate = True
        bpy.context.view_layer.objects.active = o
        bpy.ops.object.modifier_apply(modifier=m.name)
    # Sombreado suave: las caras vecinas comparten vertices (con plano, cada cara tiene los suyos)
    for poly in o.data.polygons: poly.use_smooth = True
    o.name = nombres.get(o.name, o.name)
    o.data.name = o.name

for o in [o for o in bpy.context.scene.objects if o.type == 'EMPTY']:
    bpy.data.objects.remove(o)

# UVs nuevas de todas las piezas juntas en una sola textura
bpy.ops.object.select_all(action='DESELECT')
for o in piezas: o.select_set(True)
bpy.context.view_layer.objects.active = piezas[0]
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.uv.smart_project(angle_limit=1.5, island_margin=0.01)  # angulo alto: menos islas y costuras
bpy.ops.object.mode_set(mode='OBJECT')

# Material que emite el color por vertice, para hornearlo a la textura
img = bpy.data.images.new("Espada_Color", 1024, 1024)
mat = bpy.data.materials.new("Espada")
mat.use_nodes = True
nt = mat.node_tree; nt.nodes.clear()
attr = nt.nodes.new("ShaderNodeVertexColor"); attr.layer_name = "Color"
emi = nt.nodes.new("ShaderNodeEmission")
out = nt.nodes.new("ShaderNodeOutputMaterial")
tex = nt.nodes.new("ShaderNodeTexImage"); tex.image = img
nt.links.new(attr.outputs["Color"], emi.inputs["Color"])
nt.links.new(emi.outputs["Emission"], out.inputs["Surface"])
nt.nodes.active = tex
for o in piezas:
    o.data.materials.clear(); o.data.materials.append(mat)

sc = bpy.context.scene
sc.render.engine = 'CYCLES'; sc.cycles.device = 'CPU'; sc.cycles.samples = 1
sc.render.bake.margin = 8
bpy.ops.object.bake(type='EMIT')
img.filepath_raw = os.path.join(salida, "Espada_Color.png"); img.file_format = 'PNG'; img.save()

# Material final: la textura como color base
nt.nodes.clear()
bsdf = nt.nodes.new("ShaderNodeBsdfPrincipled"); out = nt.nodes.new("ShaderNodeOutputMaterial")
tex = nt.nodes.new("ShaderNodeTexImage"); tex.image = img
nt.links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
nt.links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
for o in piezas:
    while o.data.color_attributes: o.data.color_attributes.remove(o.data.color_attributes[0])

# Raiz con el origen en el centro del mango
mango = bpy.data.objects["Mango"]
centro = sum((mango.matrix_world @ Vector(c) for c in mango.bound_box), Vector()) / 8
raiz = bpy.data.objects.new("Espada", None); sc.collection.objects.link(raiz)
for o in piezas:
    o.location -= centro
    o.parent = raiz

total = sum(sum(len(p.vertices) - 2 for p in o.data.polygons) for o in piezas)
for o in piezas: print("PIEZA", o.name, sum(len(p.vertices) - 2 for p in o.data.polygons))
punta = max((o.matrix_world @ Vector(c)).z for o in piezas for c in o.bound_box)
print("TOTAL_TRIS", total, "PUNTA_Z", round(punta, 3), "CENTRO_MANGO", tuple(round(v, 3) for v in centro))

bpy.ops.export_scene.fbx(filepath=os.path.join(salida, "Espada.fbx"), use_selection=False,
    apply_scale_options='FBX_SCALE_UNITS', bake_space_transform=True, axis_forward='-Z', axis_up='Y',
    object_types={'EMPTY', 'MESH'}, path_mode='STRIP', mesh_smooth_type='FACE')
