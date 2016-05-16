using RoomAliveToolkit;
using SharpDX;
using SharpDX.WIC;
using SharpGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAliveTestApp.Scenes {

	class MeshScene : Scene {

		Mesh mesh;
		MeshShader meshShader;
		ImagingFactory2 imagingFactory = new ImagingFactory2();
		MeshDeviceResources meshDeviceResources;
		private CameraControl cameraControl = new CameraControl();

		protected override void PostInit() {
			mesh = Mesh.FromOBJFile("Assets/FloorPlan.obj");
			meshShader = new MeshShader(device);
			meshDeviceResources = new MeshDeviceResources(device,imagingFactory,mesh);



			surface.setDepthEnabled(true);
		}

		public override void OnDraw() {

			surface.setOrthographicProjection(cameraControl.distance,-1,100);
			surface.setPerspectiveProjection(1.2f,0.01f,10);

			var world = SharpDX.Matrix.Scaling(1.0f) * SharpDX.Matrix.RotationY(90.0f / 180.0f * (float)Math.PI) *
							SharpDX.Matrix.RotationX(-40.0f / 180.0f * (float)Math.PI) * SharpDX.Matrix.Translation(0,0.7f,0.0f);
			world = SharpDX.Matrix.Identity;
			var pointLight = new PointLight();
			pointLight.position = new Vector3(0,2,0);
			pointLight.Ia = new Vector3(0.1f,0.1f,0.1f);
			SharpDX.Matrix projMat = surface.getProjectionMatrix();
			SharpDX.Matrix viewMat = cameraControl.getViewMatrix();
			//			projMat.Transpose();
						viewMat.Transpose();
			SharpDX.Matrix projViewMat = (projMat*viewMat);
			projViewMat.Transpose();
			meshShader.SetVertexShaderConstants(context,world,viewMat*projMat,pointLight.position);
			meshShader.Render(context,meshDeviceResources,pointLight,null,null,surface.viewport);
		}

		public override void RawEvent(InputEvent ev) {
			base.RawEvent(ev);
			ev.handle(cameraControl);
		}

	}

}
